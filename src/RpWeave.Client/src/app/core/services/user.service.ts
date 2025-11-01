import { Injectable } from "@angular/core";
import { jwtDecode } from "jwt-decode";

export type User = {
    id: string;
    role: string;
    name: string;
}

@Injectable({providedIn: 'root'})
export class UserService {
    private _user: User | undefined;
    private _accessToken: string | undefined;

    get user() {
        return this._user;
    }

    get accessToken() {
        return this._accessToken;
    }

    loadUser(token: string) {
        this._accessToken = token;
        this._user = jwtDecode<User>(token);
    }

    clearUser() {
        this._user = undefined;
        this._accessToken = undefined;
    }
}