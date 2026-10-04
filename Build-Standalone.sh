#!/usr/bin/env bash
set -euo pipefail

repository_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
cd "$repository_root"

supports_target_framework() {
    local sdk_version
    sdk_version="$("$1" --version 2>/dev/null)" || return 1
    [[ "$sdk_version" =~ ^([0-9]+)\. ]] && (( BASH_REMATCH[1] >= 10 ))
}

dotnet_command="$(command -v dotnet || true)"
local_dotnet="$repository_root/.tools/dotnet/dotnet"
if [[ -z "$dotnet_command" ]] || ! supports_target_framework "$dotnet_command"; then
    if [[ -x "$local_dotnet" ]] && supports_target_framework "$local_dotnet"; then
        dotnet_command="$local_dotnet"
        export DOTNET_ROOT="$repository_root/.tools/dotnet"
        export PATH="$DOTNET_ROOT:$PATH"
    else
        printf '%s\n' 'This project requires the .NET 10 SDK. Install it and add it to PATH, or install it in .tools/dotnet.' >&2
        exit 1
    fi
fi
printf 'Using .NET SDK %s: %s\n' "$("$dotnet_command" --version)" "$dotnet_command"

solution_path="$repository_root/DreadsMashedPatch.sln"
app_project_path="$repository_root/DreadsMashedPatch.App/DreadsMashedPatch.App.csproj"
test_project_path="$repository_root/DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj"
publish_directory="$repository_root/artifacts/DreadsMashedPatch-win-x64"

"$dotnet_command" restore "$solution_path"
"$dotnet_command" build "$solution_path" -c Release --no-restore
"$dotnet_command" test "$test_project_path" -c Release --no-build --no-restore
"$dotnet_command" publish "$app_project_path" -c Release -r win-x64 --self-contained true --no-restore -o "$publish_directory"

executable_path="$publish_directory/DreadsMashedPatch.exe"
if [[ ! -f "$executable_path" ]]; then
    printf 'Publish completed without producing %s\n' "$executable_path" >&2
    exit 1
fi

printf 'Standalone build completed: %s\n' "$executable_path"
