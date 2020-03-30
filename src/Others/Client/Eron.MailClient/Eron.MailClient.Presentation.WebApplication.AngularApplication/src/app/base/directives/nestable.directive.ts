import { Directive, EventEmitter, ElementRef, Output } from '@angular/core';

declare var $: any;

// tslint:disable-next-line:directive-selector
@Directive({ selector: '[nestable]' })
export class NestableDirective {

  element: any;
  @Output() change = new EventEmitter<any>(true);

  constructor(el: ElementRef) {
    this.element = el.nativeElement;
    const change = this.change;
    const emitChange = function(value){
      change.emit(value);
    };
    $(this.element).nestable({ group: 1 });
    $(this.element).on('change', function (e) {
      const value = $(this).nestable('serialize');
      emitChange(value);
    });
  }
}
