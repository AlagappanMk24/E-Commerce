namespace E_Commerce.Domain.Models
{
    public class FilterRequest
    {
        public string Primary { get; set; }
        public string Section { get; set; }
        public string Subcategory { get; set; }
        public string Collection { get; set; }
        public Dictionary<string, List<string>> Filters { get; set; }
        public string SortBy { get; set; }
    }
}