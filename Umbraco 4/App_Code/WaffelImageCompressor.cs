using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.media;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/// <summary>
/// Summary description for WaffelImageCompressor
/// </summary>

public class WaffelImageCompressor
{
    public static void compressImage(Media sender)
    {
        //Get settings file
        XmlDocument xmlDoc = new XmlDocument(); 
        xmlDoc.Load(HttpContext.Current.Server.MapPath("~/config/WaffelAutoCompress.config")); 
        var propertyAlias = xmlDoc.GetElementsByTagName("propertyalias")[0].InnerText;
        var mediaTypes = xmlDoc.GetElementsByTagName("mediatypes")[0].InnerText;
        bool allowedMediaType = mediaTypes.Split(',').Contains(sender.ContentType.Alias.ToString());

        //Check if settings allow current mediatype and if upload property has value
        if (allowedMediaType && sender.getProperty(propertyAlias).Value.ToString().Length != 0)
        {
            var triggerSize = xmlDoc.GetElementsByTagName("triggersize")[0].InnerText;
            var fileTypes = xmlDoc.GetElementsByTagName("filetypes")[0].InnerText;
            
            //Get uploaded file
            string targetFilePath = HttpContext.Current.Server.MapPath(sender.getProperty(propertyAlias).Value.ToString());
            var file = File.Open(targetFilePath, FileMode.Open, FileAccess.ReadWrite);
            string fileName = file.Name.ToString();
            var ext = fileName.Substring(fileName.LastIndexOf(".") + 1, fileName.Length - fileName.LastIndexOf(".") - 1).ToLower();

            int maxFileSize = System.Convert.ToInt32(triggerSize) * 1024; // if above image is downscaled
            bool allowedExt = fileTypes.Split(',').Contains(ext);

            //Check if file type is allowed and if size is above triggersize
            if (maxFileSize <= file.Length && allowedExt)
            {
                ImageDownscale(file, sender, ext);
            }
            else file.Dispose();
        }
    }

    private static void ImageDownscale(FileStream file, Media sender, string ext)
    {
        //Get settings
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(HttpContext.Current.Server.MapPath("~/config/WaffelAutoCompress.config"));
        var targetWidth = System.Convert.ToInt32(xmlDoc.GetElementsByTagName("targetwidth")[0].InnerText);
        var allowUpscale = xmlDoc.GetElementsByTagName("allowupscale")[0].InnerText;
        var propertyAlias = xmlDoc.GetElementsByTagName("propertyalias")[0].InnerText;
        var jpgQuality = xmlDoc.GetElementsByTagName("jpgquality")[0].InnerText;
        long compression = System.Convert.ToInt32(jpgQuality);
        string fullFilePath = HttpContext.Current.Server.MapPath(sender.getProperty(propertyAlias).Value.ToString());
        var fileNameWOExt = Path.GetFileNameWithoutExtension(file.Name.ToString());

        //Create new bitmap from uploaded file
        Bitmap originalBMP = new Bitmap(file);

        //Check if upscaling is allowed and if rule is relevant for current bitmap
        bool upscaleConflict = false;
        int longest = originalBMP.Width > originalBMP.Height ? originalBMP.Width : originalBMP.Height;
        if (allowUpscale != "true" && longest < targetWidth) upscaleConflict = true;

        //If no upscale conflict is found proceed with image scaling and compression.
        if (!upscaleConflict)
        {
            //Set correct width and height for scaled image
            int imgWidth, imgHeight;
            if (originalBMP.Width > originalBMP.Height)
            {
                imgWidth = targetWidth;
                imgHeight = originalBMP.Height * targetWidth / originalBMP.Width;
            }

            else
            {
                imgWidth = originalBMP.Width * targetWidth / originalBMP.Height;
                imgHeight = targetWidth;
            }

            //Create scaled bitmap
            Bitmap imgBMP = new Bitmap(originalBMP, imgWidth, imgHeight);
            Graphics iGraphics = Graphics.FromImage(imgBMP);
            if (ext == "png" || ext == "gif") iGraphics.Clear(Color.White); //Sets white background for transparent png and gif
            iGraphics.SmoothingMode = SmoothingMode.HighSpeed; iGraphics.InterpolationMode = InterpolationMode.Default;
            iGraphics.DrawImage(originalBMP, 0, 0, imgWidth, imgHeight);
            System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1]; //jpg
            System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
            eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression); //jpg compression

            file.Dispose();
            //Save new scaled and compressed jpg
            string targetDirectory = HttpContext.Current.Server.MapPath(string.Format("~/media/" + sender.getProperty(propertyAlias).Id.ToString() + "/"));
            imgBMP.Save(targetDirectory + fileNameWOExt + ".jpg", codec, eParams);
            imgBMP.Dispose();
            iGraphics.Dispose();
            originalBMP.Dispose();

            //Delete original file if not jpg
            if (ext != "jpg")
            {
                File.Delete(HttpContext.Current.Server.MapPath(sender.getProperty(propertyAlias).Value.ToString()));
            }

            //Populate upload property and standard Image properties if they exist
            sender.getProperty(propertyAlias).Value = "/media/" + sender.getProperty(propertyAlias).Id.ToString() + "/" + fileNameWOExt + ".jpg";
            System.IO.FileInfo fi = new FileInfo(fullFilePath);
            if (sender.getProperty("umbracoBytes") != null) sender.getProperty("umbracoBytes").Value = fi.Length.ToString();
            if (sender.getProperty("umbracoExtension") != null) sender.getProperty("umbracoExtension").Value = "jpg";
            if (sender.getProperty("umbracoWidth") != null) sender.getProperty("umbracoWidth").Value = imgWidth.ToString();
            if (sender.getProperty("umbracoHeight") != null) sender.getProperty("umbracoHeight").Value = imgHeight.ToString();
            
            
        }
        else
        {
            //If upscaleconflict is detected dispose and do nothing
            file.Dispose();
            originalBMP.Dispose();
        }
    }
    public WaffelImageCompressor()
    {
        //
        // TODO: Add constructor logic here
        //
    }

}