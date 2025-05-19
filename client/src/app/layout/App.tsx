import { Box, Container, CssBaseline, Typography } from "@mui/material";
import { useState } from "react";
import NavBar from "./NavBar";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import { useActivities } from "../../lib/hooks/useActivities";

function App() {
  // type of activities is <Activity[]> defined in index.d.ts
  const [selectedActivity, setSelectedActivity] = useState<
    Activity | undefined
  >(undefined);
  const [editMode, setEditMode] = useState(false);
  // custom hook created.
  const { activities, isPending } = useActivities();

  const handleSelectActivity = (id: string) => {
    // ! overwrites Typescript type safety.
    setSelectedActivity(activities!.find((x) => x.id === id));
  };

  const handleCancelSelectActivity = () => {
    setSelectedActivity(undefined);
  };

  const handleOpenForm = (id?: string) => {
    if (id) handleSelectActivity(id);
    else handleCancelSelectActivity();
    setEditMode(true);
  };

  const handleFormClose = () => {
    setEditMode(false);
  };

  const handleSubmitForm = (activity: Activity) => {
    // if (activity.id) {
    //   setActivities(
    //     // when the updated activity.id === activity stored.id, replace the particular activity with new activity.
    //     // if dont match, just use the original activity.
    //     activities.map((x) => (x.id === activity.id ? activity : x))
    //   );
    //   setSelectedActivity(activity);
    // } else {
    //   const newActivity = { ...activity, id: activities.length.toString() };
    //   setActivities([...activities, newActivity]);
    //   setSelectedActivity(newActivity);
    // }
    console.log(activity);
    setEditMode(false);
  };

  const handleDelete = (id: string) => {
    console.log(id);
    setSelectedActivity(undefined);
    setEditMode(false);
  };

  return (
    // can only return 1 thing in Javascript function, must wrap everything in a section.
    // background of the entire web app homepage.
    // 100vh makes Box same height as browser window, making background color covers the entire browser.
    <Box sx={{ backgroundColor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
      <NavBar openForm={handleOpenForm} />
      <Container maxWidth="xl" sx={{ marginTop: 3 }}>
        {/* pass activities prop to ActivityDashboard. */}
        {!activities || isPending ? (
          <Typography>Loading...</Typography>
        ) : (
          <ActivityDashboard
            activities={activities}
            selectActivity={handleSelectActivity}
            cancelSelectActivity={handleCancelSelectActivity}
            selectedActivity={selectedActivity}
            editMode={editMode}
            openForm={handleOpenForm}
            closeForm={handleFormClose}
            submitForm={handleSubmitForm}
            deleteActivity={handleDelete}
          />
        )}
      </Container>
    </Box>
  );
}

export default App;
