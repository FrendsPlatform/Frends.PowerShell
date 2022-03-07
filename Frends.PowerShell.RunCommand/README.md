# Frends.PowerShell.RunCommand

[![Frends.PowerShell.RunCommand Main](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/RunCommand_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.PowerShell/actions/workflows/RunCommand_build_and_test_on_main.yml)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.PowerShell.RunCommand?label=NuGet)
![GitHub](https://img.shields.io/github/license/FrendsPlatform/Frends.PowerShell?label=License)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.PowerShell/Frends.PowerShell.RunCommand|main)

Execute a single PowerShell command with parameters, the Task fails when a terminating error is encountered or an error is thrown

## Installing

You can install the task via FRENDS UI Task View or you can find the NuGet package from the following NuGet feed


## Properties

| Property             | Type                               | Description                                                                         | Example                    |
| -------------------- | ---------------------------------- | ----------------------------------------------------------------------------------- | -------------------------- |
| Command              | string                             | The PowerShell command to execute                                                   | `New-TimeSpan`             |
| Parameters           | Array{Name: string, Value: object} | Parameters for the command, provided switch parameters need to have a boolean value | `Name = Hours, Value = 1`  |
| LogInformationStream | bool                               | Whether information stream should be logged or not                                  | 'false'                    |


## Returns

| Property          | Type                               | Description                                                                        | Example                    |
| ----------------- | ---------------------------------- | ---------------------------------------------------------------------------------- | -------------------------- |
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
