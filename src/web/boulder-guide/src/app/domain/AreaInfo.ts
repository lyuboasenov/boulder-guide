import { RouteInfo } from "./RouteInfo";

export interface AreaInfo {
   rootId: string,
   id: string,
   name: string,
   index: string,
   areas: AreaInfo[],
   routes: RouteInfo[],
   images: string[],
   totalAreas: number,
   totalRoutes: number
}
