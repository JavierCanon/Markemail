import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Api } from '../api';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import 'rxjs/add/observable/throw';
import { StorageService } from './storage.service';
@Injectable()
export class HttpClient {

  constructor(
    private http: Http,
    private router: Router,
    public storageService: StorageService
  ) { }

  //#region helper

  private createAuthorizationHeader(headers: Headers) {
    const accessToken = this.storageService.getItem('accessToken');
    if (accessToken != null) {
      headers.append('Authorization', 'Bearer ' + accessToken);
    }

    if (headers.get('Access-Control-Allow-Origin') == null) {
      headers.append('Access-Control-Allow-Origin', Api.angularApplicationAddress);
    }
  }

  //#endregion

  //#region raw requests


  getRaw(url, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.get(url, {
      headers: headers
    });
  }

  getWithRequestDataRaw(url, data, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.get(url, {
      body: data,
      headers: headers
    }).catch(
      (res) => this.catchAuthError(res)
    );
  }

  postRaw(url, data, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.post(url, data, {
      headers: headers
    }).catch(
      (res) => this.catchAuthError(res)
    );
  }

  putRaw(url, data, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.put(url, data, {
      headers: headers
    }).catch(
      (res) => this.catchAuthError(res)
    );
  }

  deleteRaw(url, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.delete(url, {
      headers: headers
    }).catch(
      (res) => this.catchAuthError(res)
    );
  }

  //#endregion

  //#region json requests

  get(url, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.get(url, {
      headers: headers
    }).map((value) => value.json()).catch(
      (res) => this.catchAuthError(res)
    );
  }

  getWithRequestData(url, data, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.get(url, {
      params: data,
      headers: headers
    }).map((value) => value.json()).catch(
      (res) => this.catchAuthError(res)
    );
  }

  post(url, data, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.post(url, data, {
      headers: headers
    }).map((value) => value.json()).catch(
      (res) => this.catchAuthError(res)
    );
  }

  put(url, data, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.put(url, data, {
      headers: headers
    }).map((value) => value.json()).catch(
      (res) => this.catchAuthError(res)
    );
  }

  delete(url, headers?: Headers) {
    headers = headers || new Headers();
    this.createAuthorizationHeader(headers);
    return this.http.delete(url, {
      headers: headers
    }).map((value) => value.json()).catch(
      (res) => this.catchAuthError(res)
    );
  }
  //#endregion

  //#region Helpers

  catchAuthError(res) {
    // we have to pass HttpService's own instance here as `self`return (res: Response) => {
    if (res.status === 401 || res.status === 403) {
      // if not authenticated
      // this.storageService.removeItem(appVariables.userLocalStorage);
      this.storageService.removeItem('accessToken');
      // this.router.navigate(['/login']);
    }
    return Observable.throw(res);
  }

  //#endregion
}
