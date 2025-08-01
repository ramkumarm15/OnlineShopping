# Thrift Shop API Documentation

## Overview

The Thrift Shop API is a comprehensive e-commerce platform built with ASP.NET Core 6.0. It provides a complete online shopping experience with user management, product catalog, shopping cart functionality, and billing address management.

**Base URL:** `https://localhost:7207/api` (Development)  
**API Version:** v1  
**Authentication:** JWT Bearer Token  

## Table of Contents

1. [Authentication](#authentication)
2. [API Endpoints](#api-endpoints)
3. [Data Models](#data-models)
4. [Error Handling](#error-handling)
5. [Examples](#examples)
6. [Setup and Configuration](#setup-and-configuration)

## Authentication

The API uses JWT (JSON Web Token) based authentication. Most endpoints require authentication except for user registration and product listing.

### Authentication Flow

1. Register a new user account (no authentication required)
2. Login with username and password to get JWT token
3. Include JWT token in Authorization header for protected endpoints

**Authorization Header Format:**
```
Authorization: Bearer <your-jwt-token>
```

### Roles

- **User**: Can manage their own profile, cart, and billing addresses
- **Admin**: Can manage products (full CRUD operations)

---

## API Endpoints

### Authentication Controller

#### Login and Get Token
Generate JWT token for authenticated user.

**Endpoint:** `POST /api/Authentication/CreateToken`  
**Authentication:** Not required  

**Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe",
  "userId": 1
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Username and Password is incorrect. Try again"
}
```

---

### Users Controller

All user endpoints require authentication with "User_Admin" policy.

#### Get Current User Profile
Get the profile information of the authenticated user.

**Endpoint:** `GET /api/Users/GetMe`  
**Authentication:** Required (User/Admin)  

**Response (200 OK):**
```json
{
  "id": 1,
  "username": "john_doe",
  "name": "John Doe",
  "emailAddress": "john@example.com",
  "about": "Software developer",
  "city": "New York",
  "gender": "Male",
  "age": 30,
  "mobileNumber": 1234567890,
  "role": "User",
  "billingAddresses": [
    {
      "id": 1,
      "billingName": "John Doe",
      "address1": "123 Main St",
      "address2": "Apt 4B",
      "city": "New York",
      "state": "NY",
      "postalCode": 10001,
      "mobileNumber": 1234567890,
      "default": true,
      "created": "2024-01-01T10:00:00Z",
      "updated": "2024-01-01T10:00:00Z"
    }
  ]
}
```

#### Create New User
Register a new user account.

**Endpoint:** `POST /api/Users`  
**Authentication:** Not required  

**Request Body:**
```json
{
  "username": "john_doe",
  "password": "password123",
  "name": "John Doe",
  "emailAddress": "john@example.com",
  "about": "Software developer",
  "city": "New York"
}
```

**Response (200 OK):**
```json
{
  "user": {
    "id": 1,
    "username": "john_doe",
    "name": "John Doe",
    "emailAddress": "john@example.com",
    "about": "Software developer",
    "city": "New York",
    "gender": "NotPreferToSay",
    "age": 0,
    "mobileNumber": 0,
    "role": "User"
  },
  "message": "Profile created successfully"
}
```

#### Update User Profile
Update user profile information.

**Endpoint:** `PUT /api/Users/update`  
**Authentication:** Required (User/Admin)  

**Request Body:**
```json
{
  "name": "John Smith",
  "emailAddress": "johnsmith@example.com",
  "about": "Updated bio",
  "city": "Los Angeles",
  "gender": "Male",
  "age": 31,
  "mobileNumber": 9876543210
}
```

**Response (200 OK):**
```json
{
  "user": {
    "id": 1,
    "username": "john_doe",
    "name": "John Smith",
    "emailAddress": "johnsmith@example.com",
    "about": "Updated bio",
    "city": "Los Angeles",
    "gender": "Male",
    "age": 31,
    "mobileNumber": 9876543210,
    "role": "User"
  },
  "message": "Profile updated"
}
```

#### Update Password
Change user password.

**Endpoint:** `PATCH /api/Users/update/password`  
**Authentication:** Required (User/Admin)  

**Request Body:**
```json
{
  "oldPassword": "oldpassword123",
  "newPassword": "newpassword456",
  "confirmPassword": "newpassword456"
}
```

**Response (200 OK):**
```json
{
  "message": "Password Updated"
}
```

#### Delete User
Delete the current user account.

**Endpoint:** `DELETE /api/Users/delete`  
**Authentication:** Required (User/Admin)  

**Response (200 OK):**
```json
{
  "message": "Deleted user"
}
```

---

### Product Controller

#### Get All Products
Retrieve all active products.

**Endpoint:** `GET /api/Product`  
**Authentication:** Not required  

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Vintage T-Shirt",
    "slug": "vintage-t-shirt",
    "description": "Classic vintage style t-shirt",
    "price": 19.99,
    "image": "https://example.com/image.jpg",
    "isActive": true,
    "createdAt": "2024-01-01T10:00:00Z",
    "updatedAt": "2024-01-01T10:00:00Z"
  }
]
```

#### Get Product by ID
Retrieve a specific product by its ID.

**Endpoint:** `GET /api/Product/{id}`  
**Authentication:** Not required  

**Path Parameters:**
- `id` (integer): Product ID

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Vintage T-Shirt",
  "slug": "vintage-t-shirt",
  "description": "Classic vintage style t-shirt",
  "price": 19.99,
  "image": "https://example.com/image.jpg",
  "isActive": true,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": "2024-01-01T10:00:00Z"
}
```

#### Create Product
Create a new product (Admin only).

**Endpoint:** `POST /api/Product`  
**Authentication:** Required (Admin only)  

**Request Body:**
```json
{
  "name": "New Product",
  "slug": "new-product",
  "description": "Product description",
  "price": 29.99,
  "image": "https://example.com/new-image.jpg"
}
```

**Response (200 OK):**
```json
{
  "message": "Product created"
}
```

#### Update Product
Update an existing product (Admin only).

**Endpoint:** `PUT /api/Product/{id}`  
**Authentication:** Required (Admin only)  

**Path Parameters:**
- `id` (integer): Product ID

**Request Body:**
```json
{
  "name": "Updated Product",
  "slug": "updated-product",
  "description": "Updated description",
  "price": 39.99,
  "image": "https://example.com/updated-image.jpg"
}
```

**Response (200 OK):**
```json
{
  "message": "Product updated"
}
```

#### Delete Product
Delete a product (Admin only).

**Endpoint:** `DELETE /api/Product/{id}`  
**Authentication:** Required (Admin only)  

**Path Parameters:**
- `id` (integer): Product ID

**Response (200 OK):**
```json
{
  "message": "Product deleted"
}
```

---

### Cart Controller

#### Get User Cart
Retrieve the shopping cart for the authenticated user.

**Endpoint:** `GET /api/Cart`  
**Authentication:** Required (User/Admin)  

**Response (200 OK):**
```json
{
  "cartId": 1,
  "totalPrice": 59.98,
  "created": "2024-01-01T10:00:00Z",
  "updated": "2024-01-02T15:30:00Z",
  "userId": 1,
  "cartItemsList": [
    {
      "cartItemsId": 1,
      "product": {
        "id": 1,
        "name": "Vintage T-Shirt",
        "slug": "vintage-t-shirt",
        "description": "Classic vintage style t-shirt",
        "price": 19.99,
        "image": "https://example.com/image.jpg",
        "isActive": true,
        "createdAt": "2024-01-01T10:00:00Z",
        "updatedAt": "2024-01-01T10:00:00Z"
      },
      "quantity": 2,
      "totalPrice": 39.98
    }
  ]
}
```

#### Create Cart
Create a new cart for the authenticated user.

**Endpoint:** `POST /api/Cart`  
**Authentication:** Required (User/Admin)  

**Response (201 Created):**
```json
{
  "cartId": 1,
  "totalPrice": 0,
  "created": "2024-01-01T10:00:00Z",
  "updated": "2024-01-01T10:00:00Z",
  "userId": 1,
  "cartItemsList": []
}
```

---

### Cart Item Controller

#### Manage Cart Items
Add, update, or remove items from the shopping cart.

**Endpoint:** `POST /api/CartItem`  
**Authentication:** Required (User/Admin)  

**Request Body:**
```json
{
  "data": {
    "productId": 1,
    "quantity": 2
  },
  "operation": "add"
}
```

**Operations:**
- `"add"`: Add item to cart (or update quantity if item exists)
- `"update"`: Update item quantity
- `"delete"`: Remove item from cart

**Response (200 OK):**
```json
{
  "message": "Product added"
}
```

---

### Billing Address Controller

#### Get All Billing Addresses
Retrieve all billing addresses for the authenticated user.

**Endpoint:** `GET /api/BillingAddress`  
**Authentication:** Required (User/Admin)  

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "billingName": "John Doe",
    "address1": "123 Main St",
    "address2": "Apt 4B",
    "city": "New York",
    "state": "NY",
    "postalCode": 10001,
    "mobileNumber": 1234567890,
    "default": true,
    "created": "2024-01-01T10:00:00Z",
    "updated": "2024-01-01T10:00:00Z",
    "userId": 1
  }
]
```

#### Get Billing Address by ID
Retrieve a specific billing address.

**Endpoint:** `GET /api/BillingAddress/{id}`  
**Authentication:** Required (User/Admin)  

**Path Parameters:**
- `id` (integer): Billing address ID

**Response (200 OK):**
```json
{
  "id": 1,
  "billingName": "John Doe",
  "address1": "123 Main St",
  "address2": "Apt 4B",
  "city": "New York",
  "state": "NY",
  "postalCode": 10001,
  "mobileNumber": 1234567890,
  "default": true,
  "created": "2024-01-01T10:00:00Z",
  "updated": "2024-01-01T10:00:00Z",
  "userId": 1
}
```

#### Create Billing Address
Create a new billing address.

**Endpoint:** `POST /api/BillingAddress`  
**Authentication:** Required (User/Admin)  

**Request Body:**
```json
{
  "billingName": "John Doe",
  "address1": "123 Main St",
  "address2": "Apt 4B",
  "city": "New York",
  "state": "NY",
  "postalCode": 10001,
  "mobileNumber": 1234567890,
  "defaultAddress": true
}
```

**Response (200 OK):**
```json
{
  "message": "Address added"
}
```

#### Update Billing Address
Update an existing billing address.

**Endpoint:** `PUT /api/BillingAddress/{id}`  
**Authentication:** Required (User/Admin)  

**Path Parameters:**
- `id` (integer): Billing address ID

**Request Body:**
```json
{
  "billingName": "John Smith",
  "address1": "456 Oak Ave",
  "address2": "Suite 200",
  "city": "Los Angeles",
  "state": "CA",
  "postalCode": 90210,
  "mobileNumber": 9876543210,
  "defaultAddress": false
}
```

**Response (200 OK):**
```json
{
  "message": "Address updated"
}
```

#### Delete Billing Address
Delete a billing address.

**Endpoint:** `DELETE /api/BillingAddress/{id}`  
**Authentication:** Required (User/Admin)  

**Path Parameters:**
- `id` (integer): Billing address ID

**Response (200 OK):**
```json
{
  "message": "Address deleted"
}
```

#### Set Default Address
Set a billing address as the default address.

**Endpoint:** `POST /api/BillingAddress/address/set-default`  
**Authentication:** Required (User/Admin)  

**Request Body:**
```json
{
  "addressId": 1
}
```

**Response (200 OK):**
```json
{
  "message": "Default address changed"
}
```

---

## Data Models

### Core Models

#### User
```json
{
  "id": "integer",
  "username": "string (required, 8-20 chars)",
  "password": "string (required, 8-20 chars, hidden in responses)",
  "name": "string (required)",
  "emailAddress": "string (required, valid email)",
  "about": "string (required, max 200 chars)",
  "city": "string (required)",
  "gender": "string (Male/Female/NotPreferToSay)",
  "age": "integer",
  "mobileNumber": "number",
  "role": "string (User/Admin)",
  "billingAddresses": "array of BillingAddress objects",
  "cart": "Cart object"
}
```

#### Product
```json
{
  "id": "integer",
  "name": "string (required)",
  "slug": "string (required)",
  "description": "string (required)",
  "price": "number (required)",
  "image": "string (required, URL)",
  "isActive": "boolean",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

#### Cart
```json
{
  "cartId": "integer",
  "totalPrice": "number",
  "created": "datetime",
  "updated": "datetime",
  "userId": "integer",
  "user": "User object",
  "cartItemsList": "array of CartItems objects"
}
```

#### CartItems
```json
{
  "cartItemsId": "integer",
  "product": "Product object",
  "quantity": "integer",
  "totalPrice": "number",
  "cart": "Cart object"
}
```

#### BillingAddress
```json
{
  "id": "integer",
  "billingName": "string (required)",
  "address1": "string (required)",
  "address2": "string (required)",
  "city": "string (required)",
  "state": "string (required)",
  "postalCode": "integer (required)",
  "mobileNumber": "number (required)",
  "default": "boolean (required)",
  "created": "datetime",
  "updated": "datetime",
  "userId": "integer",
  "user": "User object"
}
```

### Data Transfer Objects (DTOs)

#### UserDto (Registration)
```json
{
  "username": "string (required, 8-20 chars)",
  "password": "string (required, 8-20 chars)",
  "name": "string (required)",
  "emailAddress": "string (required, valid email)",
  "about": "string (required, max 200 chars)",
  "city": "string (required)"
}
```

#### UserUpdateDto
```json
{
  "name": "string (required)",
  "emailAddress": "string (required, valid email)",
  "about": "string (required, max 200 chars)",
  "city": "string (required)",
  "gender": "string (required, Male/Female/NotPreferToSay)",
  "age": "integer (required)",
  "mobileNumber": "number (required)"
}
```

#### PasswordUpdateDto
```json
{
  "oldPassword": "string (required)",
  "newPassword": "string (required)",
  "confirmPassword": "string (required)"
}
```

#### ProductDto
```json
{
  "name": "string (required)",
  "slug": "string (required)",
  "description": "string (required)",
  "price": "number (required)",
  "image": "string (required, URL)"
}
```

#### CartItemDto
```json
{
  "productId": "integer",
  "quantity": "integer"
}
```

#### CartPayload
```json
{
  "data": "CartItemDto object (required)",
  "operation": "string (required: 'add', 'update', or 'delete')"
}
```

#### BillingAddressDto
```json
{
  "billingName": "string (required)",
  "address1": "string (required)",
  "address2": "string (required)",
  "city": "string (required)",
  "state": "string (required)",
  "postalCode": "integer (required)",
  "mobileNumber": "number (required)",
  "defaultAddress": "boolean (required)"
}
```

#### DefaultAddressDto
```json
{
  "addressId": "integer"
}
```

### Response Models

#### AuthenticationResponse
```json
{
  "accessToken": "string",
  "username": "string",
  "userId": "integer"
}
```

#### UserResponse
```json
{
  "user": "User object",
  "message": "string"
}
```

#### ProductResponse
```json
{
  "message": "string"
}
```

#### CartResponse
```json
{
  "message": "string"
}
```

#### BillingAddressResponse
```json
{
  "message": "string"
}
```

#### ErrorResponse
```json
{
  "message": "string"
}
```

### Enumerations

#### Roles
- `Admin`: Administrator with full access
- `User`: Regular user with limited access

#### Gender
- `Male`: Male gender
- `Female`: Female gender
- `NotPreferToSay`: Prefer not to specify

#### CartOperation
- `add`: Add item to cart
- `update`: Update item quantity
- `delete`: Remove item from cart

---

## Error Handling

The API uses standard HTTP status codes and returns consistent error responses.

### HTTP Status Codes

- `200 OK`: Request successful
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

### Error Response Format

```json
{
  "message": "Detailed error description"
}
```

### Common Error Messages

- `"Username and Password is incorrect. Try again"`: Invalid login credentials
- `"Username already exists"`: Username taken during registration
- `"No user found"`: User not found for operation
- `"Product not found"`: Product doesn't exist
- `"Address not found"`: Billing address doesn't exist
- `"Data empty"`: Required data not provided
- `"Password doesn't match. ReType"`: Password confirmation mismatch

---

## Examples

### Complete User Registration and Shopping Flow

#### 1. Register New User
```bash
curl -X POST "https://localhost:7207/api/Users" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "jane_doe",
    "password": "password123",
    "name": "Jane Doe",
    "emailAddress": "jane@example.com",
    "about": "Fashion enthusiast",
    "city": "San Francisco"
  }'
```

#### 2. Login and Get Token
```bash
curl -X POST "https://localhost:7207/api/Authentication/CreateToken" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "jane_doe",
    "password": "password123"
  }'
```

Response:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "jane_doe",
  "userId": 2
}
```

#### 3. Browse Products
```bash
curl -X GET "https://localhost:7207/api/Product"
```

#### 4. Add Product to Cart
```bash
curl -X POST "https://localhost:7207/api/CartItem" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "data": {
      "productId": 1,
      "quantity": 2
    },
    "operation": "add"
  }'
```

#### 5. View Cart
```bash
curl -X GET "https://localhost:7207/api/Cart" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### 6. Add Billing Address
```bash
curl -X POST "https://localhost:7207/api/BillingAddress" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "billingName": "Jane Doe",
    "address1": "789 Market St",
    "address2": "Floor 15",
    "city": "San Francisco",
    "state": "CA",
    "postalCode": 94102,
    "mobileNumber": 5551234567,
    "defaultAddress": true
  }'
```

### Admin Product Management

#### Create Product (Admin Only)
```bash
curl -X POST "https://localhost:7207/api/Product" \
  -H "Authorization: Bearer <admin-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Vintage Jacket",
    "slug": "vintage-jacket",
    "description": "Classic vintage leather jacket",
    "price": 89.99,
    "image": "https://example.com/jacket.jpg"
  }'
```

#### Update Product
```bash
curl -X PUT "https://localhost:7207/api/Product/1" \
  -H "Authorization: Bearer <admin-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Premium Vintage Jacket",
    "slug": "premium-vintage-jacket",
    "description": "Premium vintage leather jacket with brass fittings",
    "price": 129.99,
    "image": "https://example.com/premium-jacket.jpg"
  }'
```

---

## Setup and Configuration

### Prerequisites

- .NET 6.0 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

### Configuration

#### 1. Database Connection
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "database": "Server=YOUR_SERVER;Database=onlineshopping;Trusted_Connection=True;"
  }
}
```

#### 2. JWT Configuration
Configure JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secret-key-here-make-it-long-and-secure",
    "Issuer": "your-api-url",
    "Audience": "your-api-url"
  }
}
```

#### 3. CORS Settings
The API is configured to accept requests from `http://localhost:4200` (Angular frontend). Update CORS settings in `Program.cs` if needed.

### Database Migration

Run the following commands to set up the database:

```bash
dotnet ef database update
```

### Running the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7207`
- HTTP: `http://localhost:5207`

### Swagger Documentation

Access interactive API documentation at:
```
https://localhost:7207/swagger
```

### Logging

The application uses custom file-based logging. Logs are stored in the `logs` directory with the format:
- File: `log_YYYYMMDD.txt`
- Format: `YYYY-MM-DD HH:SS [LEVEL] MESSAGE`

---

## Component Architecture

### Repository Pattern

The application implements the Repository pattern for data access:

#### ICartRepository Interface
```csharp
public interface ICartRepository
{
    Task<CartResponse> Add(Cart cart, CartItemDto cartItem);
    Task<CartResponse> Remove(Cart cart, CartItemDto cartItem);
    Task<CartResponse> Update(Cart cart, CartItemDto cartItem);
}
```

#### CartRepository Implementation
Provides concrete implementation for cart operations including:
- Adding items to cart (with duplicate checking)
- Updating item quantities
- Removing items from cart
- Calculating total prices automatically

### Logging Component

#### Log Class
```csharp
public static class Log
{
    public static void LogWrite(LogLevel logLevel, string logMessage)
}
```

Features:
- File-based logging with daily rotation
- Configurable log levels
- Automatic directory creation
- Thread-safe operations

### Database Context

#### ApplicationDbContext
Entity Framework Core context managing:
- Users and authentication
- Products and catalog
- Shopping carts and cart items
- Billing addresses
- Relationship mappings

---

## Security Features

1. **JWT Authentication**: Secure token-based authentication
2. **Role-based Authorization**: User and Admin role separation
3. **Password Protection**: Passwords excluded from API responses
4. **CORS Configuration**: Controlled cross-origin access
5. **Input Validation**: Data annotations for request validation
6. **SQL Injection Prevention**: Entity Framework ORM protection

---

## Best Practices

1. **API Versioning**: Structured for future versioning
2. **RESTful Design**: Standard HTTP methods and status codes
3. **Consistent Responses**: Standardized response formats
4. **Error Handling**: Comprehensive error messages
5. **Documentation**: XML comments for Swagger generation
6. **Separation of Concerns**: Clean architecture with controllers, models, and repositories
7. **Dependency Injection**: Configured for testability and maintainability

---

## Support and Contact

For technical support or questions about the API:

- **Project Repository**: [GitHub Repository URL]
- **Documentation**: This document
- **API Testing**: Use Swagger UI at `/swagger` endpoint
- **Logs**: Check application logs in the `logs` directory

---

*Last Updated: [Current Date]*  
*API Version: 1.0*  
*Framework: ASP.NET Core 6.0*