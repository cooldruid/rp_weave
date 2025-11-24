import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../core/clients/rpweave.client";
import { ChangePasswordModel } from "./models/change-password.model";

@Injectable({providedIn: 'root'})
export class ChangePasswordService {
    constructor(private client: RpWeaveClient) 
    { }

    async changePassword(request: ChangePasswordModel) {
        await this.client.postAsync('api/user/change-password', request);
    }
}