import { Box, Container, CssBaseline } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";
import NavBar from "./NavBar";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";

function App() {
  // type of activities is <Activity[]> defined in index.d.ts
  const [activities, setActivities] = useState<Activity[]>([]);

  useEffect(() => {
    // axios parses response automatically.
    axios
      .get<Activity[]>("https://localhost:5001/api/activities")
      .then((response) => setActivities(response.data));

    return () => {};
  }, []);

  return (
    // can only return 1 thing in Javascript function, must wrap everything in a section.
    <Box sx={{ backgroundColor: "#eeeeee" }}>
      <CssBaseline />
      <NavBar />
      <Container maxWidth="xl" sx={{ marginTop: 3 }}>
        {/* pass activities prop to ActivityDashboard. */}
        <ActivityDashboard activities={activities} />
      </Container>
    </Box>
  );
}

export default App;
