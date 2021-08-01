import { AreaInfo } from './AreaInfo';
import { Location } from './Location'

export class RouteInfo {
   rootId!: string;
   areaInfo!: AreaInfo;

   id!: string;
   name!: string;
   index!: string;
   images!: string[];
   difficulty!: number;
   location!: Location;
}
