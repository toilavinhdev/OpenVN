import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'CountDownPipe' })
export class CountDownPipe implements PipeTransform {
  transform(countdown: number): string {
    const hours = 0;
    const minutes = Math.floor(countdown / 60);
    const seconds = countdown - minutes * 60;

    return (minutes < 10 ? `0${minutes}:` : `${minutes}:`) + (seconds < 10 ? `0${seconds}` : `${seconds}`);
  }
}
