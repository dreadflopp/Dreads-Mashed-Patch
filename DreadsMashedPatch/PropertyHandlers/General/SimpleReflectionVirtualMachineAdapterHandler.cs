using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.PropertyHandlers.General
{
    /// <summary>
    /// Adapts a VMAD property to shared name-keyed script forwarding. Script writes preserve
    /// destination Version/ObjectFormat; the complete adapter is not selected on this path.
    /// </summary>
    public class SimpleReflectionVirtualMachineAdapterHandler<TRecord, TRecordGetter> : AbstractVirtualMachineAdapterHandler<TRecordGetter, TRecord, IVirtualMachineAdapterGetter, VirtualMachineAdapter>
        where TRecord : class, IMajorRecord
        where TRecordGetter : class, IMajorRecordGetter
    {
        private readonly PropertyInfo? _getterProperty;
        private readonly PropertyInfo? _setterProperty;

        public SimpleReflectionVirtualMachineAdapterHandler()
        {
            _getterProperty = ReflectionPropertyResolver.Find(typeof(TRecordGetter), "VirtualMachineAdapter");

            _setterProperty = ReflectionPropertyResolver.Find(typeof(TRecord), "VirtualMachineAdapter");

            if (_getterProperty == null)
            {
                throw new ArgumentException(
                    $"Property 'VirtualMachineAdapter' not found on {typeof(TRecordGetter).Name}");
            }
        }

        protected override IVirtualMachineAdapterGetter? GetVirtualMachineAdapter(TRecordGetter record)
        {
            try
            {
                var value = _getterProperty?.GetValue(record);
                return value as IVirtualMachineAdapterGetter;
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not read VirtualMachineAdapter via reflection", ex);
                return null;
            }
        }

        protected override void SetVirtualMachineAdapter(TRecord record, VirtualMachineAdapter? value)
        {
            try
            {
                if (_setterProperty == null)
                {
                    LogCollector.AddError(PropertyName, $"Error: Property 'VirtualMachineAdapter' is read-only or not found on {typeof(TRecord).Name}");
                    return;
                }

                _setterProperty.SetValue(record, value);
            }
            catch (Exception ex)
            {
                LogCollector.AddError(PropertyName, "Could not apply VirtualMachineAdapter via reflection", ex);
            }
        }

        protected override VirtualMachineAdapter CreateNewAdapter()
        {
            return new VirtualMachineAdapter();
        }
    }
}
