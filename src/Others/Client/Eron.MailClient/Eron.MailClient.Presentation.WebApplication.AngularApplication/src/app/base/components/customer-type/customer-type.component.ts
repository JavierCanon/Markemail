import { Component, OnInit, Input } from '@angular/core';
import { ViewCell } from 'ng2-smart-table';

@Component({
  selector: 'app-customer-type',
  templateUrl: './customer-type.component.html',
  styleUrls: ['./customer-type.component.scss']
})
export class CustomerTypeComponent implements OnInit, ViewCell {
  valueData: any;
  labelStatus: any;
  @Input() value: string | number;
  @Input() rowData: any;

  ngOnInit() {
    switch (this.value) {
      case 0:
        this.labelStatus = 'primary';
        this.valueData = 'کاربر نهایی';
        break;
      case 1:
        this.labelStatus = 'warning';
        this.valueData = 'همکار';
        break;
      default:
        this.labelStatus = 'primary';
        this.valueData = 'کاربر نهایی';
    }
  }

}
