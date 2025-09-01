import { useParams } from "react-router";
import { useProfile } from "../../lib/hooks/useProfile";
import {
  Box,
  Button,
  ImageList,
  ImageListItem,
  Typography,
} from "@mui/material";
import { useState } from "react";
import PhotoUploadWidget from "../../app/shared/components/PhotoUploadWidget";
import StarButton from "../../app/shared/components/StarButton";
import DeleteButton from "../../app/shared/components/DeleteButton";

export default function ProfilePhotos() {
  const { id } = useParams();

  // For Object Destructuring, the name must match the return object key.
  const {
    photos,
    loadingPhotos,
    isCurrentUser,
    uploadPhoto,
    profile,
    setMainPhoto,
    deletePhoto,
  } = useProfile(id);

  // For Array Destructuring, only sequence matters. The first element is the current state value,
  // the second element is the function to update the state value.
  const [editMode, setEditMode] = useState(false);

  // passes file to uploadPhoto to make POST request to add photo and refetch photos if successful.
  const handlePhotoUpload = (file: Blob) => {
    // As there is nothing to do after uploadPhoto, no need to use mutateasync. Just fire and forget, then let onSuccess to handle the result.
    // if uploadPhoto doesnt work, then setEditMode will remain true to let user to add again.
    uploadPhoto.mutate(file, {
      onSuccess: () => {
        // Change the button back to Add Photo to show that editing is completed.
        setEditMode(false);
      },
    });
  };

  if (loadingPhotos) return <Typography>Loading photos...</Typography>;

  // Because const response = await agent.get<Photo[]>(`/profiles/${id}/photos`); returns an empty array even if there is no photo,
  if (!photos) return <Typography>No photos found for this user.</Typography>;

  return (
    <Box>
      {isCurrentUser && (
        <Box>
          <Button onClick={() => setEditMode(!editMode)}>
            {editMode ? "Cancel" : "Add Photo"}
          </Button>
        </Box>
      )}
      {editMode ? (
        <PhotoUploadWidget
          uploadPhoto={handlePhotoUpload}
          loading={uploadPhoto.isPending}
        />
      ) : (
        <ImageList sx={{ height: 450 }} cols={6} rowHeight={164}>
          {photos.map((eachPhoto) => (
            <ImageListItem key={eachPhoto.id}>
              <img
                // From MUI, srcSet={`${eachPhoto.url}?w=164&h=164&fit=crop&auto=format&dpr=2 2x`}, the query string parameters transform the image.
                // it transforms the width and height in pixels. fit=crop means crop the image based on the height and width.
                // auto=format sets the optimal image format according to the browser capability.
                srcSet={`${eachPhoto.url.replace(
                  // Perform image transformation through Cloudinary URL. c_fill fills the image according to the dimension. dpr_2 provides higher pixel images to higher resolution screen.
                  "/upload/",
                  "/upload/w_164,h_164,c_fill,f_auto,dpr_2/"
                )}`}
                src={`${eachPhoto.url.replace(
                  // src is alternative for standard screen.
                  "/upload/",
                  "/upload/w_164,h_164,c_fill,f_auto/"
                )}`}
                alt={"User Profile Image"}
                // Only load the image when the image is about to show up.
                loading="lazy"
              />
              {isCurrentUser && (
                <div>
                  {/* If the user is visiting his own profile, clicking the Star button will trigger setMain for that photo. A Put Request will be sent to set that photo as profile picture. */}
                  <Box
                    sx={{ position: "absolute", top: 0, left: 0 }}
                    onClick={() => setMainPhoto.mutate(eachPhoto)}
                  >
                    {/* If the user is visiting his own profile, if the image url matches the profile picture url, highlights the image. */}
                    <StarButton
                      selected={eachPhoto.url === profile?.imageUrl}
                    />
                  </Box>
                  {/* Only allow user to delete non profile picture photo. */}
                  {profile?.imageUrl !== eachPhoto.url && (
                    <Box
                      sx={{ position: "absolute", top: 0, right: 0 }}
                      onClick={() => deletePhoto.mutate(eachPhoto.id)}
                    >
                      {/* If the user is visiting his own profile, for all images that are not the profile picture url, shows delete button. */}
                      <DeleteButton />
                    </Box>
                  )}
                </div>
              )}
            </ImageListItem>
          ))}
        </ImageList>
      )}
    </Box>
  );
}
