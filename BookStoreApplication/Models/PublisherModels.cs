using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStoreApplication.Models
{
    // Модель издателя
    public class Publisher
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [RegularExpression(@"^[A-ZА-Я][A-Za-zА-Яа-я ]+$", ErrorMessage = "Наименование издателя должно состоять только из букв и символа пробела")]
        [Required(ErrorMessage = "Укажите наименование издателя")]
        [StringLength(200)]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }

        public Publisher()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Books = new List<Book>();
        }
    }
}