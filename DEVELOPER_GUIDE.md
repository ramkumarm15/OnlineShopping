# Developer Guide - Thrift Shop API

## Overview

This guide provides comprehensive technical documentation for developers working with the Thrift Shop API. It includes detailed code examples, testing strategies, architecture explanations, and best practices.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Code Structure](#code-structure)
3. [Database Design](#database-design)
4. [Authentication & Authorization](#authentication--authorization)
5. [Testing](#testing)
6. [Development Setup](#development-setup)
7. [API Client Examples](#api-client-examples)
8. [Troubleshooting](#troubleshooting)
9. [Performance Considerations](#performance-considerations)

---

## Architecture Overview

### Technology Stack

- **Framework**: ASP.NET Core 6.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Documentation**: Swagger/OpenAPI
- **Logging**: Custom file-based logging
- **Testing**: xUnit, Moq (as evidenced by test files)

### Project Structure

```
OnlineShopping/
├── Controllers/           # API Controllers
├── Models/               # Data Models
│   ├── DTO/             # Data Transfer Objects
│   └── *.cs             # Entity Models
├── Repository/           # Repository Pattern Implementation
├── Logger/              # Custom Logging
├── Migrations/          # EF Core Migrations
└── Program.cs           # Application Entry Point
```

### Architectural Patterns

1. **Repository Pattern**: Implemented for cart operations
2. **DTO Pattern**: Separation between API contracts and data models
3. **Controller-Service Pattern**: Controllers handle HTTP concerns, business logic in services
4. **Dependency Injection**: Built-in ASP.NET Core DI container

---

## Code Structure

### Controllers

#### Base Controller Structure

All controllers follow this pattern:

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "PolicyName")] // Optional
public class ExampleController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private ResponseType response;
    private ErrorResponse errorResponse;

    public ExampleController(ApplicationDbContext context)
    {
        _context = context;
        response = new ResponseType();
        errorResponse = new ErrorResponse();
    }
}
```

#### Controller Patterns

1. **Authentication Controller**: No authorization required for token generation
2. **User Controller**: User_Admin policy (both users and admins)
3. **Product Controller**: Admin policy for modifications, public for reading
4. **Cart/CartItem Controllers**: User_Admin policy
5. **BillingAddress Controller**: User_Admin policy

### Models and DTOs

#### Entity Models

Entity models represent database tables:

```csharp
public class User
{
    [Key] public int Id { get; set; }
    [Required] public string Username { get; set; }
    [Required] [JsonIgnore] public string Password { get; set; }
    // ... other properties
    
    // Navigation properties
    public List<BillingAddress> BillingAddresses { get; set; }
    [JsonIgnore] public Cart Cart { get; set; }
}
```

#### DTO Models

DTOs control API surface and validation:

```csharp
public class UserDto
{
    [Required]
    [MaxLength(20), MinLength(8)]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    // ... other properties
}
```

### Repository Pattern Implementation

#### Interface Definition

```csharp
public interface ICartRepository
{
    Task<CartResponse> Add(Cart cart, CartItemDto cartItem);
    Task<CartResponse> Remove(Cart cart, CartItemDto cartItem);
    Task<CartResponse> Update(Cart cart, CartItemDto cartItem);
}
```

#### Repository Implementation Highlights

```csharp
public async Task<CartResponse> Add(Cart cart, CartItemDto cartItem)
{
    // Check for existing item
    if (_context.CartItems.Any(ci => ci.Product.Id == cartItem.productId && ci.Cart.CartId == cart.CartId))
    {
        var response = await Update(cart, cartItem);
        return response;
    }

    // Add new item logic
    Product? productToBeAddToCart = await _context.Products
        .Where(p => p.Id == cartItem.productId).FirstOrDefaultAsync();

    int cartItemQuantity = cartItem.quantity > 1 ? cartItem.quantity : 1;
    double totalPriceOfCartItem = cartItemQuantity * productToBeAddToCart.Price;

    CartItems cartItemDb = new CartItems()
    {
        Cart = cart,
        Product = productToBeAddToCart,
        Quantity = cartItemQuantity,
        TotalPrice = totalPriceOfCartItem,
    };

    cart.TotalPrice += totalPriceOfCartItem;
    cart.Updated = DateTime.Now;

    _context.CartItems.Add(cartItemDb);
    _context.Carts.Update(cart);
    await _context.SaveChangesAsync();

    cartResponse.Message = "Product added";
    return cartResponse;
}
```

---

## Database Design

### Entity Relationships

```
User (1) ←→ (1) Cart
User (1) ←→ (*) BillingAddress
Cart (1) ←→ (*) CartItems
Product (1) ←→ (*) CartItems
```

### Key Database Constraints

1. **Users**: Unique username constraint
2. **BillingAddress**: One default address per user
3. **Cart**: One cart per user
4. **Products**: IsActive flag for soft deletion

### Migration History

- `UserRoleAddedToUserModel`: Added role support
- `CountryFieldChangedToStateInBillingModel`: Changed country to state
- `Cart_CartItemsAddedToDb`: Added shopping cart functionality
- `ExtraFieldsAddedToUserModel`: Enhanced user profile

---

## Authentication & Authorization

### JWT Implementation

#### Token Generation

```csharp
private string GenerateToken(User user)
{
    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    List<Claim> claims = new List<Claim>
    {
        new Claim("Id", user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.GivenName, user.Name),
        new Claim(ClaimTypes.Role, user.Role)
    };

    JwtSecurityToken token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

#### Authorization Policies

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User_Admin",
        policy => policy.RequireRole(Roles.User.GetDisplayName(), Roles.Admin.GetDisplayName()));
    options.AddPolicy("Admin", policy => policy.RequireRole(Roles.Admin.GetDisplayName()));
});
```

#### Accessing User Claims

```csharp
int userId = Convert.ToInt32(User.FindFirstValue("Id"));
string username = User.FindFirstValue(ClaimTypes.Name);
string role = User.FindFirstValue(ClaimTypes.Role);
```

---

## Testing

### Unit Test Structure

The project includes comprehensive unit tests. Here's the testing approach:

#### Controller Testing Pattern

```csharp
[Test]
public async Task GetBillingAddresses_WithValidUser_ReturnsOk()
{
    // Arrange
    var userId = 1;
    var mockUser = CreateMockUser(userId);
    var mockAddresses = CreateMockBillingAddresses(userId);
    
    _mockContext.Setup(x => x.BillingAddresses)
        .Returns(CreateMockDbSet(mockAddresses));
    
    // Act
    var result = await _controller.GetBillingAddresses();
    
    // Assert
    Assert.IsInstanceOf<OkObjectResult>(result);
    var okResult = result as OkObjectResult;
    var addresses = okResult.Value as List<BillingAddress>;
    Assert.AreEqual(2, addresses.Count);
}
```

#### Repository Testing

```csharp
[Test]
public async Task Add_NewCartItem_AddsSuccessfully()
{
    // Arrange
    var cart = CreateMockCart();
    var cartItemDto = new CartItemDto { productId = 1, quantity = 2 };
    var product = CreateMockProduct();
    
    // Act
    var result = await _repository.Add(cart, cartItemDto);
    
    // Assert
    Assert.AreEqual("Product added", result.Message);
    _mockContext.Verify(x => x.SaveChangesAsync(), Times.Once);
}
```

### Test Categories

1. **Controller Tests**: HTTP endpoint behavior
2. **Repository Tests**: Data access logic
3. **Authentication Tests**: JWT and authorization
4. **Validation Tests**: DTO validation rules

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "BillingAddressControllerTest"
```

---

## Development Setup

### Local Development Environment

#### 1. Prerequisites Installation

```bash
# Install .NET 6 SDK
winget install Microsoft.DotNet.SDK.6

# Install SQL Server Express
winget install Microsoft.SQLServerExpress

# Install Entity Framework Tools
dotnet tool install --global dotnet-ef
```

#### 2. Database Setup

```bash
# Navigate to project directory
cd OnlineShopping

# Update database connection string in appsettings.json
# Then run migrations
dotnet ef database update
```

#### 3. Configuration

Create `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "database": "Server=(localdb)\\mssqllocaldb;Database=onlineshopping_dev;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "development-secret-key-make-it-long-and-secure-for-development",
    "Issuer": "https://localhost:7207",
    "Audience": "https://localhost:7207"
  }
}
```

#### 4. Running the Application

```bash
# Development mode with hot reload
dotnet watch run

# Standard run
dotnet run

# Production build
dotnet build --configuration Release
```

### Docker Development

#### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OnlineShopping/OnlineShopping.csproj", "OnlineShopping/"]
RUN dotnet restore "OnlineShopping/OnlineShopping.csproj"
COPY . .
WORKDIR "/src/OnlineShopping"
RUN dotnet build "OnlineShopping.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineShopping.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineShopping.dll"]
```

#### Docker Compose for Development

```yaml
version: '3.8'
services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__database=Server=sqlserver;Database=onlineshopping;User Id=sa;Password=YourPassword123;
    depends_on:
      - sqlserver

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

---

## API Client Examples

### C# Client

```csharp
public class ThriftShopApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string _authToken;

    public ThriftShopApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    public async Task<AuthenticationResponse> LoginAsync(string username, string password)
    {
        var loginRequest = new { username, password };
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Authentication/CreateToken", loginRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            _authToken = authResponse.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
            return authResponse;
        }
        
        throw new Exception("Login failed");
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/Product");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Product>>();
    }

    public async Task<Cart> GetCartAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/Cart");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Cart>();
    }

    public async Task<CartResponse> AddToCartAsync(int productId, int quantity)
    {
        var cartPayload = new CartPayload
        {
            Data = new CartItemDto { productId = productId, quantity = quantity },
            Operation = "add"
        };
        
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/CartItem", cartPayload);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CartResponse>();
    }
}

// Usage example
public async Task ExampleUsage()
{
    var client = new ThriftShopApiClient("https://localhost:7207");
    
    // Login
    var authResponse = await client.LoginAsync("username", "password");
    Console.WriteLine($"Logged in as: {authResponse.Username}");
    
    // Get products
    var products = await client.GetProductsAsync();
    Console.WriteLine($"Found {products.Count} products");
    
    // Add to cart
    if (products.Any())
    {
        var response = await client.AddToCartAsync(products.First().Id, 2);
        Console.WriteLine($"Cart update: {response.Message}");
    }
}
```

### JavaScript/TypeScript Client

```javascript
class ThriftShopApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.authToken = null;
    }

    async login(username, password) {
        const response = await fetch(`${this.baseUrl}/api/Authentication/CreateToken`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, password }),
        });

        if (!response.ok) {
            throw new Error('Login failed');
        }

        const authResponse = await response.json();
        this.authToken = authResponse.accessToken;
        return authResponse;
    }

    async getProducts() {
        const response = await fetch(`${this.baseUrl}/api/Product`);
        if (!response.ok) {
            throw new Error('Failed to fetch products');
        }
        return await response.json();
    }

    async getCart() {
        const response = await fetch(`${this.baseUrl}/api/Cart`, {
            headers: {
                'Authorization': `Bearer ${this.authToken}`,
            },
        });
        
        if (!response.ok) {
            throw new Error('Failed to fetch cart');
        }
        return await response.json();
    }

    async addToCart(productId, quantity) {
        const response = await fetch(`${this.baseUrl}/api/CartItem`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.authToken}`,
            },
            body: JSON.stringify({
                data: { productId, quantity },
                operation: 'add',
            }),
        });

        if (!response.ok) {
            throw new Error('Failed to add to cart');
        }
        return await response.json();
    }

    async createUser(userData) {
        const response = await fetch(`${this.baseUrl}/api/Users`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(userData),
        });

        if (!response.ok) {
            throw new Error('Failed to create user');
        }
        return await response.json();
    }
}

// Usage example
async function exampleUsage() {
    const client = new ThriftShopApiClient('https://localhost:7207');
    
    try {
        // Login
        const authResponse = await client.login('username', 'password');
        console.log(`Logged in as: ${authResponse.username}`);
        
        // Get products
        const products = await client.getProducts();
        console.log(`Found ${products.length} products`);
        
        // Add to cart
        if (products.length > 0) {
            const response = await client.addToCart(products[0].id, 2);
            console.log(`Cart update: ${response.message}`);
        }
        
        // Get updated cart
        const cart = await client.getCart();
        console.log(`Cart total: $${cart.totalPrice}`);
        
    } catch (error) {
        console.error('Error:', error.message);
    }
}
```

### Python Client

```python
import requests
import json
from typing import Optional, List, Dict, Any

class ThriftShopApiClient:
    def __init__(self, base_url: str):
        self.base_url = base_url
        self.auth_token: Optional[str] = None
        self.session = requests.Session()

    def login(self, username: str, password: str) -> Dict[str, Any]:
        """Login and get authentication token"""
        url = f"{self.base_url}/api/Authentication/CreateToken"
        payload = {"username": username, "password": password}
        
        response = self.session.post(url, json=payload)
        response.raise_for_status()
        
        auth_response = response.json()
        self.auth_token = auth_response["accessToken"]
        self.session.headers.update({
            "Authorization": f"Bearer {self.auth_token}"
        })
        
        return auth_response

    def get_products(self) -> List[Dict[str, Any]]:
        """Get all products"""
        url = f"{self.base_url}/api/Product"
        response = self.session.get(url)
        response.raise_for_status()
        return response.json()

    def get_cart(self) -> Dict[str, Any]:
        """Get user's cart"""
        url = f"{self.base_url}/api/Cart"
        response = self.session.get(url)
        response.raise_for_status()
        return response.json()

    def add_to_cart(self, product_id: int, quantity: int) -> Dict[str, Any]:
        """Add product to cart"""
        url = f"{self.base_url}/api/CartItem"
        payload = {
            "data": {"productId": product_id, "quantity": quantity},
            "operation": "add"
        }
        
        response = self.session.post(url, json=payload)
        response.raise_for_status()
        return response.json()

    def create_user(self, user_data: Dict[str, Any]) -> Dict[str, Any]:
        """Create new user"""
        url = f"{self.base_url}/api/Users"
        response = self.session.post(url, json=user_data)
        response.raise_for_status()
        return response.json()

    def get_billing_addresses(self) -> List[Dict[str, Any]]:
        """Get user's billing addresses"""
        url = f"{self.base_url}/api/BillingAddress"
        response = self.session.get(url)
        response.raise_for_status()
        return response.json()

# Usage example
def example_usage():
    client = ThriftShopApiClient("https://localhost:7207")
    
    try:
        # Login
        auth_response = client.login("username", "password")
        print(f"Logged in as: {auth_response['username']}")
        
        # Get products
        products = client.get_products()
        print(f"Found {len(products)} products")
        
        # Add to cart
        if products:
            response = client.add_to_cart(products[0]["id"], 2)
            print(f"Cart update: {response['message']}")
        
        # Get updated cart
        cart = client.get_cart()
        print(f"Cart total: ${cart['totalPrice']}")
        
    except requests.RequestException as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    example_usage()
```

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Issues

**Problem**: `SqlException: Cannot open database`

**Solutions**:
```bash
# Check SQL Server service
Get-Service -Name "*SQL*"

# Test connection string
sqlcmd -S "Server=LTIN121778;Database=onlineshopping;Trusted_Connection=True;"

# Update connection string for your environment
# In appsettings.json, change server name to your SQL Server instance
```

#### 2. JWT Token Issues

**Problem**: `401 Unauthorized` responses

**Solutions**:
```csharp
// Check token expiration
var tokenHandler = new JwtSecurityTokenHandler();
var token = tokenHandler.ReadJwtToken(yourTokenString);
Console.WriteLine($"Token expires: {token.ValidTo}");

// Verify token configuration matches between generation and validation
// Check Jwt:Key, Jwt:Issuer, and Jwt:Audience in appsettings.json
```

#### 3. CORS Issues

**Problem**: Browser CORS errors

**Solutions**:
```csharp
// In Program.cs, update CORS policy
builder.Services.AddCors(x =>
{
    x.AddPolicy("EnableCors", build =>
    {
        build.WithOrigins("http://localhost:3000", "http://localhost:4200") // Add your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

#### 4. Validation Errors

**Problem**: `400 Bad Request` with validation errors

**Debug**:
```csharp
// Add model state debugging in controllers
if (!ModelState.IsValid)
{
    var errors = ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
    
    return BadRequest(errors);
}
```

#### 5. Entity Framework Issues

**Problem**: Migration or context issues

**Solutions**:
```bash
# Reset database
dotnet ef database drop
dotnet ef database update

# Create new migration
dotnet ef migrations add "YourMigrationName"

# Check pending migrations
dotnet ef migrations list
```

### Debugging Tips

#### 1. Enable Detailed Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

#### 2. Use Application Insights (Optional)

```csharp
// In Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

#### 3. Custom Exception Handling

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var response = new ErrorResponse
        {
            Message = "An error occurred while processing your request."
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

---

## Performance Considerations

### Database Optimization

#### 1. Query Optimization

```csharp
// Use Include for related data
var user = await _context.Users
    .Include(u => u.BillingAddresses.Where(b => b.Default))
    .FirstOrDefaultAsync(u => u.Id == userId);

// Use projection for specific fields
var productSummaries = await _context.Products
    .Where(p => p.IsActive)
    .Select(p => new { p.Id, p.Name, p.Price })
    .ToListAsync();
```

#### 2. Caching Strategies

```csharp
// Add memory cache
builder.Services.AddMemoryCache();

// Use in controller
public class ProductController : ControllerBase
{
    private readonly IMemoryCache _cache;
    
    public async Task<ActionResult> GetProducts()
    {
        if (!_cache.TryGetValue("products", out List<Product> products))
        {
            products = await _context.Products.Where(x => x.IsActive).ToListAsync();
            _cache.Set("products", products, TimeSpan.FromMinutes(10));
        }
        
        return Ok(products);
    }
}
```

#### 3. Async Best Practices

```csharp
// Always use async/await for database operations
public async Task<IActionResult> GetUser(int id)
{
    var user = await _context.Users.FindAsync(id);
    return Ok(user);
}

// Use ConfigureAwait(false) in library code
var user = await _context.Users.FindAsync(id).ConfigureAwait(false);
```

### API Performance

#### 1. Response Compression

```csharp
// In Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});
```

#### 2. Rate Limiting

```csharp
// Add rate limiting
builder.Services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
```

#### 3. Pagination

```csharp
public async Task<ActionResult> GetProducts(int page = 1, int pageSize = 10)
{
    var products = await _context.Products
        .Where(x => x.IsActive)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
        
    var totalCount = await _context.Products.CountAsync(x => x.IsActive);
    
    return Ok(new 
    {
        Products = products,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
    });
}
```

---

*This developer guide provides comprehensive technical documentation for working with the Thrift Shop API. For API usage documentation, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md).*