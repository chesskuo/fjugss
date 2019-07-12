using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetMVC_MOD.Controllers
{
    public class LibraryController : Controller
    {
        readonly Models.BookService bookService = new Models.BookService();

        // GET: Library
        public ActionResult Index()
        {
            return View();
        }



        // Auto Complete
        [HttpPost]
        public JsonResult GetBookName()
        {
            return Json(this.bookService.BookNameKendoComplete());
        }



        // Drop Down List field
        [HttpPost]
        public JsonResult GetDropDownList(string type, bool mode)
        {
            switch (type)
            {
                case "class":
                    return Json(this.bookService.GetClassTable());
                case "keeper":
                    return Json(this.bookService.GetKeeperTable(mode));
                case "status":
                    return Json(this.bookService.GetStatusTable());
                default:
                    return Json(true);
            }
        }



        [HttpPost]
        public JsonResult GetBook(Models.BookSearchArg arg)
        {
            return Json(this.bookService.GetBookByCondtioin(arg));
        }



        [HttpPost]
        public JsonResult GetRecord(int bookId)
        {
            return Json(this.bookService.GetRecordByCondtioin(bookId));
        }



        [HttpPost]
        public JsonResult DeleteBook(int bookId)
        {
            return Json(this.bookService.DeleteBook(bookId));
        }



        // Insert Book
        [HttpGet]
        public ActionResult InsertBookPage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult InsertBook(Models.Book arg)
        {
            if (arg.BookName == null || arg.Author == null || arg.Publisher == null || arg.Introduction == null || arg.BuyDate == null || arg.BookClassId == null)
            {
                return Json(false);
            }

            return Json(this.bookService.InsertBook(arg));
        }



        // Edit Book
        [HttpPost]
        public ActionResult EditBookPage(int bookId)
        {
            ViewData["BookId"] = bookId;
            return View();
        }

        [HttpPost]
        public JsonResult EditBookInfo(int bookId)
        {
            return Json(this.bookService.GetOriginData(bookId));
        }

        [HttpPost]
        public JsonResult UpdateBook(Models.Book arg)
        {
            return Json(this.bookService.UpdateBookData(arg));
        }
    }
}