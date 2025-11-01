import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../core/clients/rpweave.client";
import { UserService } from "../../core/services/user.service";

@Injectable({providedIn: 'root'})
export class LoginService {
    constructor(
        private rpWeaveClient: RpWeaveClient,
        private userService: UserService)
    { }

    public async loginAsync(username: string, password: string) {
        const model: LoginRequestModel = {
            username: username,
            password: password
        }

        const response = await this.rpWeaveClient.postAsync<LoginRequestModel, LoginResponseModel>('api/user/login', model);

        this.userService.loadUser(response.accessToken);
    }
}

export type LoginRequestModel = {
    username: string;
    password: string;
}

export type LoginResponseModel = {
    accessToken: string;
}