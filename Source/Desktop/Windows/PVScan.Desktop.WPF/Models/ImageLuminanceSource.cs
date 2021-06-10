using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace PVScan.Desktop.WPF.Models
{
    internal class ImageLuminanceSource : BaseLuminanceSource
    {
        public ImageLuminanceSource(Image<Bgr, byte> image)
           : base(image.Size.Width, image.Size.Height)
        {
            var bytes = image.Bytes;
            for (int indexB = 0, indexL = 0; indexB < bytes.Length; indexB += 3, indexL++)
            {
                var b = bytes[indexB];
                var g = bytes[indexB + 1];
                var r = bytes[indexB + 2];
                // Calculate luminance cheaply, favoring green.
                luminances[indexL] = (byte)((r + g + g + b) >> 2);
            }
        }

        protected ImageLuminanceSource(byte[] luminances, int width, int height)
           : base(luminances, width, height)
        {
        }

        protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
        {
            return new ImageLuminanceSource(newLuminances, width, height);
        }
    }
}
