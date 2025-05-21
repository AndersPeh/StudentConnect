import {makeAutoObservable} from 'mobx';

// define CounterStore class to be used in store.ts.
export default class CounterStore {

    // properties automatically become observables.
    title = 'Counter store';
    count = 40;
    events: string[] =[
        `Initial count is ${this.count}`
    ];

    constructor(){
        // target the class (this = accessing class properties), makeAutoObservable automatically makes properties of the class (title and count observables).
        // By observing these properties, React re-renders when there is any changes in them.
        makeAutoObservable(this);
    }

    // makeAutoObservable automatically turns class methods into actions which are used to modify state of observables.
    // arrow function allows this method to be bound automatically to the class without specifying bind.
    // must specify which observable state will be modify.
    increment = (amount = 1) =>{
        this.count += amount;
        this.events.push(`Incremented by ${amount}. Count is now ${this.count}.`);
    }

    decrement = (amount = 1) =>{
        this.count -= amount;
        this.events.push(`Decremented by ${amount}. Count is now ${this.count}.`);
    }

    // makeAutoObservable turns get accessor into computed property. Computed property is updated when there is any changes in observables.
    // Because the value of the computed property is derived from observables. It's cached and only changes when underlying observable changes.
    get eventCount(){
        return this.events.length;
    }
};