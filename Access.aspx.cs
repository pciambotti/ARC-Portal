using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Access : System.Web.UI.Page
{
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        Master.PageTitle = "Access Restricted";
        this.Title = String.Format("{0}: {1}", Master.MyTitle, Page.Title);
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
