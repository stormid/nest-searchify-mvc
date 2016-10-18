using Nest.Searchify;

namespace SearchifyMvcSample.Code
{
    public class SampleDocument
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public FilterField Category { get; set; }
    }
}