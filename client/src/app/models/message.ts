import { MessageData } from "./message-data";

export class Message {
  public sender: any;
  public data: MessageData;
  public callback: Function;

  constructor(sender: any, data: MessageData, callback: Function = () => { }) {
    this.sender = sender;
    this.data = data;
    this.callback = callback;
  }
}
