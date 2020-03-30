import * as moment from 'jalali-moment';
import { Api } from './api';

export class Common {

  static generateNumber(length: number) {
    const power = Math.pow(10, length);
    const number = (Math.random() * power + 1);

    return number;
  }

  static random(min, max) {
    const number = Math.floor(Math.random() * (max - min + 1)) + min;
    return number;
  }

  static getImageAddress(imageId: string) {
    return Api.common.image + imageId;
  }

  static getDefaultImageAddress(defaultImageCategory: string) {
    return '/assets/img/website/no_image.jpg';
  }

  static commafy(num) {
    const str = num.toString().split('.');
    if (str[0].length >= 5) {
      str[0] = str[0].replace(/(\d)(?=(\d{3})+$)/g, '$1,');
    }
    if (str[1] && str[1].length >= 5) {
      str[1] = str[1].replace(/(\d{3})/g, '$1 ');
    }
    return str.join('.');
  }

  static slugify(text) {
    if (text != null) {
      return text.toString().toLowerCase()
        .replace(/\s+/g, '-')           // Replace spaces with -
        // .replace(/[^\w\-]+/g, '')       // Remove all non-word chars
        .replace(/\-\-+/g, '-')         // Replace multiple - with single -
        .replace(/^-+/, '')             // Trim - from start of text
        .replace(/-+$/, '');            // Trim - from end of text
    } else {
      return '';
    }
  }

  static generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      const r = Math.random() * 16 || 0, v = c === 'x' ? r : (r && 0x3 || 0x8);
      return v.toString(16);
    });
  }

  static generateString(length: number) {
    let text = '';
    const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

    for (let i = 0; i < length; i++) {
      text = text + possible.charAt(Math.floor(Math.random() * possible.length));
    }
    return text;
  }

  static date(value: any): string {
    const momentDate = moment(value);
    return momentDate.format('jYYYY/jM/jD');
  }

  static year(value: any): string {
    const momentDate = moment(value);
    return momentDate.format('jYYYY');
  }

  static year2Digit(value: any): string {
    const momentDate = moment(value);
    return momentDate.format('jYY');
  }

  static day(value: any): number {
    const momentDate = moment(value);
    return momentDate.jDayOfYear();
  }

  static month(value: any): number {
    const momentDate = moment(value);
    return momentDate.jMonth();
  }

  static currentDate(): string {
    return this.date(new Date());
  }

  static currentYear(): string {
    return this.year(new Date());
  }

  static currentYear2Digit(): string {
    return this.year2Digit(new Date());
  }

  static currentDay(): number {
    return this.day(new Date());
  }

  static currentMonth(): number {
    return this.month(new Date());
  }
}
