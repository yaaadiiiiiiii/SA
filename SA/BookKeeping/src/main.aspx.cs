using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _BookKeeping
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 檢查用戶是否已登入
            if (Session["UserID"] == null)
            {
                // 如果沒有登入，重導向到登入頁面
                Response.Redirect("~/src/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 顯示歡迎訊息，使用 Session 中的用戶ID
                string userName = Session["UserName"].ToString();
                UserNameLabel.Text = userName;
            }
        }

        protected void AddRecipeBtn_Click(object sender, EventArgs e)
        {
            // 導向到新增食譜頁面
            Response.Redirect("~/src/add_recipe.aspx");
        }

        protected void BrowseRecipeBtn_Click(object sender, EventArgs e)
        {
            // 導向到瀏覽食譜頁面
            Response.Redirect("~/src/browse_recipe.aspx");
        }

        protected void FavoriteBtn_Click(object sender, EventArgs e)
        {
            // 導向到我的最愛頁面
            Response.Redirect("~/src/favorite.aspx");
        }

        protected void LogoutBtn_Click(object sender, EventArgs e)
        {
            // 清除 Session
            Session.Clear();
            Session.Abandon();

            // 重導向到登入頁面
            Response.Redirect("~/src/login.aspx");
        }
    }
}