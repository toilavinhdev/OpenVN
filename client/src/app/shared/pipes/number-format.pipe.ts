
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'NumberFormatPipe' })
export class NumberFormatPipe implements PipeTransform {
  transform(value: string | number): string {
    const re = '\\d(?=(\\d{3})+$)';
    return parseInt(value + "").toFixed(Math.max(0, ~~0)).replace(new RegExp(re, 'g'), '$&,');
  }
}
