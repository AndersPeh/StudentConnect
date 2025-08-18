import { Grid2 } from "@mui/material";
import ProfileHeader from "./ProfileHeader";
import ProfileContent from "./ProfileContent";

export default function ProfilePage() {
  return (
    <Grid2 container>
      <Grid2 size={12}>
        <ProfileHeader />
        <ProfileContent />
      </Grid2>
    </Grid2>
  );
}
