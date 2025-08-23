using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gorevtakipv2.adminpencere
{
    public class istatik : ContentView
    {
        public istatik()
        {
            Content = new Label
            {
                Text = "İstatistik Sayfası",
                FontSize = 22,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }
    }
}
