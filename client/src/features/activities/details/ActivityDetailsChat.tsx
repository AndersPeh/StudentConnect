import {
  Box,
  Typography,
  Card,
  CardContent,
  TextField,
  Avatar,
  CircularProgress,
} from "@mui/material";
import { Link, useParams } from "react-router";
import { useComments } from "../../../lib/hooks/useComments";
import { timeAgo } from "../../../lib/util/util";
import { useForm, type FieldValues } from "react-hook-form";
import { observer } from "mobx-react-lite";

// need to wrap function ActivityDetailsChat() as an observer so it will re-render
// when MobX observable used in it changes.
// Because MobX tracks what observables (commentStore.comments and hubConnection) are accessed during the
// render phase of an observer (ActivityDetailsChat),
// when any observable (commentStore.comments and hubConnection) changes,
// MobX will re-render the observer (ActivityDetailsChat)
const ActivityDetailsChat = observer(function ActivityDetailsChat() {
  // Get activityId from router parameter.
  const { id } = useParams();

  // pass the activityId to useComments hook to establish SignalR connection.
  const { commentStore } = useComments(id);

  // destructure methods/ properties from useForm hook.
  const {
    // Track value of registered field and apply validation rules.
    register,
    // Collect value of registered fields, validate the form and call the handler addComment to process.
    handleSubmit,
    // reset the form fields.
    reset,
    // Returns true while the form is submitting.
    formState: { isSubmitting },
  } = useForm();

  // When this handler is called, it invokes SendComment method from the backend's CommentHub.
  const addComment = async (data: FieldValues) => {
    try {
      // pass the current activityId from router parameter and body from data to the server to add comment.
      await commentStore.hubConnection?.invoke("SendComment", {
        activityId: id,
        body: data.body,
      });

      // After sending data to the server, reset the form.
      reset();
    } catch (error) {
      console.log(error);
    }
  };

  // handleKeyPress function receives an argument which is a keyboard event object generated frm an element of type HTMLDivElement.
  // This event contains key pressed.
  const handleKeyPress = (event: React.KeyboardEvent<HTMLDivElement>) => {
    // Only submit the comment if the enter key is pressed and the shift key is not pressed simultaneously.
    // so when user wants to make extra line using shift+enter, it wont trigger submit.
    if (event.key === "Enter" && !event.shiftKey) {
      // prevent the default action that the browser normally submits the form when user pressed enter.
      event.preventDefault();

      // collect body textfield, validate it and takes addComment as a parameter to call it for processing the form.
      handleSubmit(addComment)();
    }
  };

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
                // register the textfield as body and apply validation rule  that the body is required so user wont accidentally submit empty comment.
                {...register("body", { required: true })}
                variant="outlined"
                fullWidth
                multiline
                rows={2}
                placeholder="Enter your comment (Enter to submit, SHIFT + Enter for new line)"
                // For handling keyboard event when a key is pressed while the input is focused.
                onKeyDown={handleKeyPress}
                // show loading when the form is submitting.
                slotProps={{
                  input: {
                    endAdornment: isSubmitting ? (
                      <CircularProgress size={24} />
                    ) : null,
                  },
                }}
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
});

export default ActivityDetailsChat;
