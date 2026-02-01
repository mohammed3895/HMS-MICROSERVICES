# Hospital Management System (HMS) - Microservices Architecture

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Microservices](#microservices)
5. [Authentication & Authorization](#authentication--authorization)
6. [API Gateway](#api-gateway)
7. [Getting Started](#getting-started)
8. [API Documentation](#api-documentation)
9. [Security Features](#security-features)
10. [Development Guidelines](#development-guidelines)

---

## Project Overview

The Hospital Management System is a comprehensive healthcare management platform built using microservices architecture with .NET 9.0. The system manages various aspects of hospital operations including patient management, appointments, medical records, billing, pharmacy, laboratory services, and staff management.

### Key Features
- **Multi-tenant Architecture**: Support for multiple departments and roles
- **Advanced Security**: JWT authentication, 2FA, WebAuthn/FIDO2 support
- **Real-time Operations**: Appointment scheduling, notifications
- **Comprehensive Audit Logging**: Complete audit trail for compliance
- **Role-based Access Control**: Granular permissions for different user types
- **Scalable Design**: Independent microservices for horizontal scaling

---

## System Architecture

### Architectural Pattern
The system follows a **microservices architecture** with the following characteristics:

```
┌─────────────────────────────────────────────────────────────┐
│                      API Gateway (Ocelot)                    │
│                    https://localhost:7047                    │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│Authentication │    │  Appointment  │    │    Patient    │
│   Service     │    │    Service    │    │    Service    │
│   Port 5001   │    │   Port 5011   │    │   Port 5002   │
└───────────────┘    └───────────────┘    └───────────────┘
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│     Staff     │    │Medical Records│    │    Billing    │
│   Service     │    │    Service    │    │    Service    │
│   Port 7235   │    │   Port 5006   │    │   Port 5007   │
└───────────────┘    └───────────────┘    └───────────────┘
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│   Pharmacy    │    │  Laboratory   │    │ Notification  │
│   Service     │    │    Service    │    │    Service    │
│   Port 5008   │    │   Port 5009   │    │   Port 5010   │
└───────────────┘    └───────────────┘    └───────────────┘
```

### Design Principles
1. **Clean Architecture**: Each microservice follows Clean Architecture with Domain, Application, Infrastructure, and API layers
2. **CQRS Pattern**: Command-Query Responsibility Segregation using MediatR
3. **Domain-Driven Design**: Rich domain models with business logic encapsulation
4. **API Gateway Pattern**: Single entry point for all client requests
5. **Database per Service**: Each microservice owns its data

---

## Technology Stack

### Backend Framework
- **.NET 9.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **C# 13**: Programming language

### Libraries & Packages

#### Core Dependencies
- **MediatR (14.0.0)**: CQRS and mediator pattern implementation
- **AutoMapper (12.0.0)**: Object-to-object mapping
- **FluentValidation (12.1.1)**: Input validation
- **Entity Framework Core (9.0.12)**: ORM for database operations

#### Authentication & Security
- **Microsoft.AspNetCore.Authentication.JwtBearer (9.0.12)**: JWT authentication
- **Microsoft.AspNetCore.Identity (9.0.12)**: Identity management
- **Fido2 (4.0.0)**: WebAuthn/FIDO2 authentication
- **Otp.NET (1.4.1)**: One-Time Password generation
- **QRCoder (1.7.0)**: QR code generation for 2FA

#### API Gateway
- **Ocelot (24.1.0)**: API Gateway
- **Ocelot.Cache.CacheManager**: Response caching
- **Ocelot.Provider.Polly**: Circuit breaker and retry policies

#### Logging & Monitoring
- **Serilog (4.3.0)**: Structured logging
- **Serilog.AspNetCore (9.0.0)**: ASP.NET Core integration
- **Serilog.Sinks.Console**: Console output
- **Serilog.Sinks.File**: File-based logging

#### Email & Communication
- **MailKit (4.14.1)**: Email sending

#### Database
- **Microsoft.EntityFrameworkCore.SqlServer (9.0.12)**: SQL Server provider

#### Caching
- **Microsoft.Extensions.Caching.StackExchangeRedis (9.0.12)**: Redis caching (optional)
- **DistributedMemoryCache**: In-memory distributed cache

#### Documentation
- **Swashbuckle.AspNetCore (9.0.6)**: Swagger/OpenAPI documentation

---

## Microservices

### 1. Authentication Service (Port 5001)

**Responsibility**: User authentication, authorization, and identity management

#### Features
- User registration and login
- Email confirmation with OTP
- Password reset functionality
- Two-Factor Authentication (2FA/TOTP)
- WebAuthn/FIDO2 passwordless authentication
- JWT token generation and refresh
- Role-based access control
- Audit logging
- Security event tracking

#### Key Endpoints
```
POST   /api/auth/register              - Register new patient
POST   /api/auth/login                 - User login
POST   /api/auth/confirm-email         - Confirm email with OTP
POST   /api/auth/refresh-token         - Refresh access token
POST   /api/auth/forgot-password       - Request password reset
POST   /api/auth/reset-password        - Reset password
POST   /api/auth/change-password       - Change password (authenticated)
POST   /api/auth/logout                - Logout user
POST   /api/auth/2fa/enable            - Enable 2FA
POST   /api/auth/2fa/verify-code       - Verify 2FA code
POST   /api/auth/webauthn/register/*   - WebAuthn registration flow
POST   /api/auth/webauthn/authenticate/* - WebAuthn authentication flow
POST   /api/auth/staff/create          - Create staff account (Admin only)
GET    /api/profile/me                 - Get current user profile
PUT    /api/profile/basic              - Update profile
```

#### Domain Models
- `ApplicationUser`: Core user entity
- `Role`: User roles (Admin, Doctor, Nurse, etc.)
- `Permission`: Granular permissions
- `AuditLog`: Audit trail
- `SecurityEvent`: Security-related events
- `UserCredential`: WebAuthn credentials
- `TrustedDevice`: Device tracking

---

### 2. Appointment Service (Port 5011)

**Responsibility**: Appointment scheduling, doctor schedules, and calendar management

#### Features
- Appointment booking and management
- Doctor schedule configuration
- Available time slot calculation
- Appointment status tracking
- Doctor leave management
- Bulk rescheduling
- Waitlist management
- Appointment reminders

#### Key Endpoints
```
POST   /api/appointments                      - Create appointment
GET    /api/appointments/available-slots      - Get available time slots
GET    /api/appointments/{id}                 - Get appointment details
GET    /api/appointments/patient/{patientId}  - Get patient appointments
GET    /api/appointments/doctor/{doctorId}/daily - Get doctor's daily appointments
PUT    /api/appointments/{id}/reschedule      - Reschedule appointment
PUT    /api/appointments/{id}/cancel          - Cancel appointment
POST   /api/appointments/{id}/check-in        - Check-in patient
POST   /api/appointments/{id}/start-consultation - Start consultation (Doctor)
POST   /api/appointments/{id}/complete        - Complete appointment (Doctor)
POST   /api/appointments/{id}/no-show         - Mark as no-show
POST   /api/appointments/{id}/confirm         - Confirm appointment
POST   /api/appointments/waitlist             - Add to waitlist

GET    /api/doctor-schedule/{doctorId}        - Get doctor schedule
PUT    /api/doctor-schedule/{doctorId}        - Update schedule
POST   /api/doctor-schedule/{doctorId}/leave  - Apply for leave
POST   /api/doctor-schedule/{doctorId}/bulk-reschedule - Bulk reschedule
```

#### Domain Models
- `Appointment`: Appointment entity
- `AppointmentStatus`: Scheduled, Confirmed, InProgress, Completed, Cancelled, NoShow
- `DoctorSchedule`: Doctor working hours
- `DoctorLeave`: Leave requests
- `TimeSlot`: Available time slots

---

### 3. Patient Service (Port 5002)

**Responsibility**: Patient demographics, registration, and personal information

#### Features
- Patient registration
- Patient profile management
- Medical history
- Emergency contacts
- Insurance information
- Document management

---

### 4. Staff Service (Port 7235)

**Responsibility**: Staff member management and professional information

#### Features
- Staff profile management
- Department assignments
- Specialization tracking
- License and certification management
- Staff scheduling

---

### 5. Medical Records Service (Port 5006)

**Responsibility**: Electronic health records (EHR) management

#### Features
- Medical history recording
- Diagnosis management
- Treatment plans
- Prescription management
- Lab result tracking
- Document storage
- HIPAA compliance

---

### 6. Billing Service (Port 5007)

**Responsibility**: Billing, invoicing, and payment processing

#### Features
- Invoice generation
- Payment processing
- Insurance claims
- Payment plans
- Financial reporting

---

### 7. Pharmacy Service (Port 5008)

**Responsibility**: Medication management and prescription fulfillment

#### Features
- Medication inventory
- Prescription processing
- Drug interaction checking
- Medication dispensing tracking

---

### 8. Laboratory Service (Port 5009)

**Responsibility**: Laboratory test management and results

#### Features
- Test ordering
- Sample tracking
- Result management
- Reference ranges
- Quality control

---

### 9. Notification Service (Port 5010)

**Responsibility**: Multi-channel notifications

#### Features
- Email notifications
- SMS notifications
- Push notifications
- Appointment reminders
- Alert management

---

## Authentication & Authorization

### Authentication Methods

#### 1. Traditional Username/Password
- Email-based login
- Secure password hashing (ASP.NET Core Identity)
- Password strength requirements
- Account lockout after failed attempts

#### 2. Two-Factor Authentication (2FA/TOTP)
```csharp
// Enable 2FA
POST /api/auth/2fa/enable
{
  "authenticatorUri": "otpauth://totp/...",
  "manualEntryKey": "XXXXX..."
}

// Verify setup
POST /api/auth/2fa/verify-setup
{
  "code": "123456"
}

// Login with 2FA
POST /api/auth/2fa/verify-code
{
  "sessionId": "guid",
  "code": "123456"
}
```

#### 3. One-Time Password (OTP)
- Email-based OTP for registration confirmation
- OTP for login from new devices
- 10-minute expiration
- Rate limiting (max 3 resends per hour)
- IP address binding for login OTPs

#### 4. WebAuthn/FIDO2 (Passwordless)
```csharp
// Registration Flow
POST /api/auth/webauthn/register/begin
→ Returns CredentialCreateOptions

POST /api/auth/webauthn/register/complete
{
  "attestationResponse": {...},
  "deviceName": "YubiKey 5"
}

// Authentication Flow
POST /api/auth/webauthn/authenticate/begin
{
  "username": "user@example.com"
}
→ Returns AssertionOptions

POST /api/auth/webauthn/authenticate/complete
{
  "assertionResponse": {...}
}
→ Returns JWT tokens
```

**Supported Authenticators:**
- YubiKey (5 Series, Bio, FIPS)
- Windows Hello
- Touch ID / Face ID (Apple)
- Google Titan Security Key
- Android SafetyNet
- And more...

### JWT Token Structure

#### Access Token (1 hour expiry)
```json
{
  "sub": "user-guid",
  "user_id": "user-guid",
  "email": "user@example.com",
  "email_verified": "True",
  "name": "John Doe",
  "role": ["Doctor", "Admin"],
  "jti": "token-guid",
  "nbf": 1234567890,
  "exp": 1234571490,
  "iat": 1234567890,
  "iss": "HMS.Authentication",
  "aud": "HMS.Clients"
}
```

#### Refresh Token (7 days expiry)
- Stored in database with device information
- Can be revoked individually
- Single-use (rotated on refresh)

### Authorization Policies

#### Role-Based Policies
```csharp
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Doctor,Nurse")]
[Authorize(Policy = "MedicalStaff")] // Doctor + Nurse
[Authorize(Policy = "StaffOnly")]    // All staff except patients
```

#### Status-Based Policies
```csharp
[Authorize(Policy = "EmailConfirmed")]
[Authorize(Policy = "ActiveAccount")]
[Authorize(Policy = "VerifiedAccount")] // Email confirmed + Active
```

### Security Features

#### 1. Password Security
- Minimum 8 characters
- Must contain: uppercase, lowercase, number, special character
- Hashed using ASP.NET Core Identity (PBKDF2)
- Password history (prevent reuse)

#### 2. Account Protection
- Account lockout after 5 failed attempts
- Email notification on new device login
- Suspicious activity detection
- Session management (logout from all devices)

#### 3. OTP Security
- Cryptographically secure random generation
- Single-use only
- Time-limited (5-10 minutes)
- Rate limiting
- IP address binding for login OTPs
- Constant-time comparison

#### 4. WebAuthn Security
- Signature counter validation (detect cloned credentials)
- User verification required
- Resident key (discoverable credential) support
- Attestation format validation
- Challenge-response protocol

#### 5. Audit & Compliance
- Complete audit trail for all authentication events
- Security event logging
- Failed attempt tracking
- Device fingerprinting
- IP address logging

---

## API Gateway

### Configuration (Ocelot)

The API Gateway at `https://localhost:7047` routes requests to microservices.

#### Route Examples

**Authentication Service:**
```json
{
  "UpstreamPathTemplate": "/auth/{everything}",
  "DownstreamPathTemplate": "/api/auth/{everything}",
  "DownstreamScheme": "https",
  "DownstreamHostAndPorts": [
    { "Host": "localhost", "Port": 5001 }
  ]
}
```

**Appointment Service:**
```json
{
  "UpstreamPathTemplate": "/appointments/{everything}",
  "DownstreamPathTemplate": "/api/appointments/{everything}",
  "AuthenticationOptions": {
    "AuthenticationProviderKey": "GatewayAuthenticationScheme"
  }
}
```

#### Features

**1. Rate Limiting**
```json
"RateLimitOptions": {
  "EnableRateLimiting": true,
  "Period": "1m",
  "PeriodTimespan": 60,
  "Limit": 100
}
```

**2. Response Caching**
```json
"FileCacheOptions": {
  "TtlSeconds": 30
}
```

**3. CORS Configuration**
```json
"CorsSettings": {
  "AllowedOrigins": "https://web.hospital.com,http://localhost:3000"
}
```

---

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Postman (for API testing)

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd HMS-MICROSERVICES
```

2. **Restore NuGet packages**
```bash
dotnet restore
```

3. **Update connection strings**

Edit `appsettings.json` in each service:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HMS_Auth;Trusted_Connection=True;"
  }
}
```

4. **Run migrations**
```bash
cd HMS.Authentication.API
dotnet ef database update

cd ../HMS.Appointment.API
dotnet ef database update
```

5. **Configure JWT settings**

In Authentication service `appsettings.json`:
```json
{
  "JwtSettings": {
    "Secret": "YourVerySecureSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "HMS.Authentication",
    "Audience": "HMS.Clients",
    "ExpiryInHours": 1
  }
}
```

6. **Configure email settings** (optional)
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@hospital.com",
    "FromName": "Hospital Management System"
  }
}
```

### Running the Application

**Option 1: Visual Studio**
- Open `HMS-MICROSERVICES.slnx`
- Set multiple startup projects (API Gateway + desired services)
- Press F5

**Option 2: Command Line**

Terminal 1 - API Gateway:
```bash
cd APIGateway
dotnet run
```

Terminal 2 - Authentication Service:
```bash
cd HMS.Authentication.API
dotnet run
```

Terminal 3 - Appointment Service:
```bash
cd HMS.Appointment.API
dotnet run
```

### Testing

Access Swagger UI:
- API Gateway: `https://localhost:7047/swagger`
- Authentication: `https://localhost:5001/swagger`
- Appointments: `https://localhost:5011/swagger`

---

## API Documentation

### Authentication Flow Examples

#### 1. Patient Registration
```http
POST https://localhost:7047/auth/register
Content-Type: application/json

{
  "email": "patient@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-01-01"
}
```

Response:
```json
{
  "isSuccess": true,
  "message": "Registration successful. Please check your email to confirm your account.",
  "data": {
    "userId": "guid",
    "email": "patient@example.com"
  }
}
```

#### 2. Email Confirmation
```http
POST https://localhost:7047/auth/confirm-email
Content-Type: application/json

{
  "email": "patient@example.com",
  "otpCode": "123456"
}
```

#### 3. Login
```http
POST https://localhost:7047/auth/login
Content-Type: application/json

{
  "email": "patient@example.com",
  "password": "SecurePass123!",
  "rememberMe": true
}
```

Response:
```json
{
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "guid",
    "expiresIn": 3600,
    "user": {
      "id": "guid",
      "email": "patient@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "roles": ["Patient"]
    }
  }
}
```

#### 4. Create Appointment
```http
POST https://localhost:7047/appointments
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "patientId": "patient-guid",
  "doctorId": "doctor-guid",
  "appointmentDate": "2025-02-15",
  "startTime": "10:00:00",
  "appointmentType": "Consultation",
  "reason": "Annual checkup"
}
```

---

## Security Features

### 1. Input Validation
All commands are validated using FluentValidation:

```csharp
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Must contain lowercase")
            .Matches(@"[0-9]").WithMessage("Must contain number")
            .Matches(@"[!@#$%^&*]").WithMessage("Must contain special char");
    }
}
```

### 2. Audit Logging
Every significant action is logged:

```csharp
await _auditService.LogAsync(
    action: "UserLogin",
    entityName: "Authentication",
    entityId: userId.ToString(),
    oldValues: null,
    newValues: $"Login from IP: {ipAddress}"
);
```

### 3. Device Tracking
Trusted device management:
- Automatic device fingerprinting
- Trust new devices via OTP
- Revoke device access
- Monitor active sessions

### 4. Rate Limiting
API Gateway enforces rate limits:
- 50 requests/minute for authentication endpoints
- 100 requests/minute for general endpoints
- Custom limits per route

---

## Development Guidelines

### Project Structure

Each microservice follows Clean Architecture:

```
HMS.ServiceName.API/
├── Controllers/              # API endpoints
├── Program.cs               # Service configuration
└── appsettings.json         # Configuration

HMS.ServiceName.Application/
├── Commands/                # Write operations (CQRS)
├── Queries/                 # Read operations (CQRS)
├── DTOs/                    # Data transfer objects
├── Validators/              # FluentValidation validators
├── Behaviors/               # MediatR pipeline behaviors
└── ServiceExtensions/       # DI configuration

HMS.ServiceName.Domain/
├── Entities/                # Domain entities
├── Enums/                   # Domain enumerations
├── Events/                  # Domain events
└── Interfaces/              # Domain interfaces

HMS.ServiceName.Infrastructure/
├── Data/                    # DbContext, migrations
├── Services/                # Infrastructure services
├── Repositories/            # Data access
└── Configuration/           # Infrastructure configuration
```

### Coding Standards

#### 1. Command/Query Pattern
```csharp
// Command (writes data)
public class CreateAppointmentCommand : IRequest<Result<AppointmentDto>>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
}

// Query (reads data)
public class GetPatientAppointmentsQuery : IRequest<Result<List<AppointmentDto>>>
{
    public Guid PatientId { get; set; }
    public DateTime? FromDate { get; set; }
}
```

#### 2. Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; }
}
```

#### 3. Dependency Injection
```csharp
// Register in Program.cs or ServiceExtensions
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IAuditService, AuditService>();
```

#### 4. Error Handling
```csharp
try
{
    // Business logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error message with context");
    return Result<T>.Failure("User-friendly error message");
}
```

### Database Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName -p HMS.ServiceName.Infrastructure -s HMS.ServiceName.API

# Update database
dotnet ef database update -p HMS.ServiceName.Infrastructure -s HMS.ServiceName.API

# Remove last migration
dotnet ef migrations remove -p HMS.ServiceName.Infrastructure -s HMS.ServiceName.API
```

### Testing Strategy

1. **Unit Tests**: Test business logic in Application layer
2. **Integration Tests**: Test API endpoints and database operations
3. **Performance Tests**: Load testing on critical endpoints
4. **Security Tests**: Authentication/authorization testing

---

## Additional Resources

### Useful Links
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Ocelot Documentation](https://ocelot.readthedocs.io)
- [WebAuthn Guide](https://webauthn.guide)
- [FIDO2 .NET Library](https://github.com/passwordless-lib/fido2-net-lib)

**Technical Support:**
```
Email: mammado89@gmail.com
For: Bug reports, technical issues, API questions
```

---

## License
© 2025 Hospital Management System. All rights reserved.
