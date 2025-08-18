import { useQuery } from "@tanstack/react-query";
import agent from "../api/agent";

// Take the user Id and return user profile as data and loadingProfile as isLoading state after executing the query.
export const useProfile = (id?: string) => {
  const { data: profile, isLoading: loadingProfile } = useQuery<Profile>({
    // Unique queryKey for this useQuery.
    queryKey: ["profile", id],

    // Whenever useQuery is used, it sends a HTTP GET request to the /profiles.${id} endpoint to retrieve user profile.
    queryFn: async () => {
      const response = await agent.get<Profile>(`/profiles/${id}`);
      return response.data;
    },
  });

  return {
    profile,
    loadingProfile,
  };
};
