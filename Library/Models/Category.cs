using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Category
    {
        public Category()
        {
            Books = new HashSet<Book>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string Name { get; set; }

        public string? Description { get; set; }
        public DateTime DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
