import { Injectable } from '@angular/core';
import { UserManagerSettings } from 'oidc-client';
import { getClientSettings } from './oid-client-settings';

@Injectable({
  providedIn: 'root'
})
export class OidcStorageService {
  private hasStorage: boolean;
  private authConfiguration: UserManagerSettings;
  private USER_PROFILE_KEY = 'profile';
  private ACCESS_TOKEN_KEY = 'access_token';
  private ID_TOKEN_KEY = 'id_token';
  private EXPIRES_AT_KEY = 'expires_at';
  private TOKEN_TYPE_KEY = 'token_type';
  private AUTHENTICATION_METHOD_KEY = 'authentication_method';

  constructor() {
    this.hasStorage = typeof Storage !== 'undefined';
    this.authConfiguration = getClientSettings();
  }
  public read(key: string): any {
    if (this.hasStorage) {
      return JSON.parse(sessionStorage.getItem(this.authConfiguration.client_id + '_' + key));
    }

    return;
  }

  public write(key: string, value: any): void {
    if (this.hasStorage) {
      value = value === undefined ? null : value;
      sessionStorage.setItem(this.authConfiguration.client_id + '_' + key, JSON.stringify(value));
    }
  }

  public remove(key: string): void {
    if (this.hasStorage) {
      sessionStorage.removeItem(this.authConfiguration.client_id + '_' + key);
    }
  }

  public getExpirationTime(): any {
    return this.read(this.EXPIRES_AT_KEY);
  }

  public storeAuthData(user: any) {
    this.write(this.ACCESS_TOKEN_KEY, user.access_token);
    this.write(this.ID_TOKEN_KEY, user.id_token);
    this.write(this.TOKEN_TYPE_KEY, user.token_type);
    this.write(this.EXPIRES_AT_KEY, user.expires_at);
    this.write(this.USER_PROFILE_KEY, user.profile);
    if (user.profile && user.profile.amr && user.profile.amr.length > 0) {
      this.write(this.AUTHENTICATION_METHOD_KEY, user.profile.amr[0]);
    }
  }

  public clearAuthData() {
    this.remove(this.ACCESS_TOKEN_KEY);
    this.remove(this.ID_TOKEN_KEY);
    this.remove(this.TOKEN_TYPE_KEY);
    this.remove(this.EXPIRES_AT_KEY);
    this.remove(this.USER_PROFILE_KEY);
    this.remove(this.AUTHENTICATION_METHOD_KEY);
  }

  public getTokenType(): any {
    return this.read(this.TOKEN_TYPE_KEY);
  }
  public getIdToken(): any {
    return this.read(this.ID_TOKEN_KEY);
  }
  public getAccessToken(): any {
    return this.read(this.ACCESS_TOKEN_KEY);
  }
  public getProfile(): any {
    return this.read(this.USER_PROFILE_KEY);
  }
  public getAuthenticationMethod(): any {
    return this.read(this.AUTHENTICATION_METHOD_KEY);
  }
}
