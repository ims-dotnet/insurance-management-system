#!/bin/bash

echo "🚀 Starting database updates for all microservices..."

services=(
    "services/InsureTrust.IdentityService"
    "services/InsureTrust.NotificationService"
    "services/InsureTrust.ProductService"
    "services/InsureTrust.ClaimService"
    "services/InsureTrust.PaymentService"
    "services/InsureTrust.QueryService"
)

for service in "${services[@]}"; do
    echo "----------------------------------------------------"
    echo "Updating database for: $service"
    dotnet ef database update --project "$service"
done

echo "----------------------------------------------------"
echo "✅ All databases updated successfully!"
