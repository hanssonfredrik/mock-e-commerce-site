import { render, screen } from '@testing-library/react';
import { HeroBanner } from '../../../../src/frontend/src/components/HeroBanner';

describe('HeroBanner', () => {
  it('renders the promotional heading', () => {
    render(<HeroBanner />);
    expect(screen.getByRole('heading', { name: /discover quality products/i })).toBeInTheDocument();
  });

  it('renders the subtitle text', () => {
    render(<HeroBanner />);
    expect(screen.getByText(/find everything you need/i)).toBeInTheDocument();
  });

  it('renders the call-to-action link', () => {
    render(<HeroBanner />);
    expect(screen.getByText('Shop all products')).toBeInTheDocument();
  });

  it('renders as a section with accessible label', () => {
    render(<HeroBanner />);
    expect(screen.getByRole('region', { name: /promotional banner/i })).toBeInTheDocument();
  });
});
