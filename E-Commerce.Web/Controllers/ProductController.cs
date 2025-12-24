using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Controllers
{
    public class ProductController(IProductService productService, IWebHostEnvironment env) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly IWebHostEnvironment _env = env;

        [HttpGet("{primary}/{section}/{slug}/{id}")]
        public async Task<IActionResult> Details(string primary, string section, string slug, int id)
        {
            var product = await _productService.GetProductByIdAsync(primary, section, id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewodel
            {
                Product = product,
                CanonicalUrl = "",
                Breadcrumb = new BreadcrumbInfo
                {
                    Primary = primary,
                    Section = section,
                    ProductSlug = slug
                },
            };

            return View("Details", viewModel);
        }

        // Generic route handler for all category pages
        [HttpGet("{primary}/{section?}/{subcategory?}")]
        public async Task<IActionResult> CategoryPage(string primary, string section = null, string subcategory = null)
        {
            var products = await _productService.GetProductsByCategoryAsync(primary, section, subcategory);
            var filters = await _productService.GetFiltersForCategoryAsync(primary, section, subcategory, products);
            var categoryLinks = GetCategoryLinks(primary, section);

            var viewModel = new ProductListViewModel
            {
                PageTitle = GetPageTitle(primary, section, subcategory),
                PageDescription = GetPageDescription(primary, section, subcategory),
                Products = products,
                Filters = filters,
                TotalResults = products.Count,
                SortBy = "best-matches",
                Breadcrumb = new BreadcrumbInfo
                {
                    Primary = primary,
                    Section = section,
                    Subcategory = subcategory
                },
                CategoryLinks = categoryLinks
            };

            return View("ProductList", viewModel);
        }

        // Collection pages
        [HttpGet("collections/{collectionName}")]
        public async Task<IActionResult> Collection(string collectionName)
        {
            var products = await _productService.GetProductsByCollectionAsync(collectionName);

            // Determine primary category for filter context
            var primary = products.FirstOrDefault()?.Category.Primary ?? "Men";
            var section = products.FirstOrDefault()?.Category.Section ?? "Footwear";

            var filters = await _productService.GetFiltersForCategoryAsync(primary, section, null, products);

            var viewModel = new ProductListViewModel
            {
                PageTitle = $"{FormatCollectionName(collectionName)} Collection",
                PageDescription = $"Explore our {FormatCollectionName(collectionName)} collection.",
                Products = products,
                Filters = filters,
                TotalResults = products.Count,
                SortBy = "best-matches",
                Breadcrumb = new BreadcrumbInfo
                {
                    Primary = "Collections",
                    Section = collectionName
                }
            };

            return View("ProductList", viewModel);
        }

        // AJAX filter endpoint
        [HttpPost("api/products/filter")]
        public async Task<IActionResult> FilterProducts([FromBody] FilterRequest request)
        {
            List<Product> products;

            if (!string.IsNullOrEmpty(request.Collection))
            {
                products = await _productService.GetProductsByCollectionAsync(request.Collection);
            }
            else
            {
                products = await _productService.GetProductsByCategoryAsync(
                    request.Primary,
                    request.Section,
                    request.Subcategory
                );
            }

            // Apply filters
            products = _productService.ApplyFilters(products, request.Filters);

            // Sort products
            products = _productService.SortProducts(products, request.SortBy);

            // Get updated filters with counts
            var filters = await _productService.GetFiltersForCategoryAsync(
                request.Primary,
                request.Section,
                request.Subcategory,
                products
            );

            return Json(new
            {
                products,
                filters,
                totalResults = products.Count
            });
        }

        private List<CategoryLink> GetCategoryLinks(string primary, string section)
        {
            var links = new List<CategoryLink>();

            if (primary?.ToLower() == "men" && section?.ToLower() == "footwear")
            {
                links.Add(new CategoryLink { Title = "Western", Url = "/men/footwear/western" });
                links.Add(new CategoryLink { Title = "Work", Url = "/men/footwear/work" });
                links.Add(new CategoryLink { Title = "Casuals", Url = "/men/footwear/casuals" });
            }
            else if (primary?.ToLower() == "women" && section?.ToLower() == "footwear")
            {
                links.Add(new CategoryLink { Title = "Western", Url = "/women/footwear/western" });
                links.Add(new CategoryLink { Title = "Work", Url = "/women/footwear/work" });
                links.Add(new CategoryLink { Title = "Casuals", Url = "/women/footwear/casuals" });
            }
            else if (primary?.ToLower() == "work")
            {
                links.Add(new CategoryLink { Title = "Pull-On", Url = "/work/pull-on" });
                links.Add(new CategoryLink { Title = "Lace-Up", Url = "/work/lace-up" });
                links.Add(new CategoryLink { Title = "Safety Toe", Url = "/work/safety-toe" });
            }

            return links;
        }

        private string GetPageTitle(string primary, string section, string subcategory)
        {
            if (!string.IsNullOrEmpty(subcategory))
                return $"{primary?.ToUpper()} {subcategory?.ToUpper()} {section?.ToUpper()}";
            if (!string.IsNullOrEmpty(section))
                return $"{primary?.ToUpper()} {section?.ToUpper()}";
            return primary?.ToUpper() ?? "PRODUCTS";
        }

        private string GetPageDescription(string primary, string section, string subcategory)
        {
            return $"Browse our {primary?.ToLower()} {section?.ToLower()} collection.";
        }

        private string FormatCollectionName(string name)
        {
            return name.Replace("-", " ").ToUpper();
        }
    }
}