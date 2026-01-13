# Overclocked

Overclocked is a robust, modular backend for a modern e-commerce platform dedicated to PC hardware and technology products. Built with .NET 10 and adhering to Clean Architecture principles, it provides a high-performance, scalable, and maintainable foundation for online retail.

## 🏗 Architecture

The project follows **Clean Architecture** to ensure separation of concerns and testability.

-   **Overclocked.Api**: The entry point (ASP.NET Core Web API).
-   **Overclocked.Application**: Contains business logic, use cases, and CQRS handlers.
-   **Overclocked.Domain**: Core domain logic, entities, and aggregates.
-   **Overclocked.Infrastructure**: External concerns like database access, file storage, and third-party services.
-   **Overclocked.SharedKernel**: Common types, results, and extensions used across layers.

It implements a **Custom CQRS (Command Query Responsibility Segregation)** pattern with pipeline behaviors (Validation, Caching, Logging) using [Scrutor](https://github.com/khellang/Scrutor) decorators.

## 🚀 Tech Stack

-   **Framework**: [.NET 10](https://dotnet.microsoft.com/) (C#)
-   **Web API**: ASP.NET Core
-   **Database**: PostgreSQL (Entity Framework Core 10)
-   **Caching**: Redis (StackExchange.Redis)
-   **Background Jobs**: Hangfire (PostgreSQL storage)
-   **Logging**: Serilog (sinking to Seq)
-   **Object Storage**: Cloudinary
-   **Resilience**: Polly
-   **Validation**: FluentValidation
-   **Containerization**: Docker & Docker Compose
-   **Testing**: xUnit v3, Shouldly, NetArchTest, Testcontainers, Respawn, Bogus

## ✨ Features

-   **Catalog Management**: Brands, Categories, Products, Tags.
-   **User Identity**: JWT Authentication, Refresh Tokens, Email Confirmation.
-   **Shopping Experience**: Shopping Cart, Wishlist, Order Management.
-   **Social & Engagement**: Product Reviews, Review Replies.
-   **System**: Employee Activity Logging.

## 🛠 Getting Started

### Prerequisites

-   [.NET 10 SDK](https://dotnet.microsoft.com/download)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop) (or Docker Engine + Compose)

### Configuration

The application uses environment variables for configuration. You can create a `.env` file in the `Overclocked.Api` directory or set them in your environment.

**Required Variables:**

```ini
# Database
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=OverclockedDB;Username=admin;Password=post_147_gres"
ConnectionStrings__Redis="localhost:6379"

# Security (JWT)
JwtSettings__SigningKey="YOUR_SUPER_SECRET_KEY_MUST_BE_LONG_ENOUGH"
JwtSettings__Issuer="Overclocked"
JwtSettings__Audience="Overclocked"
JwtSettings__ExpiresInMinutes=60

# Cloudinary (Image Storage)
CloudinarySettings__CloudName="your_cloud_name"
CloudinarySettings__ApiKey="your_api_key"
CloudinarySettings__ApiSecret="your_api_secret"

# Logging (Seq)
Seq__ServerUrl="http://localhost:5341"
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
    cd Overclocked.Api
    dotnet run
    ```

    Alternatively, run from the root:
    ```bash
    dotnet run --project Overclocked.Api/Overclocked.Api.csproj
    ```

4.  **Access the API**:
    -   Swagger UI: `https://localhost:7049/swagger` (or the port indicated in your console)
    -   Hangfire Dashboard: `https://localhost:7049/dashboard`
    -   Seq Dashboard: `http://localhost:8001`

## 🧪 Testing

The solution includes Unit, Integration, and Architecture tests.

To run all tests:
```bash
dotnet test
```

Integration tests use **Testcontainers** to spin up real database and Redis instances, ensuring reliable results.