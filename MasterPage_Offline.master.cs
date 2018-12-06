using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage_Offline : System.Web.UI.MasterPage
{
    public virtual String MyTitle
    {
        get { return "Admin Portal"; }
    }
    public virtual String PageTitle
    {
        get;
        set;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}
