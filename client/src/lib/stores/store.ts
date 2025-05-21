import { createContext } from "react";
import CounterStore from "./counterStore";
import { UiStore } from "./uiStore";

// In typescript interface, can only define the shape of an object + its properties names and types.
// cannot assign value or instantiates classes in the interface.
interface Store{
    counterStore: CounterStore;

    // uiStore is property of the store, a class of type UiStore. 
    uiStore: UiStore;
};

// export store, a type of Store interface. can assign value here.
export const store: Store ={

    counterStore: new CounterStore(),

    // create a new instance of UiStore in uiStore.
    uiStore: new UiStore(),

};

// create a React Context for this store, will be used to make store instance available globally.
export const StoreContext = createContext(store);