import { useParams } from "react-router";
import { useProfile } from "../../lib/hooks/useProfile";
import { ImageList, ImageListItem, Typography } from "@mui/material";

export default function ProfilePhotos() {
  const { id } = useParams();

  const { photos, loadingPhotos } = useProfile(id);

  if (loadingPhotos) return <Typography>Loading photos...</Typography>;

  // Because const response = await agent.get<Photo[]>(`/profiles/${id}/photos`); returns an empty array even if there is no photo,
  // need to check array length to know if it contains any photo.
  if (!photos || photos.length === 0)
    return <Typography>No photos found for this user.</Typography>;

  return (
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
        </ImageListItem>
      ))}
    </ImageList>
  );
}
