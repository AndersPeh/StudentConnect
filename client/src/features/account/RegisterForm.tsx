import { useForm } from "react-hook-form";
import { useAccount } from "../../lib/hooks/useAccount";
import { zodResolver } from "@hookform/resolvers/zod";
import { Box, Button, Paper, Typography } from "@mui/material";
import { LockOpen } from "@mui/icons-material";
import TextInput from "../../app/shared/components/TextInput";
import { Link } from "react-router";
import {
  registerSchema,
  type RegisterSchema,
} from "../../lib/schemas/registerSchema";

export default function RegisterForm() {
  // destructure the registerUser returned by useAccount to get access to the useMutation instance to trigger register and state properties like isSubmitting.
  const { registerUser } = useAccount();
  // setup for react hook form.
  const {
    // control connects input components like TextInput to the form state and validation logic (set in registerSchema for validation logic and mode for timing of validation).
    control,
    // handles form submission, it triggers validation then calls onSubmit function.
    handleSubmit,
    // status of the form. isValid is true if all fields pass validation.
    formState: { isValid, isSubmitting },
    // React Hook Form allows manually setting error for each field based on error message from the API.
    setError,
    // the form has to match RegisterSchema.
  } = useForm<RegisterSchema>({
    // sets validation to run when a user clicks out of a field for instant feedback.
    mode: "onTouched",
    // tells react hook form to use registerSchema for validation.
    resolver: zodResolver(registerSchema),
  });

  // onSubmit only accepts RegisterSchema data type.
  const onSubmit = async (data: RegisterSchema) => {
    // calls mutate function from the registerUser to pass validated user credentials in register request through axios.post to backend API.
    // mutateAsync is needed to wait and display the error after mutation.
    await registerUser.mutateAsync(data, {
      // when receive error message from the API, if the error message is an array, loop through the error array,
      // if the individual error includes email, set it as the error message of email field so it will be displayed in red under email field.
      // same logic for password.
      onError: (error) => {
        if (Array.isArray(error)) {
          error.forEach((err) => {
            if (err.includes("Email")) setError("email", { message: err });
            else if (err.includes("Password"))
              setError("password", { message: err });
          });
        }
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
        <Typography variant="h4">Register</Typography>
      </Box>
      {/* control={control} connects TextInput to react hook form so it will be validated and managed according to useForm configuration. */}
      <TextInput label="Email" control={control} name="email" />
      <TextInput label="Display Name" control={control} name="displayName" />
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
        Register
      </Button>
      <Typography sx={{ textAlign: "center" }}>
        Existing user?
        {/* use Link instead of NavLink from React Router here because no active styling is needed. */}
        <Typography
          sx={{ marginLeft: 2 }}
          component={Link}
          to="/login"
          color="primary"
        >
          Sign In
        </Typography>
      </Typography>
    </Paper>
  );
}
