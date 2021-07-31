export class Location {
   Latitude: number = 0;
   Longitude: number = 0;

   constructor(long: number = 0, lat: number = 0) {
      this.Longitude = long;
      this.Latitude = lat;
   }

   static equals(first: Location, second: Location, delta: number = 0): boolean {
      if (second == null) {
         return false;
      }

      return Math.abs(first.Longitude - second.Longitude) <= delta &&
         Math.abs(first.Latitude - second.Latitude) <= delta;
   }
}
