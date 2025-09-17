import { Box, Typography } from "@mui/material";
import ActivityCard from "./ActivityCard";
import { useActivities } from "../../../lib/hooks/useActivities";
import { Fragment } from "react/jsx-runtime";

export default function ActivityList() {
  const { activitiesGroup, isLoading } = useActivities();

  if (isLoading) return <Typography>Loading...</Typography>;

  if (!activitiesGroup) return <Typography>No activity found.</Typography>;

  return (
    // main container, display: "flex" displays in row but activates gap property, set flexDirection to be vertical and gap of each ActivityCard 3.
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* activitiesGroup contains pages and pageParams, go to pages, map every pagedList.
      // In every pagedList, go to items, map every item which is an activity.
      */}
      {activitiesGroup.pages.map((pagedList, index) => (
        // Fragment is for attaching the key to every pagedList for React to keep track of pagedList added/ changes/ removed.
        // by grouping ActivityCard together. Fragment doesnt add extra element to the HTML DOM,
        // so the DOM thinks the Box structure is without Fragment.
        // <Box>
        // Gap will be applied properly between each ActivityCard.
        //   <ActivityCard />
        //   <ActivityCard />
        //   <ActivityCard />
        // </Box>
        <Fragment key={index}>
          {pagedList.items.map((activity) => (
            // React needs id to identify each activity to track them individually.
            <ActivityCard key={activity.id} activity={activity} />
          ))}
        </Fragment>
      ))}
    </Box>
  );
}

// activitiesGroup contains:
//     {
//   pages: [
//     Page 1's data
//     { items: [/* activities 1-3 */], nextCursor: "cursor-for-page-2 (DateTime of activity 4)" },
//     Page 2's data
//     { items: [/* activities 4-6 */], nextCursor: "cursor-for-page-3 (DateTime of activity 7)" },
//     Page 3's data
//     { items: [/* activities 7-9 */], nextCursor: null (no activity after this) }
//   ],
//   pageParams: [
//     null, // The pageParam used for page 1 because there is no cursor when the Activities page is first loaded.
//     "cursor-for-page-2", // The pageParam used for page 2
//     "cursor-for-page-3"  // The pageParam used for page 3
//   ]
// }
