import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BaseNotFound } from './not-found.component';

const routes: Routes = [
  {
    path: "",
    component: BaseNotFound
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BaseNotFoundRoutingModule { }
