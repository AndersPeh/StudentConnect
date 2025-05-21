import axios from "axios";
import { store } from "../stores/store";

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
agent.interceptors.response.use(async response =>{
    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    } finally{
        // turn off isLoading regardless of the request response.
        store.uiStore.isIdle();
    }
});

export default agent;