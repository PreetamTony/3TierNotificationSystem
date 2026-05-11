# 3-Tier Notification System - PostgreSQL Implementation

## Overview
This document outlines the transition of the Notification CRUD application from an in-memory storage system to a persistent **PostgreSQL** database.

## Architecture
The application maintains its 3-tier structure:
- **Presentation**: `ConsoleMenu` (Program.cs)
- **Business**: `NotificationService.cs`
- **Data**: `UserRepository.cs`, `NotificationRepository.cs`

## Database Schema
Two tables are used with a Foreign Key relationship:
1. **users**: `id`, `name`, `email`, `phone`
2. **notifications**: `id`, `user_id` (FK), `message`, `notification_type`, `sent_date`

## Implementation Details

### Multi-Table JOIN Query
We use a SQL `JOIN` to combine notification and user data in a single request. This is implemented in `NotificationRepository.cs`:

```csharp
private const string JoinSelect = @"
    SELECT n.id, n.user_id, n.message, n.notification_type, n.sent_date,
           u.name AS user_name, u.email AS user_email, u.phone AS user_phone
    FROM notifications n
    JOIN users u ON u.id = n.user_id";
```

### Auto-Initialization
The `DatabaseHelper.cs` handles database and table creation automatically on startup using `CREATE DATABASE IF NOT EXISTS` logic.

### Mapping SQL to Objects
Data is mapped manually using `NpgsqlDataReader` for maximum performance and clarity:

```csharp
private static NotificationWithUser MapNotificationWithUser(NpgsqlDataReader r) =>
    new NotificationWithUser
    {
        Id = r.GetInt32(r.GetOrdinal("id")),
        UserId = r.GetInt32(r.GetOrdinal("user_id")),
        Message = r.GetString(r.GetOrdinal("message")),
        NotificationType = r.GetString(r.GetOrdinal("notification_type")),
        SentDate = r.GetDateTime(r.GetOrdinal("sent_date")),
        UserName = r.GetString(r.GetOrdinal("user_name")),
        UserEmail = r.GetString(r.GetOrdinal("user_email")),
        UserPhone = r.GetString(r.GetOrdinal("user_phone"))
    };
```

## How to use
1. Update `ConnectionString` in `DatabaseHelper.cs`.
2. Run `dotnet run`.
3. The app will bootstrap its own database schema.
