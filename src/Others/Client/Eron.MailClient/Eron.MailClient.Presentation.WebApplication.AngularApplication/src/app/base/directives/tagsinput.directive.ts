import { Directive, ElementRef, Input, Output, EventEmitter } from '@angular/core';
import { NgControl } from '@angular/forms';

declare var $: any;

// tslint:disable-next-line:directive-selector
@Directive({ selector: '[tagsinput]' })

export class TagsInputDirective {

  @Input() config = this.config;

  element: any;
  constructor(el: ElementRef, private control?: NgControl) {
    this.element = el.nativeElement;
    const element = this.element;
    $(document).ready(function(){
      $(element).tagsInput();
      $(element).on('change', function(){
        const value = $(element).val();
        control.control.setValue(value);
      });
    });
    // $(this.element).on('change', function (e) {
    //   const value = $(element).code();
    //   control.control.setValue(value);
    // });
  }
}
