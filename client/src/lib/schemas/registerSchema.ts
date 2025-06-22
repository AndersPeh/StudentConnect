import { z } from "zod";
import { requiredString } from "../util/util";

export const registerSchema = z.object({
    email: z.string().email(),
    displayName: requiredString('displayName'),
    // no need to validate password complexity here, only need to ensure password is entered.
    // Identity system in the backend will validate the password complexity.
    password: requiredString('password'),
})

export type RegisterSchema = z.infer<typeof registerSchema>;