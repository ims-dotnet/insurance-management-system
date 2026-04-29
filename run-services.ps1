$projects = @(
    "frontend\InsureTrust.Web",
    "services\InsureTrust.IdentityService",
    "services\InsureTrust.CalculatorService"
)

foreach ($project in $projects) {
    Write-Host "Starting $project..."
    Start-Process "dotnet" -ArgumentList "run --project $project" -NoNewWindow:$false
}

Write-Host "All specified services have been started in new windows!"
