import { Component, OnInit } from '@angular/core';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-moving-progess',
  templateUrl: './moving-progess.component.html',
  styleUrls: ['./moving-progess.component.scss']
})
export class MovingProgessComponent implements OnInit {

  style = {};

  percentage = 100;

  constructor() { }

  ngOnInit(): void {
    this.fakeProgress();
  }

  fakeProgress() {
    const id = setInterval(() => {
      this.percentage -= Utility.randomInRange(1, 10);
      if (this.percentage <= 10) {
        clearInterval(id);
      }
      this.style['right'] = this.percentage + '%';
    }, 150);
  }
}
