import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { LaddaModule } from 'angular2-ladda';
import { PubSubModule } from 'angular2-pubsub';
import { ToastModule } from 'ng2-toastr/src/toast.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { CKEditorModule } from 'ngx-ckeditor';
import { DROPZONE_CONFIG } from 'ngx-dropzone-wrapper';
import { ModalModule } from 'ngx-modal';
import { RecaptchaModule, RecaptchaLoaderService } from 'ng2-recaptcha';
import { AgmCoreModule } from '@agm/core/core.module';
import {
  SummernoteDirective,
  ICheckDirective,
  ICheckWithModelDirective,
  TagsInputDirective,
  NestableDirective,
  TagsInputWithModelDirective
} from './directives';
import {
  IsParentPipe,
  PriceRialPipe,
  PriceTomanPipe,
  OrderStatusPipe,
  OrderStatusColorPipe,
  InvoiceStatusPipe,
  InvoiceTypePipe,
  InvoiceStatusColorPipe,
  JalaliPipe,
  DefaultFilterPipe,
  LikeFilterPipe,
  SlugPipe
} from './pipes';
import { CheckboxComponent, RadioComponent, BadgeComponent } from './components';
import { DropZoneEronConfig } from './config/DropZoneConfiguration';

@NgModule({
  declarations: [
    SummernoteDirective,
    ICheckDirective,
    ICheckWithModelDirective,
    TagsInputDirective,
    NestableDirective,
    TagsInputWithModelDirective,
    BadgeComponent,
    IsParentPipe,
    PriceRialPipe,
    PriceTomanPipe,
    OrderStatusPipe,
    OrderStatusColorPipe,
    InvoiceStatusPipe,
    InvoiceTypePipe,
    InvoiceStatusColorPipe,
    JalaliPipe,
    DefaultFilterPipe,
    LikeFilterPipe,
    SlugPipe,
    CheckboxComponent,
    RadioComponent
  ],
  imports: [
    RouterModule,
    FormsModule,
    HttpModule,
    ReactiveFormsModule,
    DropzoneModule, // .forChild()
    LaddaModule,
    CurrencyMaskModule,
    NgxPaginationModule,
    CKEditorModule,
    ModalModule,
    // OwlModule,
    PubSubModule.forRoot(),
    ToastModule.forRoot(),
    RecaptchaModule.forRoot(),
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyCs8nSEumLi8FjQrodnZEWl4H36P_UpTow'
    })
  ],
  exports: [
    CommonModule,
    RouterModule,
    FormsModule,
    HttpModule,
    ReactiveFormsModule,
    DropzoneModule,
    SummernoteDirective,
    ICheckDirective,
    ICheckWithModelDirective,
    TagsInputDirective,
    TagsInputWithModelDirective,
    BadgeComponent,
    CheckboxComponent,
    RadioComponent,
    NestableDirective,
    IsParentPipe,
    PriceRialPipe,
    PriceTomanPipe,
    OrderStatusPipe,
    InvoiceTypePipe,
    OrderStatusColorPipe,
    InvoiceStatusPipe,
    InvoiceStatusColorPipe,
    JalaliPipe,
    DefaultFilterPipe,
    LikeFilterPipe,
    SlugPipe,
    LaddaModule,
    CurrencyMaskModule,
    NgxPaginationModule,
    CKEditorModule,
    ModalModule,
    RecaptchaModule,
    ToastModule,
    PubSubModule,
    AgmCoreModule
  ],
  providers: [
    {
      provide: DROPZONE_CONFIG,
      useValue: DropZoneEronConfig()
    }
  ]
})
export class BaseModule {}
