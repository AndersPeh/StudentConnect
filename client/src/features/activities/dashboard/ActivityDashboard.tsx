import { Grid2 } from "@mui/material";
import ActivityList from "./ActivityList";
import ActivityFilters from "./ActivityFilters";

export default function ActivityDashboard() {
  return (
    // sets column based layout out of 12 columns.
    // set spacing of 3 between each Grid2.
    <Grid2 container spacing={3}>
      {/* 7 columns wide Grid, 5 columns remaining to use. */}
      <Grid2 size={8}>
        <ActivityList />
      </Grid2>
      <Grid2 size={4}>
        <ActivityFilters />
      </Grid2>
    </Grid2>
  );
}
