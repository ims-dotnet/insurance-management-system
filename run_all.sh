#!/bin/bash

CONSOLE_LOG_DIR="logs/console"
mkdir -p "$CONSOLE_LOG_DIR"

echo "Starting InsureTrust Microservices Architecture..."

# Check for existing dotnet processes and offer to kill them
if pgrep -x "dotnet" > /dev/null; then
    echo "⚠️  Existing dotnet processes detected. Cleaning up..."
    pkill -x "dotnet"
    sleep 2
fi

services=(
    "Gateway:gateways/InsureTrust.Gateway"
    "IdentityService:services/InsureTrust.IdentityService"
    "NotificationService:services/InsureTrust.NotificationService"
    "ProductService:services/InsureTrust.ProductService"
    "ClaimService:services/InsureTrust.ClaimService"
    "PaymentService:services/InsureTrust.PaymentService"
    "AdminService:services/InsureTrust.AdminService"
    "QueryService:services/InsureTrust.QueryService"
    "CalculatorService:services/InsureTrust.CalculatorService"
    "Frontend:frontend/InsureTrust.Web"
)

cleanup() {
    echo -e "\n Shutting down all services..."
    pkill -P $$
    exit
}

trap cleanup SIGINT SIGTERM

for entry in "${services[@]}"; do
    name="${entry%%:*}"
    path="${entry#*:}"
    
    echo " Starting $name..."
    log_file="$CONSOLE_LOG_DIR/${name}_startup.log"
    
    # Force use of HTTPS profile to match the dashboard ports
    (cd "$path" && dotnet run --launch-profile https > "../../$log_file" 2>&1) &
    
    sleep 1 
done

echo -e "\n All services initiated. Logs: $CONSOLE_LOG_DIR"
echo -e "\n --- SERVICE DASHBOARD ---"
echo "Frontend:        https://localhost:7282"
echo "Gateway Hub:     https://localhost:7099"
echo "----------------------------------------"
echo "Identity:        https://localhost:7001"
echo "Product:         https://localhost:7296"
echo "Notification:    https://localhost:7003"
echo "Payment:         https://localhost:7004"
echo "Claim:           https://localhost:7005"
echo "Admin:           https://localhost:7006"
echo "Query:           https://localhost:7007"
echo "Calculator:      https://localhost:7008"
----------------------------------------
echo "Press Ctrl+C to stop all services."

# Keep the script running
wait
