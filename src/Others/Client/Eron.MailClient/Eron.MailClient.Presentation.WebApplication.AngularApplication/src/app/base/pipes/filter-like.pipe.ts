import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'likefilter',
    pure: false
})
export class LikeFilterPipe implements PipeTransform {
    transform(items: any[], filter: Object): any {
        if (!items || !filter) {
            return items;
        }

        let result = items;

        for (const property in filter) {
          if (filter.hasOwnProperty(property)) {
              result = result.filter(item => item[property].indexOf(filter[property]) !== -1);
          }
        }

        return result;
    }
}
