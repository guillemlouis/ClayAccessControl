# Clay Access Control System

This project is an API for managing access control in an office environment, allowing for user authentication, door management, and access logging.

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server (LocalDB or full instance)

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/guillemlouis/clay.git
   cd ClayAccessControl
   ```

2. Restore the .NET packages:
   ```
   dotnet restore
   ```

3. Update the database with the latest migrations:
   ```
   cd ClayAccessControl.API
   dotnet ef database update
   ```

4. Run the application:
   ```
   dotnet run
   ```
5. Use existing test data [TESTDATA.md](TESTDATA.md)

The API should now be running on `http://localhost:5240`.

## Project Structure

- `ClayAccessControl.API`: Contains the API controllers and startup configuration.
- `ClayAccessControl.Core`: Contains domain entities, interfaces, and DTOs.
- `ClayAccessControl.Infrastructure`: Contains data access logic, services, and migrations.
- `ClayAccessControl.Tests`: Contains the unit tests.

## Data Seeding

The application uses a database seeder to populate initial test data. This includes:

- Sample office
- Predefined user roles (Admin, Manager, Employee)
- Sample users with various roles
- Sample doors with different access levels

You don't need to manually add this data; it will be automatically seeded when you run the application for the first time or apply migrations.

## API Documentation

### Swagger

The API is documented using Swagger. Once the application is running, you can access the Swagger UI at:

```
https://localhost:5001/swagger
```

This provides an interactive interface to explore and test the API endpoints.

### Postman Collection

A Postman collection is included in the repository to help test the API endpoints. To use it:

1. Import the `ClayAccessControl.postman_collection.json` file into Postman.
2. Update the `baseUrl` variable in the collection to match your local API URL if necessary.

The collection includes requests for all major endpoints, including authentication, user management, and door operations.

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. To access protected endpoints:

1. Use the `/api/Auth/login` endpoint to obtain a token.
2. Include the token in the Authorization header of subsequent requests:
   ```
   Authorization: Bearer <your_token_here>
   ```
## Rights Matrix

| Endpoint                            | Admin | Manager | Employee |
|-------------------------------------|-------|---------|----------|
| **Auth**                            | ✅ Login | ✅ Login | ✅ Login  |
| **Office**                          | ✅ CRUD | ❌ | ❌ |
| **Door**                            | ✅ CRUD, Grant Access, Revoke Access, Lock, Unlock | ✅ CRUD, Grant Access, Revoke Access, Lock, Unlock (for their office only) | ✅ Lock, Unlock (for their access level only) |
| **Event**                           | ✅ Access all events | ✅ Access events for their office | ❌ |
| **Users**          | ✅ Create | ❌ | ❌ |

### Explanation:

- **Admin**: Can perform all operations across all endpoints.
- **Manager**: Can manage doors (create, read, update, delete, lock, unlock) only for doors associated with their own office. Can access events for their office.
- **Employee**: Can only view and operate doors within their assigned office. The ability to lock/unlock doors is dependent on the user's role-based access level or specifically granted access for more granular control.

## Running Tests

To run the unit tests:
From ClayAccessControl.Tests

```
dotnet test
```

## License

This project is licensed under the MIT License - see the `LICENSE.md` file for details.
