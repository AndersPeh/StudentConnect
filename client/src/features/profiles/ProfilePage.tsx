import { Grid2, Typography } from "@mui/material";
import ProfileHeader from "./ProfileHeader";
import ProfileContent from "./ProfileContent";
import { useParams } from "react-router";
import { useProfile } from "../../lib/hooks/useProfile";

export default function ProfilePage() {
  // Get current user Id from the route when user visits his profile page.
  const { id } = useParams();

  // send HTTP GET request to get user profile and loadingProfile state.
  const { profile, loadingProfile } = useProfile(id);

  // Display loading message if isLoading (loadingProfile).
  if (loadingProfile) return <Typography>Loading Profile...</Typography>;

  // Display error message if profile can't be found which is unlikely due to error handling.
  if (!profile) return <Typography>Profile Not Found</Typography>;

  return (
    <Grid2 container>
      <Grid2 size={12}>
        {/* pass the user profile to ProfileHeader to display image, name... */}
        <ProfileHeader profile={profile} />
        <ProfileContent />
      </Grid2>
    </Grid2>
  );
}
