using Xamarin.Forms;

[assembly: ExportFont("fa-solid-900.ttf", Alias = "FontAwesome")]
namespace Common.Styles
{
    public partial class DefaultTheme : ResourceDictionary
    {
        public DefaultTheme()
        {
            InitializeComponent();
        }
    }
}