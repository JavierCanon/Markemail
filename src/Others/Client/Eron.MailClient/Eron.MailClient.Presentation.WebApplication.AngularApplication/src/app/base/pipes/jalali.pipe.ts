import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'jalali-moment';
@Pipe({
  name: 'jalali'
})
export class JalaliPipe implements PipeTransform {
  transform(value: any, format?: string): any {
    const momentDate = moment(value);
    if (format != null) {
      return momentDate.format(format);
    }

    return momentDate.format('jYYYY/jM/jD');
  }
}
