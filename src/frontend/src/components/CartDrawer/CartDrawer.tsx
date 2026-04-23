import type { CartItem } from '../../types';

interface CartDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  items: CartItem[];
  cartTotal: number;
  loading: boolean;
  onUpdateQuantity: (productId: number, quantity: number) => void;
  onRemove: (productId: number) => void;
  onClear: () => void;
}

export function CartDrawer({
  isOpen,
  onClose,
  items,
  cartTotal,
  loading,
  onUpdateQuantity,
  onRemove,
  onClear,
}: CartDrawerProps) {
  if (!isOpen) return null;

  return (
    <div className="cart-drawer-overlay" onClick={onClose} role="presentation">
      <aside
        className="cart-drawer"
        role="dialog"
        aria-label="Shopping cart"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="cart-drawer__header">
          <h2 className="cart-drawer__title">Your Cart</h2>
          <button
            className="cart-drawer__close"
            onClick={onClose}
            aria-label="Close cart"
          >
            ✕
          </button>
        </div>

        <div className="cart-drawer__body">
          {loading && <p className="cart-drawer__loading">Loading cart…</p>}

          {!loading && items.length === 0 && (
            <p className="cart-drawer__empty">Your cart is empty.</p>
          )}

          {!loading && items.length > 0 && (
            <ul className="cart-drawer__list" aria-label="Cart items">
              {items.map((item) => (
                <li key={item.productId} className="cart-drawer__item">
                  <div className="cart-drawer__item-info">
                    <span className="cart-drawer__item-name">{item.productName}</span>
                    <span className="cart-drawer__item-price">
                      ${item.unitPrice.toFixed(2)}
                    </span>
                  </div>
                  <div className="cart-drawer__item-controls">
                    <button
                      className="cart-drawer__qty-btn"
                      onClick={() => onUpdateQuantity(item.productId, item.quantity - 1)}
                      disabled={item.quantity <= 1}
                      aria-label={`Decrease quantity of ${item.productName}`}
                    >
                      −
                    </button>
                    <span className="cart-drawer__qty" aria-label={`Quantity: ${item.quantity}`}>
                      {item.quantity}
                    </span>
                    <button
                      className="cart-drawer__qty-btn"
                      onClick={() => onUpdateQuantity(item.productId, item.quantity + 1)}
                      disabled={item.quantity >= 5}
                      aria-label={`Increase quantity of ${item.productName}`}
                    >
                      +
                    </button>
                    <button
                      className="cart-drawer__remove-btn"
                      onClick={() => onRemove(item.productId)}
                      aria-label={`Remove ${item.productName} from cart`}
                    >
                      Remove
                    </button>
                  </div>
                  <div className="cart-drawer__item-total">
                    ${item.totalPrice.toFixed(2)}
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="cart-drawer__footer">
          <div className="cart-drawer__total">
            <span>Total:</span>
            <span>${cartTotal.toFixed(2)}</span>
          </div>
          <button
            className="cart-drawer__clear-btn"
            onClick={onClear}
            disabled={items.length === 0}
          >
            Clear Cart
          </button>
        </div>
      </aside>
    </div>
  );
}
