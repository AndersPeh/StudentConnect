import { Grid2 } from "@mui/material";
import ActivityList from "./ActivityList";
import ActivityDetail from "../details/ActivityDetail";
import ActivityForm from "../form/ActivityForm";

// ActivityDashboard expects Props type: properties and methods with their types.
type Props = {
  activities: Activity[];
  selectActivity: (id: string) => void;
  cancelSelectActivity: () => void;
  selectedActivity?: Activity;
  openForm: (id: string) => void;
  closeForm: () => void;
  editMode: boolean;
  submitForm: (activity: Activity) => void;
  deleteActivity: (id: string) => void;
};

// destructure properties from Props type object received, put that them into local variable with same names.
export default function ActivityDashboard({
  activities,
  selectActivity,
  cancelSelectActivity,
  selectedActivity,
  openForm,
  closeForm,
  editMode,
  submitForm,
  deleteActivity,
}: Props) {
  return (
    // sets column based layout out of 12 columns.
    // set spacing of 3 between each Grid2.
    <Grid2 container spacing={3}>
      {/* 7 columns wide Grid, 5 columns remaining to use. */}
      <Grid2 size={7}>
        <ActivityList
          activities={activities}
          selectActivity={selectActivity}
          deleteActivity={deleteActivity}
        />
      </Grid2>
      <Grid2 size={5}>
        {/* if selectActivity and !editMode are truthy, execute the most right component.*/}
        {selectedActivity && !editMode && (
          <ActivityDetail
            activity={selectedActivity}
            cancelSelectActivity={cancelSelectActivity}
            openForm={openForm}
          />
        )}
        {/* if it is in editMode, displays ActivityForm. */}
        {editMode && (
          <ActivityForm
            closeForm={closeForm}
            activity={selectedActivity}
            submitForm={submitForm}
          />
        )}
      </Grid2>
    </Grid2>
  );
}
