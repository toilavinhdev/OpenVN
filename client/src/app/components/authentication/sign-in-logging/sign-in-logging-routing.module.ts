import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SignInLoggingComponent } from './sign-in-logging.component';

const routes: Routes = [
  {
    path: '',
    component: SignInLoggingComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SignInLoggingRoutingModule { }
