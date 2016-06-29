using System.Collections.Generic;
using Nest.Searchify.Queries;
using Newtonsoft.Json;

namespace SearchifyMvcSample
{
    public class SampleParameters : SearchParameters
    {
        [JsonProperty("c")]
        public string Category { get; set; }

        [JsonProperty("m")]
        public IEnumerable<string> Multiple { get; set; }

    }
}