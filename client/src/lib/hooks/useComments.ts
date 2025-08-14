import { useLocalObservable } from "mobx-react-lite";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from "@microsoft/signalr";
import { useEffect } from "react";

// useComments takes optional activityId as a parameter to manage
// SignalR connection for comments on a specific activity.
export const useComments = (activityId?: string) => {
  // use () => ({}) to directly return the function without having to specify the return statement.
  // Create a MobX observable object called commentStore with a property named hubConnection.
  // useLocalObservable means commentStore is created once when the component mounts, it always returns same object (hubConnection)
  // across re-renders except when unmounted.
  // Any MobX observer wrapped component that uses this store will automatically re-render when
  // commentStore property (hubConnection) changes.
  const commentStore = useLocalObservable(() => ({
    // hubConnection is null by default. hubConnection is type HubConnection (when assigned by createHubConnection) or null.
    hubConnection: null as HubConnection | null,

    // createHubConnection method on commentStore takes activityId to build a new SignalR connection.
    createHubConnection(activityId: string) {
      // can't connect to SignalR without activityId. returns early as no connection is made.
      if (!activityId) return;

      //.withUrl adds query parameter activityId to the backend SignalR Hub URL (https://localhost:5001/comments).
      // Fir example, the URL will be https://localhost:5001/comments?activityId=1
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(
          `${import.meta.env.VITE_COMMENT_URL}?activityId=${activityId}`,
          {
            // pass user's cookie along with this request for authentication.
            withCredentials: true,
          }
        )
        // automatical reconnection if fail to connect or lose connection to SignalR.
        .withAutomaticReconnect()

        // creates the connection instance withUrl, credentials and automaticreconnect.
        // The connection instance will be stored in this.hubConnection.
        .build();

      // Start the SignalR connection. If it fails, logs the error.
      this.hubConnection
        .start()
        .catch((error) =>
          console.log("Error establishing connection: ", error)
        );
    },

    // After creating createHubConnection method, create a stopHubConnection method on commentStore
    // for stopping the SignalR connection if it is currently connected.
    stopHubConnection() {
      if (this.hubConnection?.state === HubConnectionState.Connected) {
        this.hubConnection
          .stop()

          // log error if stopping fails.
          .catch((error) => console.log("Error stopping connection: ", error));
      }
    },
  }));

  // Manage SignalR connection lifecycle with useEffect.
  useEffect(() => {
    // If activityId is set, call commentStore.createHubConnection from above to start a new SignalR connection of that activity.
    if (activityId) {
      commentStore.createHubConnection(activityId);
    }

    // Cleanup function. When the component unmounts because of activityId changes,
    // it calls stopHubConnection to clean up the connection.
    return () => {
      commentStore.stopHubConnection();
    };
    // First render or When activityId or commentStore changes, it calls createHubConnection
    // to start a new connection for the specific activity.
    // It is defensive to put commentStore in the dependency array as it doesnt change.
    // but it may be changed to become unstable in the future, putting it there to rerun if it ever happens.
  }, [activityId, commentStore]);

  // return commentStore object so components using this hook can access the SignalR
  // connection.
  return {
    commentStore,
  };
};
