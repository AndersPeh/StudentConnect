import { Box, Paper, Tab, Tabs } from "@mui/material";
import { useState, type SyntheticEvent } from "react";
import ProfilePhotos from "./ProfilePhotos";

export default function ProfileContent() {
  // useState to knowing which tab is being viewed through tab index. Displays About (index 0) by default.
  const [value, setValue] = useState(0);

  // Because the first parameter of onChange is event: React.SyntheticEvent but we dont need it.
  // use "_" for ignoring element not used, still need to specify the type.
  // newValue: number is the index of the clicked tab.
  const handleChange = (_: SyntheticEvent, newValue: number) => {
    // Whenever a tab is clicked, the index of the tab will be set as the value to display the content of the tab.
    setValue(newValue);
  };

  // Left side (labels) are all displayed in Tabs.
  // Right side (content) is displayed in the Box according to the index of the tab clicked.
  const tabContent = [
    { label: "About", content: <div>About</div> },
    { label: "Photos", content: <ProfilePhotos /> },
    { label: "Events", content: <div>Events</div> },
    { label: "Followers", content: <div>Followers</div> },
    { label: "Following", content: <div>Following</div> },
  ];

  return (
    // Make a white card container with elevation shadow, inner padding of 3. marginTop to make space after ProfileHeader.
    <Box
      component={Paper}
      marginTop={2}
      padding={3}
      elevation={3}
      height={500}
      // Flex means display items side by side (Tabs on the left and Box on the right in a row).
      sx={{ display: "flex", alignItems: "flex-start", borderRadius: 3 }}
    >
      {/* display Tabs vertically. */}
      <Tabs
        orientation="vertical"
        // Highlight the clicked tab according to useState.
        value={value}
        // update the value of the useState when user selects a new Tab.
        // The index of the tab will be provided by MUI whenever user clicks a new Tab.
        onChange={handleChange}
        sx={{ borderRight: 1, height: 450, minWidth: 200 }}
      >
        {/* MUI automatically assigns index 0,1,2... according to render order to every Tab. */}
        {tabContent.map((tab, index) => (
          <Tab key={index} label={tab.label} sx={{ marginRight: 3 }} />
        ))}
      </Tabs>
      {/* Display the content using index of the tab clicked: tabContent[value]. */}
      <Box sx={{ flexGrow: 1, padding: 3 }}>{tabContent[value].content}</Box>
    </Box>
  );
}
