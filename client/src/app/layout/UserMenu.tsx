import * as React from "react";
import Button from "@mui/material/Button";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import { useState } from "react";
import {
  Avatar,
  Box,
  Divider,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import { useAccount } from "../../lib/hooks/useAccount";
import { Link } from "react-router";
import { Add, Logout, Person } from "@mui/icons-material";

export default function UserMenu() {
  const { currentUser, logoutUser } = useAccount();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <>
      <Button
        onClick={handleClick}
        color="inherit"
        size="large"
        sx={{ fontSize: "1.1rem" }}
      >
        <Box display="flex" alignItems="center" gap={2}>
          <Avatar />
          {/* if current user exists, shows display name. */}
          {currentUser?.displayName}
        </Box>
      </Button>
      <Menu
        id="basic-menu"
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        MenuListProps={{
          "aria-labelledby": "basic-button",
        }}
      >
        <MenuItem component={Link} to="/createActivity" onClick={handleClose}>
          {/* display Add icon from Material UI with the text. */}
          <ListItemIcon>
            <Add />
          </ListItemIcon>
          <ListItemText>Create Activity</ListItemText>
        </MenuItem>
        <MenuItem component={Link} to="/profile" onClick={handleClose}>
          {/* display Person icon from Material UI with the text. */}
          <ListItemIcon>
            <Person />
          </ListItemIcon>
          <ListItemText>My Profile</ListItemText>
        </MenuItem>
        <Divider />
        <MenuItem
          onClick={() => {
            // use mutate only instead of mutateasync as we dont need to await the result.
            logoutUser.mutate();
            handleClose();
          }}
        >
          {/* display Logout icon from Material UI with the text. */}
          <ListItemIcon>
            <Logout />
          </ListItemIcon>
          <ListItemText>Logout</ListItemText>
        </MenuItem>
      </Menu>
    </>
  );
}
