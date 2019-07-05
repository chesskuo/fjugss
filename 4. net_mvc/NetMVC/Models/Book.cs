using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVC.Models
{
    public class Book
    {
        [DisplayName("圖書類別")]
        public string BookClassName { get; set; }

        [DisplayName("圖書類別ID")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookClassId { get; set; }

        [DisplayName("書名")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookName { get; set; }

        [DisplayName("書本序列")]
        public int BookId { get; set; }

        [DisplayName("借閱狀態ID")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BookStatusId { get; set; }

        [DisplayName("借閱狀態")]
        public string BookStatus { get; set; }

        [DisplayName("借閱人")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Keeper { get; set; }

        [DisplayName("借閱人ID")]
        public string KeeperId { get; set; }

        [DisplayName("購書日期")]
        [Required(ErrorMessage = "此欄位必填")]
        public string BuyDate { get; set; }

        [DisplayName("出版商")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Publisher { get; set; }

        [DisplayName("作者")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Author { get; set; }

        [DisplayName("內容簡介")]
        [Required(ErrorMessage = "此欄位必填")]
        public string Introduction { get; set; }
    }
}