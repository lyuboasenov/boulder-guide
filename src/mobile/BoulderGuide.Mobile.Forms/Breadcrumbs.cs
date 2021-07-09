using Prism.Navigation;
using System;
using System.Collections.ObjectModel;

namespace BoulderGuide.Mobile.Forms {
   public static class Breadcrumbs {
      public static ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>(new[] {
         new Item() {
            Glyph = Icons.MaterialIconFont.Home,
            Title = Strings.ClimbingAreas,
            Path = "/MainPage/NavigationPage/HomePage"
         }});

      public class Item : IEquatable<Item> {
         public string Offset {
            get {
               return new string(' ', Items.IndexOf(this) * 3);
            }
         }
         public string Glyph { get; set; }
         public string Title { get; set; }
         public string Path { get; set; }
         public INavigationParameters Parameters { get; set; }

         public override string ToString() {
            return $"{Title}: {Path}";
         }

         public override int GetHashCode() {
            return ToString().GetHashCode();
         }

         public override bool Equals(object obj) {
            return (obj is Item i) && Equals(i);
         }

         public bool Equals(Item i) {
            return Glyph == i.Glyph &&
               Title == i.Title &&
               Path == i.Path &&
               Parameters == i.Parameters;
         }
      }
   }
}
