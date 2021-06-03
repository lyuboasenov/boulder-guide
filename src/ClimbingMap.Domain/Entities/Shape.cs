using JsonSubTypes;
using Newtonsoft.Json;

namespace ClimbingMap.Domain.Entities {
   public abstract class Shape {
      public string _type => GetType().Name;

      public static JsonConverter StandardJsonConverter {
         get {
            return JsonSubtypesConverterBuilder.Of(typeof(Shape), "_type").Build();
         }
      }

      public abstract void Draw(SkiaSharp.SKCanvas canvas, Size imageSize);
   }
}
