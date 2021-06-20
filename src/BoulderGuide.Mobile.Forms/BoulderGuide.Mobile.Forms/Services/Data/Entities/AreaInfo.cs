﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.Services.Data.Entities {
   public class AreaInfo : FileBasedEntity {
      private readonly AreaInfoDTO dto;
      private readonly Region region;

      public AreaInfo(Region region, AreaInfo parent, AreaInfoDTO dto) :
         base (
            "index.json",
            region.RemoteRootPath,
            region.LocalRootPath,
            region.Access == RegionAccess.@private) {

         this.region = region ?? throw new ArgumentNullException(nameof(region));
         Parent = parent;
         this.dto = dto ?? throw new ArgumentNullException(nameof(dto));

         Areas = dto.Areas?.Select(a => new AreaInfo(region, this, a));
         Routes = dto.Routes?.Select(r => new RouteInfo(region, this, r));
         Images = dto.Images?.Select(i => new Image(region, i));
         Map = string.IsNullOrEmpty(dto.Map) ? null : new Map(region, dto.Map);
      }

      public string Index => dto.Index;
      public string Name => dto.Name;
      public AreaInfo Parent { get; }
      public IEnumerable<AreaInfo> Areas { get; }
      public IEnumerable<RouteInfo> Routes { get; }
      public IEnumerable<Image> Images { get; }

      public Map Map { get; }

      public bool IsOffline {
         get {
            bool result = true;

            if (!ExistsLocally) {
               result = false;
            }

            if (result && (Areas?.Any(f => !f.ExistsLocally) ?? false)) {
               result = false;
            }
            if (result && (Routes?.Any(f => !f.ExistsLocally) ?? false)) {
               result = false;
            }
            if (result && (Images?.Any(f => !f.ExistsLocally) ?? false)) {
               result = false;
            }
            if (result && (!Map?.ExistsLocally ?? false)) {
               result = false;
            }
            if (result && (!Area?.ExistsLocally ?? true)) {
               result = false;
            }

            return result;
         }
      }

      public Area Area { get; private set; }

      public int TotalAreaCount {
         get {
            return (Areas?.Count() ?? 0) +
               (Areas?.Sum(a => a.TotalAreaCount) ?? 0);
         }
      }

      public int TotalRouteCount {
         get {
            return (Routes?.Count() ?? 0) +
               (Areas?.Sum(a => a.TotalRouteCount) ?? 0);
         }
      }

      public async Task LoadAreaAsync(bool force = false) {
         if (Area is null) {
            var area = new Area(region, Index);
            await area.DownloadAsync(force).ConfigureAwait(false);
            Area = area;
         }
      }

      public async Task DownloadMapAsync(bool force = false) {
         await Map?.DownloadAsync(force);
      }

      public async Task DownloadImagesAsync(bool force = false) {
         var list = new List<Task>();
         foreach (var image in Images ?? Enumerable.Empty<Image>()) {
            list.Add(image.DownloadAsync(force));
         }

         await Task.WhenAll(list);
      }

      public override async Task DownloadAsync(bool force = false) {

         var tasks = new List<Task>();
         tasks.Add(DownloadMapAsync(force));
         tasks.Add(DownloadImagesAsync(force));
         tasks.Add(LoadAreaAsync(force));

         foreach (var i in Areas ?? Enumerable.Empty<AreaInfo>()) {
            tasks.Add(i.DownloadAsync(force));
         }

         foreach (var i in Routes ?? Enumerable.Empty<RouteInfo>()) {
            tasks.Add(i.DownloadAsync(force));
         }

         await Task.WhenAll(tasks);
      }

      public override string ToString() {
         return $"Area: '{dto.Name}'";
      }
   }
}