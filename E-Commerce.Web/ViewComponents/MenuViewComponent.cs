using E_Commerce.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.ViewComponents
{
    public class MenuViewComponent(IMenuService menuService) : ViewComponent
    {
        private readonly IMenuService _menuService = menuService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menuItems = await _menuService.GetMenuItemsAsync();
            return View(menuItems);
        }
    }
}
