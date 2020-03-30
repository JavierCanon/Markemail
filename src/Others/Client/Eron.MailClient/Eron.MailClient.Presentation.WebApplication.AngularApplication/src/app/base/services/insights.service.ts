import { Injectable } from '@angular/core';
import { HttpClient } from './app.http.service';
import { Api } from '../api';

@Injectable()
export class InsightsService {
  constructor(
    private http: HttpClient
  ) { }

  getLastMonthSales() {
    return this.http.get(Api.insight.getLastMonthSales);
  }

  getCurrentMonthSales() {
    return this.http.get(Api.insight.getCurrentMonthSales);
  }

  getCurrentDaySales() {
    return this.http.get(Api.insight.getLastMonthSales);
  }

  getClients() {
    return this.http.get(Api.insight.getClients);
  }

  getCustomers() {
    return this.http.get(Api.insight.getCustomers);
  }

  getPagesCount() {
    return this.http.get(Api.insight.getPagesCount);
  }

  getOrdersCount() {
    return this.http.get(Api.insight.getOrdersCount);
  }

  getProductsCounts() {
    return this.http.get(Api.insight.getProductsCounts);
  }

  getTariffCounts() {
    return this.http.get(Api.insight.getTariffCounts);
  }

  getLastWeekSales() {
    return this.http.get(Api.insight.getLastWeekSales);
  }

  getCurrentWeekSales() {
    return this.http.get(Api.insight.getCurrentWeekSales);
  }

  getLastMonthSalesWithDayByDayDetails() {
    return this.http.get(Api.insight.getLastMonthSalesWithDayByDayDetails);
  }

  getCurrentMonthSalesWithDayByDayDetails() {
    return this.http.get(Api.insight.getCurrentMonthSalesWithDayByDayDetails);
  }
}
