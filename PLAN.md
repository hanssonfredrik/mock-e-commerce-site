# Implementation Plan: Shopping Cart Feature

This plan implements the feature described in [SPEC.md](SPEC.md). Steps are sequenced so that each step's dependencies are completed in prior steps: models/DTOs → service interface → service implementation → endpoints → frontend API client → frontend types → hooks → components → tests.

---

## Step 1 — Add Backend DTOs and Response Models

**Files to modify:**
- `src/backend/MockEcommerce.Api/Endpoints/CartEndpoints.cs` — add `UpdateCartItemRequest` record
- `src/backend/MockEcommerce.Api/Models/CartResponse.cs` — **create** new file

**Tasks:**
1. Add `public record UpdateCartItemRequest(int Quantity);` to `CartEndpoints.cs` (next to the existing `AddToCartRequest` record).
2. Create `Models/CartResponse.cs` with:
   ```csharp
   public class CartResponse
   {
       public IEnumerable<CartItem> Items { get; set; } = [];
       public decimal CartTotal { get; set; }
   }
   ```

**Why first:** All subsequent backend work depends on these types existing.

---

## Step 2 — Extend ICartService Interface

**Files to modify:**
- `src/backend/MockEcommerce.Api/Services/ICartService.cs`

**Tasks:**
1. Add method signature:
   ```csharp
   CartItem? Update(int productId, int quantity);
   ```
   This sets the quantity of an existing cart item (replace semantics). Returns `null` if the product is not in the cart.

**Why now:** The interface must be updated before the implementation.

---

## Step 3 — Implement InMemoryCartService

**Files to modify:**
- `src/backend/MockEcommerce.Api/Services/InMemoryCartService.cs`

**Tasks — implement all methods using the `_cart` list and `_lock` field:**

1. **`GetAll()`** — Inside a `lock(_lock)`, return a copy of `_cart` (e.g., `_cart.ToList()`).
2. **`GetByProductId(int productId)`** — Inside a lock, return `_cart.FirstOrDefault(c => c.ProductId == productId)`.
3. **`Add(CartItem item)`** — Inside a lock:
   - Find existing item by `ProductId`.
   - If found, increment `Quantity` by `item.Quantity`, return the existing item.
   - If not found, add `item` to `_cart`, return it.
4. **`Update(int productId, int quantity)`** — Inside a lock:
   - Find existing item by `productId`.
   - If not found, return `null`.
   - Set `item.Quantity = quantity`, return the item.
5. **`Remove(int productId)`** — Inside a lock, find and remove; return `true`/`false`.
6. **`Clear()`** — Inside a lock, call `_cart.Clear()`.

**Important:** The service layer does NOT enforce the max-qty-of-5 rule. That is the endpoint layer's responsibility (keeps the service reusable).

---

## Step 4 — Implement Cart Endpoints

**Files to modify:**
- `src/backend/MockEcommerce.Api/Endpoints/CartEndpoints.cs`

**Tasks:**

1. **Define a constant:** `private const int MaxQuantityPerProduct = 5;`

2. **Register the new PUT route** in `MapCartEndpoints()`:
   ```csharp
   group.MapPut("/{productId:int}", UpdateCartItem)
       .WithName("UpdateCartItem")
       .WithSummary("Updates the quantity of an existing cart item.");
   ```

3. **Implement `GetCart`:**
   - Call `cartService.GetAll()`.
   - Compute `cartTotal` as `items.Sum(i => i.TotalPrice)`.
   - Return `TypedResults.Ok(new CartResponse { Items = items, CartTotal = cartTotal })`.

4. **Implement `AddToCart`:**
   - Validate `request.Quantity` is between 1 and 5 → return `ValidationProblem` if not.
   - Look up product via `productService.GetById(request.ProductId)` → return `NotFound` if null.
   - Check existing cart item via `cartService.GetByProductId(request.ProductId)`.
   - If exists and `existing.Quantity + request.Quantity > 5` → return `ValidationProblem` with message including current quantity.
   - Build `CartItem` from product data + requested quantity.
   - Call `cartService.Add(item)`.
   - If item was new → return `Created` with location `/api/cart/{productId}`.
   - If item was existing → return `Ok`.

5. **Implement `UpdateCartItem` (new):**
   - Validate `request.Quantity` is between 1 and 5 → `ValidationProblem`.
   - Call `cartService.Update(productId, request.Quantity)`.
   - If `null` → return `NotFound`.
   - Return `Ok(updatedItem)`.

6. **Implement `RemoveFromCart`:**
   - Call `cartService.Remove(productId)`.
   - If `false` → return `NotFound`.
   - Return `NoContent`.

7. **Implement `ClearCart`:**
   - Call `cartService.Clear()`.
   - Return `NoContent`.

---

## Step 5 — Add Frontend TypeScript Types

**Files to modify:**
- `src/frontend/src/types/index.ts`

**Tasks:**
1. Add:
   ```typescript
   export interface CartItem {
     productId: number;
     productName: string;
     unitPrice: number;
     quantity: number;
     totalPrice: number;
   }

   export interface CartResponse {
     items: CartItem[];
     cartTotal: number;
   }

   export interface UpdateCartItemRequest {
     quantity: number;
   }
   ```

---

## Step 6 — Add Frontend API Client Functions

**Files to modify:**
- `src/frontend/src/api/index.ts`

**Tasks:**
1. Remove the local `CartItem` interface (it duplicated what is now in `types/index.ts`) and import `CartItem` and `CartResponse` from `../types`.
2. Add four new functions:
   - `fetchCart(): Promise<CartResponse>` — `GET /api/cart`
   - `updateCartItem(productId: number, quantity: number): Promise<CartItem>` — `PUT /api/cart/{productId}` with body `{ quantity }`
   - `removeFromCart(productId: number): Promise<void>` — `DELETE /api/cart/{productId}`
   - `clearCart(): Promise<void>` — `DELETE /api/cart`

---

## Step 7 — Create useCart Hook

**Files to create:**
- `src/frontend/src/hooks/useCart.ts`

**Tasks:**
1. Create a custom hook that manages cart state:
   - State: `items: CartItem[]`, `cartTotal: number`, `loading: boolean`, `error: string | null`
   - Exposes: `refresh()`, `add(productId, quantity)`, `update(productId, quantity)`, `remove(productId)`, `clear()`
   - Each mutating function calls the corresponding API function, then calls `refresh()` to reload state from the server.
   - `refresh()` calls `fetchCart()` and updates `items` and `cartTotal`.

---

## Step 8 — Create CartDrawer Component

**Files to create:**
- `src/frontend/src/components/CartDrawer/CartDrawer.tsx`
- `src/frontend/src/components/CartDrawer/index.ts`

**Tasks:**
1. **CartDrawer.tsx** — props: `isOpen: boolean`, `onClose: () => void`, plus cart state/actions from `useCart`.
   - Render a backdrop overlay (click to close) + right-side sliding panel.
   - Header: "Your Cart" + close button.
   - Item list: for each item render: product name, unit price, quantity controls (− / display / +), line total, remove button.
   - The − button is `disabled` when `quantity === 1`.
   - The + button is `disabled` when `quantity === 5`.
   - Clicking − calls `update(productId, quantity - 1)`.
   - Clicking + calls `update(productId, quantity + 1)`.
   - Clicking remove calls `remove(productId)`.
   - Footer: display `cartTotal`, "Clear Cart" button (disabled when `items.length === 0`).
   - Empty state: "Your cart is empty."
2. **index.ts** — barrel export: `export { CartDrawer } from './CartDrawer';`

---

## Step 9 — Integrate Cart into App & Header

**Files to modify:**
- `src/frontend/src/App.tsx`
- `src/frontend/src/components/Header/Header.tsx`

**Tasks:**
1. **App.tsx:**
   - Add `isCartOpen` state (boolean, default `false`).
   - Instantiate `useCart` hook; destructure `items`, `cartTotal`, and action functions.
   - Compute `cartItemCount` as `items.reduce((sum, i) => sum + i.quantity, 0)`.
   - Pass `onCartClick={() => setIsCartOpen(true)}` to `Header`.
   - Render `<CartDrawer>` at the bottom of the JSX, passing `isOpen`, `onClose`, cart state, and cart actions.
   - Update `handleAddToCart` to call `cart.add()` and handle 400 errors with a user-friendly message about max quantity.
   - Refresh cart when drawer opens.

2. **Header.tsx:**
   - Add `onCartClick?: () => void` to `HeaderProps`.
   - Attach `onClick={onCartClick}` to the existing cart `<button>`.

---

## Step 10 — Backend Tests (Cart Endpoints)

**Files to create:**
- `test/backend/MockEcommerce.Api.Tests/Endpoints/CartEndpointTests.cs`

**Tasks — write integration tests using `WebApplicationFactory<Program>`:**
1. `POST /api/cart` with valid product → `201 Created`, body contains correct `CartItem`.
2. `POST /api/cart` with same product again → `200 OK`, quantity incremented.
3. `POST /api/cart` with quantity exceeding max (e.g., qty 6) → `400`.
4. `POST /api/cart` with increment that would exceed max (add 3, then add 3) → `400`.
5. `POST /api/cart` with non-existent productId → `404`.
6. `POST /api/cart` with quantity 0 → `400`.
7. `GET /api/cart` empty → `200`, items array empty, cartTotal 0.
8. `GET /api/cart` after adding items → `200`, correct items and cartTotal.
9. `PUT /api/cart/1` with valid quantity → `200`, quantity replaced.
10. `PUT /api/cart/1` with quantity > 5 → `400`.
11. `PUT /api/cart/1` with quantity 0 → `400`.
12. `PUT /api/cart/999` (not in cart) → `404`.
13. `DELETE /api/cart/1` after adding → `204`.
14. `DELETE /api/cart/1` when not in cart → `404`.
15. `DELETE /api/cart` (clear) → `204`, subsequent GET returns empty.

---

## Step 11 — Backend Tests (InMemoryCartService)

**Files to create:**
- `test/backend/MockEcommerce.Api.Tests/Services/InMemoryCartServiceTests.cs`

**Tasks — write unit tests:**
1. `GetAll` on empty cart returns empty collection.
2. `Add` new item returns item with correct quantity.
3. `Add` existing item increments quantity.
4. `GetByProductId` returns correct item.
5. `GetByProductId` returns null for missing product.
6. `Update` existing item replaces quantity.
7. `Update` non-existent item returns null.
8. `Remove` existing item returns true, item is gone.
9. `Remove` non-existent item returns false.
10. `Clear` removes all items.

---

## Step 12 — Frontend Tests (CartDrawer, useCart)

**Files to create:**
- `test/frontend/components/CartDrawer/CartDrawer.test.tsx`
- `test/frontend/hooks/useCart.test.ts`

**Tasks for CartDrawer tests:**
1. Renders "Your cart is empty." when items array is empty.
2. Renders item names, prices, quantities when items are provided.
3. + button disabled at quantity 5.
4. − button disabled at quantity 1.
5. Clicking remove calls remove function.
6. Clicking "Clear Cart" calls clear function.
7. Clicking backdrop calls onClose.

**Tasks for useCart tests:**
1. `refresh` calls fetchCart and updates state.
2. `add` calls addToCart API then refreshes.
3. `update` calls updateCartItem API then refreshes.
4. `remove` calls removeFromCart API then refreshes.
5. `clear` calls clearCart API then refreshes.

---

## Step 13 — Update Existing Frontend Tests

**Files to modify:**
- `test/frontend/App.test.tsx`
- `test/frontend/components/Header/Header.test.tsx`

**Tasks:**
1. **App.test.tsx:** Mock `useCart` hook. Add test that clicking cart icon opens the drawer. Add test that add-to-cart failure at max qty shows notification.
2. **Header.test.tsx:** Add test that clicking cart button calls `onCartClick` callback.

---

## Dependency Graph Summary

```
Step 1  (DTOs/Models)
  ↓
Step 2  (ICartService interface)
  ↓
Step 3  (InMemoryCartService impl)
  ↓
Step 4  (Cart endpoints impl)
  ↓
Step 5  (Frontend types)
  ↓
Step 6  (Frontend API client)
  ↓
Step 7  (useCart hook)
  ↓
Step 8  (CartDrawer component)
  ↓
Step 9  (App & Header integration)
  ↓
Steps 10–13  (Tests — can be done in parallel)
```
