import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
@Pipe({
  name: 'priceToman'
})
export class PriceTomanPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    value = (value / 10).toFixed();
    let price = '';
    if (args != null && args[0] === true) {
       price = Common.commafy(value);
    } else {
      price = Common.commafy(value) + ' تومان';
    }
    return price;
  }
}
