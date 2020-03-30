import { ToastOptions } from 'ng2-toastr';
import { CustomToastOption } from './CustomToastOption';

export class ToastProvider {
  public provide = ToastOptions;
  public useClass = CustomToastOption;

  forRoot() {
    return ;
  }
}
