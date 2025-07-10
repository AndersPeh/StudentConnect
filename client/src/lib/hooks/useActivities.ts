import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useLocation } from "react-router";
import type { FieldValues } from "react-hook-form";
import { useAccount } from "./useAccount";

// useQuery for fetching data, useMutation for creating / updating data.
export const useActivities = (id?: string) => {
  const queryClient = useQueryClient();

  const { currentUser } = useAccount();

  // get path URL location.
  const location = useLocation();

  // useQuery is automatically executed when the App component mounts.
  // destructures useQuery to get {data}, put it into variable named activities.
  // useQuery manages loading state while queryFn is running. isLoading when first fetch of a query is running.
  const { data: activities, isLoading } = useQuery({
    // queryKey is the unique id for useQuery to check internal cache to see if there is already data for this queryKey.
    // If there is data in the cache, useQuery returns data without fetching.
    queryKey: ["activities"],

    // If there is no data in queryKey or stale, useQuery executes this function to fetch data.
    // when data is stale, React Query returns stale data while fetching fresh data. When new data is ready, it re renders to provide new data.
    queryFn: async () => {
      // makes HTTP Get request to the backend API endpoint, return type is array of Activity objects.
      // by using agent, URL can be shortened.
      const response = await agent.get<Activity[]>("/activities");

      // axios automatically parses JSON, allowing us to directly get data from response.
      // useQuery updates its loading state, caches the fetched data against the queryKey, isPending becomes false.
      // this return caches the data in queryKey: ["activities"], it doesnt return data: activities.
      return response.data;
    },

    // dont execute this useQuery when there is id (should execute useQuery for specific activity).
    // execute this useQuery when the pathname is /activities only and currentUser exists.
    enabled: !id && location.pathname === "/activities" && !!currentUser,

    // select only runs after queryFn has successfully completed and returned data.
    // select is for transforming the cached data received from the return of queryFn, before it is returned as data: activities.
    select: (data) => {
      // This returns the final output of this query, data: activities. It maps the array of Activity from API call as individual activity.
      return data.map((activity) => {
        // This transforms individual activity using map, forming the transformed activity array.
        return {
          ...activity,
          // Add isHost to the activity object, it is true if the currentUser.id matches the hostId of the activity.
          isHost: currentUser?.id === activity.hostId,
          // Add isGoing, it is true if currentUser.id exists in the attendess.
          isGoing: activity.attendees.some((x) => x.id === currentUser?.id),
        };
      });
    },

    // make a staletime of 5 seconds so React Query won't mark any data as stale for the time period unless it is invalidated.
    // When it is refreshed, React Query will fetch data from cache instead of making new request.
    staleTime: 5000,
  });

  // send GET request to query individual activity details.
  const { data: activity, isLoading: isLoadingActivity } = useQuery({
    // React Query treats the data for each activity Id as separate cache entry.
    queryKey: ["activities", id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/activities/${id}`);
      return response.data;
    },
    // without enable, useQuery of specific activity will run everytime when the app runs, resulting in undefined.
    // only enable it when id is true (passed from ActivityDetail), (!!id) converts id into boolean.
    enabled: !!id && !!currentUser,

    // Transform the cached individual activty from queryFn API call using select.
    select: (data) => {
      // This returns data: activity.
      return {
        ...data,
        isHost: currentUser?.id === data.hostId,
        isGoing: data.attendees.some((x) => x.id === currentUser?.id),
      };
    },
  });

  // send PUT request to update a specific activity, refetch activities if successfully edited the activity.
  const updateActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      await agent.put("/activities", activity);
    },
    // If put request is successful, invalidate queryKey, internal cache becomes stale, useQuery will fetch the data again.
    // When activities key is invalidated, [activities, id] key is also invalidated because all query keys that
    // start with "activities" are invalidated.
    // If invalidate queryKey: ["activities", id] only, the detail page will reflect the changes but the activities page wont.
    // Because activities query wasnt invalidated, so the activities page will show old data.
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["activities"],
      });
    },
  });

  // send POST request to create an activity, refetch activities if successfully created the activity.
  const createActivity = useMutation({
    mutationFn: async (activity: FieldValues) => {
      const response = await agent.post("/activities", activity);
      // axios parses response automatically and returns id from HTTP Request.
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["activities"],
      });
    },
  });

  // send DELETE request to remove a specific activity, refetch activities if successful.
  const deleteActivity = useMutation({
    mutationFn: async (id: string) => {
      await agent.delete(`/activities/${id}`);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["activities"],
      });
    },
  });

  // send POST request to update the attendance of a specific activty, refetch the specific activity if successful.
  const updateAttendance = useMutation({
    mutationFn: async (id: string) => {
      await agent.post(`/activities/${id}/attend`);
    },
    onMutate: async (activityId: string) => {
      // Cancel the ongoing query for the specific activity.
      await queryClient.cancelQueries({
        queryKey: ["activities", activityId],
      });

      // get the specific activity from cached activities.
      const prevActivity = queryClient.getQueryData<Activity>([
        "activities",
        activityId,
      ]);

      // optimistically update the data (assuming the API endpoint will return positive result).
      queryClient.setQueryData<Activity>(
        ["activities", activityId],
        (oldActivity) => {
          if (!oldActivity || !currentUser) {
            return oldActivity;
          }

          const isHost = oldActivity.hostId === currentUser.id;
          const isAttending = oldActivity.attendees.some(
            (x) => x.id === currentUser.id
          );

          return {
            ...oldActivity,
            isCancelled: isHost
              ? !oldActivity.isCancelled
              : oldActivity.isCancelled,
            attendees: isAttending
              ? isHost
                ? oldActivity.attendees
                : oldActivity.attendees.filter((x) => x.id !== currentUser.id)
              : [
                  ...oldActivity.attendees,
                  {
                    id: currentUser.id,
                    displayName: currentUser.displayName,
                    imageUrl: currentUser.imageUrl,
                  },
                ],
          };
        }
      );
      return { prevActivity };
    },
    onError: (error, activityId, context) => {
      console.log(error);
      if (context?.prevActivity) {
        queryClient.setQueryData(
          ["activities", activityId],
          context.prevActivity
        );
      }
    },
  });

  return {
    activities,
    isLoading,
    updateActivity,
    createActivity,
    deleteActivity,
    activity,
    isLoadingActivity,
    updateAttendance,
  };
};
