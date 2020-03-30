import { Component, OnInit, AfterViewInit } from '@angular/core';
import { MenuService } from '../../infrastructure/services/menu.service';
import { Menu } from '../../infrastructure/models/menu.model';
import { UserService } from '../../base/user/user.service';

declare var $: any;

@Component({
  selector: 'app-side',
  templateUrl: './side.component.html',
  styleUrls: ['./side.component.scss'],
  providers: [MenuService]
})
export class SideComponent implements OnInit, AfterViewInit {
  MenuList: Array<Menu>;
  userInfo: any;
  userName: string;
  constructor(
    private MenuService: MenuService,
    private userService: UserService) {
    this.MenuList = MenuService.getMenues();
    this.userService.getUserInfo().subscribe(
      (response) => {
        this.userInfo = response;
        this.userName = this.userInfo.firstName != null && this.userInfo.lastName != null ?
          this.userInfo.firstName + ' ' + this.userInfo.lastName :
          this.userInfo.email;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    $('#side-menu').metisMenu();
  }

}
