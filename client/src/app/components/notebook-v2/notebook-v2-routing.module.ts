import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotebookV2Component } from './notebook-v2.component';

const routes: Routes =
  [
    {
      path: '',
      component: NotebookV2Component
    }
  ];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NotebookV2RoutingModule { }
