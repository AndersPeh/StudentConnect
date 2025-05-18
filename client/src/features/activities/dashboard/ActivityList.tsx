import { Box } from "@mui/material";
import ActivityCard from "./ActivityCard";

type Props = {
  activities: Activity[];
};

export default function ActivityList({ activities }: Props) {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {activities.map((activity) => (
        // React needs id to identify each activity to track them individually.
        <ActivityCard key={activity.id} activity={activity} />
      ))}
    </Box>
  );
}
