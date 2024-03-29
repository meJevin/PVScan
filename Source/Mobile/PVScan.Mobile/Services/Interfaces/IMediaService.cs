﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Svg;

namespace PVScan.Mobile.Services.Interfaces
{
    public interface IMediaService
    {
        Task SaveSvgImage(SvgImageSource image, string fileName);
    }
}
