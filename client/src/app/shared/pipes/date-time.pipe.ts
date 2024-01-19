import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'DateTimeVietnamPipe' })
export class DateTimeVietnamPipe implements PipeTransform {
  transform(date: Date): string {
    if (date == null) {
      return '';
    }

    date = new Date(date);
    let second = date.getSeconds();
    let minute = date.getMinutes();
    let hour = date.getHours();
    let day = date.getDate();
    let month = date.getMonth() + 1;

    let secondStr = second < 10 ? `0${second}` : second;
    let minuteStr = minute < 10 ? `0${minute}` : minute;
    let hourStr = hour < 10 ? `0${hour}` : hour;
    let dayStr = day < 10 ? `0${day}` : day;
    let monthStr = month < 10 ? `0${month}` : month;

    return `${hourStr}:${minuteStr}:${secondStr}  ${dayStr}-${monthStr}-${date.getFullYear()}`;
  }
}
