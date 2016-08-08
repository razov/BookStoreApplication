using System.Web.Mvc;

namespace BookStoreApplication.Controllers
{
    public class StoreController : Controller
    {
        #region Отправить пользователю представление главной страницы приложения вместе с клиентской частью приложения
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}