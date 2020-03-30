import { Directive, ElementRef, Input, Output, EventEmitter, HostBinding, HostListener } from '@angular/core';
import { NgModel } from '@angular/forms';

declare var $: any;

// tslint:disable-next-line:directive-selector
@Directive(
  {
    // tslint:disable-next-line:directive-selector
    selector: '[ngModel][tagsinputwithmodel]',
  })

export class TagsInputWithModelDirective {

  @Input() config = this.config;

  element: any;
  constructor(el: ElementRef, public model: NgModel) {
    this.element = el.nativeElement;
    const element = this.element;
    $(document).ready(function () {
      $(element).tagsInput();
      $(element).on('change', function () {
        const value = $(element).val();
        model.valueAccessor.writeValue(value);
        model.viewToModelUpdate(value);
      });

      $(element).on('restart', function () {
        $(element).tagsInput('destroy');
        $(element).tagsInput();
      });
    });
    // $(this.element).on('change', function (e) {
    //   const value = $(element).code();
    //   control.control.setValue(value);
    // });
  }

  @HostListener('restart')
  public onRestart() {
    $(this.element).importTags('');
  }
}
