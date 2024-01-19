import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotebookViewModeComponent } from './notebook-view-mode.component';

const routes: Routes = [
  {
    path: ':id',
    component: NotebookViewModeComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NotebookViewModeRoutingModule { }
