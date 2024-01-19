import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { TranslationService } from 'src/app/shared/services/base/translation.service';


@Component({
  selector: 'app-resolver-mark',
  templateUrl: './resolver-mark.component.html',
  styleUrls: ['./resolver-mark.component.scss']
})
export class ResolverMarkComponent implements OnInit {
  w = "0";
  isShow = false;
  loadingText = "";
  dots = ".";

  constructor(
    public authService: AuthService
  ) { }

  ngOnInit(): void {
    let val = 1;
    let sid = setInterval(() => {
      this.w = ++val + "%";
      if (val >= 100) {
        clearInterval(sid);
        this.isShow = true;
        this.loadingText = TranslationService.VALUES['COMMON']['LOADING'];
      }
    }, 25);
    setInterval(() => {
      if (this.dots == '...') {
        this.dots = '';
      }
      else {
        this.dots += '.';
      }
    }, 500);
  }

}
