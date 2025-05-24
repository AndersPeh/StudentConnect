import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./app/layout/styles.css";
import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import "react-toastify/dist/ReactToastify.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { RouterProvider } from "react-router";
import { router } from "./app/router/Routes";
import { store, StoreContext } from "./lib/stores/store";
import { ToastContainer } from "react-toastify";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFnsV3";

const queryClient = new QueryClient();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      {/* provide the store to the entire application. When any child components calls useStore(), 
    it will call useContext(StoreContext), React will find the store here, then pass it to useContext,
    then the component that called useStore will value of store.*/}
      <StoreContext.Provider value={store}>
        {/* For child components to access to React Query hooks like useQuery and useMutation. */}
        <QueryClientProvider client={queryClient}>
          {/* floating icon in the browser to debug. */}
          <ReactQueryDevtools />
          <ToastContainer position="bottom-right" theme="colored" />
          {/* needs access from Provider of Mobx for state management and React QueryClientProvider for managing server state, caching. */}
          <RouterProvider router={router} />
        </QueryClientProvider>
      </StoreContext.Provider>
    </LocalizationProvider>
  </StrictMode>
);
