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
    public class InverseBoolConverterTests : TestBase
    {
        [Fact]
        public void Can_Ivert_Bool()
        {
            // Arrange
            var converter = new InverseBoolConverter();

            // Act
            var result1 = (bool)converter.Convert(true, null, null, null);
            var result2 = (bool)converter.Convert(false, null, null, null);

            // Assert
            Assert.False(result1);
            Assert.True(result2);
        }
    }
}
