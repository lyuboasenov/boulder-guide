import { Injectable } from '@angular/core';
import { AreaInfo } from './domain/AreaInfo';

@Injectable()
export class Globals {
   areas: AreaInfo[] = [];
}
