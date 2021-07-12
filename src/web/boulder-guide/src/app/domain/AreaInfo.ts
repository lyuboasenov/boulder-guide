import { RouteInfo } from "./RouteInfo";

export interface AreaInfo {
   id: string,
   name: string,
   index: string,
   areas: AreaInfo[],
   routes: RouteInfo[],
   images: string[]
}
