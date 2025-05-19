import { useQuery } from "@tanstack/react-query";
import axios from "axios";

export const useActivities = () => {
  // useQuery is automatically executed when the App component mounts.
  // destructures useQuery to get {data}, put it into variable named activities.
  // useQuery manages loading state while queryFn is running. isPending is true during that process.
  const { data: activities, isPending } = useQuery({
    // queryKey is the unique id for useQuery to check internal cache to see if there is already data for this queryKey.
    // If there is data in the cache, useQuery returns data without fetching.
    queryKey: ["activities"],

    // If there is no data in queryKey, useQuery executes this function to fetch data.
    queryFn: async () => {
      // makes HTTP Get request to the backend API endpoint, return type is array of Activity objects.
      const response = await axios.get<Activity[]>(
        "https://localhost:5001/api/activities"
      );

      // axios automatically parses JSON, allowing us to directly get data from response.
      // useQuery updates its loading state, caches the fetched data against the queryKey, isPending becomes false.
      return response.data;
    },
  });

  return { activities, isPending };
};
