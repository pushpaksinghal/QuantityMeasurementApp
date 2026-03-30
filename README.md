# Quantity Measurement App

This repository contains a comprehensive .NET application for performing measurements, conversions, and arithmetic operations on various physical quantities. The application is architected with a clean, multi-layered approach, featuring a RESTful API, a console interface, and a robust business logic layer.

## Features

- **Multi-Unit Support**: Perform operations on different categories of units:
  - **Length**: Feet, Inch, Yard, Centimeters
  - **Weight**: Kilogram, Gram, Pound
  - **Volume**: Litre, Millilitre, Gallon
  - **Temperature**: Celsius, Fahrenheit, Kelvin
- **Unit Conversion**: Convert values between compatible units (e.g., Feet to Inches).
- **Arithmetic Operations**:
  - Addition and Subtraction of quantities, with results in either the first operand's unit or a specified target unit.
  - Division of quantities, resulting in a dimensionless ratio.
- **Equality Checking**: Compare two quantities for equality, regardless of their unit (e.g., `1 Foot == 12 Inches`).
- **RESTful API**: A secure ASP.NET Core Web API to expose all functionalities.
- **Authentication**: JWT-based authentication for API endpoints, supporting local registration/login and Google OAuth.
- **Console Application**: An interactive, menu-driven console app for direct use of the measurement logic.
- **Operation History**: Persists all successful operations to a SQL Server database for auditing and retrieval.
- **Performance Caching**: Utilizes Redis to cache history data, reducing database load and improving response times.
- **Robust Error Handling**: Implements global exception filters and structured logging with NLog.

## Project Structure

The solution is organized into several distinct projects, following the principles of layered architecture:

-   `QuantityMeasurementApp.APILayer`: An ASP.NET Core project that provides the RESTful API endpoints for all quantity operations and user authentication.
-   `QuantityMeasurementApp.BusinessLayer`: Contains the core business logic, services for conversion, arithmetic, validation, and application services that orchestrate operations.
-   `QuantityMeasurementApp.ModelLayer`: Defines the data models, including entities (`Quantity`), Data Transfer Objects (DTOs), and unit enumerations.
-   `QuantityMeasurementApp.RepositoryLayer`: Manages data persistence. It uses Entity Framework Core to interact with a SQL Server database for authentication (`AuthDbContext`) and operation history (`QuantityDbContext`). It also includes the Redis cache service implementation.
-   `QuantityMeasurementApp.App`: A console application that serves as a command-line interface to the business logic.
-   `QuantityMeasurementApp.Test`: A suite of MSTest unit tests ensuring the correctness of the business logic for conversions, equality, and arithmetic.

## Technology Stack

-   **Backend**: .NET, C#
-   **API Framework**: ASP.NET Core
-   **Database**: Microsoft SQL Server
-   **ORM**: Entity Framework Core
-   **Caching**: Redis
-   **Authentication**: JWT, Google OAuth, ASP.NET Core Identity
-   **Logging**: NLog
-   **Testing**: MSTest

## API Endpoints

The API is secured with JWT authentication. A valid token must be provided in the `Authorization` header as a Bearer token for all `Quantity` and `History` endpoints.

### Authentication

-   `POST /api/Auth/register`: Register a new user.
-   `POST /api/Auth/login`: Log in and receive a JWT.
-e   `GET /api/Auth/google-login`: Initiate Google OAuth2 login flow.

### Quantity Operations

The `{type}` parameter can be `length`, `weight`, `volume`, or `temperature`.

-   `POST /api/Quantity/{type}/convert`: Convert a quantity to a target unit.
-   `POST /api/Quantity/{type}/equals`: Check if two quantities are equal.
-   `POST /api/Quantity/{type}/same-add`: Add two quantities, with the result in the first quantity's unit.
-   `POST /api/Quantity/{type}/target-add`: Add two quantities, with the result in a specified target unit.
-   `POST /api/Quantity/{type}/same-subtract`: Subtract two quantities, with the result in the first quantity's unit.
-   `POST /api/Quantity/{type}/target-subtract`: Subtract two quantities, with the result in a specified target unit.
-   `POST /api/Quantity/{type}/divide`: Divide two quantities.

### History

-   `GET /api/History`: Get all saved operation history records.
-   `GET /api/History/{id}`: Get a specific history record by its ID.
-   `POST /api/History`: Manually add a new history record.
-   `DELETE /api/History/{id}`: Delete a history record by its ID.
