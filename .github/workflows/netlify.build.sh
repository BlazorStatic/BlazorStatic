#!/usr/bin/env bash
set -e

## install .NET
pushd /tmp
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
chmod u+x /tmp/dotnet-install.sh
/tmp/dotnet-install.sh --channel 9.0

## install and run tailwind
wget https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-x64
chmod +x /tmp/tailwindcss-linux-x64
popd
pushd BlazorStaticWebsite
/tmp/tailwindcss-linux-x64 --input ./wwwroot/app.css  --output ./wwwroot/app.min.css  --minify
popd

## run the project to build the static files. Ensure launch profile ASPNETCORE_ENVIRONMENT is set to Release!
dotnet run --launch-profile "netlify" --project ./BlazorStaticWebsite/BlazorStaticWebsite.csproj
