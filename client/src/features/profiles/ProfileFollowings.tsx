import { useParams } from "react-router";
import { useProfile } from "../../lib/hooks/useProfile";
import { Box, Divider, Typography } from "@mui/material";
import ProfileCard from "./ProfileCard";

type Props = {
  activeTab: number;
};

export default function ProfileFollowings({ activeTab }: Props) {
  const { id } = useParams();

  // Active Tab 3 from ProfileContent.tsx is followers.
  const predicate = activeTab === 3 ? "followers" : "followings";

  const { profile, followings, loadingFollowings } = useProfile(id, predicate);

  return (
    <Box>
      <Box display="flex">
        <Typography variant="h5">
          {activeTab === 3
            ? // If activeTab is 3, then the user is on followers Tab.
              profile?.followersCount
              ? // If user has follower.
                `${profile?.displayName}'s followers`
              : // If user doesn't have any follower.
                `Become ${profile?.displayName}'s first follower!`
            : // If activeTab is not 3, then the user is on following tab.
            profile?.followingCount
            ? `${profile?.displayName} is following`
            : `${profile?.displayName} hasn't followed anyone.`}
        </Typography>
      </Box>
      <Divider sx={{ marginY: 2 }} />
      {loadingFollowings ? (
        <Typography>Loading...</Typography>
      ) : (
        // If not loading, show list of followers or following. Followings represent either, depending on the predicate sent to the useProfile hook.
        <Box display="flex" marginTop={3} gap={3}>
          {followings?.map((profile) => (
            <ProfileCard key={profile.id} profile={profile} />
          ))}
        </Box>
      )}
    </Box>
  );
}
