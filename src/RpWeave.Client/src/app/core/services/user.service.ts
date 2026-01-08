import { Injectable } from "@angular/core";
import { jwtDecode } from "jwt-decode";
import { BehaviorSubject } from "rxjs";

export type User = {
    id: string;
    role: string;
    name: string;
    exp: number;
}

const adminRole = 'ADMIN';
const userRole = 'USER';

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

    isAdmin(): boolean {
        return this._user$.value?.role === adminRole;
    }

    isTokenValid(): boolean {
        if(!this._user$.value)
            return false;

        const date = new Date(this._user$.value.exp * 1000);
        const now = new Date();
        
        return date > now;
    }

    clearUser() {
        this._user$.next(undefined);
        this._accessToken = undefined;
        localStorage.removeItem('accessToken');
    }
}