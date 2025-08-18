import { createBrowserRouter, Navigate } from "react-router";
import App from "../layout/App";
import HomePage from "../../features/home/HomePage";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import ActivityForm from "../../features/activities/form/ActivityForm";
import ActivityDetailPage from "../../features/activities/details/ActivityDetailPage";
import Counter from "../../features/counter/Counter";
import TestErrors from "../../features/errors/TestErrors";
import NotFound from "../../features/errors/NotFound";
import ServerError from "../../features/errors/ServerError";
import LoginForm from "../../features/account/LoginForm";
import RequireAuth from "./RequireAuth";
import RegisterForm from "../../features/account/RegisterForm";
import ProfilePage from "../../features/profiles/ProfilePage";

export const router = createBrowserRouter([
  {
    // path must start with / followed by path to child routes
    path: "/",

    // root route.
    element: <App />,

    children: [
      {
        element: <RequireAuth />,
        // children (routes) specified here will require Authentication else user will be redirected to login form.
        // basically apply RequireAuth logic to children routes.
        children: [
          { path: "activities", element: <ActivityDashboard /> },

          // :id will change based on the activity clicked in ActivityCard.
          { path: "activities/:id", element: <ActivityDetailPage /> },

          { path: "manage/:id", element: <ActivityForm /> },

          { path: "profiles/:id", element: <ProfilePage /> },

          // with a key, when swapping between edit and create form, React will detect it and displays the right form.
          { path: "createActivity", element: <ActivityForm key="create" /> },
        ],
      },

      { path: "", element: <HomePage /> },

      { path: "counter", element: <Counter /> },

      { path: "errors", element: <TestErrors /> },

      { path: "not-found", element: <NotFound /> },

      { path: "server-error", element: <ServerError /> },

      { path: "login", element: <LoginForm /> },

      { path: "register", element: <RegisterForm /> },

      // when user visits a page that doesnt exist, the user will be redirected to not found.
      { path: "*", element: <Navigate replace to="/not-found" /> },
    ],
  },
]);
