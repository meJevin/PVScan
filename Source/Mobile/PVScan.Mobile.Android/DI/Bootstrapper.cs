using System;
using Autofac;
using PVScan.Mobile.Droid.Services;
using PVScan.Mobile.Services.Interfaces;

namespace PVScan.Mobile.Droid.DI
{
    public class Bootstrapper : Mobile.Bootstrapper
    {
        protected override void Initialize()
        {
            base.Initialize();

            ContainerBuilder.RegisterType<FileBarcodeReader>()
                .As<IFileBarcodeReader>();

            ContainerBuilder.RegisterType<MediaService>()
                .As<IMediaService>();
        }
    }
}
