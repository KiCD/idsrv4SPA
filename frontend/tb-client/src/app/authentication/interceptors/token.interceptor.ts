import { Injectable, Injector } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, empty } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { EMPTY } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  authService: AuthenticationService;
  constructor(private injector: Injector, private router: Router) {
    setTimeout(() => {
      this.authService = this.injector.get(AuthenticationService);
    });
  }
  addToken(req: HttpRequest<any>): HttpRequest<any> {
    return req.clone({ setHeaders: { Authorization: this.authService.getAuthorizationHeaderValue() } });
  }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (this.authService === undefined) {
      this.authService = this.injector.get(AuthenticationService);
    }

    const request = this.addToken(req);
    return next.handle(request).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse) {
          if (error.status === 400) {
            return this.handle400Error(error);
          }
          if (error.status === 401) {
            this.authService.clearAuthData();
            this.authService.startAuthentication();
            return EMPTY;
        }
      }
        return throwError(error);
      }
    ));

  }
  handle400Error(error) {
    if (this.authService.isLoggedIn() && error && error.status === 400 && error.error && error.error.error === 'invalid_grant') {
      // If we get a 400 and the error message is 'invalid_grant', the token is no longer valid so logout.
      this.authService.logout();
      return empty();
    } else {
      return throwError(error);
    }
  }
}
