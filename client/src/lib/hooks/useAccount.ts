import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import type { LoginSchema } from "../schemas/loginSchema"
import agent from "../api/agent";

export const useAccount = () => {
    // to use invalidate queries on user queryKey (to run getting user information again.)
    const queryClient = useQueryClient();

    // useMutation for handling login process.
    const loginUser = useMutation({
    // mutationFn takes creds parameter which must be LoginSchema type (loginSchema.ts strcture), it triggers the login and state properties like isPending.
    // which means creds must contain email and password.
        mutationFn: async(creds: LoginSchema) => {
    // use axios to send a post request to login url and it must contain user credentials (creds).
    // It instructs Identity endpoint to set an HttpOnly cookie in the response if the login is successful.
    // Then subsequent API requests from the browser will include cookie.
            await agent.post('/login?useCookies=true', creds);
        },
        // when user logs in, automatically invalidate user queryKey so it will rerun const response = await agent.get<User>('/account/user-info');.
        onSuccess: async() => {
            await queryClient.invalidateQueries({
                queryKey: ['user']
            })
        }
    });

    // fetch and cache the current logged in user information from backend API.The result is renamed as currentUser.
    const {data:currentUser} = useQuery({
        // unique cache key which can be used to invalidated to re-run the query function below.
        queryKey: ['user'],
        // sends a GET request to /account/user-info endpoint and the returned response data should be User type.
        queryFn: async()=> {
            const response = await agent.get<User>('/account/user-info');
            return response.data;
        }
    })

    // useAccount hook returns an object that contains this 'loginUser' constant.
    return {
        loginUser,
        currentUser
    }
}