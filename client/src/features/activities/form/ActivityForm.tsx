import { Box, Button, Paper, Typography } from "@mui/material";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useParams } from "react-router";
import { useForm } from "react-hook-form";
import { useEffect } from "react";
import {
  activitySchema,
  type ActivitySchema,
} from "../../../lib/schemas/activitySchema";
import { zodResolver } from "@hookform/resolvers/zod";
import TextInput from "../../../app/shared/components/TextInput";
import SelectInput from "../../../app/shared/components/SelectInput";
import { categoryOptions } from "./categoryOptions";
import DateTimeInput from "../../../app/shared/components/DateTimeInput";

export default function ActivityForm() {
  const {
    reset,
    handleSubmit,
    // control will be passed to useController in TextInput to manage every TextInput. control is typed with <ActivitySchema>.
    // In useController, control tells TextInput which form it belongs to and how to manage states like onSubmit, onBlur etc...
    control,
    // useForm type is ActivitySchema, so data structure of textfields in ActivityForm must match ActivitySchema.
  } = useForm<ActivitySchema>({
    // makes resolver validate whenever a textfield is touched (when blurred).
    mode: "onTouched",
    // resolver validates textfields value against activitySchema.
    resolver: zodResolver(activitySchema),
  });

  // extract id from URL path then pass it to useActivities to execute the query that is only enabled when there is id.
  const { id } = useParams();

  const { updateActivity, createActivity, activity, isLoadingActivity } =
    useActivities(id);

  // It will be triggered when activity or reset changes.
  useEffect(() => {
    // if id exists (means editing activity, activity will be truthy), reset will fill in the form with the data from activity object.
    if (activity) reset(activity);
  }, [activity, reset]);

  //
  const onSubmit = (data: ActivitySchema) => {
    console.log(data);
  };

  if (isLoadingActivity) return <Typography>Loading activity...</Typography>;

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        {activity ? "Edit Activity" : "Create Activity"}
      </Typography>

      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        display="flex"
        flexDirection="column"
        gap={3}
      >
        {/*
        pass control to TextInput to use in useController, control will tell useController about details of ActivityForm,
        React-hook-form connects to TextInput through control. When anything happens like onBlur, it will tell React-hook-form and it will trigger resolver.
        name will be used in useController to identify which field in the form it should manage.
        label is for Textfield to display label text.*/}
        <TextInput label="Title" control={control} name="title" />
        <TextInput
          label="Description"
          control={control}
          name="description"
          multiline
          rows={3}
        />
        <SelectInput
          label="Category"
          control={control}
          name="category"
          items={categoryOptions}
        />
        <DateTimeInput label="Date" control={control} name="date" />
        <TextInput label="City" control={control} name="city" />
        <TextInput label="Venue" control={control} name="venue" />

        <Box display="flex" justifyContent="end" gap={3}>
          <Button color="inherit">Cancel</Button>
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
