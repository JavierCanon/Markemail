import { HttpClient } from '../../../base/services/app.http.service';
import { Api } from '../../../base/api';
import { Injectable } from '@angular/core';
// tslint:disable-next-line:import-blacklist
import * as rxjs from 'rxjs';
@Injectable()
export class SelectListService {
  http: HttpClient;
  constructor( http: HttpClient) {
    this.http = http;
  }

  getSelectList(name: string) {
    const url = Api.common.selectListService + name;
    const result = this.http.get(url);
    return result;
  }
}
