# Frends.PowerShell.Runscript

[![Frends.PowerShell.RunScript Main](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/RunScript_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/RunScript_build_and_test_on_main.yml)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.PowerShell.RunScript?label=NuGet)
 ![GitHub](https://img.shields.io/github/license/FrendsPlatform/Frends.PowerShell?label=License)
 ![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.PowerShell/Frends.PowerShell.RunScript|main)

Run a PowerShell script with parameters, the Task fails when a terminating error is encountered or an error is thrown

## Installing

You can install the task via FRENDS UI Task View or you can find the NuGet package from the following NuGet feed


## Properties

| Property             | Type                               | Description                                                                             | Example                                                  |
| -------------------- | ---------------------------------- | --------------------------------------------------------------------------------------- | -------------------------------------------------------- |
| ReadFromFile         | bool                               | Should the script be read from a file or from the Script parameter                      | `true`                                                   |
| ScriptFilePath       | string                             | Location for the script file                                                            | `F:\myScript.ps1`                                        |
| Script               | string                             | The script to execute                                                                   | `New-TimeSpan -Hours 1 \| convertto-json`                |
| Parameters           | Array{Name: string, Value: object} | Parameters for the script, provided switch parameters need to have a boolean value      | `Name = Hours, Value = 1`                                |
| LogInformationStream | bool                               | Whether information stream should be logged or not                               | 'false'                    |


## Returns

| Property          | Type                               | Description                                                                         | Example                    |
| ----------------- | ---------------------------------- | ----------------------------------------------------------------------------------- | -------------------------- |
| Result | Array{object} |  |  |
| Errors | Array{string} |  |  |
| Log | string |  |  |



## Building

Clone a copy of the repo

`git clone https://github.com/FrendsPlatform/Frends.PowerShell.git`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`
