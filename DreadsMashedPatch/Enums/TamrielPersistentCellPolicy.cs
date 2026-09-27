namespace DreadsMashedPatch.Enums;

public enum TamrielPersistentCellPolicy
{
    /// <summary>Copy the complete CELL header authored by Dawnguard.esm.</summary>
    PreferDawnguard,

    /// <summary>Copy the complete original CELL header authored by Skyrim.esm.</summary>
    PreferSkyrim,

    /// <summary>Copy the complete winning CELL header without normal forwarding.</summary>
    KeepWinning,

    /// <summary>Process the CELL with the ordinary property-forwarding algorithm.</summary>
    StandardForwarding,

    /// <summary>
    /// Use normal forwarding unless the winning CELL header matches Skyrim.esm,
    /// in which case copy Dawnguard.esm's header.
    /// </summary>
    Hybrid
}
