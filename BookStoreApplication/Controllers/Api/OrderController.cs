using System.Linq;
using System.Web.Http;
using BookStoreApplication.Models;
using Microsoft.AspNet.Identity;

namespace BookStoreApplication.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private readonly ApplicationDbContext context;

        public OrderController()
        {
            context = new ApplicationDbContext();
        }

        //GET api/Order/
        #region Получить все заказы текущего пользователя
        [Authorize(Roles = "user")]
        [HttpGet]
        public IHttpActionResult GetOrders()
        {
            string userId = User.Identity.GetUserId();
            IQueryable<Order> orders = (from orderDetails in context.OrderDetails
                                        where orderDetails.ApplicationUserId == userId
                                        where orderDetails.Order != null
                                        select orderDetails.Order).Distinct();
                                
            return Ok(orders);
        }
        #endregion

        //GET api/Order/Uncorfirmed
        #region Получить все неподтвержденные заказы (для менеджера)
        [Authorize(Roles = "manager")]
        [HttpGet]
        [Route("Uncorfirmed")]
        public IHttpActionResult GetOrdersUncorfirmed()
        {
            IQueryable<Order> orders = from order in context.Orders
                                       where order.Confirmed == null
                                       select order;
            return Ok(orders);
        }
        #endregion

        //POST api/Order/
        #region Добавить новый заказ (сформировать заказ по текущим элементам корзины)
        [Authorize(Roles = "user")]
        [HttpPost]
        public IHttpActionResult AddNewOrder()
        {
            //Получить текущего пользователя
            string userId = User.Identity.GetUserId();
            //Находим все элементы корзины пользователя
            IQueryable<OrderDetails> orderDetailsList = from orderDetails in context.OrderDetails
                                                        where orderDetails.ApplicationUserId == userId
                                                        where orderDetails.Order == null
                                                        select orderDetails;
            if (orderDetailsList.Count() == 0)
            {
                return NotFound();
            }
            //Создаем новый заказ
            Order newOrder = new Order();
            //Добавляем книги из корзины в заказ
            foreach (OrderDetails orderDetails in orderDetailsList)
            {
                newOrder.OrderDetails.Add(orderDetails);
            }
            //Флаг проверки заказа - "обработка заказа"
            newOrder.Confirmed = null;
            context.Orders.Add(newOrder);
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //PUT api/Order/{idOrder}
        #region Подтвердить/отменить заказ по id
        [Authorize(Roles = "manager")]
        [HttpPut]
        [Route("{idOrder}")]
        public IHttpActionResult PutOrder(string idOrder, OrderUpdateModel model)
        {
            string userId = User.Identity.GetUserId();
            Order order = context.Orders.SingleOrDefault(x => x.Id == idOrder);
            if (order == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach(OrderDetails orderDetails in order.OrderDetails)
            {
                if (orderDetails.ApplicationUserId != userId)
                {
                    return BadRequest();
                }
            }
            order.Confirmed = model.Confirmed;
            context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
