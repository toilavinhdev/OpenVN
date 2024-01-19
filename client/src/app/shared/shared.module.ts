import { FirstCheckerComponent } from "./components/first-checker.component";
import { CommonHeaderComponent } from "./components/element/common-header/common-header.component";
import { ResolverMarkComponent } from "./components/element/resolver-mark/resolver-mark.component";
import { NotificationComponent } from "./components/micro/notification/notification.component";
import { LauncherComponent } from "./components/micro/launcher/launcher.component";
import { OnlineUserComponent } from "./components/micro/online-user/online-user.component";
import { MessengerComponent } from "./components/micro/messenger/messenger.component";
import { TypingComponent } from "./components/micro/typing/typing.component";
import { DateVietnamPipe } from "./pipes/date.pipe";
import { NgModule } from "@angular/core";
import { DateTimeVietnamPipe } from "./pipes/date-time.pipe";
import { TimePipe } from "./pipes/time.pipe";
import { CountDownPipe } from "./pipes/count-down.pipe";
import { NumberFormatPipe } from "./pipes/number-format.pipe";
import { BaseLoadingModule } from "./components/micro/loading/loading.module";
import { BaseHeaderModule } from "./components/element/common-header-v1/header-v1.module";
import { BaseButtonModule } from "./components/micro/button/button.module";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatInputModule } from "@angular/material/input";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatMenuModule } from "@angular/material/menu";
import { DxFileUploaderModule, DxProgressBarModule } from "devextreme-angular";
import { TranslateModule } from "@ngx-translate/core";
import { UploadAvatarModule } from "./components/element/upload-avatar/upload-avatar.module";
import { CommonModule } from "@angular/common";
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { UserAccountBoxComponent } from './components/micro/user-account-box/user-account-box.component'

@NgModule({
  declarations: [
    DateVietnamPipe,
    DateTimeVietnamPipe,
    TimePipe,
    CountDownPipe,
    NumberFormatPipe,
    FirstCheckerComponent,
    CommonHeaderComponent,
    ResolverMarkComponent,
    NotificationComponent,
    LauncherComponent,
    OnlineUserComponent,
    MessengerComponent,
    TypingComponent,
    UserAccountBoxComponent,
  ],
  imports: [
    CommonModule,
    BaseLoadingModule,
    BaseHeaderModule,
    BaseButtonModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatInputModule,
    MatFormFieldModule,
    MatMenuModule,
    DxFileUploaderModule,
    DxProgressBarModule,
    TranslateModule,
    UploadAvatarModule
  ],
  exports: [
    BaseLoadingModule,
    BaseHeaderModule,
    DateVietnamPipe,
    DateTimeVietnamPipe,
    TimePipe,
    CountDownPipe,
    NumberFormatPipe,
    CommonHeaderComponent,
    ResolverMarkComponent,
    NotificationComponent,
    MatProgressBarModule,
    MatTooltipModule,
    MatInputModule,
    MatFormFieldModule,
    MatMenuModule,
    DxFileUploaderModule,
    DxProgressBarModule,
    UploadAvatarModule,
    LauncherComponent,
    OnlineUserComponent,
    MessengerComponent,
    TypingComponent,
    UserAccountBoxComponent
  ]
})
export class SharedModule { }
