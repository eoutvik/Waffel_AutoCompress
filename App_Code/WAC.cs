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
    public void OnApplicationInitialized(UmbracoApplication httpApplication, ApplicationContext applicationContext)
    {
    }

    public void OnApplicationStarting(UmbracoApplication httpApplication, ApplicationContext applicationContext)
    {
        Media.AfterSave += Media_AfterSave;
    }

    public void OnApplicationStarted(UmbracoApplication httpApplication, ApplicationContext applicationContext)
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