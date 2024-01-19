import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { TranslationService } from 'src/app/shared/services/base/translation.service';

@Component({
  selector: 'language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.scss']
})
export class LanguageComponent {

  @Output()
  changeLanguage = new EventEmitter<string>();

  constructor(
    private translationService: TranslationService
  ) { }

  onChangeLanguage(lang: string) {
    this.translationService.use(lang);
  }
}
