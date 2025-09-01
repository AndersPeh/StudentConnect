import { Person } from "@mui/icons-material";
import {
  Box,
  Card,
  CardContent,
  CardMedia,
  Chip,
  Divider,
  Typography,
} from "@mui/material";
import { Link } from "react-router";

type Props = {
  profile: Profile;
};

// Profile Card appears when User mouse hovers to other user icon in ActivityCard.
export default function ProfileCard({ profile }: Props) {
  const following = false;
  return (
    // use Link from React Router to route user to the URL.
    <Link to={`/profiles/${profile.id}`} style={{ textDecoration: "none" }}>
      <Card
        sx={{
          borderRadius: 3,
          padding: 3,
          maxWidth: 300,
          textDecoration: "none",
        }}
        elevation={4}
      >
        {/* Profile image of the User, use standard image if the user doesn't have profile image. */}
        <CardMedia
          // By default, CardMedia renders an HTML <div>. By specifying img, it renders HTML <img> tag.
          component="img"
          src={profile?.imageUrl || "/images/user.png"}
          sx={{ width: "100%", zIndex: 50 }}
          alt={profile.displayName + " image"}
        />
        {/* Other than User image, also displays User name in profile card. */}
        <CardContent>
          <Box display="flex" flexDirection="column" gap={1}>
            <Typography variant="h5">{profile.displayName}</Typography>
            {profile.bio && (
              // ellipsis shows ... when the text is too long.
              <Typography
                variant="body2"
                sx={{
                  textOverflow: "ellipsis",
                  overflow: "hidden",
                  whiteSpace: "nowrap",
                }}
              >
                {profile.bio}
              </Typography>
            )}
            {/* if following the user, shows following chip. */}
            {following && (
              <Chip
                size="small"
                label="Following"
                color="secondary"
                variant="outlined"
              />
            )}
          </Box>
        </CardContent>
        <Divider sx={{ marginBottom: 2 }} />
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "start",
          }}
        >
          {/* Person icon followed by number of followers. */}
          <Person />
          <Typography sx={{ marginLeft: 1 }}>20 Followers</Typography>
        </Box>
      </Card>
    </Link>
  );
}
