using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApplication.Models
{
    // Модель заказа
    public class Order
    {
        public string Id { get; set; }
        
        public bool? Confirmed { get; set; }

        public virtual List<OrderDetails> OrderDetails { get; set; }

        public Order()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.OrderDetails = new List<OrderDetails>();
        }
    }

    // Модель для обновления заказа менеджером (флаг заказа)
    public class OrderUpdateModel
    {
        [Required(ErrorMessage = "Укажите флаг подтверждения заказа")]
        public bool Confirmed { get; set; }

        // Добавить дополнительные свойства при необходимости
    }
}