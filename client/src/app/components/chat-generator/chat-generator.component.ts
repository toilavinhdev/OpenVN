import { Component, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { ChatContent } from 'src/app/models/chat/chat-content';
import { ChatResult } from 'src/app/models/chat/chat-result';
import { BaseComponent } from 'src/app/shared/components/base-component';
import { BaseUploaderComponent } from 'src/app/shared/components/micro/uploader/uploader.component';
import { ChatGeneratorService } from 'src/app/shared/services/chat/chat-generator.service';

@Component({
  selector: 'app-chat-generator',
  templateUrl: './chat-generator.component.html',
  styleUrls: ['./chat-generator.component.scss']
})
export class ChatGeneratorComponent extends BaseComponent {

  chatResult = new ChatResult();

  currentChat = new ChatContent();

  messages: ChatContent[] = [];

  step = "upload";

  turnNext = false;

  availableCount = 2;

  speed = 1;

  bgColor = "#fff";

  name = "Sue";

  avatarUrl = "https://img.freepik.com/free-photo/lifestyle-business-people-holding-laptop-computer-office-desk_1150-10180.jpg";

  avatarUrl2 = "https://img.freepik.com/free-photo/young-pretty-business-woman-with-notebook-isolated_231208-301.jpg";

  isFull = true;

  @ViewChild("uploader")
  uploader: BaseUploaderComponent;

  constructor(
    public injector: Injector,
    public chatService: ChatGeneratorService,
    public activatedRoute: ActivatedRoute,
    public chatGeneratorService: ChatGeneratorService
  ) {
    super(injector);
  }

  initData() {
    super.initData();
    const id = this.activatedRoute.snapshot.params['id'];
    if (id) {
      this.isLoading = true;
      this.chatGeneratorService.getById(id)
        .pipe(takeUntil(this._onDestroySub))
        .subscribe(
          resp => {
            this.isLoading = false;
            if (resp.status == 'success') {
              this.step = "option";
              this.chatResult = resp.data.content;
            }
          },
          _ => this.isLoading = false
        );
    }
  }

  upload(files) {
    const formData = new FormData();
    const file = files[0];
    formData.append('file', file, file.name);

    this.chatService.upload(formData)
      .pipe(takeUntil(this._onDestroySub))
      .subscribe(
        resp => {
          this.uploader.uploadBtn.isFinished = true;
          if (resp.status == 'success' && resp.data) {
            console.customize(resp.data);
            this.chatResult = resp.data;
            this.step = "option";
          }
        },
        _ => this.uploader.uploadBtn.isFinished = true
      )
  }

  hasSpeak(key) {
    return this.messages.find(m => m.key == key) != null;
  }

  isFirstMessage(key, index) {
    return this.messages.findIndex(x => x.key == key) == index;
  }

  displayTime(time: number) {
    let hour = Math.floor(time / 3600);
    let minute = Math.floor((time % 3600) / 60);
    let second = Math.floor(((time % 3600) % 60));

    return `${(hour < 10) ? '0' + hour : hour}:${(minute < 10) ? '0' + minute : minute}:${(second < 10) ? '0' + second : second}`;
  }

  format(value) {
    return `x${value}`;
  }

  present() {
    if (this.bgColor == '#fff') {
      this.bgColor = "#ada9a8";
    }
    this.step = "presentation";
    for (let i = 0; i < this.chatResult.contents.length; i++) {
      const chat = this.chatResult.contents[i];
      setTimeout(() => {
        this.turnNext = this.currentChat.key != chat.key;
        if (this.turnNext) {
          if (this.messages.length > this.availableCount) {
            this.messages.splice(0, 2);
          }
        }

        if (this.messages.length >= 2 || chat.content.length >= 64 || (this.messages.length == 1 && this.messages[0].content.length + chat.content.length >= 64)) {
          this.messages = [];
        }

        this.currentChat = chat;
        this.messages.push(chat);

        console.customize(i, this.currentChat);
      }, Math.max(100, chat.at * (1000 / this.speed) - (1000 / this.speed) + 300));
    }
  }

  changeAvatar(event, num: number) {
    const file = event.target.files[0];

    var reader = new FileReader();
    reader.onloadend = () => {
      if (num == 1) {
        this.avatarUrl = reader.result + "";
      }
      else {
        this.avatarUrl2 = reader.result + "";
      }
    }
    reader.readAsDataURL(file);
  }
}


