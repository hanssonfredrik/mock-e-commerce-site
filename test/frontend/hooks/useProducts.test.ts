import { renderHook, waitFor } from '@testing-library/react';
import { useProducts } from '../../../src/frontend/src/hooks/useProducts';
import type { Product } from '../../../src/frontend/src/types';

const mockProducts: Product[] = [
  {
    id: 1,
    name: 'Headphones',
    description: 'Noise cancelling.',
    price: 79.99,
    category: 'Electronics',
    stock: 10,
    imageUrl: 'https://example.com/1.jpg',
  },
];

vi.mock('../../../src/frontend/src/api', () => ({
  fetchProducts: vi.fn(),
}));

import { fetchProducts } from '../../../src/frontend/src/api';
const mockedFetchProducts = vi.mocked(fetchProducts);

describe('useProducts', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('returns loading true initially', () => {
    mockedFetchProducts.mockReturnValue(new Promise(() => {}));

    const { result } = renderHook(() => useProducts());

    expect(result.current.loading).toBe(true);
    expect(result.current.products).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it('returns products after successful fetch', async () => {
    mockedFetchProducts.mockResolvedValue(mockProducts);

    const { result } = renderHook(() => useProducts());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    expect(result.current.products).toEqual(mockProducts);
    expect(result.current.error).toBeNull();
  });

  it('returns error message on fetch failure', async () => {
    mockedFetchProducts.mockRejectedValue(new Error('Network error'));

    const { result } = renderHook(() => useProducts());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    expect(result.current.error).toBe('Network error');
    expect(result.current.products).toEqual([]);
  });

  it('returns generic error for non-Error rejections', async () => {
    mockedFetchProducts.mockRejectedValue('something went wrong');

    const { result } = renderHook(() => useProducts());

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });
    expect(result.current.error).toBe('Unknown error');
  });
});
