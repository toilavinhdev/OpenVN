import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SnackBar } from './snackbar.component';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@NgModule({
  declarations: [SnackBar],
  imports: [
    CommonModule,
    MatSnackBarModule
  ],
  exports: [SnackBar],
})
export class SnackbarModule { }
