using E_Commerce.Application.Contracts.DTOs;

namespace E_Commerce.Application.Contracts.Services
{
    public interface IMenuService
    {
        Task<IReadOnlyList<MenuItemDto>> GetMenuItemsAsync();
    }
}