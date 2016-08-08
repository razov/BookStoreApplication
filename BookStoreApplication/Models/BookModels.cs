using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApplication.Models
{
    // Модель книги
    public class Book
    {
        public string Id { get; set; }

        [RegularExpression(@"^[A-Za-zА-Яа-я0-9 ]+$", ErrorMessage = "Наименование книги должно состоять только из букв, цифр и символа пробела")]
        [StringLength(250)]
        [Required(ErrorMessage = "Укажите наименование книги")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Укажите идентификатор автора")]
        public string AuthorId { get; set; }

        [Required(ErrorMessage = "Укажите идентификатор издателя")]
        public string PublisherId { get; set; }

        [Required(ErrorMessage = "Укажите идентификатор категории")]
        public string CategoryId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }

        [ForeignKey("PublisherId")]
        public virtual Publisher Publisher { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Required(ErrorMessage = "Укажите количество книг в наличии")]
        public int? Count { get; set; }

        [Required(ErrorMessage = "Укажите количество страниц")]
        public int? Page { get; set; }

        [Required(ErrorMessage = "Укажите стоимость книги")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Укажите ISBN")]
        public string Isbn { get; set; }

        [Required(ErrorMessage = "Укажите год издания книги")]
        public int? Year { get; set; }

        public string CoverPath { get; set; }

        public Book()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CoverPath = "/Content/Images/default.jpg";
        }
    }

    // Модель для обновления книги (количество книг)
    public class BookUpdateModel
    {
        [Required(ErrorMessage = "Укажите количество книг в наличии")]
        public int? Count { get; set; }

        // Добавить свойства при необходимости (прим. стоимость книги, обложка)
    }
}