import { Pipe, PipeTransform } from '@angular/core';
import { Common } from '../common';
@Pipe({
  name: 'slug'
})
export class SlugPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    const slug = Common.slugify(value);
    return slug;
  }
}
