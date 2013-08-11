using Umbraco.Core;
using Umbraco.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.media;

/// <summary>
/// Summary description for WAC
/// </summary>

public class WAC : IApplicationEventHandler
{
    public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
    }
    public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
        //Media.AfterSave += Media_AfterSave;
        Media.BeforeSave += Media_BeforeSave;
    }

    void Media_BeforeSave(Media sender, SaveEventArgs e)
    {
        WaffelImageCompressor.compressImage(sender);
    }

    public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
        
    }

    void Media_AfterSave(Media sender, SaveEventArgs e)
    {
        WaffelImageCompressor.compressImage(sender);
    }

    public WAC()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}