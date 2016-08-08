using BookStoreApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BookStoreApplication.Controllers
{
    [RoutePrefix("api/Publisher")]
    public class PublisherController : ApiController
    {
        private readonly ApplicationDbContext context;

        public PublisherController()
        {
            context = new ApplicationDbContext();
        }

        //GET api/Publisher/
        #region Получить все существующие издательства
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetPublishers()
        {
            List<Publisher> publishers = context.Publishers.ToList();
            return Ok(publishers);
        }
        #endregion

        //POST api/Publisher/
        #region Добавить новое издательство
        [Authorize(Roles = "manager")]
        [HttpPost]
        public IHttpActionResult AddNewPublisher(Publisher model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.Publishers.Add(model);
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //DELETE api/Publisher/
        #region Удалить издательство
        [Authorize(Roles = "manager")]
        [HttpDelete]
        [Route("{idPublisher}")]
        public IHttpActionResult DeletePublisher(string idPublisher)
        {
            Publisher publisherDelete = context.Publishers.Find(idPublisher);
            if (publisherDelete == null)
            {
                return NotFound();
            }
            context.Publishers.Remove(publisherDelete);
            context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
