import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/Request': {
        target: 'http://localhost:5239',
        changeOrigin: true,
        secure: false,
      },
      '/RequestType': {
        target: 'http://localhost:5239',
        changeOrigin: true,
        secure: false,
      },
      '/Product': {
        target: 'http://localhost:5239',
        changeOrigin: true,
        secure: false,
      },
      '/Price': {
        target: 'http://localhost:5239',
        changeOrigin: true,
        secure: false,
      },
      '/Region': {
        target: 'http://localhost:5239',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
