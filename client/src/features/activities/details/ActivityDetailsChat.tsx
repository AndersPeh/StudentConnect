import {
  Box,
  Typography,
  Card,
  CardContent,
  TextField,
  Avatar,
} from "@mui/material";
import { Link, useParams } from "react-router";
import { useComments } from "../../../lib/hooks/useComments";
import { timeAgo } from "../../../lib/util/util";

export default function ActivityDetailsChat() {
  // Get activityId from router parameter.
  const { id } = useParams();

  // pass the activityId to useComments hook to establish SignalR connection.
  const { commentStore } = useComments(id);

  return (
    <>
      <Box
        sx={{
          textAlign: "center",
          bgcolor: "primary.main",
          color: "white",
          padding: 2,
        }}
      >
        <Typography variant="h6">Chat about this event</Typography>
      </Box>
      <Card>
        <CardContent>
          <div>
            <form>
              <TextField
                variant="outlined"
                fullWidth
                multiline
                rows={2}
                placeholder="Enter your comment (Enter to submit, SHIFT + Enter for new line)"
              />
            </form>
          </div>

          {/* overflow auto for constraining comments into a box and put a scroll to view more comments. */}
          <Box sx={{ height: 400, overflow: "auto" }}>
            {commentStore.comments.map((comment) => (
              <Box key={comment.id} sx={{ display: "flex", my: 2 }}>
                <Avatar
                  src={comment.imageUrl}
                  alt={"user image"}
                  sx={{ marginRight: 2 }}
                />
                <Box display="flex" flexDirection="column">
                  <Box display="flex" alignItems="center" gap={3}>
                    <Typography
                      component={Link}
                      to={`/profiles/${comment.userId}`}
                      variant="subtitle1"
                      sx={{ fontWeight: "bold", textDecoration: "none" }}
                    >
                      {comment.displayName}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                      {timeAgo(comment.createdAt)}
                    </Typography>
                  </Box>

                  <Typography sx={{ whiteSpace: "pre-wrap" }}>
                    {comment.body}
                  </Typography>
                </Box>
              </Box>
            ))}
          </Box>
        </CardContent>
      </Card>
    </>
  );
}
