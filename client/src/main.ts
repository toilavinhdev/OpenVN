import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { inject } from '@vercel/analytics';
import { SessionStorageKey } from './app/shared/constants/sessionstorage.key';
import { DateHelper } from './app/shared/helpers/date.helper';

console.customize = (message?: any, ...options: any[]) => {
  customizeLog('customize', message, ...options);
}

console.success = (message: any) => {
  customizeLog('success', message);
}

console.warn = (message?: any, ...options: any[]) => {
  customizeLog('warning', message, ...options);
}

console.error = (message?: any, ...options: any[]) => {
  customizeLog('error', message, ...options);
}

if (environment.production) {
  enableProdMode();
}
// inject();

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));


function customizeLog(type: string, message?: any, ...options: any[]) {
  console.log(message, ...options);

  let logs = sessionStorage.getItem(SessionStorageKey.SESSION_LOG);
  let arr = [];
  message = `<span class='${type}'>[${DateHelper.getTimeOnly(new Date())}]: ${JSON.stringify(message)} ${tryGetMessage(...options)}</span>`;

  if (!logs) {
    arr.push(message);
  } else {
    arr = JSON.parse(logs);
    arr.push(message);
  }
  sessionStorage.setItem(SessionStorageKey.SESSION_LOG, JSON.stringify(arr));
}


function tryGetMessage(...options: any[]) {
  return options.map(o => {
    try {
      const msg = JSON.stringify(o);
      return msg;
    } catch {
      return "";
    }
  }).join(' ');
}
