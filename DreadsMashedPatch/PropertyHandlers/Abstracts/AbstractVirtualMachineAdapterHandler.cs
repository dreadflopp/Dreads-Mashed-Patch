using System;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;

namespace DreadsMashedPatch.PropertyHandlers.Abstracts
{
    public abstract class AbstractVirtualMachineAdapterHandler<TRecordGetter, TRecord, TAdapterGetter, TAdapter> : AbstractScriptListPropertyHandler
        where TRecordGetter : class, IMajorRecordGetter
        where TRecord : class, IMajorRecord
        where TAdapterGetter : class, IVirtualMachineAdapterGetter
        where TAdapter : class, IVirtualMachineAdapter
    {
        public override string PropertyName => "VirtualMachineAdapter";
        protected override bool CanBeNull => true;

        public override List<IScriptEntryGetter>? GetValue(IMajorRecordGetter record)
        {
            if (record is TRecordGetter typedRecord)
            {
                var adapter = GetVirtualMachineAdapter(typedRecord);
                return adapter?.Scripts?.ToList();
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement {typeof(TRecordGetter).Name} for {PropertyName}");
            }
            return null;
        }

        public override void SetValue(IMajorRecord record, List<IScriptEntryGetter>? value)
        {
            if (record is TRecord typedRecord)
            {
                if (value == null)
                {
                    SetVirtualMachineAdapter(typedRecord, null);
                    return;
                }

                var destinationAdapter = typedRecord is TRecordGetter typedGetter
                    ? GetVirtualMachineAdapter(typedGetter)
                    : null;
                var destinationScripts = destinationAdapter?.Scripts?.ToList() ?? [];

                // Like QUST/SCEN script setters, replacing scripts retains destination metadata.
                // Metadata is winner-owned on this list-only path; defaults apply only on creation.
                var newAdapter = CreateNewAdapter();
                if (destinationAdapter != null)
                {
                    newAdapter.Version = destinationAdapter.Version;
                    newAdapter.ObjectFormat = destinationAdapter.ObjectFormat;
                }

                // Add all scripts from the list
                foreach (var script in value)
                {
                    if (script == null) continue;

                    var destinationScript = destinationScripts.FirstOrDefault(candidate =>
                        string.Equals(candidate.Name, script.Name, StringComparison.Ordinal));
                    var newScript = PapyrusUnusedDataPolicy.CopyScript(script, destinationScript);
                    newAdapter.Scripts.Add(newScript);
                }

                SetVirtualMachineAdapter(typedRecord, newAdapter);
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement {typeof(TRecord).Name} for {PropertyName}");
            }
        }

        protected abstract TAdapterGetter? GetVirtualMachineAdapter(TRecordGetter record);
        protected abstract void SetVirtualMachineAdapter(TRecord record, TAdapter? value);
        protected abstract TAdapter CreateNewAdapter();

    }
}
