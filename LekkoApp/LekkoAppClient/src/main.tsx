import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import "./index.css"
import App from "./App.tsx"
import { mountChartAreaDefault } from "@/mount-main-chart";

const rootElement = document.getElementById("root")

if (rootElement) {
    createRoot(rootElement).render(
        <StrictMode>
            <App />
        </StrictMode>
    )
}

;(window as any).mountChartAreaDefault = mountChartAreaDefault
