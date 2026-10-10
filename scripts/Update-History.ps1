param(
    [string]$TrxFolder = [System.IO.Path]::Combine($PSScriptRoot, "..", "TestResults"),
    [string]$HistoryFile = [System.IO.Path]::Combine($PSScriptRoot, "..", "TestResults", "history.json")
)

function Get-TestSummary($trxFile) {
    [xml]$xml = Get-Content $trxFile
    $counters = $xml.TestRun.ResultSummary.Counters
    return @{
        File = (Split-Path $trxFile -Leaf)
        Total = $counters.total
        Passed = $counters.passed
        Failed = $counters.failed
    }
}

$trxFiles = Get-ChildItem -Path $TrxFolder -Recurse -Filter *.trx

$results = @()
foreach ($file in $trxFiles) {
    $results += Get-TestSummary $file.FullName
}

$currentRun = @{
    Date = (Get-Date).ToString("yyyy-MM-dd HH:mm")
    Results = $results
}

if (Test-Path $HistoryFile) {
    $history = Get-Content $HistoryFile | ConvertFrom-Json
    if (-not ($history -is [System.Collections.IList])) {
        $history = @($history)
    }
} else {
    $history = @()
}

$history += $currentRun

$history | ConvertTo-Json -Depth 5 | Out-File $HistoryFile -Encoding utf8

Write-Host "Histórico atualizado em $HistoryFile"