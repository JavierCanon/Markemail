import { Menu } from '../models/menu.model';

export class MenuService {

  MenuList: Menu[] =
  [
    new Menu({
      Name: 'داشبورد',
      Path: '/controlpanel/dashboard',
      Icon: 'fa fa-th-large'
    }),
    new Menu({
      Name: 'سفارشات',
      Path: '/controlpanel/orders',
      Icon: 'fa fa-credit-card'
    }),
    new Menu({
      Name: 'فروشگاه',
      Path: '/controlpanel/shop',
      Icon: 'fa fa-shopping-cart'
    }),
    new Menu({
      Name: 'محصولات',
      Icon: 'fa fa-coffee',
      Children: [
        {
          Name: 'مدیریت محصولات',
          Path: '/controlpanel/products',
        },
        {
          Name: 'مدیریت مجموعه محصولات',
          Path: '/controlpanel/productcategories',
        }
      ]
    }),
    new Menu({
      Name: 'تعرفه',
      Icon: 'fa fa-archive',
      Children: [
        {
          Name: 'مدیریت تعرفه ها',
          Path: '/controlpanel/tariff',
        },
        {
          Name: 'مدیریت مجموعه تعرفه ها',
          Path: '/controlpanel/tariffcategories',
        }
      ]
    }),
    // new Menu({
    //   Name: 'مدیریت حساب ها',
    //   Path: '/controlpanel/banks',
    //   Icon: 'fa fa-bank'
    // }),
    new Menu({
      Name: 'کاربران',
      Path: '/controlpanel/users',
      Icon: 'fa fa-users'
    }),
    new Menu({
      Name: 'اسلاید ها و بنر ها',
      Path: '/controlpanel/sliders',
      Icon: 'fa fa-desktop'
    }),
    new Menu({
      Name: 'صفحات وبسایت',
      Path: '/controlpanel/pages',
      Icon: 'fa fa-file-text'
    }),
    new Menu({
      Name: 'مدیریت پیوند ها',
      Path: '/controlpanel/links',
      Icon: 'fa fa-link'
    }),
    // new Menu({
    //   Name: 'آمار و ارقام',
    //   Path: '/controlpanel/insight',
    //   Icon: 'fa fa-line-chart'
    // }),
    new Menu({
      Name: 'پشتیبانی',
      Path: '/controlpanel/support',
      Icon: 'fa fa-life-ring'
    })
  ];

  getMenues() {
    return this.MenuList.slice();
  }
}
