using System;
using System.Linq;
using System.Reflection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.PropertyHandlers.General
{
    /// <summary>
    /// Resolves enum flags through dotted property paths. Configured masks preserve unowned bits;
    /// missing intermediate objects are created when their types can be constructed.
    /// </summary>
    public class SimpleReflectionFlagPropertyHandler<TFlag, TRecord, TRecordGetter> : AbstractFlagPropertyHandler<TFlag>
        where TFlag : struct, Enum
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
        private readonly bool _preserveUnknownBits;
        private readonly bool _includeUnnamedBits;
        private readonly TFlag[]? _includedFlags;

        public SimpleReflectionFlagPropertyHandler(
            string propertyName,
            bool preserveUnknownBits = false,
            bool includeUnnamedBits = false,
            IEnumerable<TFlag>? includedFlags = null)
        {
            _propertyName = propertyName;
            _preserveUnknownBits = preserveUnknownBits;
            _includeUnnamedBits = includeUnnamedBits;
            _includedFlags = includedFlags?.Distinct().ToArray();
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
                property = ReflectionPropertyResolver.Find(
                    currentType,
                    path[i],
                    i < path.Length - 1 ? path[i + 1] : null);

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
                var property = ReflectionPropertyResolver.Find(currentType, path[i], path[i + 1]);

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

        public override TFlag GetValue(IMajorRecordGetter record)
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

                if (value == null)
                {
                    return default;
                }

                return value is TFlag typedValue ? typedValue : default;
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not read the flag property via reflection", ex);
                return default;
            }
        }

        public override void SetValue(IMajorRecord record, TFlag value)
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

                        if (intermediateValue == null)
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

                if (_preserveUnknownBits)
                {
                    var currentValue = _setterProperty.GetValue(currentObject);
                    if (currentValue is TFlag currentFlags)
                    {
                        long knownMask = 0;
                        foreach (var flag in GetAllFlags())
                        {
                            knownMask |= Convert.ToInt64(flag);
                        }

                        var currentBits = Convert.ToInt64(currentFlags);
                        var requestedBits = Convert.ToInt64(value);
                        var mergedBits = (currentBits & ~knownMask) | (requestedBits & knownMask);
                        valueToSet = (TFlag)Enum.ToObject(typeof(TFlag), mergedBits);
                    }
                }

                // Reflection accepts the boxed enum for a nullable enum property too.
                // Keep valueToSet: replacing it with value here loses preserved unowned bits.

                _setterProperty.SetValue(currentObject, valueToSet);
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not apply the flag property via reflection", ex);
            }
        }

        protected override TFlag[] GetAllFlags()
        {
            if (_includedFlags != null)
            {
                return _includedFlags;
            }

            if (!_includeUnnamedBits)
            {
                return Enum.GetValues<TFlag>();
            }

            var bitCount = System.Runtime.InteropServices.Marshal.SizeOf(Enum.GetUnderlyingType(typeof(TFlag))) * 8;
            return Enumerable.Range(0, bitCount)
                .Select(bit => (TFlag)Enum.ToObject(typeof(TFlag), 1UL << bit))
                .ToArray();
        }

        protected override bool IsFlagSet(TFlag flags, TFlag flag)
        {
            var flagsInt = Convert.ToInt64(flags);
            var flagInt = Convert.ToInt64(flag);
            return (flagsInt & flagInt) == flagInt;
        }

        protected override TFlag SetFlag(TFlag flags, TFlag flag, bool value)
        {
            var flagsInt = Convert.ToInt64(flags);
            var flagInt = Convert.ToInt64(flag);
            long result;
            if (value)
            {
                result = flagsInt | flagInt;
            }
            else
            {
                result = flagsInt & ~flagInt;
            }
            return (TFlag)Enum.ToObject(typeof(TFlag), result);
        }

        public string FormatValue(object? value)
        {
            if (value == null) return "null";
            if (value is not TFlag flags) return value.ToString() ?? "null";

            var setFlags = Enum.GetValues<TFlag>()
                .Where(f => Convert.ToInt64(f) != 0 && IsFlagSet(flags, f))
                .Select(f => f.ToString())
                .ToList();

            return setFlags.Count > 0 ? string.Join(", ", setFlags) : flags.ToString();
        }

    }
}
