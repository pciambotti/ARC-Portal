using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;




public partial class _Default : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        try
        {
            Master.PageTitle = "Home";
            this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
        }
        catch { }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        String strRedirect = "~/Dashboard.aspx";
        if (Page.User.IsInRole("CDR Fundraising Group")) { strRedirect = "~/Search.aspx"; }
        //Response.Redirect("~/dashboard.aspx");
        Response.Redirect(strRedirect, false);
    }

}
