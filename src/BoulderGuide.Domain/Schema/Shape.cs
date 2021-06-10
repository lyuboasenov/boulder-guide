using JsonSubTypes;
using Newtonsoft.Json;

namespace BoulderGuide.Domain.Schema {
   public abstract class Shape {
      public string _type => GetType().Name;

      public static JsonConverter StandardJsonConverter {
         get {
            return JsonSubtypesConverterBuilder.Of(typeof(Shape), "_type").Build();
         }
      }

      public abstract void Draw(SkiaSharp.SKCanvas canvas, Size imageSize, Size offset);

      public override string ToString() {
         return _type;
      }
   }
}
