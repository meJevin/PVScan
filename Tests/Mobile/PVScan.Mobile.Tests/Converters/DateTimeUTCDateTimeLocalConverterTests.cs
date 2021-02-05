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
    public class DateTimeUTCDateTimeLocalConverterTests : TestBase
    {
        [Fact]
        public void Can_Convert_UTC_To_Local_Time()
        {
            // Arrange
            var converter = new DateTimeUTCDateTimeLocalConverter();

            var dateTime = new DateTime(2000, 01, 01, 12, 0, 0);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            var dateTimeExpected = new DateTime(2000, 01, 01, 12, 0, 0);
            dateTimeExpected = DateTime.SpecifyKind(dateTimeExpected, DateTimeKind.Utc);
            dateTimeExpected = dateTimeExpected.ToLocalTime();

            // Act
            var result = (DateTime)converter.Convert((object)dateTime, null, null, null);

            // Assert
            Assert.Equal(dateTimeExpected, result);
        }
    }
}
