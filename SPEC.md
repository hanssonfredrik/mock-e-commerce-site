# Feature Spec: Shopping Cart — View, Manage & Quantity Limits

## 1. Overview

Implement a fully functional shopping cart that lets users view their cart, manage item quantities, and see pricing — all accessible from the existing cart icon in the Header. A maximum purchase quantity of **5 per product** is enforced across all mutation endpoints (POST, PUT). A new **PUT /api/cart/{productId}** endpoint allows updating the quantity of an item already in the cart.

---

## 2. Ambiguities Resolved

| # | Question | Decision |
|---|----------|----------|
| 1 | **Does the max-qty limit of 5 apply to PUT (update) as well as POST (add)?** | **Yes.** The limit of 5 per product is enforced on every mutation. PUT sets the quantity to the value in the request body; if that value exceeds 5, return `400`. POST adds/increments; if the resulting quantity would exceed 5, return `400`. |
| 2 | **POST increment scenario: item already in cart with qty 3, user POSTs qty 4 — what happens?** | The resulting quantity would be 7 (3 + 4), which exceeds 5. Return `400 Bad Request` with a Problem Details body: `"Quantity for this product cannot exceed 5. Currently in cart: 3."` The cart is **not** modified. |
| 3 | **PUT semantics: does PUT replace or increment the quantity?** | **Replace.** `PUT /api/cart/{productId}` with `{ "quantity": 3 }` sets the quantity to exactly 3, regardless of the current quantity. |
| 4 | **PUT on a product not in the cart?** | Return `404 Not Found`. PUT only updates existing cart items. To add a new item, use POST. |
| 5 | **PUT with quantity 0 — what happens?** | Return `400 Bad Request`. Quantity must be between 1 and 5 inclusive. To remove an item, use `DELETE /api/cart/{productId}`. |
| 6 | **Error response format?** | All validation errors use the RFC 9457 **Problem Details** format (`application/problem+json`) via ASP.NET Core's `TypedResults.ValidationProblem()`. Field-level errors use the `errors` dictionary keyed by field name (e.g., `"quantity"`, `"productId"`). |
| 7 | **What does the frontend cart UI look like?** | Clicking the cart icon in the Header opens a **slide-out sidebar/drawer panel** overlaying the right side of the page. It is not a new page (this app has no router). The drawer lists cart items, per-item totals, a cart total, quantity controls, remove buttons, and a clear-cart button. |
| 8 | **POST with quantity ≤ 0 or non-integer?** | Return `400 Bad Request`. Quantity must be an integer between 1 and 5 inclusive. |
| 9 | **POST/PUT with a productId that doesn't exist in the product catalog?** | Return `404 Not Found` with message `"Product with ID {id} not found."` |
| 10 | **Does stock availability matter?** | **No.** The feature request only mentions a max-qty-per-product of 5. Stock is not checked when adding/updating cart items (this is a mock/demo app with no real inventory). |

---

## 3. Backend API Specification

### 3.1 Constants

- `MAX_QUANTITY_PER_PRODUCT = 5`

### 3.2 Endpoints

All endpoints are under the route group `/api/cart`.

#### GET /api/cart
- **Description:** Returns all items in the cart plus a cart-level total price.
- **Response 200:**
  ```json
  {
    "items": [
      {
        "productId": 1,
        "productName": "Wireless Headphones",
        "unitPrice": 79.99,
        "quantity": 2,
        "totalPrice": 159.98
      }
    ],
    "cartTotal": 159.98
  }
  ```
- **Response 200 (empty cart):**
  ```json
  {
    "items": [],
    "cartTotal": 0
  }
  ```

#### POST /api/cart
- **Description:** Adds a product to the cart. If the product is already in the cart, increments its quantity.
- **Request body:** `{ "productId": int, "quantity": int }`
- **Validation rules:**
  - `productId` must refer to an existing product → `404` if not found.
  - `quantity` must be ≥ 1 and ≤ 5 → `400` if out of range.
  - `existingQty + quantity` must be ≤ 5 → `400` if exceeded.
- **Response 201 (new item):** Returns the created `CartItem`. `Location` header: `/api/cart/{productId}`.
- **Response 200 (incremented existing item):** Returns the updated `CartItem`.
- **Response 400:** Problem Details with `errors` dictionary.
- **Response 404:** Problem Details with message about missing product.

#### PUT /api/cart/{productId}
- **Description:** Sets the quantity of an existing cart item to the specified value (replaces, does not increment).
- **Route parameter:** `productId` (int)
- **Request body:** `{ "quantity": int }`
- **Validation rules:**
  - `productId` must exist in the cart → `404` if not found (message: `"Product {productId} is not in the cart."`).
  - `quantity` must be ≥ 1 and ≤ 5 → `400` if out of range.
- **Response 200:** Returns the updated `CartItem`.
- **Response 400:** Problem Details with `errors` dictionary.
- **Response 404:** Plain Not Found.

#### DELETE /api/cart/{productId}
- **Description:** Removes a product from the cart entirely.
- **Response 204:** No content (success).
- **Response 404:** Product was not in the cart.

#### DELETE /api/cart
- **Description:** Clears all items from the cart.
- **Response 204:** No content (always succeeds, even if cart is already empty).

### 3.3 Request/Response Records

New records to add in `CartEndpoints.cs`:

```csharp
public record UpdateCartItemRequest(int Quantity);
```

The existing `AddToCartRequest(int ProductId, int Quantity)` remains unchanged.

New response DTO (can be a class or record in `Models/`):

```csharp
public class CartResponse
{
    public IEnumerable<CartItem> Items { get; set; }
    public decimal CartTotal { get; set; }
}
```

### 3.4 ICartService Changes

Add a new method to the interface:

```csharp
/// <summary>Updates the quantity of an existing cart item. Returns null if not found.</summary>
CartItem? Update(int productId, int quantity);
```

---

## 4. Frontend Specification

### 4.1 Cart Drawer Component (`CartDrawer`)

- **Location:** `src/frontend/src/components/CartDrawer/CartDrawer.tsx`
- **Trigger:** Clicking the existing cart button in the Header toggles the drawer open/closed.
- **Layout:**
  - Semi-transparent backdrop overlay (click to close).
  - Right-aligned panel sliding in from the right edge.
  - Header row: "Your Cart" title + close (✕) button.
  - Item list: each row shows product name, unit price, quantity controls (−/+), line total, and a remove (trash/✕) button.
  - Quantity controls: the − button is disabled at qty 1; the + button is disabled at qty 5.
  - Footer: cart total and a "Clear Cart" button (disabled when cart is empty).
  - Empty state: message "Your cart is empty." when no items.

### 4.2 New API Client Functions

Add to `src/frontend/src/api/index.ts`:

```typescript
export async function fetchCart(): Promise<CartResponse> { ... }
export async function updateCartItem(productId: number, quantity: number): Promise<CartItem> { ... }
export async function removeFromCart(productId: number): Promise<void> { ... }
export async function clearCart(): Promise<void> { ... }
```

### 4.3 New TypeScript Types

Add to `src/frontend/src/types/index.ts`:

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

### 4.4 State Management

- `App.tsx` manages `isCartOpen` (boolean) state and passes a toggle callback to `Header`.
- A new `useCart` hook (`src/frontend/src/hooks/useCart.ts`) encapsulates cart state:
  - `items: CartItem[]`, `cartTotal: number`, `loading: boolean`, `error: string | null`
  - Functions: `refresh()`, `add(productId, quantity)`, `update(productId, quantity)`, `remove(productId)`, `clear()`
  - `refresh()` is called when the drawer opens.
- `cartItemCount` in the Header badge is derived from `items.reduce((sum, i) => sum + i.quantity, 0)`.

### 4.5 Error Handling in the UI

- If POST addToCart returns 400 (max qty exceeded), show a user-friendly notification message: `"Cannot add — maximum quantity of 5 reached for this item."`
- If + button in the drawer would exceed 5, it is already disabled so the API is never called.
- Network/server errors show a generic "Something went wrong" notification.

---

## 5. Edge Cases

| Scenario | Expected Behavior |
|----------|-------------------|
| POST with `productId` not in catalog | `404` — `"Product with ID {id} not found."` |
| POST with `quantity: 0` or negative | `400` — validation error on `quantity`: `"Quantity must be between 1 and 5."` |
| POST with `quantity: 3` and item already has qty 3 | `400` — `"Quantity for this product cannot exceed 5. Currently in cart: 3."` |
| POST with `quantity: 2` and item already has qty 3 | `200` — item updated to qty 5, returned |
| POST with `quantity: 1` and item not in cart | `201` — new item created |
| PUT with `quantity: 0` | `400` — `"Quantity must be between 1 and 5."` |
| PUT with `quantity: 6` | `400` — `"Quantity must be between 1 and 5."` |
| PUT on product not in cart | `404` |
| PUT with valid quantity on item in cart | `200` — quantity replaced, item returned |
| DELETE on product not in cart | `404` |
| DELETE (clear) on already-empty cart | `204` — success (idempotent) |
| GET on empty cart | `200` — `{ "items": [], "cartTotal": 0 }` |
| Frontend: clicking + at qty 5 | Button is disabled; no API call made |
| Frontend: clicking − at qty 1 | Button is disabled; no API call made |
| Frontend: adding item from product list when already at qty 5 in cart | API returns 400; notification shown to user |

---

## 6. Out of Scope

- User authentication / per-user carts (single shared cart, singleton service)
- Stock/inventory validation on add
- Persistent storage (all data is in-memory, lost on restart)
- Checkout / payment flow
- Client-side routing
