using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using PVScan.Mobile.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;

namespace PVScan.Mobile.Droid.Services
{
    public class MediaService : IMediaService
    {
        public async Task SaveSvgImage(SvgImageSource image, string fileName)
        {
            Stream outputStream = null;

            var renderer = new Xamarin.Forms.Svg.Droid.SvgImageSourceHandler();
            Bitmap photo = await renderer.LoadImageAsync(image, Platform.AppContext);

            bool success = false;
            using (outputStream = new FileStream(fileName, FileMode.Create))
            {
                if (System.IO.Path.GetExtension(fileName).ToLower() == ".png")
                    success = await photo.CompressAsync(Bitmap.CompressFormat.Png, 100, outputStream);
                else
                    success = await photo.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, outputStream);
            }
        }
    }
}
