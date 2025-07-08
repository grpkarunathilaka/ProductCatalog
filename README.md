# Product Catalog API

A comprehensive Web API built with .NET 8 and C# for managing a product catalog. This API provides full CRUD operations, pagination, filtering, validation, logging, and audit capabilities.

## Features

### Core Functionality
- **CRUD Operations**: Create, Read, Update, Delete products
- **Duplicate Prevention**: Unique constraint on Name + Brand combination
- **Pagination**: Efficient pagination for large datasets
- **Filtering**: Filter products by Name, Brand, and Price range
- **Validation**: Comprehensive input validation with detailed error messages
- **Logging**: Structured logging with Serilog
- **Audit Logging**: Track all API operations with detailed audit logs

### Technical Features
- **Clean Architecture**: Layered architecture with proper separation of concerns
- **Dependency Injection**: Proper DI container usage
- **In-Memory Database**: Entity Framework Core with in-memory database
- **FluentValidation**: Advanced validation with custom rules
- **Swagger Documentation**: Auto-generated API documentation
- **Middleware**: Custom middleware for logging and auditing
- **Exception Handling**: Global exception handling with proper error responses
- **Unit Testing**: Comprehensive unit and integration tests

## Architecture

This project follows Clean Architecture principles with the following layers:

### Project Structure

```
src/
    ProductCatalog.Api/              # 🎯 Presentation Layer
         Controllers/                 # API Controllers
         Middleware/                  # Custom middleware
         Program.cs                   # Application entry point
         ProductCatalog.Api.csproj
    
   ProductCatalog.Application/      # 🏢 Application Layer
         DTOs/                        # Data Transfer Objects
         Interfaces/                  # Service and Repository interfaces
         Services/                    # Business logic services
         Validators/                  # FluentValidation validators
         ProductCatalog.Application.csproj

   ProductCatalog.Domain/           # 🎯 Domain Layer
         Models/                      # Domain entities
         ProductCatalog.Domain.csproj

   ProductCatalog.Infrastructure/   # 🔧 Infrastructure Layer
         Data/                        # Entity Framework DbContext
         Repositories/                # Repository implementations
         ProductCatalog.Infrastructure.csproj

tests/
   ProductCatalog.Api.Tests/        # Integration tests
         Controllers/                 # API endpoint tests
         ProductCatalog.Api.Tests.csproj
    
   ProductCatalog.Application.Tests/ # Unit tests
         Services/                    # Service layer tests
         ProductCatalog.Application.Tests.csproj
```

### Layer Responsibilities

#### 🎯 **Domain Layer** (ProductCatalog.Domain)
- **Purpose**: Core business entities and domain logic
- **Dependencies**: None (pure domain logic)
- **Contains**: 
  - Product entity
  - Domain value objects
  - Business rules and invariants

#### 🏢 **Application Layer** (ProductCatalog.Application)
- **Purpose**: Business logic and application services
- **Dependencies**: Domain layer only
- **Contains**:
  - DTOs for data transfer
  - Service interfaces and implementations
  - FluentValidation validators
  - Repository interfaces
  - Application-specific business logic

#### 🔧 **Infrastructure Layer** (ProductCatalog.Infrastructure)
- **Purpose**: External concerns (database, external services)
- **Dependencies**: Application and Domain layers
- **Contains**:
  - Entity Framework DbContext
  - Repository implementations
  - Data access logic
  - External service integrations

#### 🎯 **Presentation Layer** (ProductCatalog.Api)
- **Purpose**: HTTP API endpoints and web concerns
- **Dependencies**: Application and Infrastructure layers
- **Contains**:
  - Controllers
  - Middleware
  - Configuration
  - Dependency injection setup

## API Endpoints

### Products Controller (`/api/products`)

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/api/products/getProducts` | Get all products (with pagination and filtering) | N/A | `PagedResult<ProductDto>` |
| GET | `/api/products/getProduct/{id}` | Get product by ID | N/A | `ProductDto` |
| POST | `/api/products/createProduct` | Create new product | `CreateProductDto` | `ProductDto` |
| PUT | `/api/products/updateProduct/{id}` | Update existing product | `UpdateProductDto` | `ProductDto` |
| DELETE | `/api/products/deleteProduct/{id}` | Delete product | N/A | No Content |

### Query Parameters (GET /api/products)

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `pageNumber` | int | Page number (default: 1) | `?pageNumber=1` |
| `pageSize` | int | Page size (default: 10, max: 100) | `?pageSize=20` |
| `name` | string | Filter by product name | `?name=iPhone` |
| `brand` | string | Filter by brand name | `?brand=Apple` |
| `minPrice` | decimal | Minimum price filter | `?minPrice=100` |
| `maxPrice` | decimal | Maximum price filter | `?maxPrice=1000` |

### Example Requests

#### Create Product
```json
POST /api/products/CreateProduct
{
    "name": "iPhone 15 Pro",
    "brand": "Apple",
    "price": 999.99
}
```

#### Update Product
```json
PUT /api/products/UpdateProduct/1
{
    "name": "iPhone 15 Pro Max",
    "brand": "Apple",
    "price": 1199.99
}
```

#### Filter Products
```
GET /api/products?brand=Apple&minPrice=500&maxPrice=2000&pageNumber=1&pageSize=10
```

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) 
- [Git](https://git-scm.com/)


### Sample Test Data

The application automatically seeds the database with sample products on startup:
- iPhone 15 Pro (Apple) - $999.99
- Galaxy S24 (Samsung) - $899.99
- MacBook Pro (Apple) - $1999.99
- ThinkPad X1 Carbon (Lenovo) - $1499.99
- Surface Pro 9 (Microsoft) - $1299.99

## Configuration

### Application Settings

The application uses the following configuration files:

#### `appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### `appsettings.Development.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Database Configuration

The application uses Entity Framework Core with an in-memory database. The database is automatically created and seeded on application startup.

### Logging Configuration

The application uses Serilog for structured logging:
- Console sink for development
- Structured JSON logging
- Request/Response logging middleware
- Audit logging for sensitive operations

## Architecture & Design Patterns

### Clean Architecture Benefits

1. **Independence**: Each layer is independent and can be modified without affecting others
2. **Testability**: Business logic can be tested independently of frameworks
3. **Flexibility**: Easy to swap implementations (e.g., database providers)
4. **Maintainability**: Clear separation of concerns makes the code easier to maintain

### Design Patterns Used

1. **Repository Pattern**: Abstracts data access logic (Infrastructure layer)
2. **Service Layer Pattern**: Encapsulates business logic (Application layer)
3. **Dependency Injection**: Manages object lifecycles and dependencies
4. **DTO Pattern**: Separates API contracts from domain models
5. **Middleware Pattern**: Cross-cutting concerns (logging, auditing)

### Dependency Flow

```
Domain ← Application ← Infrastructure
   ↑                       ↑
   └──── API ──────────────┘
```

- **Domain**: No dependencies (pure domain logic)
- **Application**: Depends on Domain only
- **Infrastructure**: Depends on Application and Domain
- **API**: Depends on Application and Infrastructure

## Validation Rules

### Product Validation

- **Name**: Required, 1-100 characters
- **Brand**: Required, 1-100 characters
- **Price**: Required, must be greater than 0, less than 1,000,000
- **Uniqueness**: Name + Brand combination must be unique

### Query Validation

- **PageNumber**: Must be greater than 0
- **PageSize**: Must be between 1 and 100
- **MinPrice**: Must be greater than or equal to 0
- **MaxPrice**: Must be greater than or equal to 0
- **Price Range**: MinPrice must be less than or equal to MaxPrice

## Error Handling

### Error Response Format

```json
{
    "message": "Error description",
    "statusCode": 400
}
```

### Common HTTP Status Codes

- `200 OK`: Successful GET, PUT requests
- `201 Created`: Successful POST requests
- `204 No Content`: Successful DELETE requests
- `400 Bad Request`: Validation errors
- `404 Not Found`: Resource not found
- `409 Conflict`: Duplicate resource (Name + Brand)
- `500 Internal Server Error`: Unexpected server errors

## Logging

### Log Levels

- **Information**: Normal application flow
- **Warning**: Unexpected situations that don't stop the application
- **Error**: Error events that stop the current operation
- **Debug**: Detailed information for debugging

### Structured Logging

The application uses structured logging with the following properties:
- `RequestId`: Unique identifier for each request
- `ElapsedMilliseconds`: Request processing time
- `Method`: HTTP method
- `Path`: Request path
- `StatusCode`: HTTP status code

### Audit Logging

Special audit logs are created for:
- Product creation (POST)
- Product updates (PUT)
- Product deletion (DELETE)

## Improvements

1. Authentication & Authorization
2. Replace In-Memory Database with SQL Server
3. API Versioning - backward campatability
4. Caching
5. Add Application Insight
6. Input sanitization - XSS prevention 
1. DB Migrations - version control


