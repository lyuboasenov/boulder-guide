import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
   name: 'grade'
})
export class GradePipe implements PipeTransform {

   transform(value: number): string {
      if (value <= -1) { return "Проект"; }
      else if (value <= 20) { return "3"; }
      else if (value <= 25) { return "4-"; }
      else if (value <= 30) { return "4"; }
      else if (value <= 35) { return "4+"; }
      else if (value <= 40) { return "5-"; }
      else if (value <= 45) { return "5"; }
      else if (value <= 50) { return "5+"; }
      else if (value <= 55) { return "6A"; }
      else if (value <= 60) { return "6A+"; }
      else if (value <= 65) { return "6B"; }
      else if (value <= 70) { return "6B+"; }
      else if (value <= 75) { return "6C"; }
      else if (value <= 80) { return "6C+"; }
      else if (value <= 85) { return "7A"; }
      else if (value <= 90) { return "7A+"; }
      else if (value <= 95) { return "7B"; }
      else if (value <= 100) { return "7B+"; }
      else if (value <= 105) { return "7C"; }
      else if (value <= 110) { return "7C+"; }
      else if (value <= 115) { return "8A"; }
      else if (value <= 120) { return "8A+"; }
      else if (value <= 125) { return "8B"; }
      else if (value <= 130) { return "8B+"; }
      else if (value <= 135) { return "8C"; }
      else if (value <= 140) { return "8C+"; }
      else { return "9A"; }
   }

}
