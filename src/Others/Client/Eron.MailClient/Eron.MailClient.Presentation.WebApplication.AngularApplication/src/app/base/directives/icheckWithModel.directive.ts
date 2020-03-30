import { Directive, ElementRef, Input, Output, EventEmitter } from '@angular/core';
import { NgControl, NgModel } from '@angular/forms';

declare var $: any;

// tslint:disable-next-line:directive-selector
@Directive(
  {
    // tslint:disable-next-line:directive-selector
    selector: '[ngModel][icheck]',
    providers: [NgModel]
  })
export class ICheckWithModelDirective {

  @Input() config = this.config || {
    checkboxClass: 'icheckbox_square-green',
    radioClass: 'iradio_square-green',
  };

  @Output() ngModelChange = new EventEmitter<boolean>(true);

  element: any;
  constructor(el: ElementRef, public model: NgModel) {
    this.element = el.nativeElement;
    const element = this.element;
    const config = this.config;

    $(this.element).prop('checked', model.value).iCheck('update', config);

    this.model.valueChanges.subscribe(function (value) {
      /* Set any value of your custom control */
      $(element.nativeElement).prop('checked', value).iCheck('update', config);
    });

    $(this.element).on('ifChanged', function () {
      const value = $(element).prop('checked');
      model.valueAccessor.writeValue(value);
      model.viewToModelUpdate(value);
    });
  }
}
