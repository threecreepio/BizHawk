#!/bin/sh
if [ -z "$NUGET_PACKAGES" ]; then export NUGET_PACKAGES="$HOME/.nuget/packages"; fi
cd "$(dirname "$0")/.." && dotnet build EmuHawkMono.sln -c Debug -m "$@"
