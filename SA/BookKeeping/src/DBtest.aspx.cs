using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BookKeeping.src
{
    public partial class DBtest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    lblResult.Text = "✅ 成功連接到 MySQL 資料庫！";
                }
                catch (Exception ex)
                {
                    lblResult.Text = "❌ 連線失敗：" + ex.Message;
                    lblResult.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}