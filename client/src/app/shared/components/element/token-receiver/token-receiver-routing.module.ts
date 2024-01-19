import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TokenReceiverComponent } from './token-receiver.component';

const routes: Routes = [
  {
    path: '',
    component: TokenReceiverComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TokenReceiverRoutingModule { }
