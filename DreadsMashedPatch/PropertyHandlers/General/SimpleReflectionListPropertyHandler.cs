using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.General
{
    /// <summary>
    /// A generic list property handler that uses reflection to access list properties.
    /// Supports only string and FormLink lists. Complex Mutagen elements must use
    /// <see cref="GeneratedCopyReflectionListPropertyHandler{TGetter, TMutable, TRecord, TRecordGetter}"/>.
    /// 
    /// Features:
    /// - Automatic detection of FormLink items
    /// - Explicit list semantics; callers must classify every registration
    /// - Typed equality for strings and FormKey equality for FormLinks
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list (e.g., IFormLinkGetter&lt;IPlacedObjectGetter&gt; or ILinkedReferencesGetter)</typeparam>
    /// <typeparam name="TRecord">The record type that contains the property (e.g., IPlacedObject)</typeparam>
    /// <typeparam name="TRecordGetter">The getter record type (e.g., IPlacedObjectGetter)</typeparam>
    public class SimpleReflectionListPropertyHandler<TItem, TRecord, TRecordGetter> : AbstractListPropertyHandler<TItem>
        where TItem : class
        where TRecord : class, IMajorRecord
        where TRecordGetter : class, IMajorRecordGetter
    {
        private readonly string _propertyName;
        private readonly PropertyInfo? _getterProperty;
        private readonly PropertyInfo? _setterProperty;
        private readonly bool _isFormLinkList;
        private readonly bool _canBeNull;
        private readonly Func<TItem, object?>? _keySelector;

        public SimpleReflectionListPropertyHandler(
            string propertyName,
            ListSemantics semantics,
            bool? canBeNull = null,
            Func<TItem, object?>? keySelector = null)
        {
            _propertyName = propertyName;
            _semantics = semantics;
            _keySelector = keySelector;

            // Find the property on the getter interface
            _getterProperty = ReflectionPropertyResolver.Find(typeof(TRecordGetter), propertyName);

            // Find the property on the setter interface
            _setterProperty = ReflectionPropertyResolver.Find(typeof(TRecord), propertyName);

            if (_getterProperty == null)
            {
                throw new ArgumentException(
                    $"Property '{propertyName}' not found on {typeof(TRecordGetter).Name}");
            }

            _canBeNull = canBeNull ?? ReflectionPropertyResolver.IsNullable(_getterProperty);

            // Detect if items are FormLinks
            _isFormLinkList = IsFormLinkType(typeof(TItem));

            if (!_isFormLinkList && typeof(TItem) != typeof(string))
            {
                throw new NotSupportedException(
                    $"{nameof(SimpleReflectionListPropertyHandler<TItem, TRecord, TRecordGetter>)} supports only strings and FormLinks. " +
                    $"Use {nameof(GeneratedCopyReflectionListPropertyHandler<TItem, TItem, TRecord, TRecordGetter>)} with a Mutagen-generated copy function for {typeof(TItem).FullName}.");
            }
        }

        public override string PropertyName => _propertyName;

        private readonly ListSemantics _semantics;

        public override ListSemantics Semantics => _semantics;
        protected override bool CanBeNull => _canBeNull;

        public override List<TItem>? GetValue(IMajorRecordGetter record)
        {
            if (record is not TRecordGetter typedRecord)
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement {typeof(TRecordGetter).Name} for {PropertyName}");
                return null;
            }

            if (_getterProperty == null)
            {
                return null;
            }

            try
            {
                var value = _getterProperty.GetValue(typedRecord);
                if (value == null)
                {
                    return null;
                }

                // Handle different collection types
                List<TItem>? list;
                if (value is IEnumerable<TItem> enumerable)
                {
                    list = enumerable.ToList();
                }
                else if (value is IEnumerable enumerableNonGeneric)
                {
                    list = enumerableNonGeneric.Cast<TItem>().ToList();
                }
                else
                {
                    return null;
                }

                // For FormLink lists: normalize so items with the same FormKey share the same reference.
                // This ensures the abstract class's GroupBy(item => item) and addition logic don't treat
                // duplicate FormKeys as separate groups (which would add 2+2+1+1=6 instead of 2+1+1=4).
                if (list != null && _isFormLinkList && list.Count > 1)
                {
                    var byKey = new Dictionary<FormKey, TItem>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var key = GetFormKey(list[i]);
                        if (byKey.TryGetValue(key, out var existing))
                            list[i] = existing;
                        else
                            byKey[key] = list[i];
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not read the list property via reflection", ex);
                return null;
            }
        }

        public override void SetValue(IMajorRecord record, List<TItem>? value)
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
                var currentList = _setterProperty.GetValue(typedRecord);

                // Handle nullable properties (like LocationRefTypes)
                if (_canBeNull && value == null)
                {
                    _setterProperty.SetValue(typedRecord, null);
                    return;
                }

                // If currentList is null, we need to create a new list
                if (currentList == null && value != null)
                {
                    // Check if the property type is nullable
                    var propertyType = _setterProperty.PropertyType;
                    Type? listType = null;
                    bool propertyIsNullable = false;

                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        listType = propertyType.GetGenericArguments()[0];
                        propertyIsNullable = true;
                    }
                    else
                    {
                        listType = propertyType;
                    }

                    // If property is not nullable and we have a value, create a new list
                    if (!propertyIsNullable && listType != null && listType.IsGenericType)
                    {
                        var genericArgs = listType.GetGenericArguments();
                        if (genericArgs.Length > 0)
                        {
                            var itemType = genericArgs[0];
                            var newList = CreateListInstance(listType, itemType);
                            if (newList != null)
                            {
                                foreach (var item in value)
                                {
                                    if (item == null) continue;

                                    var itemToAdd = CopySupportedItem(item);

                                    if (itemToAdd != null)
                                    {
                                        AddItem(newList, itemToAdd);
                                    }
                                }
                                _setterProperty.SetValue(typedRecord, newList);
                                return;
                            }
                        }
                    }

                    // If we can't create a list and property is not nullable, warn and return
                    if (!propertyIsNullable)
                    {
                        Console.WriteLine($"Warning: Property '{PropertyName}' is null and cannot be set (property is not nullable and list creation failed)");
                        return;
                    }
                }

                // Clear the existing list
                if (currentList != null)
                {
                    var clearMethod = currentList.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(currentList, null);
                    }
                }

                // Add new items to existing list
                if (value != null && currentList != null)
                {
                    foreach (var item in value)
                    {
                        if (item == null) continue;

                        var itemToAdd = CopySupportedItem(item);

                        if (itemToAdd != null)
                        {
                            AddItem(currentList, itemToAdd);
                        }
                    }
                }
                else if (value != null && currentList == null && _canBeNull)
                {
                    // Create a new list if property is nullable
                    var listType = _setterProperty.PropertyType;
                    if (listType.IsGenericType)
                    {
                        var genericArgs = listType.GetGenericArguments();
                        if (genericArgs.Length > 0)
                        {
                            var itemType = genericArgs[0];
                            var newList = CreateListInstance(listType, itemType);
                            if (newList != null)
                            {
                                foreach (var item in value)
                                {
                                    if (item == null) continue;

                                    var itemToAdd = CopySupportedItem(item);

                                    if (itemToAdd != null)
                                    {
                                        AddItem(newList, itemToAdd);
                                    }
                                }
                                _setterProperty.SetValue(typedRecord, newList);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not apply the list property via reflection", ex);
            }
        }

        protected override bool IsItemEqual(TItem? item1, TItem? item2)
        {
            if (item1 == null && item2 == null) return true;
            if (item1 == null || item2 == null) return false;

            if (_isFormLinkList)
            {
                // For FormLinks, compare by FormKey
                var formKey1 = GetFormKey(item1);
                var formKey2 = GetFormKey(item2);
                return formKey1.Equals(formKey2);
            }
            return EqualityComparer<TItem>.Default.Equals(item1, item2);
        }

        protected override bool IsItemIdentityEqual(TItem? item1, TItem? item2)
        {
            if (_keySelector == null)
            {
                return base.IsItemIdentityEqual(item1, item2);
            }

            if (item1 == null || item2 == null)
            {
                return item1 == null && item2 == null;
            }

            return Equals(_keySelector(item1), _keySelector(item2));
        }

        protected override IReadOnlyList<object?> GetSortKey(TItem item)
        {
            if (_keySelector != null)
            {
                var key = _keySelector(item);
                return key is IReadOnlyList<object?> composite ? composite : [key];
            }

            if (_isFormLinkList)
            {
                return [GetFormKey(item)];
            }

            if (item is string || item is IComparable || item.GetType().IsEnum)
            {
                return [item];
            }

            return base.GetSortKey(item);
        }

        private static void AddItem(object list, object item)
        {
            if (list is IList nonGenericList)
            {
                nonGenericList.Add(item);
                return;
            }

            var addMethod = list.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(method =>
                {
                    if (method.Name != "Add") return false;
                    var parameters = method.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(item);
                });

            if (addMethod == null)
            {
                throw new InvalidOperationException($"No compatible Add method found on {list.GetType().Name} for {item.GetType().Name}.");
            }

            addMethod.Invoke(list, new[] { item });
        }

        protected override string FormatItem(TItem? item)
        {
            if (item == null) return "null";

            if (_isFormLinkList)
            {
                var formKey = GetFormKey(item);
                return $"FormLink({formKey})";
            }
            return item.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Checks if a type is a FormLink type.
        /// </summary>
        private static bool IsFormLinkType(Type type)
        {
            if (type == null) return false;

            // Check if it implements IFormLinkGetter or IFormLinkNullableGetter specifically
            // (not IFormLinkContainerGetter which is implemented by complex objects)
            var interfaces = type.GetInterfaces();
            if (interfaces.Any(i =>
                (i.IsGenericType &&
                 (i.GetGenericTypeDefinition().Name == "IFormLinkGetter`1" ||
                  i.GetGenericTypeDefinition().Name == "IFormLinkNullableGetter`1")) ||
                (i.Name == "IFormLinkGetter" || i.Name == "IFormLinkNullableGetter")))
            {
                return true;
            }

            // Check if it's a FormLink or FormLinkNullable type
            if (type.Name == "FormLink`1" || type.Name == "FormLinkNullable`1" ||
                (type.IsGenericType &&
                 (type.GetGenericTypeDefinition().Name == "FormLink`1" ||
                  type.GetGenericTypeDefinition().Name == "FormLinkNullable`1")))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the FormKey from a FormLink getter.
        /// </summary>
        private FormKey GetFormKey(TItem formLink)
        {
            var formKeyProperty = formLink.GetType().GetProperty("FormKey");
            if (formKeyProperty != null)
            {
                var formKey = formKeyProperty.GetValue(formLink);
                if (formKey is FormKey fk)
                {
                    return fk;
                }
            }
            return FormKey.Null;
        }

        /// <summary>
        /// Creates a new FormLink from a FormLink getter. For null FormKeys returns a default FormLinkNullable
        /// so that null entries are preserved in lists (e.g. LitWater).
        /// </summary>
        private object? CreateFormLinkFromGetter(TItem formLinkGetter)
        {
            var formKey = GetFormKey(formLinkGetter);
            var getterType = formLinkGetter.GetType();
            var interfaces = getterType.GetInterfaces();
            var formLinkInterface = interfaces.FirstOrDefault(i =>
                i.IsGenericType && (i.Name.StartsWith("IFormLink") || i.Name.StartsWith("IFormLinkNullable")));
            if (formLinkInterface == null)
                return null;

            var targetType = formLinkInterface.GetGenericArguments()[0];
            var isNullable = formLinkInterface.Name.StartsWith("IFormLinkNullable");
            if (formKey.IsNull)
            {
                if (isNullable)
                {
                    var formLinkNullableType = typeof(FormLinkNullable<>).MakeGenericType(targetType);
                    return System.Activator.CreateInstance(formLinkNullableType);
                }
                // Non-nullable list (e.g. IFormLinkGetter): still add FormLink(T)(FormKey.Null) so null entry is written
                var formLinkTypeNull = typeof(FormLink<>).MakeGenericType(targetType);
                var ctorNull = formLinkTypeNull.GetConstructor(new[] { typeof(FormKey) });
                return ctorNull != null ? System.Activator.CreateInstance(formLinkTypeNull, FormKey.Null) : null;
            }

            var formLinkType = isNullable
                ? typeof(FormLinkNullable<>).MakeGenericType(targetType)
                : typeof(FormLink<>).MakeGenericType(targetType);
            var constructor = formLinkType.GetConstructor(new[] { typeof(FormKey) });
            return constructor != null ? System.Activator.CreateInstance(formLinkType, formKey) : null;
        }

        private object? CopySupportedItem(TItem item)
        {
            return _isFormLinkList ? CreateFormLinkFromGetter(item) : item;
        }

        /// <summary>
        /// Creates an instance of a list type.
        /// </summary>
        private object? CreateListInstance(Type listType, Type itemType)
        {
            try
            {
                // Try ExtendedList first (common in Mutagen)
                var extendedListType = typeof(ExtendedList<>).MakeGenericType(itemType);
                if (listType.IsAssignableFrom(extendedListType))
                {
                    return System.Activator.CreateInstance(extendedListType);
                }

                // Try List<T>
                var listGenericType = typeof(List<>).MakeGenericType(itemType);
                if (listType.IsAssignableFrom(listGenericType))
                {
                    return System.Activator.CreateInstance(listGenericType);
                }

                // Try direct instantiation
                return System.Activator.CreateInstance(listType);
            }
            catch (Exception ex)
            {
                LogCollector.AddError(
                    PropertyName,
                    $"Could not create list type {listType.Name} for items of type {itemType.Name}",
                    ex);
                return null;
            }
        }
    }
}
