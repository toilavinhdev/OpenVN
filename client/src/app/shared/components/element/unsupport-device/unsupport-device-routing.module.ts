import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnsupportDeviceComponent } from './unsupport-device.component';

const routes: Routes = [
  {
    path: '',
    component: UnsupportDeviceComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UnsupportDeviceRoutingModule { }
