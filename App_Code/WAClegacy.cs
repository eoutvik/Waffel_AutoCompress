using System;
using System.Collections.Generic;
using System.Text;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.web;

/// <summary>
/// Summary description for WAClegacy
/// </summary>
public class WAClegacy : ApplicationBase
{
    public WAClegacy()
    {
        //Media.AfterSave += Media_AfterSave;
    }

    private void Media_AfterSave(Media sender, SaveEventArgs e)
    {
        WaffelImageCompressor.compressImage(sender);
    }
}