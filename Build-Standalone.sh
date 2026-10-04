#!/usr/bin/env bash
set -euo pipefail

repository_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
cd "$repository_root"

solution_path="$repository_root/DreadsMashedPatch.sln"
app_project_path="$repository_root/DreadsMashedPatch.App/DreadsMashedPatch.App.csproj"
test_project_path="$repository_root/DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj"
publish_directory="$repository_root/artifacts/DreadsMashedPatch-win-x64"

dotnet restore "$solution_path"
dotnet build "$solution_path" -c Release --no-restore
dotnet test "$test_project_path" -c Release --no-build --no-restore
dotnet publish "$app_project_path" -c Release -r win-x64 --self-contained true --no-restore -o "$publish_directory"

executable_path="$publish_directory/DreadsMashedPatch.exe"
if [[ ! -f "$executable_path" ]]; then
    printf 'Publish completed without producing %s\n' "$executable_path" >&2
    exit 1
fi

printf 'Standalone build completed: %s\n' "$executable_path"
