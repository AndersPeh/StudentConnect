import { Divider, Paper, Typography } from "@mui/material";
import { useLocation } from "react-router";

export default function ServerError() {
  // extract state from URL path.
  const { state } = useLocation();

  return (
    <Paper>
      {state.error ? (
        <>
          <Typography
            gutterBottom
            variant="h3"
            sx={{ paddingX: 4, paddingTop: 2 }}
            color="secondary"
          >
            {state.error?.message || "There has been an error"}
          </Typography>
          <Divider />
          <Typography variant="body1" sx={{ padding: 4 }}>
            {state.error?.details || "Internal server error"}
          </Typography>
        </>
      ) : (
        <Typography variant="h5">Server Error</Typography>
      )}
    </Paper>
  );
}
