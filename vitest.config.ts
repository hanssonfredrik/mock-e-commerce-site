import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['src/frontend/src/test-setup.ts'],
    include: ['test/frontend/**/*.{test,spec}.{ts,tsx}'],
    server: {
      fs: {
        allow: ['.'],
      },
    },
  },
});
