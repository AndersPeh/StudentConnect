import { createContext } from "react";
import CounterStore from "./counterStore";

// In typescript interface, can only define the shape of an object + its properties names and types.
// cannot assign value or instantiates classes in the interface.
interface Store{
    // counterStore is property of the store, a class of type CounterStore. 
    counterStore: CounterStore
};

// export store, a type of Store interface. can assign value here.
export const store: Store ={
    // create a new instance of CounterStore in counterStore.
    counterStore: new CounterStore()
};

// create a React Context for this store, will be used to make store instance available globally.
export const StoreContext = createContext(store);