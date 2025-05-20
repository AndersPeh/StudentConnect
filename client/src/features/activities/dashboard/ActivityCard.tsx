import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  Chip,
  Typography,
} from "@mui/material";
import { useActivities } from "../../../lib/hooks/useActivities";
import { Link } from "react-router";

type Props = {
  activity: Activity;
};

export default function ActivityCard({ activity }: Props) {
  const { deleteActivity } = useActivities();

  return (
    // for each activity in activities, display a Card.
    <Card sx={{ borderRadius: 3 }}>
      <CardContent>
        <Typography variant="h5">{activity.title}</Typography>
        <Typography sx={{ color: "text.secondary", marginBottom: 1 }}>
          {activity.date}
        </Typography>
        <Typography variant="body2">{activity.description}</Typography>
        <Typography variant="subtitle1">
          {activity.city} / {activity.venue}
        </Typography>
      </CardContent>

      {/* display CardActions children horizontally. */}
      <CardActions
        sx={{
          display: "flex",
          justifyContent: "space-between",
          paddingBottom: 2,
        }}
      >
        <Chip label={activity.category} variant="outlined" />
        <Box display="flex" gap={3}>
          <Button
            component={Link}
            // clicking this button will append activity.id to the URL path.
            to={`/activities/${activity.id}`}
            onClick={() => {}}
            size="medium"
            variant="contained"
          >
            View
          </Button>
          <Button
            onClick={() => deleteActivity.mutate(activity.id)}
            disabled={deleteActivity.isPending}
            color="error"
            size="medium"
            variant="contained"
          >
            Delete
          </Button>
        </Box>
      </CardActions>
    </Card>
  );
}
