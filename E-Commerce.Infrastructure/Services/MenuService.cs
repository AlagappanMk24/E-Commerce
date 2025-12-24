using AutoMapper;
using E_Commerce.Application.Contracts.DTOs;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Infrastructure.Data.Context;

namespace E_Commerce.Infrastructure.Services
{
    public class MenuService(EcomDbContext context, IMapper mapper) : IMenuService
    {
        private readonly EcomDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<IReadOnlyList<MenuItemDto>> GetMenuItemsAsync()
        {
            var items = await _context.MenuItems
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var lookup = items.ToLookup(x => x.ParentId);

            return lookup[null]
                .Select(item => BuildTree(item, lookup))
                .ToList();
        }

        private MenuItemDto BuildTree(MenuItem item, ILookup<int?, MenuItem> lookup)
        {
            var dto = _mapper.Map<MenuItemDto>(item);

            dto.Children = lookup[item.Id]
                .Select(child => BuildTree(child, lookup))
                .ToList();

            return dto;
        }
    }
}