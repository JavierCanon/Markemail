import { Injectable } from '@angular/core';
import { ToastsManager } from 'ng2-toastr';
import { SweetAlertService } from 'angular-sweetalert-service/js';
import { HttpClient } from './app.http.service';
import { PubSubService } from 'angular2-pubsub';

declare var swal: any;

@Injectable()
export class NotificationService {

  constructor(
    public toast: ToastsManager,
    private sweetAlert: SweetAlertService,
    private http: HttpClient,
    public pubService: PubSubService
  ) { }

  info(message: string, title: string) {
    this.toast.info(message, title);
  }

  warning(message: string, title: string) {
    this.toast.warning(message, title);
  }

  success(message: string, title: string) {
    this.toast.success(message, title);
  }

  error(message: string, title: string) {
    this.toast.error(message, title);
  }

  serverError(error?: any) {
    if (error != null) {
      console.log(error);
      if (error.status === '409') {
        this.error('درخواست شما قبلا در سرور ثبت شده و با اطلاعات سرور مقایرت دارد', '');
        return;
      }

      const errorDetails = error.json();
      for (const property in errorDetails.modelState) {
        if (errorDetails.modelState.hasOwnProperty(property)) {
          errorDetails.modelState[property].forEach(element => {
            this.error(element, 'درخواست شما دچار مشکل شده است');
          });
        }
      }
      if (errorDetails.error != null && errorDetails.hasOwnProperty('error')) {
        this.toast.error(errorDetails.error_description, errorDetails.error);
      }
    } else {
      this.toast.error('مشکلی در ارتباط با سرور پیش آمده، لطفا پس از بررسی مجدد دوباره تلاش نمایید', 'مشکل ارتباط با سرور');
    }
  }

  successfulOperation(operationName: string) {
    this.toast.success('عملیات ' + operationName + ' با موفقیت انجام شد', 'عملیات موفقیت آمیز');
  }

  deleteConfirmationAlert(entityDescription: string, itemId?: any, urlAddress?: string, subscriptionName?: string) {
    subscriptionName = subscriptionName || 'DeleteSuccess';
    const deleteConfirmationSettings = {
      title: 'آیا از حذف ' + entityDescription + ' اطمینان دارید؟',
      text: 'در صورت حذف این رکورد تمامی اطلاعات مرتبط حذف خواهد شد.',
      // it works
      // onOpen: () => {
      //   swal.showLoading();
      // },
      showCancelButton: true,
      cancelButtonText: 'انصراف',
      confirmButtonText: 'بله! حذف شود'
    };
    this.sweetAlert.confirm(deleteConfirmationSettings)
      .then(() => {
        this.http.delete(urlAddress + itemId).subscribe(
          (response) => {
            this.pubService.$pub(subscriptionName, itemId);
            this.sweetAlert.success({
              title: entityDescription + ' حذف شد',
              confirmButtonText: 'باشه'
            });
          },
          (error) => {
            console.log(error);
          }
        );
      })
      .catch(() => {
        this.sweetAlert.error({
          title: 'حذف ' + entityDescription + ' انجام نشد'
        });
      });
  }

  successfulOperationWithAlert(title: string, content) {
    const duplicationAlertSettings = {
      title: title,
      text: content,
      // it works
      // onOpen: () => {
      //   swal.showLoading();
      // },
      // showCancelButton: true,
      // toast: true,
      confirmButtonText: 'بستن'
    };
    this.sweetAlert.success(duplicationAlertSettings);
  }

  warningWithAlert(title: string, content) {
    const duplicationAlertSettings = {
      title: title,
      text: content,
      // it works
      // onOpen: () => {
      //   swal.showLoading();
      // },
      // showCancelButton: true,
      // toast: true,
      confirmButtonText: 'بستن'
    };
    this.sweetAlert.warning(duplicationAlertSettings);
  }

  unAuthenticatedAccessWarning(description: string) {
    // this.sweetAlert.warning(
    //   {

    //   }
    // );
  }

  duplicationRestriction(duplicationTitle: string, duplicationDescription: string) {
    const duplicationAlertSettings = {
      title: duplicationTitle,
      text: duplicationDescription,
      // it works
      // onOpen: () => {
      //   swal.showLoading();
      // },
      // showCancelButton: true,
      // toast: true,
      confirmButtonText: 'بستن'
    };
    this.sweetAlert.error(duplicationAlertSettings);
  }
}
