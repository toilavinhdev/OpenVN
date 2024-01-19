import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FirstCheckerComponent } from './shared/components/first-checker.component';
import { Routing } from './shared/constants/common.constant';
import { BaseGuard } from './shared/guards/base.guard';
import { SignInUpGuard } from './shared/guards/sign-in-up.guard';
import { BaseResolver } from './shared/resolvers/base.resolver';
import { SignOutGuard } from './shared/guards/sign-out.guard';
import { AnonymousResolver } from './shared/resolvers/anonymous.resolver';
const routes: Routes = [
  {
    path: '',
    component: FirstCheckerComponent,
  },
  {
    path: Routing.TOKEN_RECEIVER.path,
    loadChildren: () => import('./shared/components/element/token-receiver/token-receiver.module').then(m => m.TokenReceiverModule),
    data: {
      key: Routing.TOKEN_RECEIVER.key,
      exponents: Routing.TOKEN_RECEIVER.actionExponents
    }
  },
  {
    path: Routing.NOT_FOUND.path,
    loadChildren: () => import('./shared/components/element/not-found/not-found.module').then(m => m.NotFoundModule),
    runGuardsAndResolvers: 'always',
    data: {
      key: Routing.NOT_FOUND.key,
    }
  },
  {
    path: Routing.ACCESS_DENIED.path,
    loadChildren: () => import('./shared/components/element/access-denied/access-denied.module').then(m => m.AccessDeniedModule),
    data: {
      key: Routing.ACCESS_DENIED.key,
    }
  },
  {
    path: Routing.UNSUPPORT_DEVICE.path,
    loadChildren: () => import('./shared/components/element/unsupport-device/unsupport-device.module').then(m => m.UnsupportDeviceModule),
    data: {
      key: Routing.UNSUPPORT_DEVICE.key,
    }
  },
  {
    path: Routing.SIGN_IN.path,
    loadChildren: () => import('./components/authentication/sign-in/sign-in.module').then(m => m.SignInModule),
    canActivate: [SignInUpGuard],
    resolve: {
      resolver: AnonymousResolver,
    },
    data: {
      key: Routing.SIGN_IN.key,
    }
  },
  {
    path: Routing.SIGN_UP.path,
    loadChildren: () => import('./components/authentication/sign-up/sign-up.module').then(m => m.SignUpModule),
    canActivate: [SignInUpGuard],
    resolve: {
      resolver: AnonymousResolver,
    },
    data: {
      key: Routing.SIGN_UP.key,
    }
  },
  {
    path: Routing.SIGN_OUT.path,
    loadChildren: () => import('./components/authentication/sign-out/sign-out.module').then(m => m.SignOutModule),
    canActivate: [SignOutGuard],
    resolve: {
      resolver: AnonymousResolver,
    },
    data: {
      key: Routing.SIGN_OUT.key,
    }
  },
  {
    path: Routing.SIGN_IN_HISTORY.path,
    loadChildren: () => import('./components/authentication/sign-in-logging/sign-in-logging.module').then(m => m.SignInLoggingModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.SIGN_IN_HISTORY.key,
      exponents: Routing.SIGN_IN_HISTORY.actionExponents
    }
  },
  {
    path: Routing.CPANEL.path,
    loadChildren: () => import('./components/cpanel/cpanel.module').then(m => m.CpanelModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.CPANEL.key,
      exponents: Routing.CPANEL.actionExponents
    }
  },
  {
    path: Routing.AUDIT_LOG.path,
    loadChildren: () => import('./components/audit-log/audit-log.module').then(m => m.AuditLogModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.AUDIT_LOG.key,
      exponents: Routing.AUDIT_LOG.actionExponents
    }
  },
  {
    path: Routing.NOTEBOOK.path,
    loadChildren: () => import('./components/notebook-v2/notebook-v2.module').then(m => m.NotebookV2Module),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.NOTEBOOK.key,
      exponents: Routing.NOTEBOOK.actionExponents
    }
  },
  {
    path: Routing.NOTEBOOK_VIEW_MODE.path,
    loadChildren: () => import('./components/notebook-view-mode/notebook-view-mode.module').then(m => m.NotebookViewModeModule),
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.NOTEBOOK_VIEW_MODE.key,
      exponents: Routing.NOTEBOOK_VIEW_MODE.actionExponents
    }
  },
  {
    path: Routing.DIRECTORY.path,
    loadChildren: () => import('./components/cloud/cloud.module').then(m => m.CloudModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.DIRECTORY.key,
      exponents: Routing.DIRECTORY.actionExponents
    }
  },
  {
    path: Routing.INFORMATION.path,
    loadChildren: () => import('./components/request-information/request-information.module').then(m => m.RequestInformationModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.INFORMATION.key,
      exponents: Routing.INFORMATION.actionExponents
    }
  },
  {
    path: Routing.CHAT_GENERATOR.path,
    loadChildren: () => import('./components/chat-generator/chat-generator.module').then(m => m.GenChatModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.CHAT_GENERATOR.key,
      exponents: Routing.CHAT_GENERATOR.actionExponents
    }
  },
  {
    path: Routing.FEEDBACK.path,
    loadChildren: () => import('./components/feedback/feedback.module').then(m => m.FeedbackModule),
    canActivate: [BaseGuard],
    resolve: {
      resolver: BaseResolver,
    },
    data: {
      key: Routing.FEEDBACK.key,
      exponents: Routing.FEEDBACK.actionExponents
    }
  },
  {
    path: Routing.SESSION_LOG.path,
    loadChildren: () => import('./components/session-trace-log/session-trace-log.module').then(m => m.SessionTraceLogModule),
    resolve: {
      resolver: AnonymousResolver,
    },
    data: {
      key: Routing.SESSION_LOG.key,
      exponents: Routing.SESSION_LOG.actionExponents
    }
  },
  {
    path: "**",
    redirectTo: `/${Routing.NOT_FOUND.path}`,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
