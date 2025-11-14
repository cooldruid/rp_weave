import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { lastValueFrom } from "rxjs";
import { environment } from "../../../environments/environment";

@Injectable({providedIn: 'root'})
export class RpWeaveClient {
    constructor(private httpClient: HttpClient)
    { }

    public async postAsync<TRequest, TResponse>(url: string, body: TRequest): Promise<TResponse> {
        const post = this.httpClient.post<TResponse>(
            this.createFullUrl(url), 
            body);
        return await lastValueFrom(post);
    }

    public async getAsync<TResponse>(url: string, queryParams: {}) : Promise<TResponse> {
        const options = { params: new HttpParams().appendAll(queryParams)};
        
        const get = this.httpClient.get<TResponse>(
            this.createFullUrl(url),
            options
        );

        return await lastValueFrom(get);
    }

    private createFullUrl(url: string) {
        return new URL(url, environment.rpWeaveApiUrl).toString();
    }
}