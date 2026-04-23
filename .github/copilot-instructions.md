# Mock E-Commerce Site — Copilot Instructions

## Project Overview

This is a mock e-commerce web application called **"Mock Shop"**. It is a monorepo containing a .NET backend API and a React frontend. The project is for educational/demo purposes.

---

## Tech Stack

### Backend
- **Language:** C# on **.NET 10** (`net10.0`)
- **Framework:** ASP.NET Core Minimal API (no controllers — uses Minimal API endpoint mapping)
- **Solution format:** SLNX (`src/backend/MockEcommerce.slnx`)
- **Project file:** `src/backend/MockEcommerce.Api/MockEcommerce.Api.csproj`
- **NuGet packages:** `Microsoft.AspNetCore.OpenApi 10.0.5`
- **No database** — all data is in-memory (hardcoded product list, in-memory cart storage)

### Frontend
- **Language:** TypeScript (~6.0.2)
- **UI library:** React 19 (`react@^19.2.4`, `react-dom@^19.2.4`)
- **Build tool:** Vite 8 (`vite@^8.0.4`)
- **Vite plugin:** `@vitejs/plugin-react@^6.0.1`
- **Linting:** ESLint 9 with `typescript-eslint`, `eslint-plugin-react-hooks`, `eslint-plugin-react-refresh`
- **No routing library** (single-page, no client-side routing)
- **No state management library** (uses React `useState`/`useEffect` only)
- **No CSS framework** — plain CSS files (`App.css`, `index.css`)

### Testing
- **Frontend tests:** Vitest 4 (`vitest@^4.1.4`) with jsdom environment, `@testing-library/react@^16.3.2`, `@testing-library/jest-dom@^6.9.1`, `@testing-library/user-event@^14.6.1`
- **Backend tests:** xUnit 2.9.3 with `Microsoft.AspNetCore.Mvc.Testing` (integration tests) and `coverlet.collector` for coverage
- **Backend test project:** `test/backend/MockEcommerce.Api.Tests/MockEcommerce.Api.Tests.csproj` (targets `net10.0`)

### Monorepo Setup
- Root `package.json` uses npm workspaces with `"workspaces": ["src/frontend"]`
- Root-level Vitest config (`vitest.config.ts`) runs frontend tests from the root

---

## Project Structure

```
mock-e-commerce-site/              # Repository root
├── package.json                   # Root package.json (npm workspaces, test scripts)
├── tsconfig.json                  # Root TypeScript config for tests
├── vitest.config.ts               # Root Vitest config (runs frontend tests)
├── src/
│   ├── backend/
│   │   ├── MockEcommerce.slnx     # .NET solution file (SLNX format)
│   │   └── MockEcommerce.Api/
│   │       ├── MockEcommerce.Api.csproj
│   │       ├── Program.cs                   # App entry point, DI registration, middleware
│   │       ├── appsettings.json
│   │       ├── appsettings.Development.json
│   │       ├── Properties/
│   │       │   └── launchSettings.json      # Dev server URLs: http://localhost:5063, https://localhost:7296
│   │       ├── Endpoints/
│   │       │   ├── ProductEndpoints.cs      # GET /api/products, GET /api/products/{id} — FULLY IMPLEMENTED
│   │       │   └── CartEndpoints.cs         # GET/POST/DELETE /api/cart — STUBBED (NotImplementedException)
│   │       ├── Models/
│   │       │   ├── Product.cs               # Product model (Id, Name, Description, Price, Category, Stock, ImageUrl)
│   │       │   └── CartItem.cs              # CartItem model (ProductId, ProductName, UnitPrice, Quantity, TotalPrice)
│   │       └── Services/
│   │           ├── IProductService.cs       # Interface: GetAll(), GetById(int id)
│   │           ├── MockProductService.cs    # Hardcoded list of 5 products — FULLY IMPLEMENTED
│   │           ├── ICartService.cs          # Interface: GetAll(), Add(), GetByProductId(), Remove(), Clear()
│   │           └── InMemoryCartService.cs   # All methods throw NotImplementedException — STUBBED
│   └── frontend/
│       ├── package.json                     # Frontend package.json (React, Vite, dev deps)
│       ├── index.html                       # HTML entry point, title: "Mock Shop"
│       ├── vite.config.ts                   # Vite config with proxy: /api → http://localhost:5063
│       ├── eslint.config.js
│       ├── tsconfig.json / tsconfig.app.json / tsconfig.node.json
│       └── src/
│           ├── main.tsx                     # React entry point (createRoot, StrictMode)
│           ├── App.tsx                      # Main App component (Header, HeroBanner, ProductList, add-to-cart logic)
│           ├── App.css                      # App styles
│           ├── index.css                    # Global styles
│           ├── test-setup.ts                # Imports @testing-library/jest-dom
│           ├── api/
│           │   └── index.ts                 # API client: fetchProducts(), fetchProductById(), addToCart()
│           ├── components/
│           │   ├── Header/
│           │   │   ├── Header.tsx           # Site header with logo "Mock Shop", nav links, cart button with badge
│           │   │   └── index.ts             # Barrel export
│           │   ├── HeroBanner/
│           │   │   ├── HeroBanner.tsx       # Promotional banner section
│           │   │   └── index.ts
│           │   ├── ProductCard/
│           │   │   ├── ProductCard.tsx       # Individual product display with add-to-cart button
│           │   │   └── index.ts
│           │   └── ProductList/
│           │       ├── ProductList.tsx       # Renders list of ProductCard components
│           │       └── index.ts
│           ├── hooks/
│           │   └── useProducts.ts           # Custom hook: fetches products on mount, returns { products, loading, error }
│           └── types/
│               └── index.ts                 # TypeScript types: Product, AddToCartRequest
└── test/
    ├── backend/
    │   └── MockEcommerce.Api.Tests/
    │       ├── MockEcommerce.Api.Tests.csproj
    │       ├── Endpoints/
    │       │   └── ProductEndpointTests.cs   # Integration tests for product endpoints (3 tests)
    │       └── Services/
    │           └── MockProductServiceTests.cs # Unit tests for MockProductService (3 tests)
    └── frontend/
        ├── App.test.tsx                      # App integration tests (7+ tests)
        ├── components/
        │   ├── Header/
        │   │   └── Header.test.tsx           # Header component tests (6 tests)
        │   ├── HeroBanner/
        │   │   └── HeroBanner.test.tsx       # HeroBanner tests (4 tests)
        │   ├── ProductCard/
        │   │   └── ProductCard.test.tsx      # ProductCard tests (7 tests)
        │   └── ProductList/
        │       └── ProductList.test.tsx       # ProductList tests (3 tests)
        └── hooks/
            └── useProducts.test.ts           # useProducts hook tests (4 tests)
```

---

## Implementation Status

### Fully Implemented (Working)
- **Product endpoints:** `GET /api/products` (returns all 5 products), `GET /api/products/{id}` (returns one or 404)
- **MockProductService:** Returns a hardcoded list of 5 products (see product catalog below)
- **All frontend components:** Header, HeroBanner, ProductCard, ProductList, App
- **Frontend API client:** `fetchProducts()`, `fetchProductById()`, `addToCart()`
- **useProducts hook:** Fetches products on mount, manages loading/error state
- **All frontend tests and backend product tests**

### Stubbed / Not Implemented (throws NotImplementedException)
- **All CartEndpoints handler methods:** `GetCart`, `AddToCart`, `RemoveFromCart`, `ClearCart` — routes are registered but every handler body is `throw new NotImplementedException()`
- **All InMemoryCartService methods:** `GetAll()`, `GetByProductId()`, `Add()`, `Remove()`, `Clear()` — all throw `NotImplementedException()`
- **No cart-related tests exist** in the backend test project
- The frontend `addToCart()` API call will fail at runtime because the backend cart endpoint is not implemented

---

## Product Catalog (Hardcoded Data)

The `MockProductService` contains exactly **5 hardcoded products**:

| ID | Name                        | Price    | Category      | Stock | Description                                                         |
|----|------------------------------|----------|---------------|-------|----------------------------------------------------------------------|
| 1  | Wireless Headphones          | $79.99   | Electronics   | 25    | Over-ear noise-cancelling wireless headphones with 30-hour battery life |
| 2  | Running Shoes                | $59.99   | Footwear      | 40    | Lightweight breathable running shoes for all-terrain use             |
| 3  | Stainless Steel Water Bottle | $24.99   | Accessories   | 100   | Insulated 32 oz water bottle that keeps drinks cold for 24 hours     |
| 4  | Mechanical Keyboard          | $109.99  | Electronics   | 15    | Compact tenkeyless mechanical keyboard with Cherry MX Blue switches  |
| 5  | Yoga Mat                     | $34.99   | Sports        | 60    | Non-slip 6mm thick yoga mat with carrying strap                      |

All product images use placeholder URLs from `https://placehold.co/300x300?text=...`.

Product categories used: **Electronics** (2 products), **Footwear** (1), **Accessories** (1), **Sports** (1).

---

## API Endpoints

Base URL: `http://localhost:5063`

### Product Endpoints (under `/api/products`) — IMPLEMENTED
| Method | Route                 | Description                     | Status       |
|--------|-----------------------|---------------------------------|-------------|
| GET    | `/api/products`       | Returns all products            | Implemented |
| GET    | `/api/products/{id}`  | Returns product by ID or 404    | Implemented |

### Cart Endpoints (under `/api/cart`) — STUBBED
| Method | Route                      | Description                           | Status                    |
|--------|----------------------------|---------------------------------------|---------------------------|
| GET    | `/api/cart`                | Returns all cart items                | Stubbed (NotImplementedException) |
| POST   | `/api/cart`                | Adds product to cart                  | Stubbed (NotImplementedException) |
| DELETE | `/api/cart/{productId}`    | Removes product from cart by ID       | Stubbed (NotImplementedException) |
| DELETE | `/api/cart`                | Clears all cart items                 | Stubbed (NotImplementedException) |

The POST `/api/cart` endpoint accepts a JSON body: `{ "productId": int, "quantity": int }` (defined as `AddToCartRequest` record in `CartEndpoints.cs`).

---

## DI Registration (Program.cs)

```csharp
builder.Services.AddSingleton<IProductService, MockProductService>();
builder.Services.AddSingleton<ICartService, InMemoryCartService>();
```

- Both services are registered as **Singleton**.
- CORS is configured to allow `http://localhost:5173` (the Vite dev server).
- OpenAPI is enabled via `builder.Services.AddOpenApi()` and `app.MapOpenApi()`.

---

## How to Run

### Backend
```bash
cd src/backend/MockEcommerce.Api
dotnet run
```
The API runs on `http://localhost:5063` (HTTP) or `https://localhost:7296` (HTTPS).

### Frontend
```bash
cd src/frontend
npm install
npm run dev
```
The Vite dev server runs on `http://localhost:5173` and proxies `/api` requests to `http://localhost:5063`.

---

## How to Run Tests

### Frontend Tests (from repository root)
```bash
npm test
```
Or equivalently:
```bash
npm run test:frontend
```
Both run `vitest run` using the root `vitest.config.ts`. Tests are located in `test/frontend/` and use jsdom environment with `@testing-library/react`.

### Backend Tests
```bash
cd test/backend/MockEcommerce.Api.Tests
dotnet test
```
Or from the solution root:
```bash
cd src/backend
dotnet test
```
Backend tests use **xUnit** with `Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>` for integration tests. The `public partial class Program { }` declaration in `Program.cs` enables this.

---

## Key Conventions

- **Minimal API pattern:** Endpoints are organized in static classes with extension methods (e.g., `app.MapProductEndpoints()`) in the `Endpoints/` folder, not MVC controllers.
- **Barrel exports:** Each frontend component folder has an `index.ts` that re-exports the component.
- **BEM-like CSS classes:** CSS classes follow a BEM convention (e.g., `product-card__image`, `header__nav-link--active`).
- **TypeScript strict mode** is enabled.
- **Test file location:** Frontend tests are in `test/frontend/` (separate from source), mirroring the `src/frontend/src/` structure. Backend tests are in `test/backend/`.
- **No database or ORM** — the project uses purely in-memory data structures.
- **Thread safety:** `InMemoryCartService` has a `Lock` field for thread-safe cart operations (but methods are not yet implemented).
