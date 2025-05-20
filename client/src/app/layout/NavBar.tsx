import { Group } from "@mui/icons-material";
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Container,
  MenuItem,
} from "@mui/material";
import { NavLink } from "react-router";
import MenuItemLink from "../shared/components/MenuItemLink";

export default function NavBar() {
  return (
    // Box allows NavBar to take full width.
    <Box sx={{ flexGrow: 1 }}>
      {/* main NavBar */}
      <AppBar
        position="static"
        sx={{
          backgroundImage:
            "linear-gradient(135deg, #182a73 0%, #218aae 69%, #20a7ac 89%)",
        }}
      >
        {/* constrains NavBar's content width on wide screen, center it.  */}
        <Container maxWidth="xl">
          {/* arrange Items horizontally. */}
          <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
            {/* Section 1 of NavBar, Group is icon. */}
            <Box>
              {/* Homepage path is "/" */}
              <MenuItem
                component={NavLink}
                to="/"
                sx={{ display: "flex", gap: 2 }}
              >
                <Group fontSize="large" />
                <Typography variant="h4" fontWeight="bold">
                  Student Connect
                </Typography>
              </MenuItem>
            </Box>
            {/* Section 2 of NavBar with options to select. */}
            <Box sx={{ display: "flex" }}>
              <MenuItemLink to="/activities">Activities</MenuItemLink>
              <MenuItemLink to="/createActivity">Create Activity</MenuItemLink>
            </Box>
            {/* Section 3 of NavBar. */}
            <MenuItem>User Menu</MenuItem>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
