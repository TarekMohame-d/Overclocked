# ⚡ Overclocked

**Overclocked** is a robust, modular backend for a modern e-commerce platform dedicated to PC hardware and technology products. Built with **.NET 10** and adhering to **Clean Architecture** principles, it provides a high-performance, scalable, and maintainable foundation for online retail.

---

## 🏗 Architecture

The project follows **Clean Architecture** to ensure separation of concerns and testability, organized into the following layers:

- **`Overclocked.Api`**: The entry point (ASP.NET Core Web API). Handles HTTP requests, configuration, and middleware.
- **`Overclocked.Application`**: Contains business logic, use cases (Features), and CQRS handlers. It defines _what_ the system does.
- **`Overclocked.Domain`**: Core domain logic, entities, value objects, and aggregates. It represents the _heart_ of the business.
- **`Overclocked.Infrastructure`**: External concerns like database access, file storage, email services, and background jobs.
- **`Overclocked.SharedKernel`**: Common types, results, and extensions used across layers (e.g., `Result<T>`, `Error`).

### Key Patterns

- **CQRS (Command Query Responsibility Segregation)**: Separates read and write operations for better performance and scalability using **MediatR**.
- **Result Pattern**: Uses a functional approach to error handling (monads) instead of exceptions for control flow.
- **Outbox Pattern**: Ensures atomic database updates and reliable message processing.
- **Inbox Pattern**: Ensures reliable processing of external events (e.g., payment webhooks).
- **Pipeline Behaviors**: Handles Validation, Caching, and Logging transparently via decorators.
- **Vertical Slices**: Features are organized by domain concept (e.g., `ProductUseCases`, `CartUseCases`) rather than technical layers inside the Application project.

---

## 🚀 Tech Stack

### Core

- **Framework**: [.NET 10](https://dotnet.microsoft.com/) (C#)
- **Web API**: ASP.NET Core
- **Database**: PostgreSQL (Entity Framework Core 10)
- **Caching**: Redis (StackExchange.Redis)

### Services & Tools

- **Background Jobs**: [Hangfire](https://www.hangfire.io/) (PostgreSQL storage)
- **Logging**: [Serilog](https://serilog.net/) (sinking to [Seq](https://datalust.co/seq))
- **Object Storage**: [Cloudinary](https://cloudinary.com/)
- **Payment Gateway**: [Paymob](https://paymob.com/)
- **Email**: MailKit
- **Resilience**: Polly
- **Validation**: FluentValidation
- **Containerization**: Docker & Docker Compose

### Build & Tooling

- **Solution Format**: Uses the modern XML-based solution format (`.slnx`).
- **Dependency Management**: Uses [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management) (`Directory.Packages.props`) for consistent versioning.
- **Formatting**: Enforced via [CSharpier](https://csharpier.com/).

### Testing

- **Framework**: xUnit
- **Assertions**: Shouldly / FluentAssertions
- **Integration**: [Testcontainers](https://testcontainers.com/) (Real Postgres & Redis instances)
- **Architecture**: NetArchTest
- **Data Generation**: Bogus

---

## ✨ Features

- **📦 Catalog Management**: Comprehensive management of Brands, Categories, Products, and Tags.
- **🔐 User Identity**: Secure JWT Authentication, Refresh Tokens, and Email Confirmation.
- **🛒 Shopping Experience**: Persistent Shopping Cart, Wishlist, and robust Order Management.
- **💬 Social & Engagement**: Product Reviews and threaded Review Replies.
- **🛡 Security**: Per-user Rate Limiting and Global Exception Handling.
- **👮 Audit**: Employee Activity Logging.
- **🖼 Media**: Optimized image uploads via Cloudinary.

---

## 🛠 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (or Docker Engine + Compose)

### Configuration

The application uses environment variables for configuration. Create a `.env` file in the `src/Overclocked.Api` directory.

**Required Variables:**

```ini
# Database
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=OverclockedDB;Username=your_username;Password=your_password"
ConnectionStrings__Redis="localhost:6379"

# Security (JWT)
JwtSettings__SigningKey="YOUR_SUPER_SECRET_KEY_MUST_BE_LONG_ENOUGH_MIN_32_CHARS"
JwtSettings__Issuer="Overclocked"
JwtSettings__Audience="Overclocked"
JwtSettings__ExpiresInMinutes=60

# Cloudinary (Image Storage)
CloudinarySettings__CloudName="your_cloud_name"
CloudinarySettings__ApiKey="your_api_key"
CloudinarySettings__ApiSecret="your_api_secret"

# Paymob (Payment Gateway)
PaymobSettings__ApiKey="your_paymob_api_key"
PaymobSettings__IntegrationId=123456
PaymobSettings__FrameId=123456
PaymobSettings__Hmac="your_paymob_hmac"

# Email Settings
EmailSettings__From=your_from_email
EmailSettings__AppPassword=your_email_app_password
```

### Running the Application

1.  **Clone the repository**:

    ```bash
    git clone https://github.com/your-username/overclocked.git
    cd overclocked
    ```

2.  **Start Infrastructure**:
    Run the supporting services (Postgres, Redis, Seq) using Docker Compose.

    ```bash
    docker-compose up -d
    ```

3.  **Run the API**:
    Navigate to the API project and run it.

    ```bash
    cd src/Overclocked.Api
    dotnet run
    ```

4.  **Access the API**:
    - 📄 **Swagger UI**: `https://localhost:7049/swagger`
    - ⚙ **Hangfire Dashboard**: `https://localhost:7049/hangfire`
    - 📝 **Seq Dashboard**: `http://localhost:8001`

## 🧪 Testing

The solution includes Unit, Integration, and Architecture tests. Integration tests use **Testcontainers** to spin up real database and Redis instances, ensuring reliable results without mocking external dependencies.

To run all tests:

```bash
dotnet test
```
