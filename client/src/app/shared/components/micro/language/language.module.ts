import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LanguageComponent } from './language.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [LanguageComponent],
  imports: [
    CommonModule,
    MatTooltipModule,
    TranslateModule,
  ],
  exports: [LanguageComponent]
})
export class LanguageModule { }
