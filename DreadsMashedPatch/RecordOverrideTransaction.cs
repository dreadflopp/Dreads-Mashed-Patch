using Loqui;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch;

/// <summary>
/// Mutagen context insertion and property application run against isolated output ancestry.
/// Publish only after both succeed, preserving already-patched parents and siblings.
/// </summary>
internal static class RecordOverrideTransaction
{
    public static void Apply(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context,
        ISkyrimMod output,
        Action<IMajorRecord>? apply)
    {
        var staged = new SkyrimMod(output.ModKey, output.SkyrimRelease);
        var ancestry = new List<object> { context.Record };
        IMajorRecordGetter root = context.Record;
        for (var parent = context.Parent; parent != null; parent = parent.Parent)
        {
            if (parent.Record != null) ancestry.Add(parent.Record);
            if (parent.Record is IMajorRecordGetter record) root = record;
        }

        // Interior CELL ancestry is stored in list groups rather than a keyed major-record group.
        if (root is ICellGetter)
        {
            var sourceBlock = ancestry.OfType<ICellBlockGetter>().Single();
            var sourceSubBlock = ancestry.OfType<ICellSubBlockGetter>().Single();
            var index = output.Cells.Records.FindIndex(block => block.BlockNumber == sourceBlock.BlockNumber);
            if (index >= 0)
            {
                var existing = output.Cells[index];
                var copy = existing.DeepCopy(new CellBlock.TranslationMask(true) { SubBlocks = false });
                copy.SubBlocks.AddRange(existing.SubBlocks);
                var subIndex = copy.SubBlocks.FindIndex(sub => sub.BlockNumber == sourceSubBlock.BlockNumber);
                if (subIndex >= 0)
                {
                    var sub = copy.SubBlocks[subIndex];
                    var subCopy = sub.DeepCopy(new CellSubBlock.TranslationMask(true) { Cells = false });
                    subCopy.Cells.AddRange(sub.Cells.Select(cell => cell.FormKey == root.FormKey
                        ? CopyCell(cell, context.Record) : cell));
                    copy.SubBlocks[subIndex] = subCopy;
                }
                staged.Cells.Add(copy);
            }
            var candidate = context.GetOrAddAsOverride(staged);
            apply?.Invoke(candidate);
            var block = staged.Cells.Single();
            if (index >= 0) output.Cells[index] = block;
            else output.Cells.Add(block);
            return;
        }

        // Generated group dispatch accepts registered record/interface types, not
        // the BinaryOverlay runtime implementations used by disk-backed inputs.
        var rootType = ((ILoquiObject)root).Registration.GetterType;
        var outputGroup = output.TryGetTopLevelGroup(rootType)
            ?? throw new InvalidOperationException($"No output group for {rootType.Name}.");
        var stagedGroup = staged.TryGetTopLevelGroup(rootType)!;
        if (outputGroup.ContainsKey(root.FormKey))
        {
            var existing = outputGroup[root.FormKey];
            stagedGroup.SetUntyped(existing is IWorldspaceGetter world && root.FormKey != context.Record.FormKey
                ? CopyWorldspace((Worldspace)world, ancestry, context.Record)
                : existing.DeepCopy());
        }

        var stagedRecord = context.GetOrAddAsOverride(staged);
        apply?.Invoke(stagedRecord);
        outputGroup.SetUntyped((IMajorRecord)stagedGroup[root.FormKey]);
    }

    // Copy mutable ancestors and their collection containers, retaining untouched siblings.
    // The target itself is always a full generated copy. This avoids copying every already
    // patched reference in a worldspace for each new child override.
    private static Cell CopyCell(Cell source, IMajorRecordGetter target)
    {
        if (source.FormKey == target.FormKey) return source.DeepCopy();
        var copy = source.DeepCopy(new Cell.TranslationMask(true)
        {
            Persistent = false, Temporary = false, NavigationMeshes = false, Landscape = false
        });
        copy.Persistent.AddRange(source.Persistent.Select(record => record.FormKey == target.FormKey
            ? (IPlaced)record.DeepCopy() : record));
        copy.Temporary.AddRange(source.Temporary.Select(record => record.FormKey == target.FormKey
            ? (IPlaced)record.DeepCopy() : record));
        copy.NavigationMeshes.AddRange(source.NavigationMeshes.Select(record => record.FormKey == target.FormKey
            ? record.DeepCopy() : record));
        copy.Landscape = source.Landscape?.FormKey == target.FormKey
            ? source.Landscape.DeepCopy() : source.Landscape;
        return copy;
    }

    private static Worldspace CopyWorldspace(Worldspace source, List<object> ancestry, IMajorRecordGetter target)
    {
        var cellKey = ancestry.OfType<ICellGetter>().Single().FormKey;
        var copy = source.DeepCopy(new Worldspace.TranslationMask(true) { TopCell = false, SubCells = false });
        copy.TopCell = source.TopCell?.FormKey == cellKey ? CopyCell(source.TopCell, target) : source.TopCell;
        copy.SubCells.AddRange(source.SubCells);
        var sourceBlock = ancestry.OfType<IWorldspaceBlockGetter>().SingleOrDefault();
        var sourceSubBlock = ancestry.OfType<IWorldspaceSubBlockGetter>().SingleOrDefault();
        if (sourceBlock == null || sourceSubBlock == null) return copy;
        var blockIndex = copy.SubCells.FindIndex(block => block.BlockNumberX == sourceBlock.BlockNumberX
            && block.BlockNumberY == sourceBlock.BlockNumberY);
        if (blockIndex < 0) return copy;
        var block = copy.SubCells[blockIndex];
        var blockCopy = block.DeepCopy(new WorldspaceBlock.TranslationMask(true) { Items = false });
        blockCopy.Items.AddRange(block.Items);
        copy.SubCells[blockIndex] = blockCopy;
        var subIndex = blockCopy.Items.FindIndex(sub => sub.BlockNumberX == sourceSubBlock.BlockNumberX
            && sub.BlockNumberY == sourceSubBlock.BlockNumberY);
        if (subIndex < 0) return copy;
        var sub = blockCopy.Items[subIndex];
        var subCopy = sub.DeepCopy(new WorldspaceSubBlock.TranslationMask(true) { Items = false });
        subCopy.Items.AddRange(sub.Items.Select(cell => cell.FormKey == cellKey ? CopyCell(cell, target) : cell));
        blockCopy.Items[subIndex] = subCopy;
        return copy;
    }
}
