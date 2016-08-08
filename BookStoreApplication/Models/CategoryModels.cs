using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApplication.Models
{
    // Модель категории книги
    public class Category
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [RegularExpression(@"^[А-Яа-я]+$", ErrorMessage = "Название категории должно состоять только из букв")]
        [Required(ErrorMessage = "Укажите название категории")]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public Category()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Books = new List<Book>();
        }
    }
}