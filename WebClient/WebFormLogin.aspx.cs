using System;
using System.Web;

namespace WebClient
{
    public partial class WebFormLogin : System.Web.UI.Page
    {
        private string username;
        private string email;
        private string phone;


        protected void Page_Load(object sender, EventArgs e)
        {

        }

      

        protected void TextBoxName_TextChanged(object sender, EventArgs e)
        {
            this.username = TextBoxName.Text;
            ViewState["username"] = this.username;
        }

        protected void TextBoxEmail_TextChanged(object sender, EventArgs e)
        {
            this.email = TextBoxEmail.Text;
            ViewState["email"] = this.email;
        }

        protected void TextBoxPhone_TextChanged(object sender, EventArgs e)
        {
            this.phone = TextBoxPhone.Text;
            ViewState["phone"] = this.phone;
        }

      
        protected void ButtonSubmit_Click1(object sender, EventArgs e)
        {
            if (ViewState["username"] != null && ViewState["email"] != null && ViewState["phone"] != null)
            {
                HttpCookie myCookieobj = new HttpCookie("shiva");
                myCookieobj.Value = ViewState["username"].ToString() + '|' + ViewState["email"].ToString() + '|' + ViewState["phone"].ToString();
                myCookieobj.Expires = DateTime.Now.Add(TimeSpan.FromDays(500));
                Response.Cookies.Add(myCookieobj);
                Response.Redirect("WebFormClient.aspx");
            }
            else
                Response.Write("Моля попълнете всички полета за да излизат във формите, които генерирате. Регистрацията е еднократна");
        }
    }
}