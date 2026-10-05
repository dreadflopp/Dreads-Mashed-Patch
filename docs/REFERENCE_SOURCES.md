# Rebuilding external reference sources

[Documentation index](README.md) · [Development guide](DEVELOPMENT.md)

The reference sources are intentionally excluded from Git. Regenerate them after
cloning the repository or updating dependencies. They are for investigation only;
do not edit them or add their projects to the solution.

## Prerequisites

- Git and network access to GitHub and NuGet.
- .NET 10 SDK to restore the core project.
- .NET 8 runtime for the pinned ILSpy command-line tool, version `9.1.0.7988`.
  Installing the .NET 10 SDK alone does not supply this runtime.
- PowerShell 7 (`pwsh`) on Linux. Windows PowerShell also supports these scripts.

Check your installation with `git --version`, `dotnet --list-sdks`, and
`dotnet --list-runtimes`. The runtime list should include `Microsoft.NETCore.App 8.0.x`.

## Linux

Run from the repository root:

```bash
dotnet restore DreadsMashedPatch/DreadsMashedPatch.csproj
pwsh -NoProfile -File scripts/Export-MutagenDecompiled.ps1
pwsh -NoProfile -File scripts/Export-XEditSource.ps1
```

No Windows desktop build, Skyrim installation, or Wine is needed to export these
references. Restore on the Linux machine itself; do not copy Windows `obj/` files
or the `.tools/` directory across operating systems.

## Windows

Run from the repository root:

```powershell
dotnet restore DreadsMashedPatch/DreadsMashedPatch.csproj
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Export-MutagenDecompiled.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Export-XEditSource.ps1
```

The execution-policy option applies only to these processes. If using PowerShell
7, the Linux `pwsh` commands work on Windows too.

## Outputs and repeat runs

`Export-MutagenDecompiled.ps1` reads package versions and package cache locations
from `DreadsMashedPatch/obj/project.assets.json`. It installs ILSpy into `.tools/`
if absent, then exports each Mutagen assembly into `DecompiledMutagen/<assembly>/`.
The default assembly framework is `net10.0`, matching the current dependencies.
Custom NuGet cache locations are supported through the restored assets file.

Each run replaces the selected Mutagen output directory. The script checks the
input assemblies and verifies that ILSpy starts before removing existing output.
A failure during decompilation can leave a partial export; correct the reported
problem and rerun. Successful exports contain, among other files:

```text
DecompiledMutagen/Mutagen.Bethesda.Skyrim/Mutagen/Bethesda/Skyrim/
```

`Export-XEditSource.ps1` fetches a shallow Git checkout into `XEditSource/`, pinned
to commit `93cc0bc5a1251936c3c7859eee3150eda12a62d7`. It downloads the original
xEdit source rather than decompiling or building xEdit. Repeated runs fetch and
check out the pinned revision again. Keep local edits out of this reference
checkout. Verify the result with:

```bash
git -C XEditSource rev-parse HEAD
```

The checkout includes `XEditSource/Core/wbDefinitionsTES5.pas`.

## Overrides and troubleshooting

- Missing assets file or missing cached assembly: rerun the core project restore.
- ILSpy fails to start: check that the .NET 8 runtime is installed.
- No assemblies for the selected framework: use `-Framework net10.0` for the
  current packages. After future dependency upgrades, select a framework actually
  present in the restored packages; the exporter rejects empty exports.
- To inspect another assets file, pass `-AssetsFile <path>` to the Mutagen script.
- To use another xEdit revision, pass `-Revision <commit>` to the xEdit script.
  The default revision is the one used by this project's reference audits.
- Both scripts accept `-OutputRoot <repository-relative-directory>`. For Mutagen,
  use a dedicated disposable directory: its contents are replaced. Output must
  remain inside the repository. Custom output folders may need their own ignore
  rules; the defaults are already ignored.

## Regenerating coverage reports

The coverage reports under `docs/` are eligible for version control. Regenerate
them separately from the references:

```bash
pwsh -NoProfile -File scripts/Audit-RecordHandlerCoverage.ps1
```

On Windows PowerShell, use `powershell -NoProfile -ExecutionPolicy Bypass -File`
instead. Add `-FailOnUnresolved` for strict classification checks. Review both
`docs/record-handler-coverage.md` and `docs/record-handler-coverage.json` before
committing them. The static scanner can detect commented-out registrations;
confirm executable dictionaries and dispatch before documenting support.
For collection manifest regeneration and verification, see
[Collection semantics](COLLECTION_SEMANTICS.md#workflow).
