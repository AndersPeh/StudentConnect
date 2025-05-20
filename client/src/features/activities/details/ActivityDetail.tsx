import {
  Button,
  Card,
  CardActions,
  CardContent,
  CardMedia,
  Typography,
} from "@mui/material";

import { Link, useNavigate, useParams } from "react-router";
import { useActivities } from "../../../lib/hooks/useActivities";

export default function ActivityDetail() {
  // useNavigate of React Router navigates user to another route.
  const navigate = useNavigate();

  // useParams extracts id from the parameter of the URL path ("activities/:id") when user clicks ActivityCard.
  const { id } = useParams();

  // when there is id, pass id to useActivities, it will execute useQuery for the specific activity and returns isLoadingActivity and activity.
  const { activity, isLoadingActivity } = useActivities(id);

  if (isLoadingActivity) return <Typography>Loading...</Typography>;

  if (!activity) return <Typography>Activity Not Found.</Typography>;

  return (
    <Card sx={{ borderRadius: 3 }}>
      <CardMedia
        component="img"
        src={`/images/categoryImages/${activity.category}.jpg`}
      />
      <CardContent>
        <Typography variant="h5">{activity.title}</Typography>
        <Typography variant="subtitle1" fontWeight="light">
          {activity.date}
        </Typography>
        <Typography variant="body1">{activity.description}</Typography>
      </CardContent>
      <CardActions>
        <Button component={Link} to={`/manage/${activity.id}`} color="primary">
          Edit
        </Button>
        {/* Instead of using component Link or NavLink, implment use navigate from React Router. */}
        <Button onClick={() => navigate("/activities")} color="inherit">
          Cancel
        </Button>
      </CardActions>
    </Card>
  );
}
