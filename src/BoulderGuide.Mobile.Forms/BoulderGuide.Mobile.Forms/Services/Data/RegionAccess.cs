using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BoulderGuide.Mobile.Forms.Services.Data {
   [JsonConverter(typeof(StringEnumConverter))]
   public enum RegionAccess {
      @public,
      @private
   }
}