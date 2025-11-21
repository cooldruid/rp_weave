import { HttpEvent, HttpHandler, HttpHandlerFn, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { catchError, from, lastValueFrom, Observable, throwError } from "rxjs";
import { UserService } from "../services/user.service";
import { inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { RefreshTokenModel } from "../models/refresh-token.model";
import { RpWeaveClient } from "../clients/rpweave.client";

export function authenticationInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  const userService = inject(UserService);
  const router = inject(Router);
  const rpWeaveClient = inject(RpWeaveClient);
  
  let isRefreshing = false;
  let refreshPromise: Promise<RefreshTokenModel> | undefined;

  const token = userService.accessToken;

  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(authReq).pipe(
    catchError(async err => {
      if (err.status === 401) {
        if (!isRefreshing) {
          isRefreshing = true;
          refreshPromise = rpWeaveClient.postAsync<null, RefreshTokenModel>('api/user/refresh-token', null)
            .then(newToken => {
              isRefreshing = false;
              refreshPromise = undefined;
              return newToken;
            })
            .catch(err => {
              isRefreshing = false;
              refreshPromise = undefined;
              userService.clearUser();
              router.navigate(['/login']);
              throw err;
            });
        } 

        const newToken = await refreshPromise;

        if (!newToken) {
          throw new Error('Token refresh failed');
        }

        const cloned = req.clone({
          setHeaders: { Authorization: `Bearer ${newToken.accessToken}` }
        });

        return await lastValueFrom(next(cloned));
      }
      throw err;
  }));
}