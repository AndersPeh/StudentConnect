import { createBrowserRouter } from "react-router";
import App from "../layout/App";
import HomePage from "../../features/activities/home/HomePage";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import ActivityForm from "../../features/activities/form/ActivityForm";
import ActivityDetail from "../../features/activities/details/ActivityDetail";

export const router = createBrowserRouter([
  {
    // path must start with / followed by path to child routes
    path: "/",

    // root route.
    element: <App />,

    children: [
      { path: "", element: <HomePage /> },

      // can't use prop drilling from parent component to children component anymore,
      // because router instantiates below paths based on elements without passing props from <App />.
      { path: "activities", element: <ActivityDashboard /> },

      // :id will change based on the activity clicked in ActivityCard.
      { path: "activities/:id", element: <ActivityDetail /> },

      { path: "manage/:id", element: <ActivityForm /> },

      // with a key, when swapping between edit and create form, React will detect it and displays the right form.
      { path: "createActivity", element: <ActivityForm key="create" /> },
    ],
  },
]);
