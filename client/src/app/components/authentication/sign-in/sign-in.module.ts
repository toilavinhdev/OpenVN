import { NgModule } from '@angular/core';
import { CommonModule} from '@angular/common';

import { SignInRoutingModule } from './sign-in-routing.module';
import { SignInComponent } from './sign-in.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { BaseButtonModule } from 'src/app/shared/components/micro/button/button.module';
import { LanguageModule } from 'src/app/shared/components/micro/language/language.module';

@NgModule({
  declarations: [SignInComponent],
  imports: [
    CommonModule,
    SignInRoutingModule,
    LanguageModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    BaseButtonModule,
    TranslateModule,
  ],
  exports: [SignInComponent]
})
export class SignInModule { }
