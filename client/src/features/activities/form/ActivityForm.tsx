import { Box, Button, Paper, TextField, Typography } from "@mui/material";
import type { FormEvent } from "react";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useNavigate, useParams } from "react-router";

export default function ActivityForm() {
  // extract id from URL path then pass it to useActivities to execute the query that is only enabled when there is id.
  const { id } = useParams();

  const { updateActivity, createActivity, activity, isLoadingActivity } =
    useActivities(id);

  const navigate = useNavigate();

  // handleSubmit is an event handler for HTML form.
  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    // prevent the browser's default form submission to handle form submission with Javascript.
    event.preventDefault();

    // event.currentTarget gets HTML form.
    // FormData(event.currentTarget) collects data from HTML form input fields with name attribute.
    // so formData collects data from the form input fields (with name attributes like name="title").
    const formData = new FormData(event.currentTarget);

    //intialises empty object to store form data in key value pairs.
    // keys are strings (name attribute of the textfield) and values (textfield value) are form value like strings or File.
    const data: { [key: string]: FormDataEntryValue } = {};

    // for each formData, the data object will store key:value, like data[title] = Fun Activity;
    formData.forEach((value, key) => {
      data[key] = value;
    });

    // if there is activity passed from useActivities, include it to update activity.
    if (activity) {
      data.id = activity.id;

      // Because FormDataEntryValue can be string or File but Activity properties might be strictly string or something else,
      // two-step assertion as unknown as Activity makes Typescript unable to verify it then assert it that it's Activity type.
      await updateActivity.mutateAsync(data as unknown as Activity);
      navigate(`/activities/${activity.id}`);
    } else {
      // argument 1: data is mutation variable. argument 2: optional, it provides callbacks onSuccss, onError...
      createActivity.mutate(data as unknown as Activity, {
        // get id from response data when the request is successful.
        onSuccess: (id) => {
          navigate(`/activities/${id}`);
        },
      });
    }
  };

  if (isLoadingActivity) return <Typography>Loading activity...</Typography>;

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        {activity ? "Edit Activity" : "Create Activity"}
      </Typography>

      {/* onSubmit is triggered when submit type button is clicked.
      event.currentTarget needs to refer to the form element (Box), so use onSubmit={handleSubmit}
      */}

      <Box
        component="form"
        onSubmit={handleSubmit}
        display="flex"
        flexDirection="column"
        gap={3}
      >
        <TextField name="title" label="Title" defaultValue={activity?.title} />
        <TextField
          name="description"
          label="Description"
          defaultValue={activity?.description}
          multiline
          rows={3}
        />
        <TextField
          name="category"
          label="Category"
          defaultValue={activity?.category}
        />
        <TextField
          name="date"
          label="Date"
          type="date"
          defaultValue={
            activity?.date
              ? new Date(activity.date).toISOString().split("T")[0]
              : new Date().toISOString().split("T")[0]
          }
        />
        <TextField name="city" label="City" defaultValue={activity?.city} />
        <TextField name="venue" label="Venue" defaultValue={activity?.venue} />
        <Box display="flex" justifyContent="end" gap={3}>
          <Button onClick={() => navigate(`/activities`)} color="inherit">
            Cancel
          </Button>
          <Button
            type="submit"
            color="success"
            variant="contained"
            // for user to know it is loading.
            disabled={updateActivity.isPending || createActivity.isPending}
          >
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
