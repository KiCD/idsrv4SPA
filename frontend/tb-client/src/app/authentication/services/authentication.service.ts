import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { getClientSettings } from './oid-client-settings';
import { Router } from '@angular/router';
import { OidcStorageService } from './oidc-storage.service';
import { UserIdleService } from './user-idle.service';
import * as Oidc from 'oidc-client';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private manager: UserManager = new UserManager(getClientSettings());
  constructor(private router: Router, private oidcStorage: OidcStorageService, public userIdleService: UserIdleService) {
    Oidc.Log.level = Oidc.Log.DEBUG;
    Oidc.Log.logger = console;

    this.manager.events.addAccessTokenExpiring(() => {
      if (!this.userIdleService.isUserIdle()) {
        this.signinSilent();
      }
    });

    this.manager.events.addAccessTokenExpired(() => {
      console.log('Access token expired');
      this.handleUserLoggedOut();
    });

    this.manager.events.addUserLoaded(user => {
      this.oidcStorage.storeAuthData(user);
    });
    this.manager.events.addUserSignedOut(() => this.handleUserLoggedOut());
  }

  public handleUserLoggedOut() {
    this.logout();
  }

  clearAuthData() {
    this.oidcStorage.clearAuthData();
  }

  isLoggedIn(): boolean {
    const expiresAt = this.oidcStorage.getExpirationTime();
    if (!expiresAt) {
      return false;
    }
    const currentTime = Math.floor(Date.now() / 1000);
    return expiresAt > currentTime;
  }

  getAuthorizationHeaderValue(): string {
    if (!this.isLoggedIn()) {
      return '';
    }
    const tokenType = this.oidcStorage.getTokenType();
    const accessToken = this.oidcStorage.getAccessToken();
    return `${tokenType} ${accessToken}`;
  }

  signinSilent(): Promise<any> {
    return this.manager.signinSilent().then(user => {
      console.log('AuthenticationService signin Silent successful');
    }, err => {
      console.log('AuthenticationService signin Silent failed', err.message);
    });
  }

  startAuthentication(): Promise<User> {
    console.log('starting authentication..');
    return this.manager.signinRedirect().catch(error => {
      console.log('startAuthentication: failed on signin redirect: ' + error);
      this.redirectToLogin();
    });
  }

  completeAuthentication(): Promise<void> {
    console.log('completing authentication..');
    return this.manager
      .signinRedirectCallback()
      .then(user => {
        console.log('authentication completed.');
        this.oidcStorage.storeAuthData(user);
        window.location.hash = '';
      })
      .catch(function(err) {
        console.log(
          'completeAuthentication: failed on signin redirect callback: ' + err
        );
        this.redirectToLogin();
      });
  }

  revoke(): Promise<void> {
    return this.manager.revokeAccessToken().then(t => {
      this.redirectToLogin();
    });
  }

  logout(): Promise<void> {
    console.log('logout requested');
    const idToken = this.oidcStorage.getIdToken();
    this.oidcStorage.clearAuthData();
    return this.manager.signoutRedirect({ id_token_hint: idToken });
  }

  private redirectToLogin() {
    this.router.navigate([
      '/login'
    ]);
  }
}
