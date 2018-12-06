using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;

/// <summary>
/// Summary description for ghStaticClass
/// </summary>
public static class ghStaticClass
{

    public static Control GetParentOfType(this Control childControl, Type parentType)
    {
        Control parent = childControl.Parent;
        while (parent.GetType() != parentType)
        {
            parent = parent.Parent;
        }
        if (parent.GetType() == parentType)
            return parent;

        throw new Exception("No control of expected type was found");
    }
    public static Stream ToStream(this Image image, ImageFormat formaw)
    {
        var stream = new System.IO.MemoryStream();
        image.Save(stream, formaw);
        stream.Position = 0;
        return stream;
    }
    [Serializable]
    public sealed class recording_cache
    {
        public Int32 callid;
        public Int32 count;
        public DateTime lastupdate;
        public Boolean fetched = false;
        public String source = "";
    }
}