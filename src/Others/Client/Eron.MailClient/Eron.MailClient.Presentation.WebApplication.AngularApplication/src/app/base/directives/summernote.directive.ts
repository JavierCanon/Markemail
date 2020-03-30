import { Directive, ElementRef, Input, Output, EventEmitter } from '@angular/core';
import { NgControl } from '@angular/forms';

declare var $: any;

// tslint:disable-next-line:directive-selector
@Directive({ selector: '[summernote]' })

export class SummernoteDirective {

    @Input() config;

    element: any;
    constructor(el: ElementRef, private control: NgControl) {
        this.element = el.nativeElement;
    }

    // tslint:disable-next-line:use-life-cycle-interface
    ngOnInit() {
        const element = this.element;
        const control = this.control;
        $(this.element).summernote('code');
        $(this.element).parent().find('.note-editor').on('keyup', function(e){
          const value = $(element).code();
          control.control.setValue(value);
      });
    }

    setValue(value: any) {
      this.control.control.setValue(value);
    }
}
