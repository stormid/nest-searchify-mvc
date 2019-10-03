using System.Collections.Generic;
using Nest.Searchify.Queries;
using Newtonsoft.Json;

namespace SearchifyMvcSample.Code
{
    public class SampleParameters : SearchParameters
    {
        [JsonProperty("c")]
        public string Category { get; set; }

        [JsonProperty("m")]
        public IEnumerable<string> Multiple { get; set; }

        //[JsonProperty("loc")]
        //public GeoPoint Location { get; set; }

        //[JsonProperty("locbnd")]
        //public GeoBoundingBox Boundary { get; set; }

    }
}