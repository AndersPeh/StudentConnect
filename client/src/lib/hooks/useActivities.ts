import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useLocation } from "react-router";

// useQuery for fetching data, useMutation for creating / updating data.
export const useActivities = (id?: string) => {

  const queryClient = useQueryClient();
  // get path URL location.
  const location = useLocation();

  // useQuery is automatically executed when the App component mounts.
  // destructures useQuery to get {data}, put it into variable named activities.
  // useQuery manages loading state while queryFn is running. isPending is true during that process.
  const { data: activities, isPending } = useQuery({
    // queryKey is the unique id for useQuery to check internal cache to see if there is already data for this queryKey.
    // If there is data in the cache, useQuery returns data without fetching.
    queryKey: ["activities"],

    // If there is no data in queryKey or stale, useQuery executes this function to fetch data.
    // when data is stale, React Query returns stale data while fetching fresh data. When new data is ready, it re renders to provide new data.
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

    // dont execute this useQuery when there is id (should execute useQuery for specific activity).
    // execute this useQuery when the pathname is /activities only.
    enabled: !id && location.pathname==='/activities',

    // make a staletime of 5 seconds so React Query won't mark any data as stale for the time period unless it is invalidated. 
    // When it is refreshed, React Query will fetch data from cache instead of making new request.
    staleTime: 5000,

  });

  const {data:activity, isLoading: isLoadingActivity} = useQuery({
    // React Query treats the data for each activity Id as separate cache entry.
    queryKey:['activities', id],
    queryFn: async()=>{
      const response = await agent.get<Activity>(`/activities/${id}`);
      return response.data;
    },
    // without enable, useQuery of specific activity will run everytime when the app runs, resulting in undefined.
    // only enable it when id is true (passed from ActivityDetail), (!!id) converts id into boolean.
    enabled: !!id
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
      const response = await agent.post('/activities', activity);
      // axios parses response automatically and returns id from HTTP Request.
      return response.data;
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

  return { activities, 
    isPending, 
    updateActivity, 
    createActivity, 
    deleteActivity,
    activity,
    isLoadingActivity, 
  };
};
