import { UserManagerSettings } from 'oidc-client';
import { environment } from 'src/environments/environment';
export function getClientSettings(): UserManagerSettings {
  console.log('environment authority is:', environment.authority);
  return {
    authority: environment.authority,
    client_id: 'tbjsclient',
    redirect_uri: environment.authRedirectUri,
    post_logout_redirect_uri: environment.webClientEndPoint,
    response_type: 'code',
    response_mode: 'query',
    scope: 'openid profile documentapi',
    filterProtocolClaims: true,
    loadUserInfo: true,
    automaticSilentRenew: false,
    checkSessionInterval: 1700000,
    accessTokenExpiringNotificationTime: 60,
    silent_redirect_uri: environment.silentRedirectUri
  };
}
