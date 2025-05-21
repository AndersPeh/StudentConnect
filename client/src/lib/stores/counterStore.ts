import {makeAutoObservable} from 'mobx';

// define CounterStore class to be used in store.ts.
export default class CounterStore {
    title = 'Counter store';
    count = 40;

    constructor(){
        // target the class (this = accessing class properties), makeAutoObservable automatically makes properties of the class (title and count observables).
        // By observing these properties, React re-renders when there is any changes to them.
        makeAutoObservable(this);
    }

    // makeAutoObservable automatically turns class methods into actions which are used to modify state of observables.
    // arrow function allows this method to be bound automatically to the class without specifying bind.
    // must specify which observable state will be modify.
    increment = (amount = 1) =>{
        this.count += amount;
    }

    decrement = (amount = 1) =>{
        this.count -= amount;
    }


};