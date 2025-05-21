import { useContext } from "react";
import { StoreContext } from "../stores/store";

// a custom hook that returns current value of store which can be destructured into counterStore (available globally).
export function useStore() {
    // subscribes to StoreContext.
    return useContext(StoreContext);
}