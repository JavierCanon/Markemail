import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'defaultfilter',
    pure: false
})
export class DefaultFilterPipe implements PipeTransform {
    transform(items: any[], filter: Object): any {
        if (!items || !filter) {
            return items;
        }

        let result = items;

        for (const property in filter) {
          if (filter.hasOwnProperty(property)) {
              result = result.filter(item => item[property] == filter[property]);
          }
        }

        return result;
    }
}
