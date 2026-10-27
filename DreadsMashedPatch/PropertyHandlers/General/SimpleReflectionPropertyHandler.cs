using System;
using System.Globalization;
using System.Reflection;
using System.Linq;
using Mutagen.Bethesda.Plugins.Records;
using DreadsMashedPatch;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.General
{
    /// <summary>
    /// Forwards value types and plain strings; complex references require a deep-copy handler.
    /// Nested paths create missing intermediate objects when their types can be constructed.
    /// </summary>
    public class SimpleReflectionPropertyHandler<TValue, TRecord, TRecordGetter> : AbstractPropertyHandler<TValue>
        where TRecord : class, IMajorRecord
        where TRecordGetter : class, IMajorRecordGetter
    {
        private readonly string _propertyName;
        private readonly string[] _propertyPath;
        private readonly PropertyInfo? _getterProperty;
        private readonly PropertyInfo? _setterProperty;
        private readonly PropertyInfo[]? _getterPathProperties;
        private readonly PropertyInfo[]? _setterPathProperties;
        private readonly Type[]? _setterPathTypes;
        private readonly float? _p3FloatEpsilon;

        public SimpleReflectionPropertyHandler(string propertyName, float? p3FloatEpsilon = null)
        {
            if (!typeof(TValue).IsValueType && typeof(TValue) != typeof(string))
            {
                throw new NotSupportedException(
                    $"{nameof(SimpleReflectionPropertyHandler<TValue, TRecord, TRecordGetter>)} supports only value types and strings. " +
                    $"Use GeneratedCopyReflectionPropertyHandler with a Mutagen-generated copy function for {typeof(TValue).FullName}.");
            }

            _propertyName = propertyName;
            _p3FloatEpsilon = p3FloatEpsilon;
            _propertyPath = propertyName.Split('.');

            _getterProperty = FindProperty(typeof(TRecordGetter), _propertyPath);

            _setterProperty = FindProperty(typeof(TRecord), _propertyPath);

            if (_getterProperty == null)
            {
                throw new ArgumentException(
                    $"Property '{propertyName}' not found on {typeof(TRecordGetter).Name}");
            }

            if (_propertyPath.Length > 1)
            {
                _getterPathProperties = BuildPropertyPath(typeof(TRecordGetter), _propertyPath, out _);
                _setterPathProperties = BuildPropertyPath(typeof(TRecord), _propertyPath, out _setterPathTypes);
            }
        }

        public override string PropertyName => _propertyName;

        private PropertyInfo? FindProperty(Type type, string[] path)
        {
            Type currentType = type;
            PropertyInfo? property = null;

            for (int i = 0; i < path.Length; i++)
            {
                property = FindPathProperty(currentType, path[i], i < path.Length - 1 ? path[i + 1] : null);

                if (property == null)
                {
                    return null;
                }

                if (i < path.Length - 1)
                {
                    currentType = property.PropertyType;
                    if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        currentType = currentType.GetGenericArguments()[0];
                    }
                }
            }

            return property;
        }

        private PropertyInfo[] BuildPropertyPath(Type startType, string[] path, out Type[] pathTypes)
        {
            var pathProperties = new PropertyInfo[path.Length - 1];
            pathTypes = new Type[path.Length - 1];
            Type currentType = startType;

            for (int i = 0; i < path.Length - 1; i++)
            {
                var property = FindPathProperty(currentType, path[i], path[i + 1]);

                if (property == null)
                {
                    throw new ArgumentException($"Property '{path[i]}' not found in path '{_propertyName}'");
                }

                pathProperties[i] = property;

                var propType = property.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = propType.GetGenericArguments()[0];
                }

                pathTypes[i] = propType;
                currentType = propType;
            }

            return pathProperties;
        }

        private static PropertyInfo? FindPathProperty(Type type, string name, string? nextSegment)
        {
            return ReflectionPropertyResolver.Find(type, name, nextSegment);
        }

        private static bool HasProperty(Type type, string name)
        {
            return ReflectionPropertyResolver.HasProperty(type, name);
        }

        public override TValue? GetValue(IMajorRecordGetter record)
        {
            if (record is not TRecordGetter typedRecord)
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement {typeof(TRecordGetter).Name} for {PropertyName}");
                return default;
            }

            if (_getterProperty == null)
            {
                return default;
            }

            try
            {
                object? currentObject = typedRecord;

                if (_getterPathProperties != null)
                {
                    for (int i = 0; i < _getterPathProperties.Length; i++)
                    {
                        if (currentObject == null)
                        {
                            return default;
                        }

                        currentObject = _getterPathProperties[i].GetValue(currentObject);

                        if (currentObject == null && i < _getterPathProperties.Length - 1)
                        {
                            return default;
                        }
                    }
                }

                if (currentObject == null)
                {
                    return default;
                }

                var value = _getterProperty.GetValue(currentObject);
                return value is TValue typedValue ? typedValue : default;
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not read the property via reflection", ex);
                return default;
            }
        }

        public override void SetValue(IMajorRecord record, TValue? value)
        {
            if (record is not TRecord typedRecord)
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement {typeof(TRecord).Name} for {PropertyName}");
                return;
            }

            if (_setterProperty == null)
            {
                LogCollector.AddError(PropertyName, $"Error: Property '{PropertyName}' is read-only or not found on {typeof(TRecord).Name}");
                return;
            }

            try
            {
                object? currentObject = typedRecord;

                if (_setterPathProperties != null && _setterPathTypes != null)
                {
                    for (int i = 0; i < _setterPathProperties.Length; i++)
                    {
                        var pathProperty = _setterPathProperties[i];
                        var pathType = _setterPathTypes[i];

                        var intermediateValue = pathProperty.GetValue(currentObject);

                        if (intermediateValue == null && value != null)
                        {
                            var newInstance = System.Activator.CreateInstance(pathType);
                            if (newInstance == null)
                            {
                                LogCollector.AddError(PropertyName, $"Error: Could not create instance of {pathType.Name} for property path '{PropertyName}'");
                                return;
                            }

                            pathProperty.SetValue(currentObject, newInstance);
                            intermediateValue = newInstance;
                        }

                        currentObject = intermediateValue;

                        if (currentObject == null)
                        {
                            return;
                        }
                    }
                }

                if (currentObject == null)
                {
                    LogCollector.AddError(PropertyName, $"Error: Could not navigate to property '{PropertyName}'");
                    return;
                }

                object? valueToSet = value;
                if (value != null && value.GetType().IsGenericType &&
                    value.GetType().GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var hasValueProperty = value.GetType().GetProperty("HasValue");
                    var valueProperty = value.GetType().GetProperty("Value");

                    if (hasValueProperty != null && valueProperty != null)
                    {
                        var hasValue = (bool)(hasValueProperty.GetValue(value) ?? false);
                        if (hasValue)
                        {
                            valueToSet = valueProperty.GetValue(value);
                        }
                        else
                        {
                            valueToSet = null;
                        }
                    }
                }

                _setterProperty.SetValue(currentObject, valueToSet);
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not apply the property via reflection", ex);
            }
        }

        public override bool AreValuesEqual(TValue? value1, TValue? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;

            if (value1 is string s1 && value2 is string s2)
            {
                return StringComparisonHelper.EqualsNormalized(s1, s2);
            }

            if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = typeof(TValue).GetGenericArguments()[0];

                if (underlyingType == typeof(float))
                {
                    var v1 = value1 as float?;
                    var v2 = value2 as float?;
                    if (v1.HasValue && v2.HasValue)
                    {
                        return Math.Abs(v1.Value - v2.Value) < 0.0001f;
                    }
                }

                // Nullable vectors use the same configured tolerance as non-nullable vectors,
                // so serialization noise cannot change reversion classification.
                if (underlyingType == typeof(P3Float))
                {
                    var hasValueProp = typeof(TValue).GetProperty("HasValue");
                    var valueProp = typeof(TValue).GetProperty("Value");
                    if (hasValueProp != null && valueProp != null)
                    {
                        var hasValue1 = (bool)(hasValueProp.GetValue(value1) ?? false);
                        var hasValue2 = (bool)(hasValueProp.GetValue(value2) ?? false);

                        if (!hasValue1 && !hasValue2) return true;
                        if (!hasValue1 || !hasValue2) return false;

                        var v1 = valueProp.GetValue(value1);
                        var v2 = valueProp.GetValue(value2);

                        if (v1 != null && v2 != null)
                        {
                            if (v1 is P3Float p1 && v2 is P3Float p2)
                            {
                                return P3FloatComparison.EqualsWithin(p1, p2, _p3FloatEpsilon ?? P3FloatComparison.DefaultEpsilon);
                            }

                            try
                            {
                                var p1Fallback = (P3Float)v1;
                                var p2Fallback = (P3Float)v2;
                                return P3FloatComparison.EqualsWithin(p1Fallback, p2Fallback, _p3FloatEpsilon ?? P3FloatComparison.DefaultEpsilon);
                            }
                            catch (Exception ex)
                            {
                                LogCollector.AddDiagnostic(
                                    PropertyName,
                                    "Nullable P3Float conversion failed; trying another equality strategy",
                                    ex);
                            }
                        }

                    }
                }
            }

            if (typeof(TValue) == typeof(P3Float))
            {
                if (value1 is P3Float p1 && value2 is P3Float p2)
                {
                    return P3FloatComparison.EqualsWithin(p1, p2, _p3FloatEpsilon ?? P3FloatComparison.DefaultEpsilon);
                }
            }

            if (typeof(TValue) == typeof(float))
            {
                if (value1 is float f1 && value2 is float f2)
                {
                    return Math.Abs(f1 - f2) < 0.0001f;
                }
            }

            // Mutagen value types can implement IEquatable against a getter interface.
            var valueType = value1.GetType();

            var equatableInterfaces = valueType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEquatable<>))
                .ToList();

            foreach (var equatableInterface in equatableInterfaces)
            {
                var equalsMethod = equatableInterface.GetMethod("Equals");
                if (equalsMethod != null)
                {
                    try
                    {
                        var result = equalsMethod.Invoke(value1, new object[] { value2! });
                        if (result is bool boolResult)
                        {
                            return boolResult;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCollector.AddDiagnostic(
                            PropertyName,
                            $"IEquatable comparison failed for {valueType.Name}; trying another equality strategy",
                            ex);
                    }
                }
            }

            if (value1 is IEquatable<TValue> equatable1)
            {
                return equatable1.Equals(value2!);
            }

            try
            {
                return value1.Equals(value2);
            }
            catch (Exception ex)
            {
                LogCollector.AddDiagnostic(
                    PropertyName,
                    $"Equals failed for {valueType.Name}; using object equality fallback",
                    ex);
                return Equals(value1, value2);
            }
        }

        public override string FormatValue(object? value)
        {
            if (value == null) return "null";

            string FormatP3Float(P3Float p3Float)
            {
                // Use invariant culture to ensure period as decimal separator
                // Also handle -0.0 by converting to 0.0
                float x = p3Float.X == 0f ? 0f : p3Float.X;
                float y = p3Float.Y == 0f ? 0f : p3Float.Y;
                float z = p3Float.Z == 0f ? 0f : p3Float.Z;

                return $"{x.ToString("F4", CultureInfo.InvariantCulture)}, {y.ToString("F4", CultureInfo.InvariantCulture)}, {z.ToString("F4", CultureInfo.InvariantCulture)}";
            }

            if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = typeof(TValue).GetGenericArguments()[0];
                if (underlyingType == typeof(P3Float))
                {
                    var hasValueProp = typeof(TValue).GetProperty("HasValue");
                    var valueProp = typeof(TValue).GetProperty("Value");
                    if (hasValueProp != null && valueProp != null)
                    {
                        var hasValue = (bool)(hasValueProp.GetValue(value) ?? false);
                        if (!hasValue) return "null";

                        var p3FloatValue = valueProp.GetValue(value);
                        if (p3FloatValue is P3Float p3Float)
                        {
                            return FormatP3Float(p3Float);
                        }
                    }
                }
            }

            if (typeof(TValue) == typeof(P3Float))
            {
                if (value is P3Float p3Float)
                {
                    return FormatP3Float(p3Float);
                }
            }

            return base.FormatValue(value);
        }
    }
}
