import { RouteInfo } from "./RouteInfo";

export interface AreaInfo {
   name: string,
   index: string,
   areas: AreaInfo[],
   routes: RouteInfo[],
   images: string[]
}
