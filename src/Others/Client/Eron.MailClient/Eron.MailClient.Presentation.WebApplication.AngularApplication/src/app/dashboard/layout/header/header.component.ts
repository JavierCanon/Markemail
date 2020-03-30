import { Component, OnInit, Input, Output, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { PubSubService } from 'angular2-pubsub';
import { AuthenticationService } from '../../../authentication/auth.service';
import { AfterViewInit } from '@angular/core/src/metadata/lifecycle_hooks';

declare var $: any;

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() BackgroundColor = 'white-bg';
  BackgroundColorSubscriber: any;
  constructor(
    private pubsub: PubSubService,
    private authService: AuthenticationService,
    private router: Router
  ) {

    this.BackgroundColorSubscriber = this.pubsub.$sub('darkBackgroundHeader').subscribe((from) => {
      this.BackgroundColor = '';
    });

    this.BackgroundColorSubscriber = this.pubsub.$sub('lightBackgroundHeader').subscribe((from) => {
      this.BackgroundColor = 'white-bg';
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
      this.BackgroundColorSubscriber.unsubscribe();
  }

  ngAfterViewInit() {
    const smoothlyMenu = this.SmoothlyMenu;
    $('.navbar-minimalize').click(function (event) {
      event.preventDefault();
      $('body').toggleClass('mini-navbar');
      smoothlyMenu();
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }

  onMinimizeClick() {
    const smoothlyMenu = this.SmoothlyMenu;
    $('.navbar-minimalize').click(function () {
      $('body').toggleClass('mini-navbar');
      smoothlyMenu();
    });
  }

  private SmoothlyMenu() {
    if (!$('body').hasClass('mini-navbar') || $('body').hasClass('body-small')) {
      // Hide menu in order to smoothly turn on when maximize menu
      $('#side-menu').hide();
      // For smoothly turn on menu
      setTimeout(
        function () {
          $('#side-menu').fadeIn(400);
        }, 200);
    } else if ($('body').hasClass('fixed-sidebar')) {
      $('#side-menu').hide();
      setTimeout(
        function () {
          $('#side-menu').fadeIn(400);
        }, 100);
    } else {
      // Remove all inline style from jquery fadeIn function to reset menu state
      $('#side-menu').removeAttr('style');
    }
  }
}
