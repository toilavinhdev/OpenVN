import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'DateVietnamPipe' })
export class DateVietnamPipe implements PipeTransform {
  transform(date: Date): string {
    if (date == null) {
      return '';
    }

    date = new Date(date);
    let day = date.getDate();
    let month = date.getMonth() + 1;
    let dayStr = day < 10 ? `0${day}` : day;
    let monthStr = month < 10 ? `0${month}` : month;

    return `${dayStr}-${monthStr}-${date.getFullYear()}`;
  }
}
