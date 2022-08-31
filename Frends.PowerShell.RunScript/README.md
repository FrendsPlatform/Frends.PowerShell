# Frends.PowerShell.Runscript
Run a PowerShell script with parameters, the Task fails when a terminating error is encountered or an error is thrown.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Build](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/Runscript_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.PowerShell/actions)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.PowerShell.Runscript)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.PowerShell/Frends.PowerShell.Runscript|main)

# Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed https://www.myget.org/F/frends-tasks/api/v2.

## Building

Rebuild the project

`dotnet build`

Run tests

Tests will change the PowerShell execution policy for the current user as unrestricted.

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`