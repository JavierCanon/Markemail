import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
import { OrderStatusType } from '../../website/financial/order/order-status.type';
@Pipe({
  name: 'orderstatuscolor'
})
export class OrderStatusColorPipe implements PipeTransform {
  transform(value: OrderStatusType, args?: any): any {
    switch (value) {
      case OrderStatusType.Received:
      return 'warning';
      case OrderStatusType.CanceledByAdmin:
      return 'danger';
      case OrderStatusType.Delivered:
      return 'primary';
      case OrderStatusType.Canceled:
      return 'default';
      case OrderStatusType.WaitingForPayment:
      return 'primary';
      case OrderStatusType.Processing:
      return 'sucess';
      case OrderStatusType.Posting:
      return 'primary';
      case OrderStatusType.Posted:
      return 'primary';
      case OrderStatusType.NeedInvoice:
      return 'success';
      default:
      return 'default';
    }
  }
}
