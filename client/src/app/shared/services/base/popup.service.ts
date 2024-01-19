import { Injectable } from '@angular/core';
import { DialogPosition, MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { BreakPoint } from '../../constants/common.constant';

@Injectable({
  providedIn: 'root'
})
export class PopupService {

  private config!: MatDialogConfig;

  constructor(
    public dialog: MatDialog
  ) {

  }

  getBaseConfig() {
    this.config = new MatDialogConfig();
    const position: DialogPosition = {};
    position.top = '50px';

    const currentScreenWidth = window.innerWidth;
    let configWidth = '80%';
    let configHeight = '120px';
    const maxWidth = '80%';
    const maxHeight = '280px';

    if (currentScreenWidth < BreakPoint.SM) {
      configWidth = '80%';
      configHeight = '100px';
    } else if (currentScreenWidth >= BreakPoint.SM && currentScreenWidth < BreakPoint.MD) {
      configWidth = '480px';
    } else {
      configWidth = '440px';
    }

    // this._config.minWidth = '440px';
    this.config.minWidth = configWidth;
    this.config.maxWidth = maxWidth;
    this.config.minHeight = configHeight;
    this.config.maxHeight = maxHeight;
    this.config.position = position;

    return this.config;
  }

  maxPingConfig(minWidth?: number, minHeight?: number) {
    if(!minWidth) {
      minWidth = 760;
    }
    if(!minHeight) {
      minHeight = 240;
    }

    const config = new MatDialogConfig();
    config['minWidth'] = (window.innerWidth * 0.88 < minWidth ? window.innerWidth * 0.88 : minWidth) + 'px';
    config['maxWidth'] = window.innerWidth * 0.88 + 'px';
    config['minHeight'] =  (window.innerHeight * 0.88 < minHeight ? window.innerHeight * 0.88 : minHeight) + 'px';
    config['maxHeight'] = window.innerHeight * 0.88 + 'px';

    return config;
  }
}
