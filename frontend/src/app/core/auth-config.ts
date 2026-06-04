import { Configuration, BrowserCacheLocation } from '@azure/msal-browser';

export const msalConfig: Configuration = {
  auth: {
    clientId: '1bd82282-13b3-455d-8109-765597db213f',
    authority: 'https://login.microsoftonline.com/common',
    redirectUri: 'http://localhost:4200'
  },
  cache: {
    cacheLocation: BrowserCacheLocation.SessionStorage
  },
};

export const loginRequest = {
  scopes: ['openid', 'profile', 'email']
};
