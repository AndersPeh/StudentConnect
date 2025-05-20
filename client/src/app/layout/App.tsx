import { Box, Container, CssBaseline } from "@mui/material";
import NavBar from "./NavBar";
import { Outlet, useLocation } from "react-router";
import HomePage from "../../features/activities/home/HomePage";

function App() {
  // check current path for location.
  const location = useLocation();

  return (
    // can only return 1 thing in Javascript function, must wrap everything in a section.
    // background of the entire web app homepage.
    // 100vh makes Box same height as browser window, making background color covers the entire browser.
    <Box sx={{ backgroundColor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
      {location.pathname === "/" ? (
        <HomePage />
      ) : (
        <>
          <NavBar />
          <Container maxWidth="xl" sx={{ marginTop: 3 }}>
            {/* when user browses to one of its child routes, the element of the child route will replace the outlet section. */}
            <Outlet />
          </Container>
        </>
      )}
    </Box>
  );
}

export default App;
