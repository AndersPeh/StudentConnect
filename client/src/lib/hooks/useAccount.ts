import { useMutation } from "@tanstack/react-query"
import type { LoginSchema } from "../schemas/loginSchema"
import agent from "../api/agent";

export const useAccount = () => {
    // useMutation for creating or updating data.
    const loginUser = useMutation({
    // mutationFn takes creds parameter which must be LoginSchema type (loginSchema.ts strcture), it triggers the login and state properties like isPending.
    // which means creds must contain email and password.
        mutationFn: async(creds: LoginSchema) => {
    // use axios to send a post request to login url and it must contain user credentials (creds).
    // It instructs Identity endpoint to set an HttpOnly cookie in the response if the login is successful.
    // Then subsequ
            await agent.post('/login?useCookies=true', creds);
        }
    });

    // useAccount hook returns an object that contains this 'loginUser' constant.
    return {
        loginUser
    }
}