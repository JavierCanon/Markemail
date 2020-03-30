import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
import { InvoiceStatusType } from '../types/InvoiceStatus.type';
@Pipe({
  name: 'invoicestatuscolor'
})
export class InvoiceStatusColorPipe implements PipeTransform {
  transform(value: InvoiceStatusType, args?: any): any {
    switch (value) {
      case InvoiceStatusType.Received:
      return 'warning';
      case InvoiceStatusType.CanceledByAdmin:
      return 'danger';
      case InvoiceStatusType.Delivered:
      return 'primary';
      case InvoiceStatusType.Canceled:
      return 'default';
      case InvoiceStatusType.WaitingForPayment:
      return 'primary';
      case InvoiceStatusType.Processing:
      return 'sucess';
      case InvoiceStatusType.Posting:
      return 'primary';
      case InvoiceStatusType.Posted:
      return 'primary';
      case InvoiceStatusType.NeedInvoice:
      return 'success';
      default:
      return 'default';
    }
  }
}
