using BoulderGuide.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoulderGuide.Wpf.Domain {
   public class Area : BaseEntity {
      private readonly AreaDTO dto;
      private readonly List<Route> routes = new List<Route>();
      private readonly List<Area> areas = new List<Area>();
      private readonly string path;

      public string Path => path;

      public IEnumerable<object> Items => GetItems();
      public IEnumerable<Location> Locations => dto.Location;
      public IEnumerable<PointOfInterest> POIs => dto.POIs;
      public IEnumerable<Track> Tracks => dto.Tracks;

      private IEnumerable<object> GetItems() {
         var items = new List<object>();
         items.AddRange(areas);
         items.AddRange(routes);

         return items;
      }

      public string Id {
         get {
            return dto?.Id;
         }
         set {
            dto.Id = value;
            RaisePropertyChanged();
         }
      }

      public string Name {
         get {
            return dto?.Name;
         }
         set {
            dto.Name = value;
            RaisePropertyChanged();
         }
      }

      public string Info {
         get {
            return dto?.Info;
         }
         set {
            dto.Info = value;
            RaisePropertyChanged();
         }
      }

      public string Access {
         get {
            return dto?.Access;
         }
         set {
            dto.Access = value;
            RaisePropertyChanged();
         }
      }

      public string Accommodations {
         get {
            return dto?.Accommodations;
         }
         set {
            dto.Accommodations = value;
            RaisePropertyChanged();
         }
      }

      public string History {
         get {
            return dto?.History;
         }
         set {
            dto.History = value;
            RaisePropertyChanged();
         }
      }

      public string Ethics {
         get {
            return dto?.Ethics;
         }
         set {
            dto.Ethics = value;
            RaisePropertyChanged();
         }
      }

      public string Restrictions {
         get {
            return dto?.Restrictions;
         }
         set {
            dto.Restrictions = value;
            RaisePropertyChanged();
         }
      }

      public string Tags {
         get {
            return string.Join(", ", dto?.Tags);
         }
         set {
            dto.Tags = value.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            RaisePropertyChanged();
         }
      }

      public Area(string path) {
         dto = Newtonsoft.Json.JsonConvert.DeserializeObject<AreaDTO>(System.IO.File.ReadAllText(path));
         this.path = path;
      }

      internal void AddRoute(string filePath) {
         routes.Add(new Route(filePath));
         RaisePropertyChanged(nameof(Items));
      }

      public void AddArea(Area area) {
         areas.Add(area);
         RaisePropertyChanged(nameof(Items));
      }

      public void AddLocation(Location location) {
         dto.Location = dto.Location.Append(location).ToArray();
         RaisePropertyChanged(nameof(Locations));
      }

      public void RemoveLocation(Location location) {
         var list = new List<Location>(dto.Location);
         list.Remove(location);
         dto.Location = list.ToArray();
         RaisePropertyChanged(nameof(Locations));
      }

      public void AddPOI(PointOfInterest poi) {
         dto.POIs = dto.POIs.Append(poi).ToArray();
         RaisePropertyChanged(nameof(POIs));
      }

      public void RemovePOI(PointOfInterest poi) {
         var list = new List<PointOfInterest>(dto.POIs);
         list.Remove(poi);
         dto.POIs = list.ToArray();
         RaisePropertyChanged(nameof(POIs));
      }

      public void AddTrack(Track track) {
         dto.Tracks = dto.Tracks.Append(track).ToArray();
         RaisePropertyChanged(nameof(Tracks));
      }

      public void RemoveTrack(Track track) {
         var list = new List<Track>(dto.Tracks);
         list.Remove(track);
         dto.Tracks = list.ToArray();
         RaisePropertyChanged(nameof(Tracks));
      }

      public void Save() {
         System.IO.File.WriteAllText(
            path,
            Newtonsoft.Json.JsonConvert.SerializeObject(
               dto,
               Newtonsoft.Json.Formatting.Indented));
      }
   }
}
