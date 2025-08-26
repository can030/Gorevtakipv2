using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gorevtakipv2.adminpncr
{
    public static class tema
    {
        public static Color BackgroundColor { get; set; } = Color.FromArgb("#12121C");
        public static Color CardColor { get; set; } = Color.FromArgb("#1E1E2E");
        public static Color TextColor { get; set; } = Colors.White;

        public static void beyaztema()
        {
            BackgroundColor = Colors.White;
            CardColor = Color.FromArgb("#F0F0F0");
            TextColor = Colors.Black;
        }

        public static void garatema()
        {
            BackgroundColor = Color.FromArgb("#12121C");
            CardColor = Color.FromArgb("#1E1E2E");
            TextColor = Colors.White;
        }
    }
}