using BoulderGuide.Mobile.Forms.Services.Errors;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Views {
   public class MapView : Mapsui.UI.Forms.MapView {

      private ReadOnlyPoint lastNavigatedToCenter;
      private bool isNavigating = false;
      private Mapsui.UI.Forms.Position[] missedGoToLocation;

      public static readonly BindableProperty MapProperty =
         BindableProperty.Create(
            nameof(Map),
            typeof(Map),
            typeof(MapView),
            null,
            propertyChanged: (b, o, n) => {
               ((MapView) b).OnMapChanged();
            });

      public static readonly BindableProperty MyLocationProperty =
         BindableProperty.Create(
            nameof(MyLocation),
            typeof(Mapsui.UI.Forms.Position),
            typeof(MapView),
            null,
            propertyChanged:(b, _, __) => (b as MapView)?.OnMyLocationChanged());

      public static readonly BindableProperty MyDirectionProperty =
         BindableProperty.Create(
            nameof(MyDirection),
            typeof(double),
            typeof(MapView),
            -1d,
            propertyChanged: (b, _, __) => (b as MapView)?.OnMyRotationChanged());

      public static readonly BindableProperty TargetLocationProperty =
         BindableProperty.Create(
            nameof(TargetLocation),
            typeof(IEnumerable<Mapsui.UI.Forms.Position>),
            typeof(MapView),
            null,
            propertyChanged: (b, _, __) => (b as MapView)?.OnTargetLocationChanged());

      public static readonly BindableProperty ResolutionProperty =
         BindableProperty.Create(
            nameof(Resolution),
            typeof(double),
            typeof(MapView),
            0.2d,
            propertyChanged: (b, _, __) => ((MapView) b).OnResolutionChanged(),
            validateValue: (b, v) => (b as MapView)?.IsValidResolution(v) ?? false);

      public static readonly BindableProperty MinResolutionProperty =
         BindableProperty.Create(
            nameof(MinResolution),
            typeof(double),
            typeof(MapView),
            0.2d,
            validateValue: (b,  v) => (b as MapView)?.IsValidMinResolution(v) ?? false);


      public static readonly BindableProperty MaxResolutionProperty =
         BindableProperty.Create(
            nameof(MaxResolution),
            typeof(double),
            typeof(MapView),
            20000d,
            validateValue: (b, v) => (b as MapView)?.IsValidMaxResolution(v) ?? false);

      public static new readonly BindableProperty RotationProperty =
         BindableProperty.Create(
            nameof(Rotation),
            typeof(double),
            typeof(MapView),
            0d,
            propertyChanged: (b, _, __) => {
               ((MapView) b).OnRotationChanged();
            });

      public static readonly BindableProperty FollowModeProperty =
         BindableProperty.Create(
            nameof(FollowMode),
            typeof(FollowMode),
            typeof(MapView),
            Views.FollowMode.None,
            propertyChanged: (b, _, __) => (b as MapView)?.OnFollowModeChanged());

      public new Map Map {
         get { return (Map) GetValue(MapProperty); }
         set { SetValue(MapProperty, value); }
      }

      public Mapsui.UI.Forms.Position MyLocation {
         get { return (Mapsui.UI.Forms.Position) GetValue(MyLocationProperty); }
         set { SetValue(MyLocationProperty, value); }
      }

      public double MyDirection {
         get { return (double) GetValue(MyDirectionProperty); }
         set { SetValue(MyDirectionProperty, value); }
      }

      public IEnumerable<Mapsui.UI.Forms.Position> TargetLocation {
         get { return (IEnumerable<Mapsui.UI.Forms.Position>) GetValue(TargetLocationProperty); }
         set { SetValue(TargetLocationProperty, value); }
      }

      public double Resolution {
         get { return (double) GetValue(ResolutionProperty); }
         set { SetValue(ResolutionProperty, value); }
      }

      public double MinResolution {
         get { return (double) GetValue(MinResolutionProperty); }
         set { SetValue(MinResolutionProperty, value); }
      }

      public double MaxResolution {
         get { return (double) GetValue(MaxResolutionProperty); }
         set { SetValue(MaxResolutionProperty, value); }
      }

      public new double Rotation {
         get { return (double) GetValue(RotationProperty); }
         set { SetValue(RotationProperty, value); }
      }

      public FollowMode FollowMode {
         get { return (FollowMode) GetValue(FollowModeProperty); }
         set { SetValue(FollowModeProperty, value); }
      }

      public MapView() {
         Viewport.ViewportChanged += Viewport_ViewportChanged;
      }

      private void OnMyLocationChanged() {
         MyLocationLayer.UpdateMyLocation(MyLocation);
         if (FollowMode == FollowMode.MyLocation) {
            GoToLocation(MyLocation);
         }
      }
      private void OnTargetLocationChanged() {
         if (FollowMode == FollowMode.TargetLocation) {
            GoToLocation(TargetLocation?.ToArray());
         }
      }

      private void Viewport_ViewportChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
         if (e.PropertyName == nameof(Resolution)) {
            var resolution = Viewport.Resolution;

            resolution = Math.Max(resolution, MinResolution);
            resolution = Math.Min(resolution, MaxResolution);

            Resolution = resolution;
         } else if (e.PropertyName == nameof(Rotation)) {
            Rotation = Viewport.Rotation;
         } else if (e.PropertyName == "Center" && !isNavigating) {
            OnCenterChanged();
         } else if (e.PropertyName == "HasSize" && Viewport.HasSize && null != missedGoToLocation) {
            GoToLocation(missedGoToLocation);
         }
      }

      private void OnCenterChanged() {
         if (lastNavigatedToCenter != null &&
            (Viewport.Center.X != lastNavigatedToCenter.X ||
            Viewport.Center.Y != lastNavigatedToCenter.Y)) {
            FollowMode = FollowMode.None;
         }
      }

      private void OnMapChanged() {
         try {
            if (Map is null) {
               base.Map.Layers.Clear();
               base.Map.ClearCache();
               base.Map = new Map();
            } else {
               base.Map = Map;
            }
         } catch (Exception ex) {
            ex.Handle();
         }
      }

      private void OnResolutionChanged() {
         try {
            Navigator.ZoomTo(Math.Max(Resolution, 0.2));
         } catch (Exception ex) {
            ex.Handle();
         }
      }

      private void OnRotationChanged() {
         try {
            Navigator.RotateTo(Rotation);
            OnMyRotationChanged();
         } catch (Exception ex) {
            ex.Handle();
         }
      }

      private bool IsValidResolution(object v) {
         if (v is double d) {
            return MinResolution <= d && d <= MaxResolution;
         }

         return false;
      }
      private bool IsValidMinResolution(object v) {
         if (v is double d) {
            return 0 < d && d < MaxResolution;
         }

         return false;
      }

      private bool IsValidMaxResolution(object v) {
         if (v is double d) {
            return d > MinResolution;
         }

         return false;
      }

      private void OnFollowModeChanged() {
         OnMyLocationChanged();
         OnTargetLocationChanged();
      }

      private void GoToLocation(params Mapsui.UI.Forms.Position[] location) {
         try {
            if (!Viewport.HasSize) {
               missedGoToLocation = location;
               return;
            }

            isNavigating = true;
            if (location?.Length == 1) {
               Navigator.NavigateTo(
                  SphericalMercator.FromLonLat(location[0].Longitude, location[0].Latitude),
                  Resolution);
            } else if (location?.Length > 1) {
               BoundingBox boundingBox =
                  new BoundingBox(location.Select(l => SphericalMercator.FromLonLat(l.Longitude, l.Latitude)));

               Navigator.NavigateTo(boundingBox);
            }
            lastNavigatedToCenter = Viewport.Center;
            isNavigating = false;
         } catch (Exception ex) {
            ex.Handle();
         }
      }

      private void OnMyRotationChanged() {
         try {
            MyLocationLayer.IsMoving = true;
            var adjustedDirection = (MyDirection + Rotation) % 360;
            MyLocationLayer.UpdateMyDirection(adjustedDirection, 0);
         } catch (Exception ex) {
            ex.Handle();
         }

      }
   }

   public enum FollowMode {
      None,
      MyLocation,
      TargetLocation
   }
}
