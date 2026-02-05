# User Service

A clean and minimal backend API built with **.NET 10** and **Clean Architecture** principles.

The service intentionally focuses on **code structure, boundaries, and production-oriented design**
rather than feature richness or domain complexity.

---

## 🎯 Purpose

This repository demonstrates how I approach building backend services that are:

- easy to reason about and maintain
- explicit in their responsibilities and error handling
- suitable for long-term evolution in production environments

The domain itself (User CRUD) is intentionally simple in order to keep the focus on
architecture and engineering decisions.

---

## 🧩 Architecture Overview

The project follows **Clean Architecture**, ensuring a clear separation of concerns:

| Layer | Responsibility |
|------|----------------|
| **Domain** | Core entities, invariants, and error modeling (`Error`, `Result`, `ErrorType`) |
| **Application** | Business logic and validation (`UserService`, `CrudServiceBase`) |
| **Infrastructure** | Persistence and repository implementations (EF Core) |
| **API (V1)** | ASP.NET Core Web API, versioning, Swagger, exception handling |
| **Host** | Application composition and runtime configuration (`Program.cs`) |

This structure allows independent evolution of layers and keeps business logic
decoupled from infrastructure and transport concerns.

---

## ⚙️ Key Engineering Characteristics

- clear architectural boundaries between layers
- explicit error modeling instead of exception-driven control flow
- centralized exception handling middleware
- soft delete and audit timestamps (`CreatedAt`, `UpdatedAt`)
- API versioning and OpenAPI/Swagger documentation
- test coverage for critical paths using **xUnit** and **FluentAssertions**

Although the service is small, it is structured to reflect how production systems
are typically designed and maintained.

---

## 🧠 Design Notes

### DTOs

DTOs were intentionally **not introduced** in this sample.

In a larger production system, separate DTOs would be the correct choice.
However, for this service they would add boilerplate and duplication
without improving clarity or safety.

This trade-off keeps the codebase lean while still making boundaries explicit.

> The goal of this repository is to demonstrate **engineering judgment**,
> not maximal abstraction.

---

## ☁️ Production & Cloud Considerations

While this service runs locally, its design reflects patterns commonly used in
**cloud-hosted, distributed backend systems**, such as:

- predictable startup and shutdown behavior
- configuration-driven setup
- clear separation between business logic and infrastructure
- testable components without reliance on external systems
- centralized error handling and consistent API behavior

These patterns are applicable to systems that require reliability,
safe evolution over time, and operational clarity in production.

---

## 🔮 What I Would Add Next in a Real System

If this service were to evolve further, the next steps would naturally include:

- replacing local persistence with a managed datastore
- introducing request idempotency and retry policies
- adding structured logging and distributed tracing
- implementing background processing for long-running operations
- tightening API contracts with explicit DTO boundaries

The current implementation keeps the surface area minimal while leaving
clear seams for these extensions.

---

## 🚀 Running the Service

```bash
dotnet run --project src/UserService.Host
