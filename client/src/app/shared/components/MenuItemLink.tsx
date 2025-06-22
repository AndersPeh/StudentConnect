import { MenuItem } from "@mui/material";
import type { ReactNode } from "react";
import { NavLink } from "react-router";

export default function MenuItemLink({
  // these 2 props are needed to use MenuItemLink.
  children,
  to,
}: // to specify the type of children and to.
{
  // children can be anything React can render inside the menu item like Group, Typography or string like Activities, Create Activity.
  children: ReactNode;
  to: string;
}) {
  return (
    <MenuItem
      // NavLink adds an active class when the current route matches. for active menu styling.
      // it also enables react router navigation using to={to}.
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
