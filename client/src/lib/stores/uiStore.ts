import { makeAutoObservable } from "mobx";

export class UiStore {

    // observable:
    isLoading = false;

    constructor(){
        makeAutoObservable(this);
    }

    // actions:
    isBusy(){
        this.isLoading = true;
    }

    isIdle(){
        this.isLoading = false;
    }
}