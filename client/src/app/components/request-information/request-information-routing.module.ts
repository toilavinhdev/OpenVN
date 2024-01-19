import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RequestInformationComponent } from './request-information.component';

const routes: Routes = [
  {
    path: '',
    component: RequestInformationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RequestInformationRoutingModule { }
