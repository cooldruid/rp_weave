import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { lastValueFrom } from "rxjs";

@Injectable({providedIn: 'root'})
export class RpWeaveClient {
    constructor(private httpClient: HttpClient)
    { }

    public async postAsync<TRequest, TResponse>(url: string, body: TRequest): Promise<TResponse> {
        const post = this.httpClient.post<TResponse>(
            url, 
            body,
            {
                withCredentials: true
            });
        return await lastValueFrom(post);
    }

    public async getAsync<TResponse>(url: string, queryParams: {}) : Promise<TResponse> {
        const options = { 
            params: new HttpParams().appendAll(queryParams),
            withCredentials: true
        };
        
        const get = this.httpClient.get<TResponse>(
            url,
            options
        );

        return await lastValueFrom(get);
    }
}