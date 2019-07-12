using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetMVC_MOD.Models
{
    public class BookService
    {
        private string GetDBConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }



        // list field begin
        // class list
        public List<SelectListItem> GetClassTable()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BOOK_CLASS_ID AS CodeId,
                            BOOK_CLASS_NAME AS CodeText
                    FROM BOOK_CLASS
                    ORDER BY CodeText";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapDropDownListData(dt);
        }

        // keeper list
        /// <summary>
        /// false is normal list, 
        /// true is edit page list
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetKeeperTable(bool mode)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT [USER_ID] AS CodeId,
                                [USER_ENAME] AS CodeText,
                                [USER_CNAME] AS KeeperC
                            FROM MEMBER_M
                            ORDER BY CodeText";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            if (!mode) return this.MapDropDownListData(dt);
            else return this.MapEditKeeperData(dt);
        }

        // status list
        public List<SelectListItem> GetStatusTable()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT CODE_ID AS CodeId,
                            CODE_NAME AS CodeText
                    FROM BOOK_CODE
                    WHERE CODE_TYPE = 'BOOK_STATUS'";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapDropDownListData(dt);
        }

        // Map to Data
        private List<SelectListItem> MapDropDownListData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Text = "請選擇",
                Value = ""
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["CodeText"].ToString(),
                    Value = row["CodeId"].ToString()
                });
            }
            return result;
        }

        private List<SelectListItem> MapEditKeeperData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Text = "請選擇",
                Value = ""
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["CodeText"].ToString() + '-' + row["KeeperC"].ToString(),
                    Value = row["CodeId"].ToString()
                });
            }
            return result;
        }
        // list field end





        // Book Search
        public List<Models.Book> GetBookByCondtioin(Models.BookSearchArg arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BD.BOOK_CLASS_ID AS BookClassId,
		                            BCL.BOOK_CLASS_NAME AS BookClassName,
		                            BD.BOOK_ID AS BookId,
		                            BD.BOOK_NAME AS BookName,
		                            ISNULL(BD.BOOK_KEEPER, '') AS KeeperId,
		                            ISNULL(MM.USER_ENAME, '') AS Keeper,
		                            BD.BOOK_STATUS AS BookStatusId,
		                            BCO.CODE_NAME AS BookStatus,
		                            FORMAT(BD.BOOK_BOUGHT_DATE, 'yyyy/MM/dd') AS BuyDate

                            FROM BOOK_DATA AS BD
                            INNER JOIN BOOK_CLASS BCL ON BD.BOOK_CLASS_ID = BCL.BOOK_CLASS_ID
                            INNER JOIN BOOK_CODE AS BCO ON BD.BOOK_STATUS = BCO.CODE_ID AND BCO.CODE_TYPE = 'BOOK_STATUS'
                            LEFT JOIN MEMBER_M AS MM ON BD.BOOK_KEEPER = MM.[USER_ID]

                            WHERE BD.BOOK_NAME LIKE '%'+@BookName+'%' AND
		                            (BD.BOOK_CLASS_ID = @BookClassId OR '' = @BookClassId) AND
		                            (ISNULL(BD.BOOK_KEEPER, '') = @KeeperId OR '' = @KeeperId) AND
		                            (BD.BOOK_STATUS = @BookStatusId OR '' = @BookStatusId)
                            ORDER BY BuyDate DESC";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId == null ? string.Empty : arg.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@KeeperId", arg.KeeperId == null ? string.Empty : arg.KeeperId));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", arg.BookStatusId == null ? string.Empty : arg.BookStatusId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookData(dt);
        }

        private List<Models.Book> MapBookData(DataTable bookData)
        {
            List<Models.Book> result = new List<Book>();
            foreach (DataRow row in bookData.Rows)
            {
                result.Add(new Book()
                {
                    BookClassId = row["BookClassId"].ToString(),
                    BookClassName = row["BookClassName"].ToString(),
                    BookId = (int)row["BookID"],
                    BookName = row["BookName"].ToString(),
                    KeeperId = row["KeeperId"].ToString(),
                    Keeper = row["Keeper"].ToString(),
                    BookStatusId = row["BookStatusId"].ToString(),
                    BookStatus = row["BookStatus"].ToString(),
                    BuyDate = row["BuyDate"].ToString()
                });
            }
            return result;
        }





        // Record
        public List<Models.BookRecord> GetRecordByCondtioin(int bookId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT FORMAT(BLR.LEND_DATE, 'yyyy/MM/dd') AS LendDate,
		                            BLR.KEEPER_ID AS KeeperId,
		                            MM.USER_ENAME AS KeeperEName,
		                            MM.USER_CNAME as KeeperCName,
		                            BLR.BOOK_ID
                            FROM BOOK_LEND_RECORD AS BLR
                            INNER JOIN MEMBER_M AS MM ON BLR.KEEPER_ID = MM.[USER_ID]
                            WHERE BLR.BOOK_ID = @Id
                            ORDER BY LendDate DESC";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@Id", bookId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapRecordData(dt);
        }

        private List<Models.BookRecord> MapRecordData(DataTable dt)
        {
            List<Models.BookRecord> result = new List<BookRecord>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new BookRecord()
                {
                    LendDate = row["LendDate"].ToString(),
                    KeeperId = row["KeeperId"].ToString(),
                    KeeperEName = row["KeeperEName"].ToString(),
                    KeeperCName = row["KeeperCName"].ToString()
                });
            }
            return result;
        }





        // Delect Book
        public bool DeleteBook(int bookId)
        {
            DataTable dt = new DataTable();
            string sql = @"DELETE BOOK_DATA WHERE BOOK_ID = @BookId";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", bookId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return true;
        }





        // Insert Book
        public bool InsertBook(Models.Book arg)
        {
            DataTable dt = new DataTable();
            string sql = @"INSERT INTO BOOK_DATA (BOOK_NAME, BOOK_AUTHOR, BOOK_PUBLISHER, BOOK_NOTE, BOOK_BOUGHT_DATE, BOOK_CLASS_ID, BOOK_STATUS, CREATE_DATE, MODIFY_DATE)
		                         VALUES (@BookName, @Author, @Publisher, @Introduction, CONVERT(DATETIME, @BuyDate), @BookClassId, 'A', CONVERT(DATETIME, @CreateDate), CONVERT(DATETIME, @ModDate))";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@Author", arg.Author == null ? string.Empty : arg.Author));
                cmd.Parameters.Add(new SqlParameter("@Publisher", arg.Publisher == null ? string.Empty : arg.Publisher));
                cmd.Parameters.Add(new SqlParameter("@Introduction", arg.Introduction == null ? string.Empty : arg.Introduction));
                cmd.Parameters.Add(new SqlParameter("@BuyDate", arg.BuyDate == null ? string.Empty : arg.BuyDate));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId == null ? string.Empty : arg.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@CreateDate", arg.CreateDate == null ? string.Empty : arg.CreateDate));
                cmd.Parameters.Add(new SqlParameter("@ModDate", arg.ModDate == null ? string.Empty : arg.ModDate));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return true;
        }





        // Edit Book
        public Models.Book GetOriginData(int bookId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BOOK_ID AS BookId,
                                    BOOK_NAME AS BookName,
		                            BOOK_AUTHOR AS Author,
		                            BOOK_PUBLISHER AS Publisher,
		                            BOOK_NOTE AS Introduction,
		                            FORMAT(BOOK_BOUGHT_DATE, 'yyyy/MM/dd') AS BuyDate,
		                            BOOK_CLASS_ID AS BookClassId,
		                            BOOK_STATUS AS BookStatusId,
		                            BOOK_KEEPER AS KeeperId
                            FROM BOOK_DATA
                            WHERE BOOK_ID = @BookId";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", bookId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return this.MapEditData(dt);
        }

        private Models.Book MapEditData(DataTable dt)
        {
            Models.Book result = new Models.Book();

            result.BookId = (int)dt.Rows[0]["BookId"];
            result.BookName = dt.Rows[0]["BookName"].ToString();
            result.Author = dt.Rows[0]["Author"].ToString();
            result.Publisher = dt.Rows[0]["Publisher"].ToString();
            result.Introduction = dt.Rows[0]["Introduction"].ToString();
            result.BuyDate = dt.Rows[0]["BuyDate"].ToString();
            result.BookClassId = dt.Rows[0]["BookClassId"].ToString();
            result.BookStatusId = dt.Rows[0]["BookStatusId"].ToString();
            result.KeeperId = dt.Rows[0]["KeeperId"].ToString();

            return result;
        }

        public bool UpdateBookData(Models.Book arg)
        {
            DataTable dt = new DataTable();
            string sql = @"UPDATE BOOK_DATA
                            SET BOOK_NAME = @BookName,
	                            BOOK_AUTHOR = @Author,
	                            BOOK_PUBLISHER = @Publisher,
	                            BOOK_NOTE = @Introduction,
	                            BOOK_BOUGHT_DATE = CONVERT(DATETIME, @BuyDate),
	                            BOOK_CLASS_ID = @BookClassId,
	                            BOOK_STATUS = @BookStatusId,
	                            BOOK_KEEPER = @KeeperId,
                                MODIFY_DATE = CONVERT(DATETIME, @ModDate)
                            WHERE BOOK_ID = @BookId";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? "" : arg.BookName.ToString()));
                cmd.Parameters.Add(new SqlParameter("@Author", arg.Author == null ? "" : arg.Author.ToString()));
                cmd.Parameters.Add(new SqlParameter("@Publisher", arg.Publisher == null ? "" : arg.Publisher.ToString()));
                cmd.Parameters.Add(new SqlParameter("@Introduction", arg.Introduction == null ? "" : arg.Introduction.ToString()));
                cmd.Parameters.Add(new SqlParameter("@BuyDate", arg.BuyDate.ToString()));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId.ToString()));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", arg.BookStatusId.ToString()));
                cmd.Parameters.Add(new SqlParameter("@KeeperId", arg.KeeperId == null ? "" : arg.KeeperId.ToString()));
                cmd.Parameters.Add(new SqlParameter("@BookId", arg.BookId));
                cmd.Parameters.Add(new SqlParameter("@ModDate", arg.ModDate));
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return true;
        }





        // Auto Complete
        public List<string> BookNameKendoComplete()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BOOK_NAME AS BookName
                            FROM BOOK_DATA";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return this.MapAutoData(dt);
        }

        private List<string> MapAutoData(DataTable dt)
        {
            List<string> result = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(row["BookName"].ToString());
            }

            return result;
        }
    }
}