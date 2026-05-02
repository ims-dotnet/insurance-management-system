# Update-Databases.ps1
# Script to update all InsureTrust microservice databases

$services = @(
    "services/InsureTrust.IdentityService",
    "services/InsureTrust.ProductService",
    "services/InsureTrust.ClaimService",
    "services/InsureTrust.PaymentService",
    "services/InsureTrust.QueryService"
)

Write-Host "--- InsureTrust Database Update Utility ---" -ForegroundColor Cyan
Write-Host "Scanning services for Entity Framework migrations..." -ForegroundColor Gray

# Check for dotnet-ef tool
$efCheck = dotnet ef --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: 'dotnet-ef' tool is not installed or not in PATH." -ForegroundColor Red
    Write-Host "Please install it using: dotnet tool install --global dotnet-ef" -ForegroundColor Yellow
    exit 1
}

foreach ($path in $services) {
    if (Test-Path "$path") {
        Write-Host "`nProcessing: $path" -ForegroundColor Yellow
        
        # Check if Migrations folder exists
        if (Test-Path "$path/Migrations") {
            Write-Host "  -> Migrations found. Updating database..." -ForegroundColor Gray
            
            # Navigate to service directory
            Push-Location "$path"
            
            try {
                # Run the update
                # --no-build is used to speed up if already built, but let's allow build for safety
                dotnet ef database update
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "  [SUCCESS] Database updated for $(Split-Path $path -Leaf)" -ForegroundColor Green
                } else {
                    Write-Host "  [FAILURE] Database update failed for $(Split-Path $path -Leaf)" -ForegroundColor Red
                }
            } catch {
                Write-Host "  [ERROR] An unexpected error occurred in $path" -ForegroundColor Red
            }
            
            Pop-Location
        } else {
            Write-Host "  [SKIP] No Migrations folder found in $path." -ForegroundColor DarkGray
        }
    } else {
        Write-Host "`n[!] Path not found: $path" -ForegroundColor DarkRed
    }
}

Write-Host "`n-------------------------------------------" -ForegroundColor Cyan
Write-Host "Database update sequence completed." -ForegroundColor Cyan
