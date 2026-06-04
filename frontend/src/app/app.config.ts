import {
  APP_INITIALIZER,
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import Aura from '@primeuix/themes/aura';

import { routes } from './app.routes';
import { provideAnimations } from '@angular/platform-browser/animations';
import { providePrimeNG } from 'primeng/config';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { MessageService } from 'primeng/api';
import { authInterceptor } from './core/interceptors/auth-interceptor';
import { InitService } from './core/services/init.service';
import { MSAL_INSTANCE, MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './core/auth-config';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([errorInterceptor, authInterceptor])),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAnimations(),
    providePrimeNG({ theme: { preset: Aura, options: { darkModeSelector: false } } }),
    MessageService,
    MsalService,
    {
      provide: MSAL_INSTANCE,
      useFactory: () => new PublicClientApplication(msalConfig)
    },
    MsalBroadcastService,
    provideAppInitializer(() => {
      const initService = inject(InitService);
      return initService.init();
    }),
  ],
};
