import { Card, Badge, CardMedia, Box, Typography, Button } from "@mui/material";
import { Link } from "react-router";
import { formatDate } from "../../../lib/util/util";
import { useActivities } from "../../../lib/hooks/useActivities";

type Props = {
  activity: Activity;
};

export default function ActivityDetailsHeader({ activity }: Props) {
  // pass activity.id to useActivities for it to be used in parts other than queryFn/ mutationFn like queryKey, enabled etc.
  const { updateAttendance } = useActivities(activity.id);

  return (
    <Card
      sx={{
        position: "relative",
        marginBottom: 2,
        backgroundColor: "transparent",
        overflow: "hidden",
      }}
    >
      {activity.isCancelled && (
        <Badge
          sx={{ position: "absolute", left: 40, top: 20, zIndex: 1000 }}
          color="error"
          badgeContent="Cancelled"
        />
      )}
      <CardMedia
        component="img"
        height="300"
        image={`/images/categoryImages/${activity.category}.jpg`}
        alt={`${activity.category} image`}
      />
      <Box
        sx={{
          position: "absolute",
          bottom: 0,
          width: "100%",
          color: "white",
          padding: 2,
          display: "flex",
          flexDirection: "row",
          justifyContent: "space-between",
          alignItems: "flex-end",
          background:
            "linear-gradient(to top, rgba(0, 0, 0, 1.0), transparent)",
          boxSizing: "border-box",
        }}
      >
        {/* Text Section */}
        <Box>
          <Typography variant="h4" sx={{ fontWeight: "bold" }}>
            {activity.title}
          </Typography>
          <Typography variant="subtitle1">
            {formatDate(activity.date)}
          </Typography>
          <Typography variant="subtitle2">
            Hosted by{" "}
            <Link
              to={`/profiles/${activity.hostId}`}
              style={{ color: "white", fontWeight: "bold" }}
            >
              {activity.hostDisplayName}
            </Link>
          </Typography>
        </Box>

        {/* Buttons aligned to the right */}
        <Box sx={{ display: "flex", gap: 2 }}>
          {/* When user is a host, updateAttendance.mutate will reverse the activity status when the button is pressed as set in the backend. 
          By default, the activity is active. So when user clicks it, it will be cancelled, then press again to re-activate.... */}
          {activity.isHost ? (
            <>
              <Button
                variant="contained"
                color={activity.isCancelled ? "success" : "error"}
                // Pass activity.id to updateAttendance.mutate to use it in mutationFn.
                onClick={() => updateAttendance.mutate(activity.id)}
                // isPending status from React Query.
                disabled={updateAttendance.isPending}
              >
                {activity.isCancelled
                  ? "Re-activate Activity"
                  : "Cancel Activity"}
              </Button>
              <Button
                variant="contained"
                color="primary"
                component={Link}
                to={`/manage/${activity.id}`}
                disabled={activity.isCancelled}
              >
                Manage Event
              </Button>
            </>
          ) : (
            <Button
              variant="contained"
              color={activity.isGoing ? "primary" : "info"}
              // the server will check if the user exists in the attendee database. If user exists, then user will be removed.
              // else, user will be added.
              onClick={() => updateAttendance.mutate(activity.id)}
              disabled={updateAttendance.isPending || activity.isCancelled}
            >
              {activity.isGoing ? "Cancel Attendance" : "Join Activity"}
            </Button>
          )}
        </Box>
      </Box>
    </Card>
  );
}
