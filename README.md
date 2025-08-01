# 🛍️ Thrift Shop API

A comprehensive e-commerce API built with ASP.NET Core 6.0, featuring user management, product catalog, shopping cart functionality, and secure JWT authentication.

[![.NET](https://img.shields.io/badge/.NET-6.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/6.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](#)
[![API](https://img.shields.io/badge/API-REST-orange.svg)](#)

## 🚀 Features

- **🔐 Authentication & Authorization**: JWT-based authentication with role-based access control
- **👥 User Management**: Complete user registration, profile management, and authentication
- **🛒 Shopping Cart**: Full shopping cart functionality with add, update, and remove operations
- **📦 Product Catalog**: Product management with CRUD operations (admin-only)
- **📍 Address Management**: Billing address management with default address support
- **📊 Comprehensive Logging**: Custom file-based logging with daily rotation
- **📚 API Documentation**: Swagger/OpenAPI documentation with interactive testing
- **🧪 Unit Testing**: Comprehensive test coverage with xUnit framework
- **🐳 Docker Support**: Containerized deployment ready

## 📋 Quick Start

Get up and running in 5 minutes! See our [Quick Start Guide](QUICK_START_GUIDE.md) for immediate setup.

```bash
# 1. Navigate to project
cd OnlineShopping

# 2. Restore packages
dotnet restore

# 3. Setup database
dotnet ef database update

# 4. Run the API
dotnet run

# 5. Open Swagger UI
# https://localhost:7207/swagger
```

## 📖 Documentation

### 📚 Complete API Reference
**[API Documentation](API_DOCUMENTATION.md)** - Comprehensive guide covering all endpoints, data models, authentication, and examples.

**What's included:**
- All API endpoints with request/response examples
- Complete data model documentation
- Authentication and authorization guide
- Error handling and status codes
- Real-world usage examples
- Setup and configuration instructions

### 🔧 Developer Guide
**[Developer Guide](DEVELOPER_GUIDE.md)** - Technical documentation for developers working with the codebase.

**What's included:**
- Architecture overview and design patterns
- Code structure and organization
- Database design and relationships
- Testing strategies and examples
- Development environment setup
- Client library examples (C#, JavaScript, Python)
- Troubleshooting and performance tips

### ⚡ Quick Start Guide
**[Quick Start Guide](QUICK_START_GUIDE.md)** - Get the API running in minutes with step-by-step instructions.

**What's included:**
- 5-minute setup process
- Essential endpoints and payloads
- Test scripts and examples
- Common troubleshooting
- Development tips

## 🏗️ Architecture

### Technology Stack
- **Framework**: ASP.NET Core 6.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, Moq
- **Logging**: Custom file-based logging

### Project Structure
```
├── OnlineShopping/              # Main API Project
│   ├── Controllers/             # API Controllers
│   ├── Models/                  # Data Models & DTOs
│   ├── Repository/              # Repository Pattern Implementation
│   ├── Logger/                  # Custom Logging
│   ├── Migrations/              # EF Core Migrations
│   └── Program.cs               # Application Entry Point
├── OnlineShopping.Tests/        # Unit Tests
├── API_DOCUMENTATION.md         # Complete API Reference
├── DEVELOPER_GUIDE.md           # Technical Documentation
├── QUICK_START_GUIDE.md         # Quick Setup Guide
└── README.md                    # This file
```

## 🔑 Key Components

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control (User/Admin)
- Secure password handling
- Token expiration and refresh

### Core Entities
- **Users**: User management with profiles and roles
- **Products**: Product catalog with admin management
- **Cart & CartItems**: Shopping cart functionality
- **BillingAddresses**: Address management with default support

### API Endpoints Overview
| Component | Endpoints | Description |
|-----------|-----------|-------------|
| Authentication | `/api/Authentication/CreateToken` | JWT token generation |
| Users | `/api/Users/*` | User registration, profile management |
| Products | `/api/Product/*` | Product catalog (public read, admin write) |
| Cart | `/api/Cart/*` | Shopping cart management |
| CartItems | `/api/CartItem/*` | Cart item operations |
| Billing | `/api/BillingAddress/*` | Address management |

## 🧪 Testing

The project includes comprehensive unit tests covering:
- Controller behavior and HTTP responses
- Repository pattern implementation
- Authentication and authorization
- Data validation and business logic

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter "ControllerTests"
```

## 🐳 Docker Support

```bash
# Build and run with Docker
docker build -t thrift-shop-api .
docker run -p 8080:80 thrift-shop-api

# Or use Docker Compose
docker-compose up
```

## 🔒 Security Features

- **JWT Authentication**: Secure token-based authentication
- **Role-based Authorization**: Separate permissions for users and admins
- **Password Security**: Passwords are hidden in API responses
- **Input Validation**: Comprehensive data validation using Data Annotations
- **CORS Configuration**: Controlled cross-origin access
- **SQL Injection Prevention**: Entity Framework ORM protection

## 📊 Logging

The API includes comprehensive logging with:
- File-based logging with daily rotation
- Configurable log levels
- Request/response logging
- Error tracking and debugging information

Logs are stored in the `logs` directory:
```
logs/log_20241225.txt
```

## 🌐 API Clients

Example clients are provided for:
- **C#/.NET**: HttpClient-based wrapper
- **JavaScript/TypeScript**: Fetch API implementation
- **Python**: Requests library implementation

See the [Developer Guide](DEVELOPER_GUIDE.md) for complete client examples.

## 🚀 Deployment

### Local Development
```bash
dotnet watch run  # Hot reload development
```

### Production
```bash
dotnet publish -c Release
```

### Environment Configuration
- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Development overrides
- `appsettings.Production.json`: Production settings

## 📈 Performance

### Database Optimization
- Entity Framework Core with optimized queries
- Proper indexing on key fields
- Repository pattern for data access

### API Performance
- Async/await throughout the application
- Response compression ready
- Caching strategies documented

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📝 API Usage Examples

### Register and Login
```bash
# Register
curl -X POST "https://localhost:7207/api/Users" \
  -H "Content-Type: application/json" \
  -d '{"username":"user123","password":"pass123","name":"Test User","emailAddress":"test@example.com","about":"Test user","city":"Test City"}'

# Login
curl -X POST "https://localhost:7207/api/Authentication/CreateToken" \
  -H "Content-Type: application/json" \
  -d '{"username":"user123","password":"pass123"}'
```

### Shopping Flow
```bash
# View products
curl -X GET "https://localhost:7207/api/Product"

# Add to cart (requires auth token)
curl -X POST "https://localhost:7207/api/CartItem" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"data":{"productId":1,"quantity":2},"operation":"add"}'

# View cart
curl -X GET "https://localhost:7207/api/Cart" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## 🆘 Support

### Documentation
- **Complete API Reference**: [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- **Developer Guide**: [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md)
- **Quick Start**: [QUICK_START_GUIDE.md](QUICK_START_GUIDE.md)

### Interactive Testing
- **Swagger UI**: `https://localhost:7207/swagger`
- **Logs**: Check the `logs` directory for detailed application logs

### Common Issues
- Database connection problems → Check connection string in `appsettings.json`
- Authentication errors → Verify JWT configuration
- SSL certificate issues → Run `dotnet dev-certs https --trust`

## 📄 License

This project is licensed under the MIT License. See the LICENSE file for details.

## 🏷️ Version History

- **v1.0**: Initial release with core e-commerce functionality
- JWT authentication and authorization
- User management and profiles
- Product catalog management
- Shopping cart functionality
- Billing address management
- Comprehensive logging
- Unit test coverage

## 🎯 Roadmap

Future enhancements may include:
- Order management and checkout process
- Payment integration
- Product reviews and ratings
- Advanced search and filtering
- Email notifications
- Admin dashboard
- API rate limiting
- Redis caching integration

---

## 🔗 Quick Links

- **[📚 API Documentation](API_DOCUMENTATION.md)** - Complete API reference
- **[🔧 Developer Guide](DEVELOPER_GUIDE.md)** - Technical documentation
- **[⚡ Quick Start](QUICK_START_GUIDE.md)** - 5-minute setup
- **[🔍 Swagger UI](https://localhost:7207/swagger)** - Interactive API testing (when running)

---

*Ready to build amazing e-commerce experiences? Start with our [Quick Start Guide](QUICK_START_GUIDE.md)!*