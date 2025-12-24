using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Domain.Entities
{
    [Table("MenuItems")]
    public class MenuItem
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int? ParentId { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }  

        [MaxLength(255)]
        public string? Url { get; set; } 
        public int SortOrder { get; set; }

        [MaxLength(50)]
        public string? CssClass { get; set; } 
        public bool IsGroupHeader { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; } 

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual MenuItem? Parent { get; set; }
        public virtual ICollection<MenuItem> Children { get; set; } = [];
    }
}