# InsurancePlatform.Microservices

# InsurancePlatform.Microservices

A sample microservices-based backend for an insurance domain, built with **.NET 8** and **C#**, plus a **Blazor WebAssembly** frontend.

The goal of this project is to demonstrate a professional, production-style architecture with:

- Independent business microservices
- A dedicated Identity Provider (IdP) using OpenID Connect (OpenIddict – planned)
- A Blazor WebAssembly UI as presentation layer
- Clean separation between presentation, business logic, and data access

># Current status

- IdentityService:
  - ASP.NET Core Identity + OpenIddict configured.
  - Client credentials flow tested via Postman (/connect/token).
- WebClient (Blazor WASM):
  - Basic dashboard, policies and claims demo pages.

