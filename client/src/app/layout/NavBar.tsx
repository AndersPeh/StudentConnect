import { Group } from "@mui/icons-material";
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Button,
  Container,
  MenuItem,
} from "@mui/material";

type Props = {
  openForm: () => void;
};

export default function NavBar({ openForm }: Props) {
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
              <MenuItem sx={{ display: "flex", gap: 2 }}>
                <Group fontSize="large" />
                <Typography variant="h4" fontWeight="bold">
                  Student Connect
                </Typography>
              </MenuItem>
            </Box>
            {/* Section 2 of NavBar with options to select. */}
            <Box sx={{ display: "flex" }}>
              <MenuItem
                sx={{
                  fontSize: "1.2rem",
                  textTransform: "uppercase",
                  fontWeight: "bold",
                }}
              >
                Activities
              </MenuItem>
              <MenuItem
                sx={{
                  fontSize: "1.2rem",
                  textTransform: "uppercase",
                  fontWeight: "bold",
                }}
              >
                About
              </MenuItem>
              <MenuItem
                sx={{
                  fontSize: "1.2rem",
                  textTransform: "uppercase",
                  fontWeight: "bold",
                }}
              >
                Contact
              </MenuItem>
            </Box>
            {/* Section 3 of NavBar. */}
            <Button
              size="large"
              variant="contained"
              color="warning"
              onClick={openForm}
            >
              Create Activity
            </Button>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
