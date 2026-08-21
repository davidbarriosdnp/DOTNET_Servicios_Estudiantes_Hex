# Este script crea la tabla RefreshTokens en la instancia local de DynamoDB (puerto 8000)

aws dynamodb create-table `
    --table-name RefreshTokens `
    --attribute-definitions AttributeName=TokenHash,AttributeType=S `
    --key-schema AttributeName=TokenHash,KeyType=HASH `
    --billing-mode PAY_PER_REQUEST `
    --endpoint-url http://localhost:8000 `
    --region us-east-1

Write-Host "Tabla 'RefreshTokens' creada exitosamente en DynamoDB Local."
