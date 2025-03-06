import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true, // This allows the server to be accessible externally
    port: 5173,
  },
  resolve: {
    extensions: ['.js', '.jsx']
  },
  build: {
    rollupOptions: {
      external: ['react-router-dom']
    }
  }
})