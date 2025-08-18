import { AccessTime, Place } from "@mui/icons-material";
import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Chip,
  Divider,
  Typography,
} from "@mui/material";
import { Link } from "react-router";
import { formatDate } from "../../../lib/util/util";
import AvatarPopover from "../../../app/shared/components/AvatarPopover";

type Props = {
  activity: Activity;
};

export default function ActivityCard({ activity }: Props) {
  const label = activity.isHost ? "You are hosting" : "You are going";
  const color = activity.isHost
    ? "secondary"
    : activity.isGoing
    ? "warning"
    : "default";

  return (
    // for each activity in activities, display a Card.
    <Card elevation={3} sx={{ borderRadius: 3 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <CardHeader
          avatar={
            <Avatar
              src={activity.hostImageUrl}
              sx={{ height: 80, width: 80 }}
              alt="Image of Host"
            />
          }
          title={activity.title}
          titleTypographyProps={{
            fontWeight: "bold",
            fontSize: 20,
          }}
          subheader={
            <>
              Hosted by{" "}
              <Link to={`/profiles/${activity.hostId}`}>
                {activity.hostDisplayName}
              </Link>
            </>
          }
        />
        <Box display="flex" flexDirection="column" gap={2} marginRight={2}>
          {(activity.isHost || activity.isGoing) && (
            <Chip
              variant="outlined"
              label={label}
              color={color}
              sx={{ borderRadius: 2 }}
            />
          )}
          {activity.isCancelled && (
            <Chip label="Cancelled" color="error" sx={{ borderRadius: 2 }} />
          )}
        </Box>
      </Box>

      <Divider sx={{ marginBottom: 3 }} />

      <CardContent sx={{ padding: 0 }}>
        <Box display="flex" alignItems="center" marginBottom={2} paddingX={2}>
          <Box display="flex" flexGrow={0} alignItems="center">
            <AccessTime sx={{ marginRight: 1 }} />
            <Typography variant="body2" noWrap>
              {formatDate(activity.date)}
            </Typography>
          </Box>

          <Place
            sx={{
              marginLeft: 3,
              marginRight: 1,
            }}
          />
          <Typography variant="body2">{activity.venue}</Typography>
        </Box>

        <Divider />

        <Box
          display="flex"
          gap={2}
          sx={{ backgroundColor: "grey.200", paddingY: 3, paddingLeft: 3 }}
        >
          {activity.attendees.map((attendee) => (
            <AvatarPopover profile={attendee} key={attendee.id} />
          ))}
        </Box>
      </CardContent>

      {/* display CardActions children horizontally. */}
      <CardContent
        sx={{
          paddingBottom: 2,
        }}
      >
        <Typography variant="body2">{activity.description}</Typography>
        <Button
          // Button behaves as a {Link} component from the React Router library.
          component={Link}
          // clicking this button will append activity.id to the URL path.
          // This URL will match the route set in Routes.tsx { path: "activities/:id", element: <ActivityDetailPage /> },
          // so React Router will automatically unmount the ActivityDashboard (parent section of ActivityCard),
          // then mounts ActivityDetailPage.
          to={`/activities/${activity.id}`}
          size="medium"
          variant="contained"
          sx={{ display: "flex", justifySelf: "self-end", borderRadius: 3 }}
        >
          View
        </Button>
      </CardContent>
    </Card>
  );
}
