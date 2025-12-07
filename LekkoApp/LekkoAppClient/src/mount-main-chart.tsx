
import { createRoot } from "react-dom/client"
import { ChartAreaDefault } from "@/charts/MainChart"
import "./index.css"

export function mountChartAreaDefault(id: string) {
    const el = document.getElementById(id)
    if (!el) {
        console.error("Mount point not found:", id)
        return
    }

    const root = createRoot(el)
    root.render(<ChartAreaDefault />)
}

(window as any).mountChartAreaDefault = mountChartAreaDefault

// Auto-mount if the element exists
const MOUNT_POINT_ID = "chart-area-default"
if (document.getElementById(MOUNT_POINT_ID)) {
    mountChartAreaDefault(MOUNT_POINT_ID)
}
