namespace E_Commerce.Domain.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Brand { get; set; }
        public string StyleNumber { get; set; }
        public Price Price { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public List<string> Features { get; set; }
        public Media Media { get; set; }
        public List<Variant> Variants { get; set; }
        public ProductFilters Filters { get; set; }
        public List<string> image_urls { get; set; }
    }
    public class Price
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Formatted { get; set; }
        public int DiscountPercentage { get; set; }
    }
    public class Category
    {
        public string Primary { get; set; } // Men, Women, Kids, Work
        public string Section { get; set; } // Footwear, Apparel, Accessories
        public string Subcategory { get; set; } // Western, Work, Casuals, etc.
        public string Collection { get; set; }
    }
    public class Media
    {
        public string MainImage { get; set; }
        public string OnFootImage { get; set; }
    }
    public class Variant
    {
        public string Color { get; set; }
        public string ColorFamily { get; set; }
        public bool IsSelected { get; set; }
        public string StyleRef { get; set; }
        public string SwatchImage { get; set; }
    }
    public class ProductFilters
    {
        public string Style { get; set; }
        public bool UsaMade { get; set; }
        public string Size { get; set; }
        public string Width { get; set; }
        public string ToeShape { get; set; }
        public string Color { get; set; }
        public string VampColor { get; set; }
        public string UpperColor { get; set; }
        public string Skin { get; set; }
        public bool SafetyToe { get; set; }
        public List<string> Features { get; set; }
        public string Sole { get; set; }
        public string Heel { get; set; }
        public string BootHeight { get; set; }
        public string PriceRange { get; set; }
        public List<string> Collections { get; set; }
        public List<string> Technology { get; set; }

        // Additional filters for different categories
        public string Gender { get; set; } // For Kids
        public string AgeGroup { get; set; } // Junior, Infant
        public string ProductType { get; set; } // For Apparel/Accessories
        public string Fit { get; set; } // For Jeans
        public string Material { get; set; }
    }
    public class FilterDefinition
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public FilterType Type { get; set; }
        public List<FilterOption> Options { get; set; }
    }
    public class FilterOption
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public int Count { get; set; }
    }
    public enum FilterType
    {
        Checkbox,
        Radio,
        Range,
        MultiSelect
    }
    public class ProductViewodel
    {
        public Product Product { get; set; }
        public string CanonicalUrl { get; set; }
        public BreadcrumbInfo Breadcrumb { get; set; }

    }
    public class ProductListViewModel
    {
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public List<Product> Products { get; set; }
        public List<FilterDefinition> Filters { get; set; }
        public AppliedFilters AppliedFilters { get; set; }
        public int TotalResults { get; set; }
        public string SortBy { get; set; }
        public BreadcrumbInfo Breadcrumb { get; set; }
        public List<CategoryLink> CategoryLinks { get; set; }
    }
    public class BreadcrumbInfo
    {
        public string Primary { get; set; }
        public string Section { get; set; }
        public string Subcategory { get; set; }
        public string ProductSlug { get; set; }
    }
    public class CategoryLink
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
    }
    public class AppliedFilters
    {
        public Dictionary<string, List<string>> SelectedFilters { get; set; } = new Dictionary<string, List<string>>();
    }
}
