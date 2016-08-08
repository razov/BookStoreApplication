using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BookStoreApplication.Models;

namespace BookStoreApplication.Controllers
{
    [RoutePrefix("api/Author")]
    public class AuthorController : ApiController
    {
        private readonly ApplicationDbContext context;

        public AuthorController()
        {
            context = new ApplicationDbContext();
        }

        //GET api/Account/Author/
        #region Получить список всех существующих авторов
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetAuthors()
        {
            List<Author> authors = context.Authors.ToList();
            return Ok(authors);
        }
        #endregion

        //POST api/Account/Author/
        #region Добавить нового автора
        [Authorize(Roles = "manager")]
        [HttpPost]
        public IHttpActionResult AddNewAuthor(Author model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.Authors.Add(model);
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //DELETE api/Account/Author/
        #region Удалить автора по id
        [Authorize(Roles = "manager")]
        [HttpDelete]
        [Route("{idAuthor}")]
        public IHttpActionResult DeleteAuthor(string idAuthor)
        {
            Author authorDelete = context.Authors.Find(idAuthor);
            if (authorDelete == null)
            {
                return NotFound();
            }
            context.Authors.Remove(authorDelete);
            context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
