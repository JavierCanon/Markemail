import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
import { InvoiceType } from '../types/Invoice.type';
@Pipe({
  name: 'invoicetype'
})
export class InvoiceTypePipe implements PipeTransform {
  transform(value: InvoiceType, args?: any): any {
    switch (value) {
      case InvoiceType.Cart:
      return 'محصول';
      case InvoiceType.Order:
      return 'سفارش';
      default:
      return 'نامعلوم';
    }
  }
}
