import { EventEmitter, Injectable } from '@angular/core';
import { SnackBarParameter } from 'src/app/models/snackbar.param';
import { SocketMessage } from 'src/app/models/socket/socket-message';

@Injectable({
  providedIn: 'root'
})
export class TransferDataService {

  listenSignedInEvent = new EventEmitter<any>();
  changeScreenEvent = new EventEmitter<any>();
  reloadListNotesEvent = new EventEmitter<any>();
  resolvedEvent = new EventEmitter<any>();
  changeNotebookTypeEvent = new EventEmitter<any>();
  appClickEvent = new EventEmitter<any>();
  updateAvatarEvent = new EventEmitter<any>();
  receivedNotificationEvent = new EventEmitter<SocketMessage>();
  receivedFileEvent = new EventEmitter<SocketMessage>();
  receivedNewFeedbackEvent = new EventEmitter<SocketMessage>();
  receivedSomeOnTypeingEvent = new EventEmitter<SocketMessage>();
  publishNotificationEvent = new EventEmitter<{param: SnackBarParameter, type: string}>();

  constructor() { }
}
