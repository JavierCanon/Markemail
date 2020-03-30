import {ToastOptions} from 'ng2-toastr';

export class CustomToastOption extends ToastOptions {
  // animate = 'flyRight'; // you can override any options available
  newestOnTop = false;
  showCloseButton = true;
  positionClass = 'toast-bottom-left';
  progressBar = true;
  // dismiss = 'click';
}
