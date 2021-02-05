using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Mocks;

namespace PVScan.Mobile.Tests
{
    public class TestBase
    {
        public TestBase()
        {
            MockForms.Init();
            Application.Current = new Application();
        }
    }
}
