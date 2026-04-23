import { useState, useCallback } from 'react';
import type { CartItem } from '../types';
import { fetchCart, addToCart as apiAddToCart, updateCartItem, removeFromCart, clearCart as apiClearCart } from '../api';

interface UseCartResult {
  items: CartItem[];
  cartTotal: number;
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
  add: (productId: number, quantity: number) => Promise<void>;
  update: (productId: number, quantity: number) => Promise<void>;
  remove: (productId: number) => Promise<void>;
  clear: () => Promise<void>;
}

export function useCart(): UseCartResult {
  const [items, setItems] = useState<CartItem[]>([]);
  const [cartTotal, setCartTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const cart = await fetchCart();
      setItems(cart.items);
      setCartTotal(cart.cartTotal);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  }, []);

  const add = useCallback(async (productId: number, quantity: number) => {
    await apiAddToCart({ productId, quantity });
    await refresh();
  }, [refresh]);

  const update = useCallback(async (productId: number, quantity: number) => {
    await updateCartItem(productId, quantity);
    await refresh();
  }, [refresh]);

  const remove = useCallback(async (productId: number) => {
    await removeFromCart(productId);
    await refresh();
  }, [refresh]);

  const clear = useCallback(async () => {
    await apiClearCart();
    await refresh();
  }, [refresh]);

  return { items, cartTotal, loading, error, refresh, add, update, remove, clear };
}
