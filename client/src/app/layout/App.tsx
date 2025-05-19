import { Box, Container, CssBaseline } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import NavBar from "./NavBar";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";

function App() {
  // type of activities is <Activity[]> defined in index.d.ts
  const [activities, setActivities] = useState<Activity[]>([]);
  const [selectedActivity, setSelectedActivity] = useState<
    Activity | undefined
  >(undefined);
  const [editMode, setEditMode] = useState(false);

  useEffect(() => {
    // axios parses response automatically.
    axios
      .get<Activity[]>("https://localhost:5001/api/activities")
      .then((response) => setActivities(response.data));

    return () => {};
  }, []);

  const handleSelectActivity = (id: string) => {
    setSelectedActivity(activities.find((x) => x.id === id));
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
    if (activity.id) {
      setActivities(
        // when the updated activity.id === activity stored.id, replace the particular activity with new activity.
        // if dont match, just use the original activity.
        activities.map((x) => (x.id === activity.id ? activity : x))
      );
      setSelectedActivity(activity);
    } else {
      const newActivity = { ...activity, id: activities.length.toString() };
      setActivities([...activities, newActivity]);
      setSelectedActivity(newActivity);
    }
    setEditMode(false);
  };

  const handleDelete = (id: string) => {
    setActivities(activities.filter((x) => x.id !== id));
    setSelectedActivity(undefined);
    setEditMode(false);
  };

  return (
    // can only return 1 thing in Javascript function, must wrap everything in a section.
    // background of the entire web app homepage.
    <Box sx={{ backgroundColor: "#eeeeee" }}>
      <CssBaseline />
      <NavBar openForm={handleOpenForm} />
      <Container maxWidth="xl" sx={{ marginTop: 3 }}>
        {/* pass activities prop to ActivityDashboard. */}
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
      </Container>
    </Box>
  );
}

export default App;
