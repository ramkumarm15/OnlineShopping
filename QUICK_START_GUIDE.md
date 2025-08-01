# Quick Start Guide - Thrift Shop API

## Get Started in 5 Minutes

This guide will get you up and running with the Thrift Shop API in just a few minutes.

## Prerequisites

- Windows/Mac/Linux
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server)

## Step 1: Clone and Setup

```bash
# Navigate to the OnlineShopping project directory
cd OnlineShopping

# Restore packages
dotnet restore

# Update database connection string in appsettings.json if needed
# Default points to: Server=LTIN121778;Database=onlineshopping;Trusted_Connection=True;
```

## Step 2: Setup Database

```bash
# Install EF tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create and update database
dotnet ef database update
```

## Step 3: Run the API

```bash
# Start the API
dotnet run

# API will be available at:
# https://localhost:7207
# http://localhost:5207
```

## Step 4: Test the API

### Option A: Use Swagger UI (Recommended)

1. Open your browser and go to: `https://localhost:7207/swagger`
2. You'll see interactive API documentation
3. Test endpoints directly from the browser

### Option B: Use cURL

```bash
# 1. Register a new user
curl -X POST "https://localhost:7207/api/Users" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser123",
    "password": "password123",
    "name": "Test User",
    "emailAddress": "test@example.com",
    "about": "Testing the API",
    "city": "Test City"
  }'

# 2. Login to get authentication token
curl -X POST "https://localhost:7207/api/Authentication/CreateToken" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser123",
    "password": "password123"
  }'

# Copy the "accessToken" from the response

# 3. Get your profile (replace YOUR_TOKEN with the actual token)
curl -X GET "https://localhost:7207/api/Users/GetMe" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 4. View products (no auth required)
curl -X GET "https://localhost:7207/api/Product"
```

## Quick API Test Script

Save this as `test-api.sh` (Linux/Mac) or `test-api.bat` (Windows):

```bash
#!/bin/bash

API_BASE="https://localhost:7207"

echo "🚀 Testing Thrift Shop API..."

# Register user
echo "📝 Registering new user..."
curl -s -X POST "$API_BASE/api/Users" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "quicktest123",
    "password": "password123",
    "name": "Quick Test User",
    "emailAddress": "quicktest@example.com",
    "about": "Quick test user",
    "city": "Test City"
  }' | echo "User registration response received"

# Login
echo "🔐 Logging in..."
TOKEN_RESPONSE=$(curl -s -X POST "$API_BASE/api/Authentication/CreateToken" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "quicktest123",
    "password": "password123"
  }')

TOKEN=$(echo $TOKEN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  echo "❌ Login failed"
  exit 1
fi

echo "✅ Login successful! Token received."

# Get profile
echo "👤 Getting user profile..."
curl -s -X GET "$API_BASE/api/Users/GetMe" \
  -H "Authorization: Bearer $TOKEN" | echo "Profile retrieved"

# Get products
echo "🛍️ Getting products..."
curl -s -X GET "$API_BASE/api/Product" | echo "Products retrieved"

echo "✅ API test completed successfully!"
```

Make it executable and run:
```bash
chmod +x test-api.sh
./test-api.sh
```

## Common Endpoints Quick Reference

| Action | Method | Endpoint | Auth Required |
|--------|---------|----------|---------------|
| Register | POST | `/api/Users` | No |
| Login | POST | `/api/Authentication/CreateToken` | No |
| Get Profile | GET | `/api/Users/GetMe` | Yes |
| List Products | GET | `/api/Product` | No |
| Get Cart | GET | `/api/Cart` | Yes |
| Add to Cart | POST | `/api/CartItem` | Yes |
| Get Addresses | GET | `/api/BillingAddress` | Yes |

## Sample Data Payloads

### Register User
```json
{
  "username": "youruser123",
  "password": "yourpassword",
  "name": "Your Name",
  "emailAddress": "your@email.com",
  "about": "Your bio",
  "city": "Your City"
}
```

### Login
```json
{
  "username": "youruser123",
  "password": "yourpassword"
}
```

### Add to Cart
```json
{
  "data": {
    "productId": 1,
    "quantity": 2
  },
  "operation": "add"
}
```

### Create Billing Address
```json
{
  "billingName": "Your Name",
  "address1": "123 Main St",
  "address2": "Apt 1",
  "city": "Your City",
  "state": "Your State",
  "postalCode": 12345,
  "mobileNumber": 1234567890,
  "defaultAddress": true
}
```

## Development Tips

### Hot Reload Development
```bash
# Use watch for automatic restarts during development
dotnet watch run
```

### Check Logs
Logs are stored in the `logs` directory with daily rotation:
```
logs/log_20241225.txt
```

### Environment Configuration
Create `appsettings.Development.json` for local settings:
```json
{
  "ConnectionStrings": {
    "database": "Your local connection string"
  },
  "Jwt": {
    "Key": "your-development-secret-key"
  }
}
```

## Troubleshooting

### Database Issues
```bash
# If database connection fails, check:
1. SQL Server is running
2. Connection string in appsettings.json is correct
3. Database exists (run: dotnet ef database update)
```

### SSL Certificate Issues
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```

### Port Already in Use
```bash
# Check what's using the port
netstat -ano | findstr :7207

# Or run on different port
dotnet run --urls="https://localhost:7208"
```

## Next Steps

1. **Read Full Documentation**: Check `API_DOCUMENTATION.md` for complete API reference
2. **Developer Guide**: See `DEVELOPER_GUIDE.md` for technical details
3. **Create Admin User**: Manually set a user's role to "Admin" in the database for admin features
4. **Add Sample Data**: Create some products through the admin endpoints
5. **Build Frontend**: Use the API with your favorite frontend framework

## Need Help?

- **Swagger UI**: `https://localhost:7207/swagger` - Interactive API documentation
- **Logs**: Check the `logs` directory for detailed application logs
- **Database**: Use SQL Server Management Studio to inspect the database
- **Postman Collection**: Import the API endpoints into Postman for testing

---

🎉 **You're ready to start building with the Thrift Shop API!**

For advanced usage, architecture details, and development guidelines, see the complete documentation files.