import { MenuItem } from "@mui/material";
import type { ReactNode } from "react";
import { NavLink } from "react-router";

// children refer to sections inside MenuItem like Group, Typography or string like Activities, Create Activity.
export default function MenuItemLink({
  children,
  to,
}: {
  children: ReactNode;
  to: string;
}) {
  return (
    <MenuItem
      component={NavLink}
      //  paths like "/activities".
      to={to}
      sx={{
        fontSize: "1.2rem",
        textTransform: "uppercase",
        fontWeight: "bold",
        color: "inherit",
        // apply this style if the class is active.
        "&.active": {
          color: "yellow",
        },
      }}
    >
      {children}
    </MenuItem>
  );
}
