import { Box } from "@mui/material";
import ActivityCard from "./ActivityCard";

type Props = {
  activities: Activity[];
  selectActivity: (id: string) => void;
  deleteActivity: (id: string) => void;
};

export default function ActivityList({
  activities,
  selectActivity,
  deleteActivity,
}: Props) {
  return (
    // main container, display: "flex" displays in row but activates gap property, set flexDirection to be vertical and gap of each ActivityCard 3.
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {activities.map((activity) => (
        // React needs id to identify each activity to track them individually.
        <ActivityCard
          key={activity.id}
          activity={activity}
          selectActivity={selectActivity}
          deleteActivity={deleteActivity}
        />
      ))}
    </Box>
  );
}
