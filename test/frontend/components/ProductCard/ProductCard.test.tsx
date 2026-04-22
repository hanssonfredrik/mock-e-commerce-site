import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ProductCard } from '../../../../src/frontend/src/components/ProductCard';
import type { Product } from '../../../../src/frontend/src/types';

const mockProduct: Product = {
  id: 1,
  name: 'Test Headphones',
  description: 'Great sound quality.',
  price: 79.99,
  category: 'Electronics',
  stock: 10,
  imageUrl: 'https://example.com/headphones.jpg',
};

describe('ProductCard', () => {
  it('renders product name', () => {
    render(<ProductCard product={mockProduct} onAddToCart={() => {}} />);
    expect(screen.getByText('Test Headphones')).toBeInTheDocument();
  });

  it('renders product price formatted to 2 decimal places', () => {
    render(<ProductCard product={mockProduct} onAddToCart={() => {}} />);
    expect(screen.getByText('$79.99')).toBeInTheDocument();
  });

  it('renders product category', () => {
    render(<ProductCard product={mockProduct} onAddToCart={() => {}} />);
    expect(screen.getByText('Electronics')).toBeInTheDocument();
  });

  it('calls onAddToCart when button is clicked', async () => {
    const onAddToCart = vi.fn();
    render(<ProductCard product={mockProduct} onAddToCart={onAddToCart} />);
    await userEvent.click(screen.getByRole('button', { name: /add test headphones to cart/i }));
    expect(onAddToCart).toHaveBeenCalledWith(mockProduct);
  });

  it('disables button when product is out of stock', () => {
    const outOfStock: Product = { ...mockProduct, stock: 0 };
    render(<ProductCard product={outOfStock} onAddToCart={() => {}} />);
    expect(screen.getByRole('button', { name: /out of stock/i })).toBeDisabled();
  });

  it('renders product description', () => {
    render(<ProductCard product={mockProduct} onAddToCart={() => {}} />);
    expect(screen.getByText('Great sound quality.')).toBeInTheDocument();
  });

  it('renders product image with alt text', () => {
    render(<ProductCard product={mockProduct} onAddToCart={() => {}} />);
    const img = screen.getByRole('img', { name: 'Test Headphones' });
    expect(img).toBeInTheDocument();
    expect(img).toHaveAttribute('src', 'https://example.com/headphones.jpg');
  });
});
