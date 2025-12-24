using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Models;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace E_Commerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IHostEnvironment _environment;
        private readonly List<Product> _products = [];

        private static readonly JsonSerializerOptions JsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public ProductService(IHostEnvironment environment)
        {
            _environment = environment;
            LoadProducts();
        }

        #region Data Loading
        private void LoadProducts()
        {
            //var filePath = Path.Combine(
            //   _environment.ContentRootPath,
            //   "data",
            //   "featuredProduct.json");
            // Navigates up from the current project root into the Infrastructure folder
            var filePath = Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "..",
                "E-Commerce.Infrastructure",
                "Data",
                "featuredProduct.json"));

            if (!File.Exists(filePath))
                return;

            var json = File.ReadAllText(filePath);
            var products = JsonSerializer.Deserialize<List<Product>>(json, JsonOptions);

            if (products != null)
                _products.AddRange(products);
        }
        #endregion

        #region Get Products
        public Task<List<Product>> GetAllProductsAsync() => Task.FromResult(_products);
        public Task<Product?> GetProductByIdAsync(string id) => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        public Task<Product?> GetProductByIdAsync(string primary, string section, int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id.ToString());

            if (product == null)
                return Task.FromResult<Product?>(null);

            if (!string.Equals(product.Category?.Primary, primary, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult<Product?>(null);

            if (!string.IsNullOrEmpty(section) &&
                !string.Equals(product.Category?.Section, section, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult<Product?>(null);

            return Task.FromResult(product);
        }

        public Task<List<Product>> GetProductsByCategoryAsync(
              string primary,
              string? section = null,
              string? subcategory = null)
        {
            var query = _products.Where(p =>
                string.Equals(p.Category?.Primary, primary, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(section))
            {
                query = query.Where(p =>
                    string.Equals(p.Category?.Section, section, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(subcategory) && subcategory != "all")
            {
                query = query.Where(p =>
                    string.Equals(p.Category?.Subcategory, subcategory, StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult(query.ToList());
        }

        public Task<List<Product>> GetProductsByCollectionAsync(string collection)
        {
            var result = _products.Where(p =>
                p.Filters?.Collections?.Any(c =>
                    string.Equals(c, collection, StringComparison.OrdinalIgnoreCase)) == true)
                .ToList();

            return Task.FromResult(result);
        }
        #endregion

        #region Filters
        public Task<List<FilterDefinition>> GetFiltersForCategoryAsync(
            string primary,
            string section,
            string subcategory,
            List<Product> products)
        {
            var filters = new List<FilterDefinition>();

            switch (section?.ToLowerInvariant())
            {
                case "footwear":
                    AddFootwearFilters(filters, products);
                    break;

                case "apparel":
                    AddApparelFilters(filters, products);
                    break;

                case "accessories":
                    AddAccessoriesFilters(filters, products);
                    break;
            }

            AddCommonFilters(filters, products);

            return Task.FromResult(filters.Where(f => f.Options.Count != 0).ToList());
        }

        private static void AddFootwearFilters(List<FilterDefinition> filters, List<Product> products)
        {
            filters.Add(Create("style", "Style", products, p => p.Filters?.Style));
            filters.Add(Create("usaMade", "USA Made", products, p => p.Filters?.UsaMade == true ? "Yes" : null));
            filters.Add(Create("size", "Size", products, p => p.Filters?.Size));
            filters.Add(Create("width", "Width", products, p => p.Filters?.Width));
            filters.Add(Create("toeShape", "Toe Shape", products, p => p.Filters?.ToeShape));
            filters.Add(Create("color", "Color", products, p => p.Filters?.Color));
            filters.Add(Create("features", "Features", products, p => p.Filters?.Features));
        }

        private static void AddApparelFilters(List<FilterDefinition> filters, List<Product> products)
        {
            filters.Add(Create("productType", "Product Type", products, p => p.Filters?.ProductType));
            filters.Add(Create("size", "Size", products, p => p.Filters?.Size));
            filters.Add(Create("color", "Color", products, p => p.Filters?.Color));
            filters.Add(Create("fit", "Fit", products, p => p.Filters?.Fit));
        }

        private static void AddAccessoriesFilters(List<FilterDefinition> filters, List<Product> products)
        {
            filters.Add(Create("productType", "Product Type", products, p => p.Filters?.ProductType));
            filters.Add(Create("color", "Color", products, p => p.Filters?.Color));
        }

        private static void AddCommonFilters(List<FilterDefinition> filters, List<Product> products)
        {
            filters.Add(Create("priceRange", "Price Range", products, p => p.Filters?.PriceRange));
            filters.Add(Create("collections", "Collections", products, p => p.Filters?.Collections));
            filters.Add(Create("technology", "Technology", products, p => p.Filters?.Technology));
        }

        private static FilterDefinition Create(
            string name,
            string displayName,
            List<Product> products,
            Func<Product, object?> selector)
        {
            var counts = new Dictionary<string, int>();

            foreach (var product in products)
            {
                var value = selector(product);

                if (value is string str && !string.IsNullOrWhiteSpace(str))
                    Increment(counts, str);

                else if (value is IEnumerable<string> list)
                    foreach (var item in list.Where(i => !string.IsNullOrWhiteSpace(i)))
                        Increment(counts, item);
            }

            return new FilterDefinition
            {
                Name = name,
                DisplayName = displayName,
                Type = FilterType.Checkbox,
                Options = counts
                    .OrderBy(x => x.Key)
                    .Select(x => new FilterOption
                    {
                        Value = x.Key,
                        Label = x.Key,
                        Count = x.Value
                    })
                    .ToList()
            };
        }

        private static void Increment(Dictionary<string, int> dict, string key)
        {
            dict[key] = dict.TryGetValue(key, out var count) ? count + 1 : 1;
        }

        #endregion

        #region Sorting & Filtering

        public List<Product> ApplyFilters(
            List<Product> products,
            Dictionary<string, List<string>> filters)
        {
            if (filters == null || filters.Count == 0)
                return products;

            IEnumerable<Product> query = products;

            foreach (var (key, values) in filters)
            {
                if (values.Count == 0) continue;

                query = key.ToLowerInvariant() switch
                {
                    "style" => query.Where(p => values.Contains(p.Filters?.Style)),
                    "collections" => query.Where(p => p.Filters?.Collections?.Any(values.Contains) == true),
                    "technology" => query.Where(p => p.Filters?.Technology?.Any(values.Contains) == true),
                    _ => query
                };
            }

            return query.ToList();
        }

        public List<Product> SortProducts(List<Product> products, string? sortBy)
        {
            return sortBy?.ToLowerInvariant() switch
            {
                "price-low" => products.OrderBy(p => p.Price.Amount).ToList(),
                "price-high" => products.OrderByDescending(p => p.Price.Amount).ToList(),
                "name-asc" => products.OrderBy(p => p.Name).ToList(),
                "name-desc" => products.OrderByDescending(p => p.Name).ToList(),
                "newest" => products.OrderByDescending(p => p.Id).ToList(),
                _ => products
            };
        }
        #endregion
    }
}