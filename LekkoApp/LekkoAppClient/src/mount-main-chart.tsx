import React from "react"
import ReactDOM from "react-dom/client"
import MainChart from "./charts/MainChart.tsx"
import "./index.css"

const rootElement = document.getElementById("main-chart")

if (rootElement) {
    const root = ReactDOM.createRoot(rootElement)
    root.render(
        <React.StrictMode>
            <MainChart />
        </React.StrictMode>
    )
}
