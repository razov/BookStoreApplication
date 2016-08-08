using BookStoreApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BookStoreApplication.Controllers
{
    [RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        private readonly ApplicationDbContext context;

        public CategoryController()
        {
            context = new ApplicationDbContext();
        }

        //GET api/Category/
        #region Получить все существующие категории книг
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetCategories()
        {
            List<Category> categories = context.Categories.ToList();
            return Ok(categories);
        }
        #endregion

        //POST api/Category/
        #region Добавить новую категорию
        [Authorize(Roles = "manager")]
        [HttpPost]
        public IHttpActionResult AddNewCategory(Category model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.Categories.Add(model);
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //DELETE api/Category/{idCategory}
        #region Удалить категорию книги по id категории
        [Authorize(Roles = "manager")]
        [HttpDelete]
        [Route("{idCategory}")]
        public IHttpActionResult DeleteCategory(string idCategory)
        {
            Category categoryDelete = context.Categories.Find(idCategory);
            if (categoryDelete == null)
            {
                return NotFound();
            }
            context.Categories.Remove(categoryDelete);
            context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
