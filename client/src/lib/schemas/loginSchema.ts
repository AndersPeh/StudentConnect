import { z } from "zod";

// actually, dont need to validate email and password because Identity system from the server will check them and return error messages.
// but checking in the frontend makes it quicker.
// z.object defines the object schema so the login form must have following keys (email and password).
export const loginSchema = z.object({
    // z.string means the values of the email and password keys must be string.
    // to use email validator from zod to validate email.
    email: z.string().email(),
    password: z.string().min(6)
})

// export type creates a Typescript type to be used in login form to provide strong typing.
export type LoginSchema = z.infer<typeof loginSchema>;