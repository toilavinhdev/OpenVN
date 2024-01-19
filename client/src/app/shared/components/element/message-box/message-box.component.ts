import { Component, OnInit } from '@angular/core';
import { DialogPosition, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { of } from 'rxjs';
import { MessageData } from 'src/app/models/message-data';
import { NotiBoxComponent } from './noti-box/noti-box.component';
import { BreakPoint } from 'src/app/shared/constants/common.constant';
import { MessageBoxType } from 'src/app/shared/enumerations/common.enum';
import { Message } from 'src/app/models/message';

@Component({
  selector: 'message-box',
  template: '',
  styles: [],
})
export class MessageBox implements OnInit {
  private static _dialog: MatDialog;
  private static _config: MatDialogConfig;
  private static isOpenning = false;
  private static dialogRef: MatDialogRef<NotiBoxComponent>;

  constructor(
    public dialog: MatDialog
  ) {
    MessageBox._dialog = dialog;
  }

  ngOnInit(): void {
    MessageBox.initConfig();
  }

  /**
   * Khởi tạo common config
   */
  private static initConfig() {
    this._config = new MatDialogConfig();
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
      configWidth = '400px';
    } else {
      configWidth = '440px';
    }

    // this._config.minWidth = '440px';
    this._config.minWidth = configWidth;
    this._config.maxWidth = maxWidth;
    this._config.minHeight = configHeight;
    this._config.maxHeight = maxHeight;
    this._config.position = position;
    this._config.panelClass = ['message-box'];
  }

  /**
   * Chuẩn bị config để open dialog
   */
  private static prepareConfig(message: Message, boxType: MessageBoxType) {
    if (!this._config) {
      MessageBox.initConfig();
    }
    const config = this._config;

    if (!message.data) {
      message.data = new MessageData();
    }
    message.data.boxType = boxType;
    config.data = message;

    return config;
  }


  /**
   * Mở dialog confirm
   * Cách dùng: Ví dụ tại component A, muốn mở dialog:
   * MessageBox.openConfirm( new Message(this, {title: 'your-title', content: 'your-content', 'your-callback'}) );
   */
  public static confirm(message: Message) {
    const config = this.prepareConfig(message, MessageBoxType.Confirm);
    if (MessageBox.dialogRef && MessageBox.dialogRef._containerInstance) {
      const configInstance = MessageBox.dialogRef._containerInstance._config;
      const messageInstance = configInstance?.data?.data?.content;
      if (message.data.content == messageInstance) {
        console.customize("blocked 1 pop-up with the same message");
        return MessageBox.dialogRef.afterClosed();
      }
    }

    MessageBox.dialogRef?.close();
    MessageBox.dialogRef = MessageBox._dialog.open(NotiBoxComponent, config);
    MessageBox.dialogRef.afterClosed().subscribe(resp => {
      MessageBox.dialogRef = null;
    });
    return MessageBox.dialogRef.afterClosed();
  }

  /**
   * Mở dialog confirm
   * Cách dùng: Ví dụ tại component A, muốn mở dialog:
   * MessageBox.openConfirmDelete( new Message(this, {title: 'your-title', content: 'your-content', 'your-callback'}) );
   */
  public static confirmDelete(message: Message) {
    const config = this.prepareConfig(message, MessageBoxType.ConfirmDelete);
    if (MessageBox.dialogRef && MessageBox.dialogRef._containerInstance) {
      const configInstance = MessageBox.dialogRef._containerInstance._config;
      const messageInstance = configInstance?.data?.data?.content;
      if (message.data.content == messageInstance) {
        console.customize("blocked 1 pop-up with the same message");
        return MessageBox.dialogRef.afterClosed();
      }
    }

    MessageBox.dialogRef?.close();
    MessageBox.dialogRef = MessageBox._dialog.open(NotiBoxComponent, config);
    MessageBox.dialogRef.afterClosed().subscribe(resp => {
      MessageBox.dialogRef = null;
    });
    return MessageBox.dialogRef.afterClosed();
  }

  /**
   * Mở dialog confirm
   * Cách dùng: Ví dụ tại component A, muốn mở dialog:
   * MessageBox.openInformation( new Message(this, {title: 'your-title', content: 'your-content', 'your-callback'}) );
   */
  public static information(message: Message) {
    const config = this.prepareConfig(message, MessageBoxType.Information);
    if (MessageBox.dialogRef && MessageBox.dialogRef._containerInstance) {
      const configInstance = MessageBox.dialogRef._containerInstance._config;
      const messageInstance = configInstance?.data?.data?.content;
      if (message.data.content == messageInstance) {
        console.customize("blocked 1 pop-up with the same message");
        return of(false);
      }
    }

    MessageBox.dialogRef?.close();
    MessageBox.dialogRef = MessageBox._dialog.open(NotiBoxComponent, config);
    MessageBox.dialogRef.afterClosed().subscribe(resp => {
      MessageBox.dialogRef = null;
    });
    return MessageBox.dialogRef.afterClosed();
  }

  /**
   * Customize
   */
  public static openCustom(component: any, message: Message) {
    const config = this.prepareConfig(message, MessageBoxType.None);
    if (MessageBox.dialogRef && MessageBox.dialogRef._containerInstance) {
      const configInstance = MessageBox.dialogRef._containerInstance._config;
      const messageInstance = configInstance?.data?.data?.content;
      if (message.data.content == messageInstance) {
        console.customize("blocked 1 pop-up with the same message");
        return MessageBox.dialogRef.afterClosed();
      }
    }

    MessageBox.dialogRef?.close();
    MessageBox.dialogRef = MessageBox._dialog.open(component, config);
    MessageBox.dialogRef.afterClosed().subscribe(resp => {
      MessageBox.dialogRef = null;
    });
    return MessageBox.dialogRef.afterClosed();
  }

  /**
   * Close dialog
   */
  public static close() {
    MessageBox.dialogRef?.close();
  }
}
