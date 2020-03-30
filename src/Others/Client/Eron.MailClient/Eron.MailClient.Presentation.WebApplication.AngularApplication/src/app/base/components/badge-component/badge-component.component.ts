import { Component, OnInit, Input } from '@angular/core';
import { Common } from '../../common';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'badge',
  templateUrl: './badge-component.component.html',
  styleUrls: ['./badge-component.component.scss']
})
export class BadgeComponent implements OnInit {

  constructor() { }

  @Input() value: any;
  @Input() class: any;
  @Input() labelStatus: string;

  ngOnInit() {
    if (this.labelStatus === 'random') {
      const random = Common.random(0, 5);
      switch (random) {
        case 0:
          this.labelStatus = 'default';
          break;
        case 1:
          this.labelStatus = 'primary';
          break;
        case 2:
          this.labelStatus = 'warning';
          break;
        case 3:
          this.labelStatus = 'success';
          break;
        case 4:
          this.labelStatus = 'danger';
          break;
        case 5:
          this.labelStatus = 'info';
          break;
      }
    }
  }

}
