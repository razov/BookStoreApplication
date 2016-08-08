using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApplication.Models
{
    // Модель элемента корзины
    public class OrderDetails
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Укажите идентификатор книги")]
        public string BookId { get; set; }

        [Required(ErrorMessage = "Укажите идентификатор пользователя")]
        public string ApplicationUserId { get; set; }

        public string OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public OrderDetails()
        {
            this.Id = System.Guid.NewGuid().ToString();
        }
    }

    // Модель для добавления новой книги в корзину
    public class OrderDetailsAddModel
    {
        [Required(ErrorMessage = "Укажите идентификатор книги")]
        public string IdBook { get; set; }
    }
}