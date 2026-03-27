import { useMemo } from "react";
import { getApiBaseUrl } from "./lib/api";

export function App() {
    const apiBaseUrl = useMemo(() => getApiBaseUrl(), []);

    return (
        <main>
            <h1>Workhours</h1>
            <p>Frontend scaffold is ready.</p>
            <p>API base URL: {apiBaseUrl}</p>
        </main>
    );
}
