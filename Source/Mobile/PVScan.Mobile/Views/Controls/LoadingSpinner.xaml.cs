using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PVScan.Mobile.Views.Controls
{
    public partial class LoadingSpinner : ContentView
    {
        public LoadingSpinner()
        {
            InitializeComponent();

            StartSpinning();
        }

        private void StartSpinning()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await SpinnerImage.RotateTo(360, 2000, Easing.Linear);
                    SpinnerImage.Rotation = 0;
                }
            });
        }
    }
}
