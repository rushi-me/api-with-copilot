# User CRUD API

A simple ASP.NET Core Web API for managing users with comprehensive middleware implementation.

## Features

- **User CRUD Operations**: Create, Read, Update, Delete users
- **Email Validation**: Format validation and uniqueness checking
- **Input Validation**: Comprehensive validation with data annotations
- **Error Handling**: Global error handling middleware
- **Authentication**: API key-based authentication
- **Request Logging**: Detailed request/response logging
- **Swagger Documentation**: Interactive API documentation

## Middleware Stack

The application uses the following middleware in order:

1. **Error Handling Middleware** - Catches and handles unhandled exceptions
2. **Authentication Middleware** - Validates API key for all requests
3. **Request Logging Middleware** - Logs HTTP method, path, and response status

## API Authentication

All API endpoints require authentication using an API key header:

```
X-API-Key: xk9m2n8p4q7r3s5t1v6w
```

**Note**: Swagger endpoints (`/swagger`) are exempt from authentication for development purposes.

## API Endpoints

### Users

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update existing user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/check-email` - Check email uniqueness

### User Model

```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@company.com",
  "department": "Engineering",
  "createdAt": "2024-01-01T00:00:00Z",
  "isActive": true
}
```

### Create User Request

```json
{
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@company.com",
  "department": "HR",
  "isActive": true
}
```

## Running the Application

1. **Build the project**:
   ```bash
   dotnet build
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Access Swagger UI**:
   ```
   https://localhost:5225/swagger
   ```

## Testing

Use the provided `copilot-api.http` file to test the API endpoints. The file includes:

- All CRUD operations
- Validation scenarios
- Authentication tests
- Error handling tests

## Logging

The application logs:
- Request details (method, path, start time)
- Response details (status code, duration, completion time)
- Authentication events
- Error details

## Error Handling

The global error handling middleware provides:
- Structured error responses
- Appropriate HTTP status codes
- Error logging
- Exception type-based status code mapping

## Security Features

- API key authentication
- Input validation and sanitization
- Email format validation
- Email uniqueness enforcement
- Request/response logging for audit trails 