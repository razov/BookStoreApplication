using BookStoreApplication.Models;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace BookStoreApplication.Controllers
{
    [RoutePrefix("api/OrderDetails")]
    public class OrderDetailsController : ApiController
    {
        private readonly ApplicationDbContext context;

        public OrderDetailsController()
        {
            context = new ApplicationDbContext();
        }

        //GET api/OrderDetails/
        #region Получить элементы корзины пользователя
        [Authorize(Roles = "user")]
        [HttpGet]
        public IHttpActionResult GetOrderDetails()
        {
            string userId = User.Identity.GetUserId();
            IQueryable<OrderDetails> orderDetails = from item in context.OrderDetails
                                                    where item.ApplicationUserId == userId
                                                    where item.Order == null
                                                    select item;
            return Ok(orderDetails);
        }
        #endregion

        //POST api/OrderDetails/
        #region Добавить книгу в корзину
        [Authorize(Roles = "user")]
        [HttpPost]
        public IHttpActionResult AddNewOrderDetails(OrderDetailsAddModel model)
        {
            // Создаем менеджер по работе с пользователями
            var userManager = new ApplicationUserManager(new ApplicationUserStore(context));

            // Ищем необходимую книгу по id
            Book book = context.Books.SingleOrDefault(x => x.Id == model.IdBook);
            if (book == null)
            {
                ModelState.AddModelError("model.IdBook", "Книга с данным идентификатором отсутствует");
            }
            // Книга должна быть на складе
            else if (book.Count == 0) 
            {
                ModelState.AddModelError("model.IdBook", "Данная книга отсутствует на складе");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Формируем новый элемент корзины
            context.OrderDetails.Add(new OrderDetails()
            {
                // Добавляем книгу
                Book = context.Books.SingleOrDefault(x => x.Id == model.IdBook),
                BookId = context.Books.SingleOrDefault(x => x.Id == model.IdBook).Id,
                // Получаем текущего пользователя (прим. клиента магазина)
                ApplicationUserId = User.Identity.GetUserId(),
                ApplicationUser = userManager.FindById(User.Identity.GetUserId())
            });
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //DELETE api/OrderDetails/
        #region Удалить книгу из корзины
        [Authorize(Roles = "user")]
        [HttpDelete]
        [Route("{idOrderDetails}")]
        public IHttpActionResult DeleteOrderDetails(string idOrderDetails)
        {
            OrderDetails orderDetails = context.OrderDetails.SingleOrDefault(x => x.Id == idOrderDetails);
            if (orderDetails == null)
            {
                return NotFound();
            }
            context.OrderDetails.Remove(orderDetails);
            context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
