import { Navigate, Outlet, useLocation } from "react-router";
import { useAccount } from "../../lib/hooks/useAccount";
import { Typography } from "@mui/material";

export default function RequireAuth() {
  const { currentUser, loadingUserInfo } = useAccount();
  // to get user current location and send user back to this location after authentication.
  const location = useLocation();

  if (loadingUserInfo) return <Typography>Loading...</Typography>;

  if (!currentUser)
    // if user is not logged, navigate user to the login page. Remember the state of user current location,
    // so user can be sent back to where they were after logging in instead of them trying to find the page again.
    return <Navigate to="login" state={{ from: location }}></Navigate>;

  // Outlet from React Router renders the page which is placed as children in Routes.tsx and using RequireAuth as an element.
  // It allows RequireAuth to guard routes specified as children in Routes.tsx.
  return <Outlet />;
}
