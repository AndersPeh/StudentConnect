import {action, makeObservable, observable} from 'mobx';

// define CounterStore class to be used in store.ts.
export default class CounterStore {
    title = 'Counter store';
    count = 40;

    constructor(){

        // target the class (this = accessing class properties), make its properties title and count observables.
        // By observing these properties, React can react when there is any changes to them.
        makeObservable(this, {
            title: observable,
            count: observable,

            // actions are used to modify state of observables.
            increment: action,
            decrement: action,
        })
    }

    // arrow function allows this method to be bound automatically to the class without specifying bind.
    // must specify which observable state will be modify.
    increment = (amount = 1) =>{
        this.count += amount;
    }

    decrement = (amount = 1) =>{
        this.count -= amount;
    }


};