import { DropzoneConfigInterface, DROPZONE_CONFIG } from 'ngx-dropzone-wrapper';
import { Api } from '../../../base/api';

export class DropZoneConfiguration {

  public DefaultConfig: DropzoneConfigInterface = {
    // Change this to your upload POST address:
    url: Api.common.fileUpload,
    maxFilesize: 50,
    acceptedFiles: 'image/*',
    addRemoveLinks: true,
    dictDefaultMessage: 'برای بارگزاری فایل های خود را اینجا بکشید یا کلیک نمایید',
    dictCancelUpload: 'لغو بارگزاری',
    dictCancelUploadConfirmation: 'آیا از لغو بارگزاری مطمئنید؟',
    dictRemoveFile: 'حذف فایل'
  };

  public getConfiguration() {
    return this.DefaultConfig;
  }

  public forRoot() {
    return {
      provide: DROPZONE_CONFIG,
      useValue: this.DefaultConfig
    };
  }

}

export function DropZoneEronConfig() {
  return {
    // Change this to your upload POST address:
    url: Api.common.fileUpload,
    maxFilesize: 50,
    acceptedFiles: 'image/*',
    addRemoveLinks: true,
    dictDefaultMessage: 'برای بارگزاری فایل های خود را اینجا بکشید یا کلیک نمایید',
    dictCancelUpload: 'لغو بارگزاری',
    dictCancelUploadConfirmation: 'آیا از لغو بارگزاری مطمئنید؟',
    dictRemoveFile: 'حذف فایل'
  } as DropzoneConfigInterface;
}
