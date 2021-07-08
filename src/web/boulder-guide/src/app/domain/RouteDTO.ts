import * as topos from './Topo'
import * as videos from './Video'

export interface RouteDTO {
   Id: string,
   Location: Location,
   Difficulty: number,
   Name: string,
   Info: string,
   Tags: string[],
   Links: string[],
   Videos: videos.Video[],
   Schemas: topos.Topo[]
}
