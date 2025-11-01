import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { catchError, from, lastValueFrom, Observable, throwError } from "rxjs";
import { UserService } from "../services/user.service";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { RefreshTokenModel } from "../models/refresh-token.model";
import { RpWeaveClient } from "../clients/rpweave.client";

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshPromise: Promise<RefreshTokenModel> | undefined;

  constructor(
    private userService: UserService, 
    private router: Router,
    private rpWeaveClient: RpWeaveClient) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.userService.accessToken;

    let authReq = req;
    if (token) {
      authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(authReq).pipe(
      catchError(err => {
        if (err.status === 401) {
          return from(this.handle401(authReq, next));
        }
        return throwError(() => err);
      })
    );
  }

  private async handle401(req: HttpRequest<any>, next: HttpHandler): Promise<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshPromise = this.rpWeaveClient.postAsync<null, RefreshTokenModel>('user/refresh-token', null)
        .then(newToken => {
          this.isRefreshing = false;
          this.refreshPromise = undefined;
          return newToken;
        })
        .catch(err => {
          this.isRefreshing = false;
          this.refreshPromise = undefined;
          this.userService.clearUser();
          this.router.navigate(['/login']);
          throw err;
        });
    }

    const newToken = await this.refreshPromise;

    if (!newToken) {
      throw new Error('Token refresh failed');
    }

    const cloned = req.clone({
      setHeaders: { Authorization: `Bearer ${newToken.accessToken}` }
    });

    return await lastValueFrom(next.handle(cloned));
  }
}
