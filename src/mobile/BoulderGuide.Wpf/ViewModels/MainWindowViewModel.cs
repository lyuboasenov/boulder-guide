using BoulderGuide.DTOs;
using BoulderGuide.Wpf.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace BoulderGuide.Wpf.ViewModels {
   internal class MainWindowViewModel : BaseViewModel<MainWindow> {

      private string rootPath;

      public MainWindowViewModel() {
         AddAreaCommand = new Command(AddArea, CanAddArea);
         AddRouteCommand = new Command(AddRoute, CanAddRoute);
         CopyRouteCommand = new Command(CopyRoute, CanCopyRoute);
         ImportRouteCommand = new Command(ImportRoute, CanImportRoute);
         ImportOruxToGpxCommand = new Command(ImportOruxToGpx);
         ReloadCommand = new Command(Reload);
         RecalculateAreaBounderiesCommand = new Command(RecalculateAreaBounderies);
      }

      public IEnumerable<Area> Items { get; set; }
      public ICommand AddAreaCommand { get; }
      public ICommand AddRouteCommand { get; }
      public ICommand CopyRouteCommand { get; }
      public ICommand ImportRouteCommand { get; }
      public ICommand ImportOruxToGpxCommand { get; }
      public ICommand ReloadCommand { get; }
      public ICommand RecalculateAreaBounderiesCommand { get; }

      public object SelectedItem { get; set; }
      public string Title { get; set; } = "Initial";

      public override void Initialize() {
         base.Initialize();

         string indexPath = SelectFile("Map index | index.json");
         if (!string.IsNullOrEmpty(indexPath)) {
            var fi = new FileInfo(indexPath);

            rootPath = fi.Directory.FullName;

            Reload();
            Title = $"Climbing region: {Items?.FirstOrDefault()?.Name}";
         } else {
            Close();
         }
      }

      public void OnSelectedItemChanged() {
         (AddAreaCommand as Command)?.RaiseCanExecuteChanged();
         (AddRouteCommand as Command)?.RaiseCanExecuteChanged();
         (CopyRouteCommand as Command)?.RaiseCanExecuteChanged();
         (ImportRouteCommand as Command)?.RaiseCanExecuteChanged();
      }

      private void RecalculateAreaBounderies() {
         foreach(var item in Items) {
            RecalculateAreaBounderies(item);
         }
      }

      private void RecalculateAreaBounderies(Area area) {
         AreaViewModel.RecalculateBorders(area);
         foreach (var item in area.Items) {
            if (item is Area a) {
               RecalculateAreaBounderies(a);
            }
         }
         area.Save();
      }

      private void Reload() {
         Items = new[] { GetArea(rootPath) };
      }

      private bool CanImportRoute() {
         return SelectedItem is Area;
      }

      private bool CanCopyRoute() {
         return SelectedItem is Route;
      }

      private bool CanAddRoute() {
         return SelectedItem is Area;
      }

      private bool CanAddArea() {
         return SelectedItem is Area;
      }

      private void AddRoute() {
         if (SelectedItem is Area a) {
            var route = new Route(a);
            route.Save();
         }

         Reload();
      }

      private void AddArea() {
         Reload();
      }

      private void CopyRoute() {
         if (SelectedItem is Route r) {
            r.Id = r.Id + "-copy";
            r.Save();

            Reload();
         }
      }

      private void ImportOruxToGpx() {
         string oruxGpxPath = SelectFile("OruxMap gpx file | *.gpx");
         string areaGpxPath = SelectFile("Area gpx file | *.gpx");

         XmlDocument oruxDoc = new XmlDocument();
         oruxDoc.Load(oruxGpxPath);
         XmlNamespaceManager oruxNsMgr = new XmlNamespaceManager(oruxDoc.NameTable);
         oruxNsMgr.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");
         oruxNsMgr.AddNamespace("om", "http://www.oruxmaps.com/oruxmapsextensions/1/0");

         XmlDocument areaDoc = new XmlDocument();
         areaDoc.Load(areaGpxPath);
         XmlNamespaceManager areaNsMgr = new XmlNamespaceManager(areaDoc.NameTable);
         areaNsMgr.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");

         foreach (XmlNode node in oruxDoc.DocumentElement.SelectNodes($"//gpx:wpt", oruxNsMgr)) {
            var latitude = node.Attributes["lat"].Value;
            var longitude = node.Attributes["lon"].Value;

            string img = null; // <extensions>/<om:oruxmapsextensions>/<om:ext type="IMAGEN">

            string name = GetXmlSubNodeInnerText(node, nameof(name)).
               Replace("<![CDATA[", "").
               Replace("]]>", ""); // <![CDATA[0000440]]>
            string desc = GetXmlSubNodeInnerText(node, nameof(desc));
            string ele = GetXmlSubNodeInnerText(node, nameof(ele));
            string type = GetXmlSubNodeInnerText(node, nameof(type)); // <type>Photo</type> <type>Waypoint</type>
            string time = GetXmlSubNodeInnerText(node, nameof(time));

            if (type.Equals("Photo", StringComparison.InvariantCultureIgnoreCase)) {
               var extentions = GetFirstChild(node, "extensions");
               var oruxExt = GetFirstChild(extentions, "om:oruxmapsextensions");
               var omExt = GetFirstChild(oruxExt, "om:ext", (n) => n.Attributes["type"]?.Value?.Equals("IMAGEN", StringComparison.InvariantCultureIgnoreCase) ?? false);

               img = omExt?.InnerText;
               img = img?.Substring(img.LastIndexOf("/") + 1);

               if (!string.IsNullOrEmpty(desc)) {
                  desc += Environment.NewLine;
               }

               desc += img;
            }

            var wptEl = areaDoc.CreateElement("wpt", "http://www.topografix.com/GPX/1/1");
            wptEl.SetAttribute("lat", latitude);
            wptEl.SetAttribute("lon", longitude);
            areaDoc.DocumentElement.InsertAfter(wptEl, GetFirstChild(areaDoc.DocumentElement, "metadata"));

            var eleEl = areaDoc.CreateElement("ele", "http://www.topografix.com/GPX/1/1");
            eleEl.InnerText = ele;
            wptEl.AppendChild(eleEl);
            var timeEl = areaDoc.CreateElement("time", "http://www.topografix.com/GPX/1/1");
            timeEl.InnerText = time;
            wptEl.AppendChild(timeEl);
            var nameEl = areaDoc.CreateElement("name", "http://www.topografix.com/GPX/1/1");
            nameEl.InnerText = name;
            wptEl.AppendChild(nameEl);
            var descEl = areaDoc.CreateElement("desc", "http://www.topografix.com/GPX/1/1");
            descEl.InnerText = desc;
            wptEl.AppendChild(descEl);
         }

         areaDoc.Save(areaGpxPath);
      }

      private XmlNode GetFirstChild(XmlNode node, string name, Func<XmlNode, bool> filter) {
         return node?.
            ChildNodes?.
            Cast<XmlNode>()?.
            FirstOrDefault(
               n =>
                  n.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                  filter(n));
      }

      private XmlNode GetFirstChild(XmlNode node, string name) {
         return GetFirstChild(node, name, (n) => true);
      }

      private string GetXmlSubNodeInnerText(XmlNode node, string nodeName) {
         foreach (XmlNode subNode in node.ChildNodes) {
            if (subNode.Name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase)) {
               return subNode.InnerText;
            }
         }

         return null;
      }

      private void ImportRoute() {
         string indexPath = SelectFile("Importing routes gpx | *.gpx");
         string picturesPath = string.IsNullOrEmpty(indexPath) ? string.Empty : SelectDirectory(indexPath);

         if (!string.IsNullOrEmpty(indexPath) && File.Exists(indexPath) && SelectedItem is Area a) {
            XmlDocument doc = new XmlDocument();
            doc.Load(indexPath);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");

            var points = new List<Location>();

            foreach (XmlNode node in doc.DocumentElement.SelectNodes($"//gpx:wpt", nsmgr)) {
               points.Add(new Location() {
                  Latitude = double.Parse(node.Attributes["lat"].Value),
                  Longitude = double.Parse(node.Attributes["lon"].Value)
               });

               string name = GetXmlSubNodeInnerText(node, nameof(name));
               string desc = GetXmlSubNodeInnerText(node, nameof(desc));
               string type = GetXmlSubNodeInnerText(node, nameof(type));

               string photo = string.Empty;
               if (type.Equals("Photo", StringComparison.InvariantCultureIgnoreCase)) {
                  var extentions = GetFirstChild(node, "extensions");
                  var oruxExt = GetFirstChild(extentions, "om:oruxmapsextensions");
                  var omExt = GetFirstChild(oruxExt, "om:ext", (n) => n.Attributes["type"]?.Value?.Equals("IMAGEN", StringComparison.InvariantCultureIgnoreCase) ?? false);

                  photo = omExt?.InnerText;
                  photo = photo?.Substring(photo.LastIndexOf("/") + 1);

                  if (!string.IsNullOrEmpty(picturesPath) 
                     && Directory.Exists(picturesPath) 
                     && File.Exists(System.IO.Path.Combine(picturesPath, photo))) {
                     photo = System.IO.Path.Combine(picturesPath, photo);
                  } else {
                     desc += $@"

Picture: {photo}
                        ";
                     photo = string.Empty;
                  }
               }

               var route = new Route(a) {
                  Id = name.ToLower(),
                  Name = name,
                  Info = desc,
                  Location = $"N{node.Attributes["lat"].Value} E{node.Attributes["lon"].Value}"
               };
               if (!string.IsNullOrEmpty(photo)) {
                  route.AddTopo(new Topo() {
                     Id = photo,
                  });
               }

               route.Save();
            }

            Reload();
         }
      }

      private Area GetArea(string path) {

         Area area = null;
         var areaPath = System.IO.Path.Combine(path, "area.json");
         if (File.Exists(areaPath)) {
            // load area
            area = new Area(areaPath);


            foreach (var filePath in Directory.GetFiles(path, "*.json")) {
               if (!filePath.EndsWith("index.json") &&
                  !filePath.EndsWith("area.json")) {
                  area.AddRoute(filePath);
               }
            }

            foreach(var subAreaPath in Directory.GetDirectories(path)) {
               if (!subAreaPath.EndsWith(".git") &&
                  !subAreaPath.EndsWith(".github")) {
                  area.AddArea(GetArea(subAreaPath));
               }
            }
         } else {
            MessageBox.Show($"No area file found in {path}");
         }

         return area;
      }
   }
}
