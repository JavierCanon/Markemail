import { Component, OnInit, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { NgModel, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: '[ngModel]checkbox',
  templateUrl: './checkbox.component.html',
  styleUrls: ['./checkbox.component.scss'],
  providers: [
    NgModel
  ]
})
export class CheckboxComponent implements OnInit {

  @Input() status: string;
  @Input() checked: boolean;
  @Input() id: string;
  @Input() label: string;
  @Input() name: string;
  value: boolean = this.checked;
  @Output() ngModelChange = new EventEmitter<boolean>(true);

  constructor(private model: NgModel) {
    if (model.value as boolean === true) {
      this.checked = true;
    }
  }

  ngOnInit() {
  }

  changed(event) {
      this.model.valueAccessor.writeValue(event.target.checked);
  }

}
