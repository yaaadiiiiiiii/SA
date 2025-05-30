using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace _BookKeeping
{
    public partial class register : System.Web.UI.Page
    {
        protected DateTime BirthDate { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            string userid = RegAcc.Text;
            string nickname = RegNickname.Text;
            string password = RegPwd.Text;
            string confirmPassword = ReRegPwd.Text;
            string connectionStrings = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            // 检查是否有字段为空
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
            {
                // 有一个或多个字段为空，显示错误消息
                string script = "alert('請填寫所有欄位');";
                ClientScript.RegisterStartupScript(GetType(), "請填寫所有欄位", script, true);
                return; // 阻止注册流程
            }


            // 检查帐号只包含英文和数字
            if (ContainsNonChineseCharacters(userid) || userid.Contains(" "))
            {
                string script = "alert('帳號只能包含英文與數字，且不能有空白');";
                ClientScript.RegisterStartupScript(GetType(), "帳號格式錯誤", script, true);
                return;
            }

            if(password.Length < 6 ||password.Length > 10)
            {
                string script = "alert('密碼只能為 6~10 位');";
                ClientScript.RegisterStartupScript(GetType(), "密碼格式錯誤", script, true);
                return;

            }

            if (ContainsNonChineseCharacters(password) || password.Contains(" "))
            {
                // 帐号包含非英文或数字字符，显示错误消息
                string script = "var overlay = document.getElementById('overlay');";
                script += "overlay.style.display = 'block';"; // 顯示背景遮罩
                script += "var imageBox = document.createElement('img');";
                script += "imageBox.src = 'images/alert_pw_rule.png';";
                script += "imageBox.className = 'custom-image2';";
                script += "document.body.appendChild(imageBox);";
                script += "setTimeout(function() { overlay.style.display = 'none'; }, 2000);"; // 隱藏背景遮罩
                script += "setTimeout(function() { imageBox.style.display = 'none'; }, 2000);"; // 自动隐藏图像
                ClientScript.RegisterStartupScript(GetType(), "密碼只能含英文及數字", script, true);
                return; // 阻止注册流程
            }

            if (ContainsNonChineseCharacters(confirmPassword) || confirmPassword.Contains(" "))
            {
                string script = "alert('確認密碼只能包含英文與數字，且不得包含空白字元');";
                ClientScript.RegisterStartupScript(GetType(), "確認密碼格式錯誤", script, true);
                return;
            }


            if (password != confirmPassword)
            {
                string script = "alert('密碼與確認密碼不一致，請重新輸入');";
                ClientScript.RegisterStartupScript(GetType(), "確認密碼不一致", script, true);
                return;
            }


            if (IsUsernameAlreadyExists(userid))
            {
                string script = "alert('此帳號名稱已存在，請重新輸入');";
                ClientScript.RegisterStartupScript(GetType(), "帳號重複", script, true);
                return;
            }



            string sql = "INSERT INTO `sa`.user (user_id, user_name, password) VALUES (@user_id, @user_name, @password)";
            int rowsaffected = 0;

            using (MySqlConnection conn = new MySqlConnection(connectionStrings)) 
            {
                try
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userid);
                        cmd.Parameters.AddWithValue("@user_name", nickname);
                        cmd.Parameters.AddWithValue("@password", password);
                        rowsaffected = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    /// 將資料庫錯誤訊息顯示在頁面上
                    string errorMessage = $"資料庫錯誤：{ex.Message}";
                    ClientScript.RegisterStartupScript(GetType(), "DatabaseError", $"alert('{errorMessage}');", true);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                

            }
            // 彈出視窗
            if (rowsaffected > 0)
            {
                RegAcc.Text = "";
                RegNickname.Text = "";
                RegPwd.Text = "";
                string script = "alert('註冊成功！即將前往登入頁面');";
                script += "window.setTimeout(function() { window.location.href = '" + ResolveUrl("~/src/login.aspx") + "'; }, 10);";
                ClientScript.RegisterStartupScript(GetType(), "註冊成功", script, true);
            }
            else
            {
                string script = "alert('註冊失敗，請稍後再試');";
                ClientScript.RegisterStartupScript(GetType(), "註冊失敗", script, true);
            }

        }


        private bool IsUsernameAlreadyExists(string userid)
        {
            int count = 0;
            string connectionStrings = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            string sql = "SELECT COUNT(*) FROM `sa`.user WHERE user_id = @userid";
            using (MySqlConnection conn = new MySqlConnection(connectionStrings))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", userid);

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    /// 將資料庫錯誤訊息顯示在頁面上
                    string errorMessage = $"資料庫錯誤：{ex.Message}";
                    ClientScript.RegisterStartupScript(GetType(), "DatabaseError", $"alert('{errorMessage}');", true);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                
            }
            return count > 0;
        }

        private bool ContainsChineseCharacters(string input)
        {
            // 使用正則表達式檢查輸入是否包含中文字符
            string pattern = @"[\u4e00-\u9fa5]";
            return Regex.IsMatch(input, pattern);
        }

        private bool ContainsNonChineseCharacters(string input)
        {
            // 使用正则表达式检查输入是否包含非中文字符
            string pattern = @"[^\u4e00-\u9fa5]";
            return !Regex.IsMatch(input, pattern);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           
        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void TextBox4_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}