import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
import { InvoiceStatusType } from '../types/InvoiceStatus.type';
@Pipe({
  name: 'invoicestatus'
})
export class InvoiceStatusPipe implements PipeTransform {
  transform(value: InvoiceStatusType, args?: any): any {
    switch (value) {
      case InvoiceStatusType.Received:
      return 'در حال بررسی';
      case InvoiceStatusType.WaitingForPayment:
      return 'در انتظار پرداخت';
      case InvoiceStatusType.Processing:
      return 'در حال انجام';
      case InvoiceStatusType.Posting:
      return 'در حال ارسال';
      case InvoiceStatusType.Posted:
      return 'ارسال شده';
      case InvoiceStatusType.Delivered:
      return 'تحویل داده شده';
      case InvoiceStatusType.Canceled:
      return 'لغو شده';
      case InvoiceStatusType.CanceledByAdmin:
      return 'مردود شده';
      case InvoiceStatusType.NeedInvoice:
      return 'نیاز به صدور فاکتور';
      default:
      return 'نامعلوم';
    }
  }
}
