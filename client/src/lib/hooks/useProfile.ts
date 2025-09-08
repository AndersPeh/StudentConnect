import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useMemo } from "react";

export const useProfile = (id?: string, predicate?: string) => {
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
    enabled: !!id && !predicate,
  });

  // send GET request to get array of Photo of the user.
  const { data: photos, isLoading: loadingPhotos } = useQuery<Photo[]>({
    queryKey: ["photos", id],

    // Receives id from queryKey.
    queryFn: async () => {
      const response = await agent.get<Photo[]>(`/profiles/${id}/photos`);
      return response.data;
    },

    // Only execute get photos when id is passed in but predicate is not passed in.
    enabled: !!id && !predicate,
  });

  const { data: followings, isLoading: loadingFollowings } = useQuery<
    Profile[]
  >({
    queryKey: ["followings", id, predicate],
    queryFn: async () => {
      const response = await agent.get<Profile[]>(
        `/profiles/${id}/follow-list?predicate=${predicate}`
      );
      return response.data;
    },

    // Only enable this query to run when both id and predicate are passed in as arguments.
    enabled: !!id && !!predicate,
  });

  // takes file as a argument and sends it to add-photo endpoint.
  const uploadPhoto = useMutation({
    mutationFn: async (file: Blob) => {
      // FormData creates a special object similar to HTML form submission for sending the photo as a form.
      // Because [HttpPost("add-photo")] expects IFormFile which looks for file in multipart/form-data in the payload.
      const formData = new FormData();
      // Add the file as "file" name to match AddPhoto(IFormFile file) of the backend.
      formData.append("file", file);
      // headers tells the endpoint the content type to expect which matches IFormFile.
      const response = await agent.post("/profiles/add-photo", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });

      // returns Photo object from the API's response and passes it to onSuccess.
      return response.data;
    },

    // photo is passed from the backend (public async Task<ActionResult<Photo>> AddPhoto(IFormFile file)).
    onSuccess: async (photo: Photo) => {
      // refetch photos from the backend when the new photo is successfully added.
      await queryClient.invalidateQueries({
        queryKey: ["photos", id],
      });

      // Update the cache for the query with the key ["user"] (from useAccount),
      queryClient.setQueryData(
        ["user"],
        // In the "user" query cache, modify the User data.
        (data: User) => {
          // if there is no User data cached, dont do anything.
          if (!data) return data;

          return {
            ...data,
            // If the cached data doesnt have main image (imageUrl), set the newly added photo as main image.
            imageUrl: data.imageUrl ?? photo.url,
          };
        }
      );

      // change the cached profile data.
      queryClient.setQueryData(
        ["profile", id],
        // In the ["profile", id] query cache, modify the Profile data.
        (data: Profile) => {
          // if cant find Profile data, dont do anything.
          if (!data) return data;

          // If Profile data doesnt have main image (imageUrl), assign new photo url to it.
          return {
            ...data,
            imageUrl: data.imageUrl ?? photo.url,
          };
        }
      );
    },
  });

  // When calling setMainPhoto, need to pass photo argument into it.
  const setMainPhoto = useMutation({
    mutationFn: async (photo: Photo) => {
      // As the photoId is in the Url, no need to provide anything in the body.
      await agent.put(`/profiles/${photo.id}/setMain`);
    },

    // use _ as the first argument is the returned data from HTTP response. As there is no data to return, use _ to replace it.
    // Second argument refers to the variable passed from mutationFn (photo).
    onSuccess: (_, photo) => {
      queryClient.setQueryData(
        ["user"],
        // For the cached query "user", set the imageUrl to the photo.url of the argument passed from mutationFn.
        (userData: User) => {
          if (!userData) return userData;
          return {
            ...userData,
            imageUrl: photo.url,
          };
        }
      );

      queryClient.setQueryData(
        ["profile", id],
        // For the cached query "profile", set the imageUrl to the photo.url of the argument passed from mutationFn.
        (profile: Profile) => {
          if (!profile) return profile;
          return {
            ...profile,
            imageUrl: photo.url,
          };
        }
      );
    },
  });

  // Need to pass photoId into deletePhoto to use it for making DELETE request and onSuccess process.
  const deletePhoto = useMutation({
    mutationFn: async (photoId: string) => {
      await agent.delete(`/profiles/${photoId}/photos`);
    },

    // First argument: As there is no data returned from the DELETE request, use _ to replace it.
    // Second argument:
    onSuccess: (_, photoId) => {
      queryClient.setQueryData(
        ["photos", id],
        // When the photo is deleted successfully, modify the cached photos in queryKey ["photos", id] to use filter to hide it.
        (photos: Photo[]) => {
          return photos?.filter((photo) => photo.id !== photoId);
        }
      );
    },
  });

  const updateFollowing = useMutation({
    mutationFn: async () => {
      await agent.post(`/profiles/${id}/follow`);
    },

    onSuccess: () => {
      queryClient.setQueryData(
        ["profile", id],
        // Modify the cached profile in the profile query key by adding or reducing the followersCount.
        (profile: Profile) => {
          // Because we can only click follow button which affects followers list, just need to invalidate the followers.
          // Put queryClient.invalidateQueries inside queryClient.setQueryData as optimistic update for users to see instant update.
          queryClient.invalidateQueries({
            queryKey: ["followings", id, "followers"],
          });
          // !profile is when profile doesnt exist, but !profile.followersCount can mean when profile.followersCount is 0.
          // so need to use === undefined instead of !.
          if (!profile || profile.followersCount === undefined) return profile;

          // If the user was following, then the user is no longer following.
          // If the user was not following, then the user is now following.
          return {
            ...profile,
            following: !profile.following,
            followersCount: profile.following
              ? profile.followersCount - 1
              : profile.followersCount + 1,
          };
        }
      );
    },
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
    uploadPhoto,
    setMainPhoto,
    deletePhoto,
    updateFollowing,
    followings,
    loadingFollowings,
  };
};
