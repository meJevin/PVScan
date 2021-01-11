using PVScan.API.Services;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.API.Tests.Services
{
    public class ExperienceCalculatorTests : TestBase
    {
        // For now PrevLevel + (Level-1) * 100, starting from 200
        [Theory]
        [InlineData(1, 200)]
        [InlineData(2, 400)]
        [InlineData(3, 700)]
        [InlineData(4, 1100)]
        public void Can_Calculate_Level_Experience(int level, double expectedVal)
        {
            // Arrange
            ExperienceCalculator calc = new ExperienceCalculator(_context);

            // Act
            var result = calc.GetRequiredLevelExperience(level);

            // Assert
            Assert.Equal(expectedVal, result);
        }

        // For now 0.1 of total level experience required
        [Theory]
        [InlineData(1, 20)]
        [InlineData(2, 40)]
        [InlineData(3, 70)]
        [InlineData(4, 110)]
        public void Can_Calculate_Experience_For_Barcode(int level, double expectedVal)
        {
            // Arrange
            ExperienceCalculator calc = new ExperienceCalculator(_context);
            UserInfo info = new UserInfo()
            {
                Level = level,
            };

            // Act
            var result = calc.GetExperienceForBarcode(info);

            // Assert
            Assert.Equal(expectedVal, result);
        }
    }
}
