import {
  Avatar,
  Box,
  Button,
  Chip,
  Divider,
  Grid2,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import { useParams } from "react-router";
import { useProfile } from "../../lib/hooks/useProfile";

export default function ProfileHeader() {
  const { id } = useParams();

  // isCurrentUser is for making sure the user on his own profile shouldnt see the follow button.
  const { isCurrentUser, profile, updateFollowing } = useProfile(id);

  if (!profile) return null;

  return (
    // White card container with rounded corners and elevation shadow. Internal padding of 4.
    <Paper elevation={3} sx={{ padding: 4, borderRadius: 3 }}>
      {/* Split the Header into left and right columns. */}
      <Grid2 container spacing={2}>
        {/* For left Header column, stack items in row (Profile Picture and Display Name). */}
        <Grid2 size={8}>
          <Stack direction="row" spacing={3} alignItems="center">
            {/* Large size Profile Picture. */}
            <Avatar
              src={profile.imageUrl}
              alt={profile.displayName + " image"}
              sx={{ width: 150, height: 150 }}
            />
            {/* Box by default displays items vertically, it works without specifying display="flex" flexDirection="column". */}
            {/* However, gap={2} only works with display="flex" flexDirection="column" specified, so they are provided for gap, not to display items vertically. */}
            <Box display="flex" flexDirection="column" gap={2}>
              {/* Large Display Name on top of the vertical box. */}
              <Typography variant="h4">{profile.displayName}</Typography>
              {/* Following chip in the bottom of the vertical box */}
              {profile.following && (
                <Chip
                  variant="outlined"
                  color="secondary"
                  label="Following"
                  sx={{ borderRadius: 1 }}
                />
              )}
            </Box>
          </Stack>
        </Grid2>
        {/* Right side of the Header column. */}
        <Grid2 size={4}>
          {/* By default, Stack displays everything vertically. (Folllowers/Following, Divider, Unfollow/ Follow button)*/}
          <Stack spacing={2} alignItems="center">
            {/* First part of the Stack. */}
            {/* Flex box to display Followers and Following side by side. */}
            <Box display="flex" justifyContent="space-around" width="100%">
              {/* Box by default displays everything vertically. */}
              {/* Left side the of Flex box. */}
              <Box textAlign="center">
                <Typography variant="h6">Followers</Typography>
                <Typography variant="h3">{profile.followersCount}</Typography>
              </Box>
              {/* Right side the of Flex box. */}
              <Box textAlign="center">
                <Typography variant="h6">Following</Typography>
                <Typography variant="h3">{profile.followingCount}</Typography>
              </Box>
            </Box>
            {!isCurrentUser && (
              <>
                {/* Second part of the Stack. */}
                {/* Divider between Followers/Following and Unfollow/ Follow button. */}
                <Divider sx={{ width: "100%" }} />
                {/* Third part of the Stack. */}
                {/* Button changes color and display according to isFollowing. */}
                <Button
                  onClick={() => updateFollowing.mutate()}
                  disabled={updateFollowing.isPending}
                  fullWidth
                  variant="outlined"
                  color={profile.following ? "error" : "success"}
                >
                  {profile.following ? "Unfollow" : "Follow"}
                </Button>
              </>
            )}
          </Stack>
        </Grid2>
      </Grid2>
    </Paper>
  );
}
