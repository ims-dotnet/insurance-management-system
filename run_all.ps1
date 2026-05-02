# run_all.ps1
$CONSOLE_LOG_DIR = "logs/console"
if (-not (Test-Path $CONSOLE_LOG_DIR)) {
    New-Item -ItemType Directory -Force -Path $CONSOLE_LOG_DIR | Out-Null
}

Write-Host "Starting InsureTrust Microservices Architecture..." -ForegroundColor Cyan

# Check for existing dotnet processes and offer to kill them
$existingProcesses = Get-Process dotnet -ErrorAction SilentlyContinue
if ($existingProcesses) {
    Write-Host "⚠️  Existing dotnet processes detected. Cleaning up..." -ForegroundColor Yellow
    $existingProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
}

$services = @(
    @{ Name = "Gateway"; Path = "gateways/InsureTrust.Gateway" },
    @{ Name = "IdentityService"; Path = "services/InsureTrust.IdentityService" },
    @{ Name = "NotificationService"; Path = "services/InsureTrust.NotificationService" },
    @{ Name = "ProductService"; Path = "services/InsureTrust.ProductService" },
    @{ Name = "ClaimService"; Path = "services/InsureTrust.ClaimService" },
    @{ Name = "PaymentService"; Path = "services/InsureTrust.PaymentService" },
    @{ Name = "AdminService"; Path = "services/InsureTrust.AdminService" },
    @{ Name = "QueryService"; Path = "services/InsureTrust.QueryService" },
    @{ Name = "CalculatorService"; Path = "services/InsureTrust.CalculatorService" },
    @{ Name = "Frontend"; Path = "frontend/InsureTrust.Web" }
)

$runningProcesses = @()

function Cleanup {
    Write-Host "`nShutting down all services..." -ForegroundColor Red
    foreach ($proc in $runningProcesses) {
        if (-not $proc.HasExited) {
            Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
        }
    }
    exit
}

# Trap Ctrl+C (SIGINT)
# Note: PowerShell handles Ctrl+C automatically in some contexts, 
# but using a try-finally block is often more reliable for scripts.

try {
    foreach ($service in $services) {
        $name = $service.Name
        $path = $service.Path
        
        Write-Host " Starting $name..." -ForegroundColor Green
        
        $stdoutFile = Join-Path $CONSOLE_LOG_DIR "$($name)_stdout.log"
        $stderrFile = Join-Path $CONSOLE_LOG_DIR "$($name)_stderr.log"
        
        # Ensure we have absolute paths for redirection
        $absStdoutFile = [System.IO.Path]::GetFullPath($stdoutFile)
        $absStderrFile = [System.IO.Path]::GetFullPath($stderrFile)

        # Start the process in the background
        $proc = Start-Process dotnet -ArgumentList "run --launch-profile https" `
            -WorkingDirectory $path `
            -NoNewWindow `
            -RedirectStandardOutput $absStdoutFile `
            -RedirectStandardError $absStderrFile `
            -PassThru

        $runningProcesses += $proc
        Start-Sleep -Seconds 1
    }

    Write-Host "`n All services initiated. Logs (stdout/stderr) are in: $CONSOLE_LOG_DIR" -ForegroundColor Cyan
    Write-Host "`n --- SERVICE DASHBOARD ---" -ForegroundColor White
    Write-Host "Frontend:        https://localhost:7282"
    Write-Host "Gateway Hub:     https://localhost:7099"
    Write-Host "----------------------------------------"
    Write-Host "Identity:        https://localhost:7001"
    Write-Host "Product:         https://localhost:7296"
    Write-Host "Notification:    https://localhost:7003"
    Write-Host "Payment:         https://localhost:7004"
    Write-Host "Claim:           https://localhost:7005"
    Write-Host "Admin:           https://localhost:7006"
    Write-Host "Query:           https://localhost:7007"
    Write-Host "Calculator:      https://localhost:7008"
    Write-Host "----------------------------------------"
    Write-Host "Press Ctrl+C to stop all services."

    # Keep the script running
    while ($true) {
        Start-Sleep -Seconds 1
    }
}
finally {
    Cleanup
}
