import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import type { LoginSchema } from "../schemas/loginSchema"
import agent from "../api/agent";
import { useNavigate } from "react-router";

export const useAccount = () => {
    // to use invalidate queries on user queryKey (to run getting user information again.)
    const queryClient = useQueryClient();

    // for navigating to other pages.
    const navigate = useNavigate();

    // useMutation for handling login process.
    const loginUser = useMutation({
    // mutationFn takes creds parameter which must be LoginSchema type (loginSchema.ts strcture), it triggers the login and state properties like isPending.
    // which means creds must contain email and password.
        mutationFn: async(creds: LoginSchema) => {
    // use axios to send a post request to login url and it must contain user credentials (creds).
    // It instructs Identity endpoint to set an HttpOnly cookie in the response if the login is successful.
    // Then subsequent API requests from the browser will include cookie.
    // For login, we didnt make custom controller for that, just /login by Identity system is fine.
            await agent.post('/login?useCookies=true', creds);
        },
        // when user logs in, automatically invalidate user queryKey so it will rerun const response = await agent.get<User>('/account/user-info');.
        onSuccess: async() => {
            await queryClient.invalidateQueries({
                queryKey: ['user']
            });
        }
    });

    // send POST request to logout endpoint to log out and remove the user information and activities received from useQuery previously.
    const logoutUser = useMutation({
        mutationFn: async() => {
            // For logout, we made custom controller for that, need to use /account/logout.
            await agent.post('/account/logout');
        },
        onSuccess: () => {
            queryClient.removeQueries({queryKey: ['user']});
            queryClient.removeQueries({queryKey: ['activities']});
            // automatically navigate user back to home page after logging out.
            navigate('/');
        }
    })

    // fetch and cache the current logged in user information from backend API.The result is renamed as currentUser.
    const {data:currentUser, isLoading: loadingUserInfo} = useQuery({
        // unique cache key which can be used to invalidated to re-run the query function below.
        queryKey: ['user'],
        // sends a GET request to /account/user-info endpoint and the returned response data should be User type.
        // For user info, we made custom controller for that, need to use /account/user-info.
        queryFn: async()=> {
            const response = await agent.get<User>('/account/user-info');
            return response.data;
        },
        // only get user information when user data doesnt exist else it will be stale and try again to get user info.
        enabled: !queryClient.getQueryData(['user'])
    })

    // useAccount hook returns an object that contains this 'loginUser' constant.
    return {
        loginUser,
        currentUser,
        logoutUser,
        loadingUserInfo,
    }
}