import { Box, Typography } from "@mui/material";
import ActivityCard from "./ActivityCard";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useInView } from "react-intersection-observer";
import { useEffect } from "react";

export default function ActivityList() {
  const { activitiesGroup, isLoading, hasNextPage, fetchNextPage } =
    useActivities();
  // threshold: 0.5 means when user sees half of the activity that is tagged as ref, it will automatically load next page.
  // ref tells useInView which element to watch, inView is false by default.
  // inView returns true when 50% of the ref element becomes visible.
  const { ref, inView } = useInView({ threshold: 0.5 });

  useEffect(() => {
    if (inView && hasNextPage) {
      // fetchNextPage calls queryFn and puts the next cursor from the latestPagedList to make API call.
      fetchNextPage();
    }
    // useEffect reruns when any of the dependencies change.
    // When inView changes (ref element is 50% visible) and if inView && hasNextPage are true, useEffect will fetch the next page.
    // fetchNextPage is a function defined outside the useEffect but is used inside it, so need to include it in the dependency array in case it changes.
    // hasNextPage dependency is for ensuring useEffect to rerun after it becomes false when there is no next cursor,
    // so it knows hasNextPage has become false and doesnt execute fetchNextPage anymore.
  }, [fetchNextPage, inView, hasNextPage]);

  if (isLoading) return <Typography>Loading...</Typography>;

  if (!activitiesGroup) return <Typography>No activity found.</Typography>;

  return (
    // main container, display: "flex" displays in row but activates gap property, set flexDirection to be vertical and gap of each ActivityCard 3.
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {/* activitiesGroup contains pages and pageParams, go to pages, map every pagedList.
      // In every pagedList, go to items, map every item which is an activity.
      */}
      {activitiesGroup.pages.map((pagedList, index) => (
        // Key needs to be attached to every pagedList for React to keep track of pagedList added/ changes/ removed.
        <Box
          key={index}
          // ref element must be the last pagedList, length counts from 1, so it is 1 more than the index which starts from 0, use -1 to find the last element.
          // Add a ref tag to the last pagedList so when user sees 50% of the last pagedList (half of the activities inside it),
          // it will trigger the useEffect to fetch next page.
          ref={index === activitiesGroup.pages.length - 1 ? ref : null}
          display="flex"
          flexDirection="column"
          gap={3}
        >
          {pagedList.items.map((activity) => (
            // React needs id to identify each activity to track them individually.
            <ActivityCard key={activity.id} activity={activity} />
          ))}
        </Box>
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
