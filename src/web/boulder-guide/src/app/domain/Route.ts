import { Topo } from './Topo'
import { Video } from './Video'
import { Location } from './Location'

export class Route {
   Id!: string;
   Location!: Location;
   Difficulty!: number;
   Name!: string;
   Info!: string;
   Tags!: string[];
   Links!: string[];
   Videos!: Video[];
   Schemas!: Topo[];
}
