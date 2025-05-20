import { Box, Typography } from "@mui/material";
import ActivityCard from "./ActivityCard";
import { useActivities } from "../../../lib/hooks/useActivities";

export default function ActivityList() {
  const { activities, isPending } = useActivities();

  if (!activities || isPending) return <Typography>Loading...</Typography>;

  return (
    // main container, display: "flex" displays in row but activates gap property, set flexDirection to be vertical and gap of each ActivityCard 3.
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {activities.map((activity) => (
        // React needs id to identify each activity to track them individually.
        <ActivityCard key={activity.id} activity={activity} />
      ))}
    </Box>
  );
}
