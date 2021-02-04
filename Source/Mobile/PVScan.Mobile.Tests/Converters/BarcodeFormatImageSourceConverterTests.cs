using PVScan.Mobile.Converters;
using PVScan.Mobile.Styles;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xunit;
using ZXing;

namespace PVScan.Mobile.Tests.Converters
{
    public class BarcodeFormatImageSourceConverterTests : TestBase
    {
        [Fact]
        public void Can_Convert_QR_Code_To_QR_Code_Font_Image()
        {
            // Arrange
            var converter = new BarcodeFormatImageSourceConverter();

            // Act
            var result = converter.Convert(BarcodeFormat.QR_CODE, null, null, null) as FontImageSource;

            // Assert
            Assert.Equal(IconFont.Qrcode, result.Glyph);
        }

        [Fact]
        public void Can_Convert_1D_Code_To_Barcode_Font_Image()
        {
            // Arrange
            var converter = new BarcodeFormatImageSourceConverter();

            // Act
            var result1 = converter.Convert(BarcodeFormat.CODE_128, null, null, null) as FontImageSource;
            var result2 = converter.Convert(BarcodeFormat.CODE_39, null, null, null) as FontImageSource;
            var result3 = converter.Convert(BarcodeFormat.CODE_93, null, null, null) as FontImageSource;

            // Assert
            Assert.Equal(IconFont.Barcode, result1.Glyph);
            Assert.Equal(IconFont.Barcode, result2.Glyph);
            Assert.Equal(IconFont.Barcode, result3.Glyph);
        }
    }
}
