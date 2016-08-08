using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApplication.Models
{
    // Модель автора книги
    public class Author
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [RegularExpression(@"^[A-ZА-Я][A-Za-zА-Яа-я ]+$", ErrorMessage = "Имя автора должно состоять только из букв и символа пробела")]
        [Required(ErrorMessage = "Укажите имя автора")]
        [StringLength(200)]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public Author()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Books = new List<Book>();
        }
    }
}