import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";

// useQuery for fetching data, useMutation for creating / updating data.
export const useActivities = () => {

  const queryClient = useQueryClient();

  // useQuery is automatically executed when the App component mounts.
  // destructures useQuery to get {data}, put it into variable named activities.
  // useQuery manages loading state while queryFn is running. isPending is true during that process.
  const { data: activities, isPending } = useQuery({
    // queryKey is the unique id for useQuery to check internal cache to see if there is already data for this queryKey.
    // If there is data in the cache, useQuery returns data without fetching.
    queryKey: ["activities"],

    // If there is no data in queryKey or stale, useQuery executes this function to fetch data.
    queryFn: async () => {
      // makes HTTP Get request to the backend API endpoint, return type is array of Activity objects.
      // by using agent, URL can be shortened.
      const response = await agent.get<Activity[]>(
        "/activities"
      );

      // axios automatically parses JSON, allowing us to directly get data from response.
      // useQuery updates its loading state, caches the fetched data against the queryKey, isPending becomes false.
      return response.data;
    },
  });

  const updateActivity = useMutation({
    mutationFn: async(activity: Activity) =>{
      await agent.put('/activities', activity)
    },
    // If put request is successful invalidate queryKey, internal cache becomes stale, useQuery will fetch the data again.
    onSuccess: async()=>{
      await queryClient.invalidateQueries({
        queryKey: ["activities"]
      });
    }
  });

  const createActivity = useMutation({
    mutationFn: async(activity: Activity)=>{
      await agent.post('/activities', activity)
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey:["activities"]
      })
    }
  })

  const deleteActivity = useMutation({
    mutationFn: async(id: string) =>{
      await agent.delete(`/activities/${id}`)
    },
    onSuccess: async()=>{
      await queryClient.invalidateQueries({
        queryKey:["activities"]
      })
    }
  })

  return { activities, isPending, updateActivity, createActivity, deleteActivity };
};
