param(
    [string]$TrxFolder = ".\src\Tests",
    [string]$HistoryFile = ".\TestResults\history.json"
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

# Busca todos os arquivos .trx nas subpastas
$trxFiles = Get-ChildItem -Path $TrxFolder -Recurse -Filter *.trx

# Cria lista de resultados
$results = @()
foreach ($file in $trxFiles) {
    $results += Get-TestSummary $file.FullName
}

# Cria objeto da execução atual
$currentRun = @{
    Date = (Get-Date).ToString("yyyy-MM-dd HH:mm")
    Results = $results
}

# Carrega histórico existente ou cria lista nova
if (Test-Path $HistoryFile) {
    $history = Get-Content $HistoryFile | ConvertFrom-Json
    if (-not ($history -is [System.Collections.IList])) {
        $history = @($history)
    }
} else {
    $history = @()
}

# Adiciona execução atual ao histórico
$history += $currentRun

# Salva de volta em JSON
$history | ConvertTo-Json -Depth 5 | Out-File $HistoryFile -Encoding utf8

Write-Host "Histórico atualizado em $HistoryFile"
