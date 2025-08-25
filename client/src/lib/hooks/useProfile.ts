import { useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useMemo } from "react";

// Take the user Id and return user profile as data and loadingProfile as isLoading state after executing the query.
export const useProfile = (id?: string) => {
  const queryClient = useQueryClient();

  const { data: profile, isLoading: loadingProfile } = useQuery<Profile>({
    // Unique queryKey that pass id to queryFn.
    queryKey: ["profile", id],

    // Whenever useQuery is used, it sends a HTTP GET request to the /profiles.${id} endpoint to retrieve user profile.
    queryFn: async () => {
      const response = await agent.get<Profile>(`/profiles/${id}`);
      return response.data;
    },

    // Only execute get profile when id is available.
    enabled: !!id,
  });

  // send GET request to get array of Photo of the user.
  const { data: photos, isLoading: loadingPhotos } = useQuery<Photo[]>({
    queryKey: ["photos", id],

    // Receives id from queryKey.
    queryFn: async () => {
      const response = await agent.get<Photo[]>(`/profiles/${id}/photos`);
      return response.data;
    },

    // Only execute get photos when id is available.
    enabled: !!id,
  });

  // queryClient.getQueryData<User>(["user"]) means look for ['user'] key in the cache of React Query,
  // it is the queryKey from useAccount.ts hook that fetches logged-in user's information and stores it in the cache with ['user'] key.
  // There is no point doing useQuery here as there is already an existing query that gets user information.
  // useMemo for remembering the result of comparison so it wont rerun across re-render unless id changes (another user page).
  const isCurrentUser = useMemo(() => {
    // After fetching current user id (if exists), compare with the user id in the URL,
    // if matches, means we're in our own profile.
    return id === queryClient.getQueryData<User>(["user"])?.id;
    // queryClient is placed in the dependency array to signal that the comparison calculation depends on the queryClient existing.
  }, [id, queryClient]);

  return {
    profile,
    loadingProfile,
    photos,
    loadingPhotos,
    isCurrentUser,
  };
};
