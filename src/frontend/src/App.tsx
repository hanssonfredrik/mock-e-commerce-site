import { useState, useRef, useEffect } from 'react';
import type { Product } from './types';
import { Header } from './components/Header';
import { HeroBanner } from './components/HeroBanner';
import { ProductList } from './components/ProductList';
import { CartDrawer } from './components/CartDrawer';
import { useProducts } from './hooks/useProducts';
import { useCart } from './hooks/useCart';
import './App.css';

export function App() {
  const { products, loading, error } = useProducts();
  const cart = useCart();
  const [cartMessage, setCartMessage] = useState<string | null>(null);
  const [isCartOpen, setIsCartOpen] = useState(false);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, []);

  const cartItemCount = cart.items.reduce((sum, i) => sum + i.quantity, 0);

  function showNotification(message: string) {
    setCartMessage(message);
    if (timerRef.current) clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => setCartMessage(null), 3000);
  }

  async function handleAddToCart(product: Product) {
    try {
      await cart.add(product.id, 1);
      showNotification(`"${product.name}" added to cart!`);
    } catch {
      showNotification('Failed to add item to cart.');
    }
  }

  async function handleOpenCart() {
    setIsCartOpen(true);
    await cart.refresh();
  }

  async function handleUpdateQuantity(productId: number, quantity: number) {
    try {
      await cart.update(productId, quantity);
    } catch {
      showNotification('Failed to update cart item.');
    }
  }

  async function handleRemoveFromCart(productId: number) {
    try {
      await cart.remove(productId);
    } catch {
      showNotification('Failed to remove item from cart.');
    }
  }

  async function handleClearCart() {
    try {
      await cart.clear();
    } catch {
      showNotification('Failed to clear cart.');
    }
  }

  return (
    <div className="app">
      <Header cartItemCount={cartItemCount} onCartClick={handleOpenCart} />
      <HeroBanner />

      <main className="app__main">
        <h1 className="app__section-heading">Our products</h1>

        {cartMessage && (
          <div className="app__notification" role="status">
            {cartMessage}
          </div>
        )}

        {loading && <p className="app__loading">Loading products…</p>}
        {error && <p className="app__error">Error: {error}</p>}
        {!loading && !error && (
          <ProductList products={products} onAddToCart={handleAddToCart} />
        )}
      </main>

      <CartDrawer
        isOpen={isCartOpen}
        onClose={() => setIsCartOpen(false)}
        items={cart.items}
        cartTotal={cart.cartTotal}
        loading={cart.loading}
        onUpdateQuantity={handleUpdateQuantity}
        onRemove={handleRemoveFromCart}
        onClear={handleClearCart}
      />
    </div>
  );
}
