import { Config } from '../app.config';

const base = Config.baseApiAddress + 'api/';
const websiteHost = Config.websiteHost;

export class Api {

  static auth = {
    isInRole: base + 'Account/IsInRole/',
    token: base + 'token',
    register: base + 'Account/register/',
    changePassword: base + 'Account/ChangePassword/',
    forgetPassword: base + 'Account/ForgotPassword/',
    resetPassword: base + 'Account/ResetPassword/'
  };

  static common = {
    selectListService: base + 'selectlist/get/',
    fileUpload: base + 'filemanagement/',
    image: base + 'image/'
  };

  static contact = {
    default: base + '/contact'
  };

  static tariff = {
    default: base + 'tariff/',
    category: base + 'tariff/category/',
  };

  static product = {
    default: base + 'products/',
    category: base + 'products/category/',
    productCode: base + 'products/productcode/',
    productCodeForUpdate: base + 'products/productcode/update/',
    promotion: base + 'products/promotion/',
    related: base + 'products/related/',
    bestSeller: base + 'products/bestseller/',
    getAllAsPagedList: base + 'products/aspagedlist/'
  };

  static productCategory = {
    default: base + 'productcategories/',
    reOrder: base + 'productcategories/reorder/',
    getFlat: base + 'productcategories/flat/',
    getFull: base + 'productcategories/full/',
    getHome: base + 'productcategories/homePage/'
  };

  static tariffCategory = {
    default: base + 'tariffcategories/',
    reOrder: base + 'tariffcategories/reorder/',
    getFlat: base + 'tariffcategories/flat/',
    getFull: base + 'tariffcategories/full/',
  };

  static page = {
    default: base + 'page/',
    slug: base + 'page/slug/',
  };

  static user = {
    default: base + 'user/',
    checkUserInfoIsComplete: base + 'user/checkUserInfo/',
    userInfo: base + 'user/UserInfo/',
    getRoles: base + 'user/GetRoles/',
    fullDetails: base + 'user/fullDetails/',
    userRoles: base + 'user/userRoles/',
    updateAsAdmin: base + 'user/asAdmin/'
  };

  static link = {
    default: base + 'link/',
    reOrder: base + 'link/reorder/',
    tree: base + 'link/tree/'
  };

  static slider = {
    default: base + 'bannerslider/'
  };

  static cart = {
    default: base + 'cart/'
  };

  static invoice = {
    default: base + 'invoice/',
    postByCart: base + 'invoice/ByCart/',
    postByOrder: base + 'invoice/ByOrders/',
    getByNumber: base + 'invoice/number/',
    getAllInvoices: base + 'invoice/AllInvoices/',
    postPayment: base + 'invoice/Payment/SendToBank/',
    getPayment: base + 'invoice/Payment/CallBack/',
    getProductInvoices: base + 'invoice/AllProductInvoices',
    getByNumberAsAdmin: base + 'invoice/management/number/',
    updateStateOfInvoiceList: base + 'invoice/changeStateOfInvoiceList/',
    createReference: base + 'invoice/CreateReferenceNumber/',
    getTransactionInfo: base + 'invoice/GetTransactionDetails/'
  };

  static order = {
    default: base + 'order/',
    addDesignPrice: base + 'order/designprice',
    userAll: base + 'order/user/all/',
    pagedList: base + 'order/getaspagedlist/',
    changeStateOfOrderList: base + 'order/changeStateOfOrderList/',
    assignDesignPrice: base + 'order/assigndesignprice/',
    getByNumber: base + 'order/byNumber/'
  };

  static insight = {
    getLastMonthSales: base + 'insight/GetLastMonthSales/',
    getCurrentMonthSales: base + 'insight/GetCurrentMonthSales/',
    getCurrentDaySales: base + 'insight/GetCurrentDaySales/',
    getClients: base + 'insight/GetClients/',
    getCustomers: base + 'insight/GetCustomers/',
    getPagesCount: base + 'insight/GetPagesCount/',
    getOrdersCount: base + 'insight/GetOrdersCount/',
    getProductsCounts: base + 'insight/GetProductsCounts/',
    getTariffCounts: base + 'insight/GetTariffCounts/',
    getLastWeekSales: base + 'insight/GetLastWeekSales/',
    getCurrentWeekSales: base + 'insight/GetCurrentWeekSales/',
    getLastMonthSalesWithDayByDayDetails: base + 'insight/GetLastMonthSalesWithDayByDayDetails/',
    getCurrentMonthSalesWithDayByDayDetails: base + 'insight/GetCurrentMonthSalesWithDayByDayDetails/'
  };

  static wishList = {
    default: base + 'wishlist/'
  };

  static search = {
    default: base + 'search/'
  };

  static angularApplicationAddress = websiteHost;
  static webApiApplicationAddress = base;
}
