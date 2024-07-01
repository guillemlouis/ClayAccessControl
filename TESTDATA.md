# Test Data
The application is seeded with initial test data to facilitate development and testing. This data provides a basic structure of users, roles, an office, doors, and access permissions. Below is an overview of the seeded data:

## Roles
Three roles are created with different access levels:

| Role                            | Access Level | Description |
|-------------------------------------|-------|---------|
| **Admin**                            | 5 | Full access to all systems |
| **Manager**                            | 3 | Access to most areas and management functions in their office |
| **Employee**                            | 1 | Basic access to common areas |

## Office
A single office is created:

- Name: Clay Headquarters
- Address: 123 Clay Street, Amsterdam

## Users
Four users are created, each associated with the Clay Headquarters office:

### Admin User

- Username: admin
- Email: admin@clay.com
- Role: Admin


### Manager User

- Username: manager
- Email: manager@clay.com
- Role: Manager


### Employee One

- Username: employee1
- Email: employee1@clay.com
- Role: Employee


### Employee Two

- Username: employee2
- Email: employee2@clay.com
- Role: Employee



Note: All users have the password "password" for testing purposes. In a production environment, you should change these passwords immediately.

## Doors
Two doors are created in the Clay Headquarters:

### Main Entrance

- Required Access Level: 1 (All employees can access)
- Initial Status: Locked


### Storage Room

- Required Access Level: 3 (Only managers and admins can access)
- Initial Status: Locked



## User Door Access
Initial user door access is set up based on the users' roles and the doors' required access levels:

All users have access to the Main Entrance.
Only the admin and manager users have access to the Storage Room.

This test data provides a starting point for exploring the application's functionality. You can use these pre-configured users, roles, and doors to test various access control scenarios.