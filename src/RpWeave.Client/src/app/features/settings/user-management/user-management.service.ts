import { Injectable } from "@angular/core";
import { RpWeaveClient } from "../../../core/clients/rpweave.client";
import { UserManagementList } from "./models/user-management-list.model";
import { AddUserDialogResponse } from "./add-user-dialog/models/add-user-dialog-response.model";

@Injectable({providedIn: 'root'})
export class UserManagementService {
    constructor(private client: RpWeaveClient)
    { }

    public async listUsers() : Promise<UserManagementList> {
        return await this.client.getAsync<UserManagementList>('api/settings/list-users', {});
    }

    public async addUser(user: AddUserDialogResponse) {
        await this.client.postAsync<AddUserDialogResponse, void>(
            'api/settings/create-user',
            user
        );
    }
}