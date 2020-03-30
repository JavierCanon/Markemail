import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'isParent'
})
export class IsParentPipe implements PipeTransform {
    transform(items: Array<any>): Array<any> {
        return items.filter(item => item.isParent == null);
    }
}
