import { Component, OnInit, OnDestroy, Input, Output } from '@angular/core';
import { PubSubService } from 'angular2-pubsub';
import { BreadCrump } from '../../../base/models/breadCrump.model';
@Component({
  selector: 'app-bread-crump',
  templateUrl: './bread-crump.component.html',
  styleUrls: ['./bread-crump.component.scss']
})
export class BreadCrumpComponent implements OnInit, OnDestroy {
  @Input() Title: string;
  @Input() FirstNode: string;
  @Input() FirstNodeUrl: string;
  @Input() SecondNode: string;
  @Input() SecondNodeUrl: string;
  @Input() ThirdNode: string;
  @Input() ThirdNodeUrl: string;
  @Input() Button: string;
  @Input() ButtonUrl: string;
  @Input() DarkBackground: boolean;
  @Input() BreadCrump: BreadCrump;

  constructor(private pubsub: PubSubService) { }

  ngOnInit() {
    if (this.DarkBackground || this.BreadCrump.DarkBackground) {
      this.pubsub.$pub('darkBackgroundHeader');
    }else {
      this.pubsub.$pub('lightBackgroundHeader');
    }
  }

  ngOnDestroy() {
      this.pubsub.$pub('lightBackgroundHeader');
  }

}
