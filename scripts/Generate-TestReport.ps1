param(
    [string]$TrxFolder = [System.IO.Path]::Combine($PSScriptRoot, "..", "src", "Tests"),
    [string]$OutputHtml = [System.IO.Path]::Combine($PSScriptRoot, "..", "TestResults", "Dashboard.html")
)

$historyFile = [System.IO.Path]::Combine($PSScriptRoot, "..", "TestResults", "history.json")
if (-not (Test-Path $historyFile)) {
    Write-Host "Arquivo de histórico não encontrado em $historyFile"
    exit
}

$history = Get-Content $historyFile | ConvertFrom-Json

function Get-ProjectData($projectName) {
    $dates = @()
    $passed = @()
    $failed = @()
    foreach ($run in $history) {
        $dates += $run.Date
        $proj = $run.Results | Where-Object { $_.File -like "*$projectName*" }
        if ($proj) {
            $passed += [int]$proj.Passed
            $failed += [int]$proj.Failed
        } else {
            $passed += 0
            $failed += 0
        }
    }
    return @{ Dates = $dates; Passed = $passed; Failed = $failed }
}

$infra = Get-ProjectData "Infra"
$app = Get-ProjectData "Application"
$integration = Get-ProjectData "Integration"
$unit = Get-ProjectData "Unit"

$totalPassed = ($infra.Passed + $app.Passed + $integration.Passed + $unit.Passed | Measure-Object -Sum).Sum
$totalFailed = ($infra.Failed + $app.Failed + $integration.Failed + $unit.Failed | Measure-Object -Sum).Sum
$totalTests = $totalPassed + $totalFailed
$successRate = if ($totalTests -gt 0) { [math]::Round(($totalPassed / $totalTests) * 100, 2) } else { 0 }

$html = @"
<html>
<head>
    <meta charset="UTF-8">
    <title>Relatório de Testes - SJInovacao.Acesso</title>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
    body { font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; background: #f8f9fa; color: #212529; }
    h1 { background-color: #0d6efd; color: white; padding: 15px; border-radius: 8px; }
    .chart-row { display: flex; flex-wrap: wrap; justify-content: center; gap: 20px; margin-bottom: 40px; }
    .chart-container { flex: 1 1 45%; max-width: 45%; min-width: 400px; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.08); }
    .summary { font-size: 18px; margin-bottom: 30px; background: #fff; padding: 15px; border-radius: 8px; }
    @media (prefers-color-scheme: dark) {
        body { background: #121212; color: #e9ecef; }
        .chart-container { background: #1e1e1e; }
        .summary { background: #1e1e1e; }
    }
</style>
</head>
<body>
    <h1>📊 Histórico de Execuções de Testes</h1>
    <div class="summary">
        <strong>Taxa de sucesso geral:</strong> $successRate% <br>
        <strong>Total de testes:</strong> $totalTests <br>
        <strong>Passaram:</strong> $totalPassed <br>
        <strong>Falharam:</strong> $totalFailed
    </div>
    <div class="chart-row">
        <div class="chart-container"><canvas id="overallChart"></canvas></div>
        <div class="chart-container"><canvas id="infraChart"></canvas></div>
    </div>
    <div class="chart-row">
        <div class="chart-container"><canvas id="appChart"></canvas></div>
        <div class="chart-container"><canvas id="integrationChart"></canvas></div>
    </div>
    <div class="chart-row">
        <div class="chart-container"><canvas id="unitChart"></canvas></div>
    </div>
    <script>
        const labels = [$(($infra.Dates | ForEach-Object { "'$_'" }) -join ",")];
        new Chart(document.getElementById('overallChart'), {
            type: 'doughnut',
            data: {
                labels: ['Passaram', 'Falharam'],
                datasets: [{ data: [$totalPassed, $totalFailed], backgroundColor: ['rgba(75,192,192,0.7)', 'rgba(255,99,132,0.7)'] }]
            },
            options: { plugins: { title: { display: true, text: 'Resumo Geral de Sucesso' } }, responsive: true }
        });
        function createChart(id, title, passed, failed, color) {
            new Chart(document.getElementById(id), {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        { label: 'Passou', data: passed, backgroundColor: color },
                        { label: 'Falhou', data: failed, backgroundColor: 'rgba(255,99,132,0.6)' }
                    ]
                },
                options: { plugins: { title: { display: true, text: title } }, responsive: true, scales: { y: { beginAtZero: true } } }
            });
        }
        createChart('infraChart', 'InfraTests', [$(($infra.Passed -join ","))], [$(($infra.Failed -join ","))], 'rgba(54,162,235,0.6)');
        createChart('appChart', 'ApplicationTests', [$(($app.Passed -join ","))], [$(($app.Failed -join ","))], 'rgba(75,192,192,0.6)');
        createChart('integrationChart', 'IntegrationTests', [$(($integration.Passed -join ","))], [$(($integration.Failed -join ","))], 'rgba(153,102,255,0.6)');
        createChart('unitChart', 'UnitTests', [$(($unit.Passed -join ","))], [$(($unit.Failed -join ","))], 'rgba(255,206,86,0.6)');
    </script>
</body>
</html>
"@

[System.Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$htmlUtf8 = [System.Text.Encoding]::UTF8.GetString([System.Text.Encoding]::Default.GetBytes($html))
[System.IO.File]::WriteAllText($OutputHtml, $htmlUtf8, $utf8NoBom)

Write-Host "Dashboard gerado em $OutputHtml"