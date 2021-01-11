using PVScan.API.Services.Interfaces;
using PVScan.Database;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.API.Services
{
    public class ExperienceCalculator : IExperienceCalculator
    {
        readonly PVScanDbContext _context;

        public ExperienceCalculator(PVScanDbContext context)
        {
            _context = context;
        }

        public double GetExperienceForBarcode(UserInfo info)
        {
            // TODO:  Make this a little more interesting using DbContext to fetch some info
            double result = GetRequiredLevelExperience(info.Level) * 0.1;

            return result;
        }

        public double GetRequiredLevelExperience(int level)
        {
            double result = 200;

            for (int i = 1; i < level; ++i)
            {
                result += 100 * (i+1);
            }
            
            return result;
        }
    }
}
