import { Button, styled, type ButtonProps } from "@mui/material";
import type { LinkProps } from "react-router";

// In ActivityDetailsHeader, there are some buttons that dont have "to", so use Partial<LinkProps> to make LinkProps optional.
type StyledButtonProps = ButtonProps & Partial<LinkProps>;

// apply the theme style on top of the Button component of MUI. so there is a new compoent called StyledButton which has additional styling and inherits
// features of Button component. StyledButton is a button that must have ButtonProps and optional LinkProps.
const StyledButton = styled(Button)<StyledButtonProps>(({ theme }) => ({
  // & refers to the button itself, .Mui-disabled means MUI automatically adds to the button when it's disabled.
  // so it means When the button is disabled, executes the styles inside.
  "&.Mui-disabled": {
    // It will set  grey background color and disabled text color from the theme.
    backgroundColor: theme.palette.grey[600],
    color: theme.palette.text.disabled,
  },
}));

export default StyledButton;
