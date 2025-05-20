import { Box, Container, CssBaseline } from "@mui/material";
import NavBar from "./NavBar";
import { Outlet } from "react-router";

function App() {
  return (
    // can only return 1 thing in Javascript function, must wrap everything in a section.
    // background of the entire web app homepage.
    // 100vh makes Box same height as browser window, making background color covers the entire browser.
    <Box sx={{ backgroundColor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
      <NavBar />
      <Container maxWidth="xl" sx={{ marginTop: 3 }}>
        {/* when user browses to one of its child routes, the element of the child route will replace the outlet section. */}
        <Outlet />
      </Container>
    </Box>
  );
}

export default App;
