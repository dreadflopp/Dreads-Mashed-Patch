using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("LogCollector")]
public sealed class RaceEquipmentFlagsTests : IDisposable
{
    private const int NamedFlags = 0x06FF;
    private const int UnknownBits = unchecked((int)0xFFFFE000);
    private const string FlagNames = "HandToHand, OneHandSword, OneHandDagger, OneHandAxe, OneHandMace, TwoHandSword, TwoHandAxe, Bow, Spell, Shield";

    public void Dispose() => CoverageTestHistory.Reset();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UnnamedBitDifferencesKeepWinningFlagsAndLogNames(bool forwardOtherProperty)
    {
        CoverageTestHistory.Configure();
        LoggingSettings.Apply(new DiagnosticsSettings
        {
            DebugMode = true,
            Verbosity = PatcherLogVerbosity.Detailed,
            DeepDiveRecordSignatures = ["RACE"]
        });
        var mods = CoverageTestHistory.Mods();
        for (var i = 0; i < mods.Length; i++)
        {
            var race = NewRace(i == 1 ? NamedFlags : -6401);
            if (forwardOtherProperty && i == 1) race.BaseMass = 2;
            mods[i].Races.Add(race);
        }
        using var state = CoverageTestHistory.State(mods);
        var previousOutput = Console.Out;
        using var output = new StringWriter();
        try
        {
            Console.SetOut(output);
            Program.RunPatch(state);
        }
        finally
        {
            Console.SetOut(previousOutput);
        }

        if (forwardOtherProperty)
        {
            var patched = Assert.Single(state.PatchMod.Races);
            Assert.Equal(2f, patched.BaseMass);
            Assert.Equal((EquipTypeFlag)(-6401), patched.EquipmentFlags);
            AssertBinaryFlags(state.PatchMod, -6401);
        }
        else
        {
            Assert.Empty(state.PatchMod.Races);
        }

        var log = output.ToString();
        Assert.Contains($"[EquipmentFlags] Original: {FlagNames}, Winning: {FlagNames}", log);
        foreach (var mod in mods)
            Assert.Contains($"[EquipmentFlags]     {mod.ModKey}: {FlagNames}", log);
        Assert.Contains($"[EquipmentFlags]   Computed value: {FlagNames}", log);
        Assert.Contains("[EquipmentFlags]   Decision: KEEP WINNING (computed value equals winning)", log);
        Assert.DoesNotContain("-6401", log);
        Assert.DoesNotContain("[EquipmentFlags]   Decision: FORWARD", log);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(UnknownBits)]
    public void NamedSetsAndClearsMergeIndependentlyAndPreserveOnlyWinningUnnamedBits(int winnerUnknownBits)
    {
        CoverageTestHistory.Configure();
        var mods = CoverageTestHistory.Mods();
        int[] flags =
        [
            NamedFlags | UnknownBits,
            (NamedFlags & ~(int)EquipTypeFlag.HandToHand) | (int)EquipTypeFlag.Staff,
            NamedFlags | (int)EquipTypeFlag.Crossbow | winnerUnknownBits
        ];
        for (var i = 0; i < mods.Length; i++) mods[i].Races.Add(NewRace(flags[i]));
        using var state = CoverageTestHistory.State(mods);

        Program.RunPatch(state);

        var expected = (NamedFlags & ~(int)EquipTypeFlag.HandToHand)
            | (int)(EquipTypeFlag.Staff | EquipTypeFlag.Crossbow) | winnerUnknownBits;
        Assert.Equal((EquipTypeFlag)expected, Assert.Single(state.PatchMod.Races).EquipmentFlags);
        AssertBinaryFlags(state.PatchMod, expected);
        for (var i = 0; i < mods.Length; i++)
            Assert.Equal((EquipTypeFlag)flags[i], Assert.Single(mods[i].Races).EquipmentFlags);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void NamedFlagReversionsRequireDeclaredOrVirtualMasterPermission(int permission)
    {
        CoverageTestHistory.Configure(authorized: permission == 1);
        var mods = CoverageTestHistory.Mods();
        if (permission == 2)
            mods[2].ModHeader.MasterReferences.Add(new MasterReference { Master = mods[1].ModKey });
        var edited = (NamedFlags & ~(int)EquipTypeFlag.HandToHand) | (int)EquipTypeFlag.Staff;
        for (var i = 0; i < mods.Length; i++)
            mods[i].Races.Add(NewRace(i == 1 ? edited : -6401));
        using var state = CoverageTestHistory.State(mods);

        Program.RunPatch(state);

        if (permission != 0)
            Assert.Empty(state.PatchMod.Races);
        else
        {
            var expected = edited | UnknownBits;
            Assert.Equal((EquipTypeFlag)expected, Assert.Single(state.PatchMod.Races).EquipmentFlags);
            AssertBinaryFlags(state.PatchMod, expected);
        }
    }

    [Fact]
    public void EquipmentFlagsRetainAbsentAndPresentZeroAsDifferentValues()
    {
        var handler = new RaceRecordHandler().PropertyHandlers["EquipmentFlags"];
        var absent = NewRace(0);
        absent.EquipmentFlags = null;
        var present = NewRace(0);
        Assert.False(handler.AreValuesEqual(handler.GetValue(absent), handler.GetValue(present)));
        Assert.Equal("<absent>", handler.FormatValue(handler.GetValue(absent)));
        Assert.Equal("None (present)", handler.FormatValue(handler.GetValue(present)));
    }

    private static Race NewRace(int flags) => new(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)
    {
        EditorID = "SkeletonArmorRace",
        BaseMass = 1,
        EquipmentFlags = (EquipTypeFlag)flags
    };

    private static void AssertBinaryFlags(ISkyrimModGetter patch, int expected)
    {
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, patch.ModKey);
        Assert.Equal((EquipTypeFlag)expected, Assert.Single(overlay.Races).EquipmentFlags);
    }
}
