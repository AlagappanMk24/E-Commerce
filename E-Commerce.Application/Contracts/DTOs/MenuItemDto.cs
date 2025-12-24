namespace E_Commerce.Application.Contracts.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsGroupHeader { get; set; }
        public List<MenuItemDto> Children { get; set; } = [];
    }
}
