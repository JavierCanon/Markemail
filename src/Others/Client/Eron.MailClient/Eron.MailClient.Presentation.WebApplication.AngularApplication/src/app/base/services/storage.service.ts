import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class StorageService {
  private storageSub= new Subject<boolean>();

  watchStorage(): Observable<any> {
    return this.storageSub.asObservable();
  }

  setItem(key: string, data: any) {
    localStorage.setItem(key, data);
    this.storageSub.next();
  }

  getItem(key: string) {
    return localStorage.getItem(key);
  }

  removeItem(key) {
    localStorage.removeItem(key);
    this.storageSub.next();
  }

  removeItemWithoutSubscription(key) {
    localStorage.removeItem(key);
  }
}
