namespace BoulderGuide.Mobile.Forms.Services.Data {
   public class RegionDTO {
      [Newtonsoft.Json.JsonProperty(PropertyName = "urn")]
      public string Name { get; set; }
      public string Url { get; set; }
      public RegionAccess Access { get; set; }
      public bool Compressed { get; set; }
      public bool Encrypted { get; set; }
   }
}
