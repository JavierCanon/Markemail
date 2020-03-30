import {NgModule, ErrorHandler} from '@angular/core';

export class DefaultErrorHandler implements ErrorHandler {
  handleError(error) {
    console.log(error);
    // do something with the exception
  }
}
