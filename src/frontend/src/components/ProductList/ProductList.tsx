import type { Product } from '../../types';
import { ProductCard } from '../ProductCard';

interface ProductListProps {
  products: Product[];
  onAddToCart: (product: Product) => void;
}

export function ProductList({ products, onAddToCart }: ProductListProps) {
  if (products.length === 0) {
    return <p className="product-list__empty">No products available.</p>;
  }

  return (
    <ul className="product-list" aria-label="Product list">
      {products.map((product) => (
        <li key={product.id}>
          <ProductCard product={product} onAddToCart={onAddToCart} />
        </li>
      ))}
    </ul>
  );
}
