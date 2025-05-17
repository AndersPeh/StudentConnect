import { List, ListItem, ListItemText, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";

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
    <>
      <Typography variant="h3">Student Connect</Typography>

      <List>
        {activities.map((activity) => (
          // React needs id to identify each activity to track them individually.
          <ListItem key={activity.id}>
            <ListItemText>{activity.title}</ListItemText>
          </ListItem>
        ))}
      </List>
    </>
  );
}

export default App;
