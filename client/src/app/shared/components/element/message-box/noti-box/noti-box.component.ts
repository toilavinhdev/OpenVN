import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { MessageBoxType } from 'src/app/shared/enumerations/common.enum';
import { Message } from 'src/app/models/message';

@Component({
  selector: 'noti-box',
  templateUrl: './noti-box.component.html',
  styleUrls: ['./noti-box.component.scss']
})
export class NotiBoxComponent implements OnInit, OnDestroy {

  public _onDestroySub: Subject<void> = new Subject<void>();

  MessageBoxType = MessageBoxType;

  constructor(
    public dialogRef: MatDialogRef<NotiBoxComponent>,
    @Inject(MAT_DIALOG_DATA) public message: Message,
  ) { }

  ngOnInit(): void {
  }

  /**
   * Xác nhận
   */
  confirm(e: any) {
    this.message.callback(e);
  }

  /**
   * Xác nhận xóa
   */
  confirmDelete(e: any) {
    this.message.callback(e);
  }

  /**
   * Đồng ý
   */
  agree(e: any) {
    this.message.callback(e);
  }

  ngOnDestroy(): void {
    // unsubscribe khi destroy
    if (this._onDestroySub) {
      this._onDestroySub.next();
      this._onDestroySub.complete();
      this._onDestroySub.unsubscribe();
    }
  }
}
