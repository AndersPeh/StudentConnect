import axios from "axios";
import { store } from "../stores/store";
import { toast } from "react-toastify";
import { router } from "../../app/router/Routes";

const sleep = (delay:number) =>{
    return new Promise(resolve =>{
        setTimeout(resolve, delay)
    });
}

const agent = axios.create({
    // by using .env.development, no need to hardcode URL.
    baseURL:import.meta.env.VITE_API_URL
});

// when the request is going out, set isLoading to true.
agent.interceptors.request.use(config=>{
    store.uiStore.isBusy();
    return config;
})

// response interceptor tells axios, for every successful response from HTTP request,
// pass response object to the async function.
agent.interceptors.response.use(

    async response =>{
        // upon receiving successful response, sleep for 1 sec, turn off loading and return response.
        await sleep(1000);
        store.uiStore.isIdle();
        return response;
    },

    async error =>{
        await sleep(1000);
        store.uiStore.isIdle();

        const {status, data} = error.response;
        switch (status) {
            case 400:
                if(data.errors){
                    const modalStateErrors = [];

                    for(const key in data.errors){
                        if(data.errors[key]){
                            modalStateErrors.push(data.errors[key]);
                        }
                    }
                    throw modalStateErrors.flat();
                } else {
                    toast.error(data);
                }
                break;

            case 401:
                toast.error('Unauthorised');
                break;

            case 404:
                router.navigate('/not-found');
                break;

            case 500:
                router.navigate('/server-error', {state: {error:data}});
                break;
        
            default:
                break;
        }
        // tell the catch block of original request to handle this error.
        return Promise.reject(error);

    }
);



export default agent;