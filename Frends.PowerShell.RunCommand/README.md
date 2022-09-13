# Frends.PowerShell.RunCommand
Execute a single PowerShell command with parameters, the Task fails when a terminating error is encountered or an error is thrown.

[![Frends.PowerShell.RunCommand Main](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/RunCommand_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/RunCommand_build_and_test_on_main.yml)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.PowerShell.RunCommand?label=NuGet)
![GitHub](https://img.shields.io/github/license/FrendsPlatform/Frends.PowerShell?label=License)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.PowerShell/Frends.PowerShell.RunCommand|main)

## Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-tasks/api/v2.

## Building

Rebuild the project

`dotnet build`

Run tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`