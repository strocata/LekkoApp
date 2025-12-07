import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from "path";


// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "../wwwroot/app",
    emptyOutDir: true,
    rollupOptions: {
      input: {
        main: "./index.html",
        exampleWidget: "./src/mount-main-chart.tsx"
      }
    }
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
      "@components": path.resolve(__dirname, "./src/components"),
    },
  }
})
