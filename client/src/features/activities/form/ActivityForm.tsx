import { Box, Button, Paper, TextField, Typography } from "@mui/material";
import type { FormEvent } from "react";

type Props = {
  closeForm: () => void;
  activity?: Activity;
  submitForm: (activity: Activity) => void;
};

export default function ActivityForm({
  closeForm,
  activity,
  submitForm,
}: Props) {
  // handleSubmit is an event handler for HTML form.
  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    // prevent the browser's default form submission to handle form submission with Javascript.
    event.preventDefault();
    // event.currentTarget gets HTML form.
    // FormData(event.currentTarget) collects data from HTML form input fields with name attribute.
    // so formData collects data from the form input fields (with name attributes like name="title").
    const formData = new FormData(event.currentTarget);
    //intialises empty object to store form data in key value pairs.
    // keys are strings (name attribute of the textfield) and values (textfield value) are form value like strings or File.
    const data: { [key: string]: FormDataEntryValue } = {};
    // for each data, the data object will store key:value, like data[title] = Fun Activity;
    formData.forEach((value, key) => {
      data[key] = value;
    });

    if (activity) data.id = activity.id;
    // Because FormDataEntryValue can be string or File but Activity properties might be strictly string or something else,
    // two-step assertion as unknown as Activity makes Typescript unable to verify it then assert it that it's Activity type.
    submitForm(data as unknown as Activity);
  };

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        Create Activity
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
          defaultValue={activity?.date}
        />
        <TextField name="city" label="City" defaultValue={activity?.city} />
        <TextField name="venue" label="Venue" defaultValue={activity?.venue} />
        <Box display="flex" justifyContent="end" gap={3}>
          <Button onClick={closeForm} color="inherit">
            Cancel
          </Button>
          <Button type="submit" color="success" variant="contained">
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
