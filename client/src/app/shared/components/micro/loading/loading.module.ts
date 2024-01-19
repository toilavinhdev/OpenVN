import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonLoading } from './skeleton-loading/skeleton-loading.component';
import { LoadingComponent } from './loading/loading.component';
import { ProgessSpinnerLoadingComponent } from './progess-spinner-loading/progess-spinner-loading.component';
import { OpenLoadingComponent } from './open-loading/open-loading.component';

@NgModule({
  declarations: [LoadingComponent, SkeletonLoading, ProgessSpinnerLoadingComponent, OpenLoadingComponent],
  imports: [
    CommonModule,
  ],
  exports: [LoadingComponent, SkeletonLoading, ProgessSpinnerLoadingComponent, OpenLoadingComponent],
})
export class BaseLoadingModule { }
