import { Grid2 } from "@mui/material";
import ActivityList from "./ActivityList";

// ActivityDashboard expects Props type: property named activities and activities must be array of Activity objects.
type Props = {
  activities: Activity[];
};

// destructure property named activities from Props type object received, put that activities into local variable also named activities.
export default function ActivityDashboard({ activities }: Props) {
  return (
    <Grid2 container>
      {/* 9 columns wide Grid */}
      <Grid2 size={9}>
        <ActivityList activities={activities}></ActivityList>
      </Grid2>
    </Grid2>
  );
}
