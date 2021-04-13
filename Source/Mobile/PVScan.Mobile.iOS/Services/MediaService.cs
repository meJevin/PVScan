using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using PVScan.Mobile.Services.Interfaces;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Svg;

namespace PVScan.Mobile.iOS.Services
{
    public class MediaService : IMediaService
    {
        public async Task SaveSvgImage(SvgImageSource image, string fileName)
        {
            NSData imgData = null;
            var renderer = new Xamarin.Forms.Svg.iOS.SvgImageSourceHandler();
            UIImage photo = await renderer.LoadImageAsync(image);

            if (Path.GetExtension(fileName).ToLower() == ".png")
                imgData = photo.AsPNG();
            else
                imgData = photo.AsJPEG(100);

            NSError err = null;
            imgData.Save(fileName, NSDataWritingOptions.Atomic, out err);
        }
    }
}
