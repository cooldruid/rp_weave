import { Injectable } from "@angular/core";
import { jwtDecode } from "jwt-decode";
import { BehaviorSubject } from "rxjs";

export type User = {
    id: string;
    role: string;
    name: string;
}

@Injectable({providedIn: 'root'})
export class UserService {
    private _user$ = new BehaviorSubject<User | undefined>(undefined);
    user$ = this._user$.asObservable();
    
    private _accessToken: string | undefined;

    constructor() {
        // Restore access token from localStorage on app start
        const token = localStorage.getItem('accessToken');
        if (token) this.loadUser(token);
    }

    get user() {
        return this._user$.value;
    }

    get accessToken() {
        return this._accessToken;
    }

    loadUser(token: string) {
        this._accessToken = token;
        localStorage.setItem('accessToken', token);
        this._user$.next(jwtDecode<User>(token));
    }

    clearUser() {
        this._user$.next(undefined);
        this._accessToken = undefined;
        localStorage.removeItem('accessToken');
    }
}