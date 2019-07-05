using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NetMVC_MOD.Models
{
    public class BookRecord
    {
        [DisplayName("借閱日期")]
        public string LendDate { get; set; }

        [DisplayName("借閱人編號")]
        public string KeeperId { get; set; }

        [DisplayName("英文姓名")]
        public string KeeperEName { get; set; }

        [DisplayName("中文姓名")]
        public string KeeperCName { get; set; }
    }
}