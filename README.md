# Dread's Mashed Patch

Dread's Mashed Patch is an automatic conflict-resolution patcher for Skyrim modlists. It combines changes to supported record properties from your load order into `MashedPatch.esp`, using property-specific rules, plugin master relationships, and configurable forwarding policies.

The standalone desktop application uses the **Mutagen Bethesda framework** and **Synthesis pipeline libraries**, with a WPF interface on .NET. It works on **Windows and through Proton on Linux**. Launch the executable directly or through your mod manager; setup details are in the [application guide](docs/STANDALONE_APP.md).

It may be useful to modlist creators, patch creators, and users who patch their own modlists.

## Download

Download the patcher from **Nexus Mods**: [Nexus download page — placeholder](https://www.nexusmods.com/skyrimspecialedition/mods/REPLACE_WITH_MOD_ID).

The link above will be replaced with the published mod page.

## Testing and manual review

**The current version is still being tested, and patching bugs are possible.** The patcher handles a large number of record types and properties. Some records rarely conflict and their conflict-handling behavior has not yet been tested in real modlists. Automated tests and successful patch generation do not guarantee correct results for your modlist.

**Inspect the generated patch in xEdit before using it.** Check that changes were patched as intended. An automatic patcher cannot resolve everything correctly: the end user must manually correct some of the patching and create additional compatibility patches where needed. Review every output plugin if the patch is split into multiple files.

Please report bugs through this repository's **Issues** tab. Include the patcher version, load order, relevant plugins, record FormID/FormKey and property, expected and actual results, xEdit screenshots, and the run log. See [logs and diagnostics](docs/STANDALONE_APP.md#diagnostics).

## Documentation

- [Application setup, settings, and logs](docs/STANDALONE_APP.md)
- [Technical guide: how records and properties are patched](docs/record-patching/README.md)
- [All supported records and their property tables](docs/record-patching/INDEX.md)
- [Unsupported records](docs/record-patching/unsupported.md) and [known limits](docs/record-patching/KNOWN-ISSUES.md)
- [Documentation index and developer resources](docs/README.md)
