using System;
using Autofac;
using PVScan.Mobile.iOS.Services;
using PVScan.Mobile.Services.Interfaces;

namespace PVScan.Mobile.iOS.DI
{
    public class Bootstrapper : Mobile.Bootstrapper
    {
        protected override void Initialize()
        {
            base.Initialize();

            ContainerBuilder.RegisterType<FileBarcodeReader>()
                .As<IFileBarcodeReader>();
        }
    }
}
