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
    public class BarcodeFormatStringConverterTests : TestBase
    {
        [Theory]
        [InlineData(BarcodeFormat.QR_CODE, "QR CODE")]
        [InlineData(BarcodeFormat.DATA_MATRIX, "DATA MATRIX")]
        [InlineData(BarcodeFormat.CODE_128, "CODE 128")]
        [InlineData(BarcodeFormat.AZTEC, "AZTEC")]
        public void Can_Convert_Barcode_Format_To_String(BarcodeFormat format, string expected)
        {
            // Arrange
            var converter = new BarcodeFormatStringConverter();

            // Act
            var result = converter.Convert(format, null, null, null) as string;

            // Assert
            Assert.Equal(result, expected);
        }
    }
}
