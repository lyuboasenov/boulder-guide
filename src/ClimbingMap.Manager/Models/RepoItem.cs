using ClimbingMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClimbingMap.Manager.Models {
   public class RepoItem {
      public string Name { get; }

      public object Context { get; }
      public ObservableCollection<RepoItem> Children { get; } = new ObservableCollection<RepoItem>();

      public RepoItem(Area area) {
         Context = area;
         Name = area.Name;
      }

      public RepoItem(Route route) {
         Context = route;
         Name = route.ToString();
      }
   }
}
