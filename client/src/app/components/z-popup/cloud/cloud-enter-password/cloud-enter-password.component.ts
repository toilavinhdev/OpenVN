import { AfterViewInit, Component, INJECTOR, Inject, Injector, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DxTextBoxComponent } from 'devextreme-angular';
import { Directory } from 'src/app/models/cloud/directory';
import { Tracking } from 'src/app/models/core/tracking';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { BaseButton } from 'src/app/shared/components/micro/button/button.component';
import { AuthCloudService } from 'src/app/shared/services/cloud/auth-cloud.service';
import { Utility } from 'src/app/shared/utility/utility';

@Component({
  selector: 'app-cloud-enter-password',
  templateUrl: './cloud-enter-password.component.html',
  styleUrls: ['./cloud-enter-password.component.scss']
})
export class CloudEnterPasswordComponent extends BaseComponent implements AfterViewInit {

  Utility = Utility;

  directory: Directory;

  password = "";

  className = "show";

  mode = "password";

  @ViewChild("nextBtn")
  nextBtn: BaseButton;

  @ViewChild("textBox")
  textBox: DxTextBoxComponent;

  constructor(
    public injector: Injector,
    public authCloudService: AuthCloudService,
    public dialogRef: MatDialogRef<CloudEnterPasswordComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    super(injector);
    this.directory = data.otherDir ?? data.dir;
  }
  ngAfterViewInit(): void {
    this.textBox.instance.focus();
  }

  tracking(event?: Tracking, callback?: Function): void {
  }

  validate() {
    if (this.password == '') {
      this.nextBtn.isFinished = true;
      return;
    }
    this.data.callback(this);
  }

  execute(event) {
    this.nextBtn.clickExecute(event);
  }

  toggle() {
    if (this.className == 'show') {
      this.className = 'hide';
      this.mode = 'text';
    } else {
      this.className = 'show';
      this.mode = 'password';
    }
  }

  moveToReset(event) {
    event.preventDefault();
    Utility.featureIsInDevelopment(event);
  }
}
