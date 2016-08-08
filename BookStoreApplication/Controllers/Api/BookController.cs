using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using BookStoreApplication.Models;
using System.Threading.Tasks;

namespace BookStoreApplication.Controllers
{
    [RoutePrefix("api/Book")]
    public class BookController : ApiController
    {
        private readonly ApplicationDbContext context;

        public BookController()
        {
            context = new ApplicationDbContext();
        }

        //GET api/Books/

        // offset - индекс первого элемента из отсортированного списка 
        // limit - необходимая длина списка
        // sort - вид сортировки (по названию name, по году year, по стоимости price). name по умолчанию
        // category - фильтр категории
        // author - фильтр автора
        // publisher - фильтр издателя
        // name - фильтр наименования
        #region Получить список книг по заданным параметрам
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetBooks(int offset = 0, int limit = 12, string sort = "name", string category = null, string author = null, string publisher = null, string name = null)
        {
            IQueryable<Book> books = from book in context.Books
                                     select book;
            if (name != null)
            {
                books = from book in books
                        where book.Name == name
                        select book;
            }

            if (category != null)
            {
                books = from book in books
                        where book.Category.Name == category
                        select book;
            }

            if (author != null)
            {
                books = from book in books
                        where book.Author.Name == author
                        select book;
            }

            if (publisher != null)
            {
                books = from book in books
                        where book.Publisher.Name == publisher
                        select book;
            }

            int lengthBooksList = books.Count();

            List<Book> listBook;

            switch (sort)
            {
                case "name":
                    listBook = books.OrderBy(x => x.Name).Skip(offset).Take(limit).ToList();
                    break;
                case "year":
                    listBook = books.OrderBy(x => x.Year).Skip(offset).Take(limit).ToList();
                    break;
                case "price":
                    listBook = books.OrderBy(x => x.Price).Skip(offset).Take(limit).ToList();
                    break;
                default:
                    listBook = books.OrderBy(x => x.Name).Skip(offset).Take(limit).ToList();
                    break;
            }

            return Ok(new
            {
                data = listBook,
                paging = new
                {
                    isBack = (offset > 0) ? true : false,
                    isNext = (lengthBooksList > offset + limit) ? true : false
                }
            });
        }
        #endregion

        //GET api/Books/{idBook}
        #region Получить данные книги по id
        [HttpGet]
        [AllowAnonymous]
        [Route("{idBook}")]
        public IHttpActionResult GetBook(string idBook)
        {
            Book book = context.Books.SingleOrDefault(x => x.Id == idBook);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        #endregion

        //POST api/Books/
        #region Добавить новую книгу
        [Authorize(Roles = "manager")]
        [HttpPost]
        public IHttpActionResult AddNewBook(Book model)
        {
            Publisher publisher = context.Publishers.SingleOrDefault(x => x.Id == model.PublisherId);
            if (publisher == null)
            {
                ModelState.AddModelError("model.PublisherId", "Издательство с данным идентификатором отсутствует");
            }
            Category category = context.Categories.SingleOrDefault(x => x.Id == model.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("model.CategoryId", "Категория с данным идентификатором отсутствует");
            }
            Author author = context.Authors.SingleOrDefault(x => x.Id == model.AuthorId);
            if (author == null)
            {
                ModelState.AddModelError("model.AuthorId", "Автор с данным идентификатором отсутствует");
            }
            if (model.Year > DateTime.Now.Year)
            {
                ModelState.AddModelError("model.Year", "Год издания книги указан не корректно");
            }
            if (model.Page <= 0)
            {
                ModelState.AddModelError("model.Page", "Количество страниц книги должно быть больше нуля");
            }
            if (model.Price <= 0)
            {
                ModelState.AddModelError("model.Price", "Стоимость книги должна быть больше нуля");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.Author = author;
            model.Category = category;
            model.Publisher = publisher;
            context.Books.Add(model);
            context.SaveChanges();
            return Ok(model);
        }
        #endregion

        //POST api/Books/CoverUpload/{idBook}
        // Название изображения обложки равно идентификатору книги
        #region Загрузка обложки для книги с id = idBook вместо default img
        [Authorize(Roles = "manager")]
        [HttpPost]
        [Route("CoverUpload/{idBook}")]
        public async Task<IHttpActionResult> Post(string idBook)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }
            Book book = context.Books.SingleOrDefault(x => x.Id == idBook);
            if (book == null)
            {
                return NotFound();
            }
            if (book.CoverPath != "/Content/Images/default.jpg")
            {
                return BadRequest();
            }
            var provider = new MultipartMemoryStreamProvider();
            string imagesPath = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/");
            await Request.Content.ReadAsMultipartAsync(provider);
            string newFileName = null;

            foreach (var file in provider.Contents)
            {
                string oldFileName = file.Headers.ContentDisposition.FileName;
                string typeFile = oldFileName.Substring(oldFileName.IndexOf('.'), oldFileName.Length - 1 - oldFileName.IndexOf('.'));
                newFileName = book.Id + typeFile;
                byte[] fileArray = await file.ReadAsByteArrayAsync();

                using (System.IO.FileStream fs = new System.IO.FileStream(imagesPath + newFileName, System.IO.FileMode.Create))
                {
                    await fs.WriteAsync(fileArray, 0, fileArray.Length);
                }
            }
            book.CoverPath = "/Content/Images/" + newFileName;
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //DELETE api/Books/{idBook}
        #region Удалить книгу по id
        [Authorize(Roles = "manager")]
        [HttpDelete]
        [Route("{idBook}")]
        public IHttpActionResult DeleteBook(string idBook)
        {
            Book bookDelete = context.Books.Find(idBook);
            if (bookDelete == null)
            {
                return NotFound();
            }
            context.Books.Remove(bookDelete);
            context.SaveChanges();
            return Ok();
        }
        #endregion

        //PUT api/Books/{idBook}
        #region Обновить количество книги с id = idBook на складе 
        [Authorize(Roles = "manager")]
        [HttpPut]
        [Route("{idBook}")]
        public IHttpActionResult PutBook(string idBook, BookUpdateModel model)
        {
            Book bookUpdate = context.Books.Find(idBook);
            if (bookUpdate == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bookUpdate.Count = model.Count;
            // Добавить изменяемые свойства при необходимости (ред. Models/BookModel.cs)
            context.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
