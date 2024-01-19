import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
// import {MatDividerModule} from '@angular/material/divider';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BaseButton } from './button.component';

@NgModule({
  declarations: [BaseButton],
  imports: [
    CommonModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  exports: [
    BaseButton
  ]
})
export class BaseButtonModule { }
