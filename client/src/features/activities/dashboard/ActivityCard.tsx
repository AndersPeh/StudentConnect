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

type Props = {
  activity: Activity;
};

export default function ActivityCard({ activity }: Props) {
  const isHost = false;
  const isGoing = false;
  const label = isHost ? "You are hosting" : "You are going";
  const isCancelled = false;
  const color = isHost ? "secondary" : isGoing ? "warning" : "default";

  return (
    // for each activity in activities, display a Card.
    <Card elevation={3} sx={{ borderRadius: 3 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <CardHeader
          avatar={<Avatar sx={{ height: 80, width: 80 }} />}
          title={activity.title}
          titleTypographyProps={{
            fontWeight: "bold",
            fontSize: 20,
          }}
          subheader={
            <>
              Hosted by <Link to={`/profiles/Anders`}>Anders</Link>
            </>
          }
        />
        <Box display="flex" flexDirection="column" gap={2} marginRight={2}>
          {(isHost || isGoing) && (
            <Chip label={label} color={color} sx={{ borderRadius: 2 }} />
          )}
          {isCancelled && (
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
          Attendees
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
          component={Link}
          // clicking this button will append activity.id to the URL path.
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
