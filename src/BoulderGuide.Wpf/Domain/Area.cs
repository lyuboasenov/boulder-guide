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
            RaisePropertyChanged(nameof(Id));
         }
      }

      public string Name {
         get {
            return dto?.Name;
         }
         set {
            dto.Name = value;
            RaisePropertyChanged(nameof(Name));
         }
      }

      public string Info {
         get {
            return dto?.Info;
         }
         set {
            dto.Info = value;
            RaisePropertyChanged(nameof(Info));
         }
      }

      public string Access {
         get {
            return dto?.Access;
         }
         set {
            dto.Access = value;
            RaisePropertyChanged(nameof(Access));
         }
      }

      public string Accommodations {
         get {
            return dto?.Accommodations;
         }
         set {
            dto.Accommodations = value;
            RaisePropertyChanged(nameof(Accommodations));
         }
      }

      public string History {
         get {
            return dto?.History;
         }
         set {
            dto.History = value;
            RaisePropertyChanged(nameof(History));
         }
      }

      public string Ethics {
         get {
            return dto?.Ethics;
         }
         set {
            dto.Ethics = value;
            RaisePropertyChanged(nameof(Ethics));
         }
      }

      public string Restrictions {
         get {
            return dto?.Restrictions;
         }
         set {
            dto.Restrictions = value;
            RaisePropertyChanged(nameof(Restrictions));
         }
      }

      public string Tags {
         get {
            return string.Join(", ", dto?.Tags);
         }
         set {
            dto.Tags = value.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            RaisePropertyChanged(nameof(Tags));
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

      public void Save() {
         System.IO.File.WriteAllText(
            path,
            Newtonsoft.Json.JsonConvert.SerializeObject(
               dto,
               Newtonsoft.Json.Formatting.Indented));
      }
   }
}
