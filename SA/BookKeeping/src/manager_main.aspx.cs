using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _BookKeeping
{
    public partial class manager_main : System.Web.UI.Page
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

        protected void RecipeManageBtn_Click(object sender, EventArgs e)
        {
            // 導向到食譜管理頁面
            Response.Redirect("~/src/recipe_manager.aspx");
        }

        protected void FoodManageBtn_Click(Object sender, EventArgs e)
        {
            // 導向到食材管理頁面
            Response.Redirect("~/src/ingredient_manager.aspx"); 
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