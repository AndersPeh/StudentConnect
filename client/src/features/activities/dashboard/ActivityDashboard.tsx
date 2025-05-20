import { Grid2 } from "@mui/material";
import ActivityList from "./ActivityList";

export default function ActivityDashboard() {
  return (
    // sets column based layout out of 12 columns.
    // set spacing of 3 between each Grid2.
    <Grid2 container spacing={3}>
      {/* 7 columns wide Grid, 5 columns remaining to use. */}
      <Grid2 size={7}>
        <ActivityList />
      </Grid2>
      <Grid2 size={5}>Activity filters</Grid2>
    </Grid2>
  );
}
