using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.API.Services.Interfaces
{
    public interface IExperienceCalculator
    {
        double GetExperienceForBarcode(UserInfo info);
        double GetRequiredLevelExperience(int level);
    }
}
