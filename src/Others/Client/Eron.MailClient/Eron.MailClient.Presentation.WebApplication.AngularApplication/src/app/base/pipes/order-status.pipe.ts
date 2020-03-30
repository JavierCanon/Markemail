import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
import { OrderStatusType } from '../../website/financial/order/order-status.type';
@Pipe({
  name: 'orderstatus'
})
export class OrderStatusPipe implements PipeTransform {
  transform(value: OrderStatusType, args?: any): any {
    switch (value) {
      case OrderStatusType.Received:
      return 'در حال بررسی';
      case OrderStatusType.WaitingForPayment:
      return 'در انتظار پرداخت';
      case OrderStatusType.Processing:
      return 'در حال انجام';
      case OrderStatusType.Posting:
      return 'در حال ارسال';
      case OrderStatusType.Posted:
      return 'ارسال شده';
      case OrderStatusType.Delivered:
      return 'تحویل داده شده';
      case OrderStatusType.Canceled:
      return 'لغو شده';
      case OrderStatusType.CanceledByAdmin:
      return 'مردود شده';
      case OrderStatusType.NeedInvoice:
      return 'نیاز به صدور فاکتور';
      default:
      return 'نامعلوم';
    }
  }
}
