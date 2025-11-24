import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../core/clients/rpweave.client";
import { UserService } from "../../core/services/user.service";

@Injectable({providedIn: 'root'})
export class SignupService {
    constructor(
        private rpWeaveClient: RpWeaveClient,
        private userService: UserService)
    { }

    public async signUpAsync(username: string, password: string) {
        const model: SignupRequestModel = {
            username: username,
            password: password
        }

        await this.rpWeaveClient.postAsync<SignupRequestModel, void>('api/user/register', model);
    }
}

export type SignupRequestModel = {
    username: string;
    password: string;
}