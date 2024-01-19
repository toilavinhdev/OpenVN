import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SessionTraceLogComponent } from './session-trace-log.component';

const routes: Routes = [
  {
    path: '',
    component: SessionTraceLogComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SessionTraceLogRoutingModule { }
