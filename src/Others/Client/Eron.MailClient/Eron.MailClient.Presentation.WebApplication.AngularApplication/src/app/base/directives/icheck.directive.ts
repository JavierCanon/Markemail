import { Directive, ElementRef, Input, Output, EventEmitter } from '@angular/core';
import { NgControl } from '@angular/forms';

declare var $: any;

// tslint:disable-next-line:directive-selector
@Directive({ selector: '[icheck]' })
export class ICheckDirective {

  @Input() config = this.config || {
    checkboxClass: 'icheckbox_square-green',
    radioClass: 'iradio_square-green',
  };

  element: any;
  constructor(el: ElementRef) {
    this.element = el.nativeElement;
    const element = this.element;
    $(this.element).iCheck(this.config);
  }
}
