import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { BaseNotFound as NotFound } from './not-found.component';
import { BaseNotFoundRoutingModule as NotFoundRoutingModule } from './not-found-routing.module';
import { BaseButtonModule } from '../../micro/button/button.module';

@NgModule({
  declarations: [NotFound],
  imports: [
    CommonModule,
    NotFoundRoutingModule,
    BaseButtonModule,
  ],
  exports: [NotFound]
})
export class NotFoundModule { }
