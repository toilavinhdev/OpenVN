import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UnsupportDeviceRoutingModule } from './unsupport-device-routing.module';
import { UnsupportDeviceComponent } from './unsupport-device.component';


@NgModule({
  declarations: [UnsupportDeviceComponent],
  imports: [
    CommonModule,
    UnsupportDeviceRoutingModule
  ]
})
export class UnsupportDeviceModule { }
