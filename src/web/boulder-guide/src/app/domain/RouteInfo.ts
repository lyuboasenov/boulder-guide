import { Location } from './Location'

export interface RouteInfo {
   id: string,
   name: string,
   index: string,
   images: string[],
   difficulty: number,
   location: Location
}
