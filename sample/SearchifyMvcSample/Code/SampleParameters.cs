using System.Collections.Generic;
using System.Linq;
using Nest;
using Nest.Searchify.Queries;
using Nest.Searchify.SearchResults;
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

    public class SampleSearchResults : SearchResult<SampleParameters, SampleDocument>
    {
        public SampleSearchResults(SampleParameters parameters, ISearchResponse<SampleDocument> response) : base(parameters, response)
        {
        }

        public override IEnumerable<SampleDocument> Documents => Enumerable.Repeat(new SampleDocument(), 100);

        protected override long GetSearchResultTotal()
        {
            return 100;
        }
    }

    public class SampleDocument
    {
        
    }
}