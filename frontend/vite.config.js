import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Node.js process.env is always reliable here (unlike browser bundle injection).
// AppHost sets REACT_APP_API_URL; .env.local can set VITE_API_URL for standalone dev.
const apiTarget = process.env.REACT_APP_API_URL
  || process.env.VITE_API_URL
  || 'http://localhost:5000';

export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT || '3000'),
    proxy: {
      '/catalog': { target: apiTarget, changeOrigin: true },
      '/cart':    { target: apiTarget, changeOrigin: true },
      '/orders':  { target: apiTarget, changeOrigin: true },
    },
  },
})
