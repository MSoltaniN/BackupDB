import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'toArray'
})
export class toArrayPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return  Array.from(value);
  }

}
