import { Injectable } from '@angular/core';
import { HttpClient } from './app.http.service';
import { Api } from '../api';


@Injectable()
export class FileService {
  constructor(private http: HttpClient) {
  }

  deleteFile(fileId: string) {
    this.http.delete(Api.common.fileUpload + fileId)
    .subscribe((response) => {
      console.log(response);
    });
  }
}
