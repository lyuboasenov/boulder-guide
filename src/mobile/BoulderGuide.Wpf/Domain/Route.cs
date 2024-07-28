using BoulderGuide.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace BoulderGuide.Wpf.Domain {
   public class Route : BaseEntity {
      private RouteDTO dto;
      private string path;
      private string directory;
      private List<Topo> topos;

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

      public string Tags {
         get {
            return string.Join(", ", dto?.Tags);
         }
         set {
            dto.Tags = value.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
            RaisePropertyChanged();
         }
      }

      public string Location {
         get {
            return dto.Location.ToString();
         }
         set {
            dto.Location = new Location(value);
            RaisePropertyChanged();
         }
      }

      public DTOs.Location RawLocation {
         get { return dto.Location; }
      }

      public double Difficulty {
         get {
            return dto.Difficulty;
         }
         set {
            dto.Difficulty = value;
            RaisePropertyChanged();
         }
      }

      public string EightALink {
         get {
            return dto?.EightALink;
         }
         set {
            dto.EightALink = value;
            RaisePropertyChanged();
         }
      }

      public IEnumerable<Video> Videos => dto.Videos;
      public IEnumerable<Topo> Topos => dto.Topos;

      public Route(string path) {
         this.path = path;
         directory = new System.IO.FileInfo(path).Directory.FullName;
         dto =
            Newtonsoft.Json.JsonConvert.DeserializeObject<RouteDTO>(
               System.IO.File.ReadAllText(path),
               Shape.StandardJsonConverter);

         topos = dto.Topos.Select(t => {
            return new Topo() {
               Id = System.IO.Path.Combine(directory, t.Id),
               Shapes = t.Shapes
            };
         }).ToList();

         dto.Topos = topos.ToArray();
      }

      public Route(Area a) {
         var areaInfo = new System.IO.FileInfo(a.Path);
         directory = areaInfo.Directory.FullName;
         dto = new RouteDTO();
         dto.Id = "_new-route-" + System.Guid.NewGuid().ToString();
         dto.Name = "Нов маршрут";
         path = System.IO.Path.Combine(directory, $"{dto.Id}.json");
         topos = new List<Topo>();
      }

      internal void Save() {
         var name = dto.Id.ToLowerInvariant();
         int counter = 0;

         var schemaList = new List<Topo>();
         foreach (var schema in topos ?? Enumerable.Empty<Topo>()) {
            var fi = new System.IO.FileInfo(schema.Id);
            string id = $"{name}_{counter++}{fi.Extension.ToLowerInvariant()}";

            var imgFilePath = System.IO.Path.Combine(directory, id);
            if (!System.IO.File.Exists(imgFilePath)) {
               System.IO.File.Copy(
                  fi.FullName,
                  imgFilePath);
            }

            schemaList.Add(new Topo() {
               Id = id,
               Shapes = schema.Shapes.ToArray()
            });
         }

         dto.Topos = schemaList.ToArray();
         System.IO.File.WriteAllText(
            System.IO.Path.Combine(directory, $"{name}.json"),
            Newtonsoft.Json.JsonConvert.SerializeObject(
               dto,
               Newtonsoft.Json.Formatting.Indented));
      }

      public void AddVideo(Video video) {
         dto.Videos = dto.Videos.Append(video).ToArray();
         RaisePropertyChanged(nameof(Videos));
      }

      public void RemoveVideo(Video video) {
         var list = new List<Video>(dto.Videos);
         list.Remove(video);
         dto.Videos = list.ToArray();
         RaisePropertyChanged(nameof(Videos));
      }

      internal void RemoveShape(Topo topo, Shape shape) {
         var list = new List<Shape>(topo.Shapes);
         list.Remove(shape);
         topo.Shapes = list.ToArray();

         RaisePropertyChanged(nameof(Topos));
      }

      public void AddTopo(Topo topo) {
         topos.Add(topo);
         dto.Topos = topos.ToArray();
         RaisePropertyChanged(nameof(Topos));
      }

      public void RemoveTopo(Topo topo) {
         topos.Remove(topo);
         dto.Topos = topos.ToArray();
         RaisePropertyChanged(nameof(Topos));
      }
   }
}
