using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Nest.Searchify.SearchResults;

namespace SearchifyMvcSample.Code
{
    public class SampleSearchResults : SearchResult<SampleParameters, SampleDocument>
    {
        public SampleSearchResults(SampleParameters parameters, ISearchResponse<SampleDocument> response) : base(parameters, response)
        {
        }

        public override IEnumerable<SampleDocument> Documents
        {
            get
            {
                var docs = Enumerable.Range(1, 65).Select(counter => new SampleDocument()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Document #" + counter,
                    Category = (counter%2) == 0 ? "Alpha" : "Beta",
                    Summary = "This is a search summary"
                }).ToList();

                return !string.IsNullOrWhiteSpace(Parameters.Category) ? docs.Where(d => d.Category.Value.Equals(Parameters.Category, StringComparison.InvariantCultureIgnoreCase)) : docs;
            }
        }

        protected override long GetSearchResultTotal()
        {
            return 100;
        }
    }
}