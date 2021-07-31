import { PointOfInterest } from "./PointOfInterest";
import { Track } from "./Track";
import { Location } from "./Location";

export class Area {
   Id!: string;
   Name!: string;
   Info!: string;
   Access!: string;
   History!: string;
   Ethics!: string;
   Accommodations!: string;
   Restrictions!: string;
   Tags!: string[];
   Location!: Location[];
   Links!: string[];
   POIs!: PointOfInterest[];
   Tracks!: Track[];
}
