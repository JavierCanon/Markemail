import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
@Pipe({
  name: 'priceRial'
})
export class PriceRialPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    const price = Common.commafy(value) + ' ريال';
    return price;
  }
}
