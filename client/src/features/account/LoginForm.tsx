import { useForm } from "react-hook-form";
import { useAccount } from "../../lib/hooks/useAccount";
import { loginSchema, type LoginSchema } from "../../lib/schemas/loginSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Paper, Typography } from "@mui/material";
import { LockOpen } from "@mui/icons-material";
import TextInput from "../../app/shared/components/TextInput";
import { useLocation, useNavigate } from "react-router";

export default function LoginForm() {
  //destructure the loginUser returned by useAccount to get access to the useMutation instance to trigger login and state properties like isSubmitting.
  const { loginUser } = useAccount();
  const navigate = useNavigate();
  const location = useLocation();
  // setup for react hook form.
  const {
    // control connects input components like TextInput to the form state and validation logic (set in loginSchema for validation logic and mode for timing of validation).
    control,
    // handles form submission, it triggers validation then calls onSubmit function.
    handleSubmit,
    // status of the form. isValid is true if all fields pass validation.
    formState: { isValid, isSubmitting },
    // the form has to match LoginSchema.
  } = useForm<LoginSchema>({
    // sets validation to run when a user clicks out of a field for instant feedback.
    mode: "onTouched",
    // tells react hook form to use loginSchema for validation.
    resolver: zodResolver(loginSchema),
  });

  // onSubmit only accepts LoginSchema data type.
  const onSubmit = async (data: LoginSchema) => {
    // calls mutate function from the loginUser to pass validated user credentials in login request through axios.post to backend API.
    // mutateAsync instead of mutate only because need to await for the user info data.
    await loginUser.mutateAsync(data, {
      onSuccess: () => {
        // if the state stores the page where user was initially from, navigate user to there after logging in.
        // if the user directly visited the login page, send user to homepage displaying activities list.
        navigate(location.state?.from || "/activities");
      },
    });
  };

  return (
    <Paper
      component="form"
      // handleSubmit of react hook form automatically validates input based on useForm configuration then only calls onSubmit.
      onSubmit={handleSubmit(onSubmit)}
      sx={{
        display: "flex",
        flexDirection: "column",
        padding: 3,
        gap: 3,
        maxWidth: "md",
        marginX: "auto",
        borderRadius: 3,
      }}
    >
      <Box
        display="flex"
        alignItems="center"
        justifyContent="center"
        gap={3}
        color="secondary.main"
      >
        {/* displays unlocked icon */}
        <LockOpen fontSize="large" />
        <Typography variant="h4">Sign In</Typography>
      </Box>
      {/* control={control} connects TextInput to react hook form so it will be validated and managed according to useForm configuration. */}
      <TextInput label="Email" control={control} name="email" />
      {/* type='password' for hiding password. */}
      <TextInput
        label="Password"
        type="password"
        control={control}
        name="password"
      />
      <Button
        type="submit"
        disabled={!isValid || isSubmitting}
        variant="contained"
        size="large"
      >
        Login
      </Button>
    </Paper>
  );
}
