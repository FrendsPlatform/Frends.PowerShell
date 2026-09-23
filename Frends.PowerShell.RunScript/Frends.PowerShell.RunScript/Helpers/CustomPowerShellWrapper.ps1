param(
    [Parameter(Mandatory = $true)][string]$Target,
    [Parameter(Mandatory = $true)][string]$ParametersPath,
    [Parameter(Mandatory = $true)][string]$ResultPath,
    [Parameter(Mandatory = $true)][string]$ErrorsPath,
    [Parameter(Mandatory = $true)][string]$InformationPath
)

$parameters = Import-Clixml -LiteralPath $ParametersPath
$errors = @()
$information = @()
$terminatingError = $false

try {
    $allOutput = @(& $Target @parameters *>&1)
    $results = @($allOutput | Where-Object {
        if ($_ -is [System.Management.Automation.ErrorRecord]) {
            $errors += $_
            return $false
        }

        if ($_ -is [System.Management.Automation.InformationRecord]) {
            $information += $_
            return $false
        }

        return $_ -isnot [System.Management.Automation.WarningRecord] -and
            $_ -isnot [System.Management.Automation.VerboseRecord] -and
            $_ -isnot [System.Management.Automation.DebugRecord] -and
            $_ -isnot [System.Management.Automation.ProgressRecord]
    })
}
catch {
    $errors += $_
    $results = @()
    $terminatingError = $true
}

$errorMessages = @($errors | ForEach-Object { "$($_.ScriptStackTrace): $($_.Exception.Message)" })
$informationMessages = @($information | ForEach-Object { "$($_.MessageData)" })

Export-Clixml -InputObject $results -LiteralPath $ResultPath -Depth 100
Export-Clixml -InputObject $errorMessages -LiteralPath $ErrorsPath -Depth 10
Export-Clixml -InputObject $informationMessages -LiteralPath $InformationPath -Depth 10

if ($terminatingError) {
    exit 1
}
