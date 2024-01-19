import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'TimePipe' })
export class TimePipe implements PipeTransform {
  transform(date: Date): string {
    if (date == null) {
      return '';
    }

    date = new Date(date);
    let second = date.getSeconds();
    let minute = date.getMinutes();
    let hour = date.getHours();

    let secondStr = second < 10 ? `0${second}` : second;
    let minuteStr = minute < 10 ? `0${minute}` : minute;
    let hourStr = hour < 10 ? `0${hour}` : hour;

    return `${hourStr}:${minuteStr}:${secondStr}`;
  }
}
