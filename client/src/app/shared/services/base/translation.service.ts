import { Injectable, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LocalStorageKey } from '../../constants/localstorage.key';
import { of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { DateHelper } from '../../helpers/date.helper';
import { Utility } from '../../utility/utility';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {

  public changeLanguageEvent = new EventEmitter<string>();

  public static VALUES = {};

  private firstChange = true;

  constructor(public translateService: TranslateService) {
    this.handleEvents();
    if (JSON.stringify(TranslationService.VALUES) == '{}') {
      this.getValues();
    }
  }

  private getValues() {
    this.translateService.use(this.translateService.getDefaultLang()).subscribe(v => {
      TranslationService.VALUES = v;
    });
  }

  handleEvents() {
    this.translateService.onLangChange.subscribe(resp => TranslationService.VALUES = resp.translations);
  }

  ensureCompleteGetValue() {
    if (JSON.stringify(TranslationService.VALUES) == '{}') {
      return this.translateService
        .use(this.translateService.getDefaultLang())
        .pipe(switchMap(v => {
          TranslationService.VALUES = v;
          return of(v);
        }));
    }
    return of(TranslationService.VALUES);
  }

  use(lang: string) {
    this.translateService.use(lang).subscribe(
      () => {
        console.customize(`using language: `, lang);
        if (!this.firstChange) {
          this.changeLanguageEvent.emit(lang);
        } else {
          this.firstChange = false;
        }
      }
    );
  }
}
