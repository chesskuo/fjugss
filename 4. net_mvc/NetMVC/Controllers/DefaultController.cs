using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetMVC.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        Models.BookService bookService = new Models.BookService();
        public ActionResult Index()
        {
            ViewBag.BookClassData = this.bookService.GetClassTable("");
            ViewBag.KeeperData = this.bookService.GetKeeperTable("", 0);
            ViewBag.StatusData = this.bookService.GetStatusTable("");
            return View();
        }

        [HttpPost()]
        public ActionResult Index(Models.BookSearch arg)
        {
            ViewBag.BookClassData = this.bookService.GetClassTable("");
            ViewBag.KeeperData = this.bookService.GetKeeperTable("", 0);
            ViewBag.StatusData = this.bookService.GetStatusTable("");
            ViewBag.SearchResult = this.bookService.GetBookByCondition(arg);
            return View();
        }



        // Insert Book Page
        [HttpGet()]
        public ActionResult InsertBook()
        {
            ViewBag.BookClassData = this.bookService.GetClassTable("");
            ViewBag.InsertResult = false;
            return View("InsertBook");
        }

        [HttpPost()]
        public ActionResult InsertBook(Models.Book arg)
        {
            ViewBag.BookClassData = this.bookService.GetClassTable("");
            if (arg.BookName == null || arg.Author == null || arg.Publisher == null || arg.Introduction == null || arg.BuyDate == null || arg.BookClassId == null)
            {
                ViewBag.InsertResult = false;
                return View("InsertBook");
            }
            else
            {
                ViewBag.InsertResult = bookService.InsertBookInfo(arg);
                return View("InsertBook");
            }
        }



        // Delete Book
        [HttpPost()]
        public JsonResult DeleteBook(int BookId)
        {
            bookService.DeleteBook(BookId);
            return this.Json(true);
        }



        // Book Record Page
        [HttpPost()]
        public ActionResult Record(int BookId)
        {
            ViewBag.RecordResult = this.bookService.GetRecordByCondition(BookId);
            return View("Record");
        }



        // Book Edit Page
        [HttpPost]
        public ActionResult EditBook(int BookId)
        {
            ViewBag.CheckPoint = false;
            ViewBag.EditResult = this.bookService.GetOriginData(BookId);
            ViewBag.BookClassData = this.bookService.GetClassTable(ViewBag.EditResult.BookClassId);
            ViewBag.StatusData = this.bookService.GetStatusTable(ViewBag.EditResult.BookStatusId);
            ViewBag.KeeperData = this.bookService.GetKeeperTable(ViewBag.EditResult.KeeperId, 1);
            return View("EditBook");
        }

        [HttpPost()]
        public ActionResult UpdateBook(Models.Book arg)
        {
            bookService.UpdateBookData(arg);
            ViewBag.CheckPoint = true;
            return View("UpdateBook");
        }
    }
}