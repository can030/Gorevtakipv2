using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gorevtakipv2.adminpncr
{
    public static class tema
    {
        public static Color BackgroundColor { get; private set; }
        public static Color TextColor { get; private set; }
        public static Color CardColor { get; private set; }
        public static Color EntryBackground { get; private set; }
        public static Color ButtonColor { get; private set; }
        public static Color AccentColor { get; private set; }
        public static Color SecondaryText { get; private set; }
        public static Color SuccessColor { get; private set; }
        public static Color WarningColor { get; private set; }
        public static Color DangerColor { get; private set; }
        public static Color PlaceholderColor { get; private set; }

        public static bool IsDark { get; private set; }

        // 🌙 Koyu Tema
        public static void KoyuTema()
        {
            BackgroundColor = Color.FromArgb("#12121C");
            PlaceholderColor = Color.FromArgb("#4B5563"); // gri placeholder
            CardColor = Color.FromArgb("#1E1E2C");
            EntryBackground = Color.FromArgb("#2A2A3C");
            TextColor = Colors.White;
            SecondaryText = Color.FromArgb("#A1A1B5");
            ButtonColor = Color.FromArgb("#3A3A55");
            AccentColor = Color.FromArgb("#4E9FFF");
            SuccessColor = Color.FromArgb("#4CAF50");
            WarningColor = Color.FromArgb("#FFC107");
            DangerColor = Color.FromArgb("#F44336");
            IsDark = true;
        }

        // ☀️ Açık Tema
        public static void AcikTema()
        {
            BackgroundColor = Color.FromArgb("#F9FAFB");
            PlaceholderColor = Color.FromArgb("#9CA3AF"); // gri placeholder
            CardColor = Colors.White;
            EntryBackground = Color.FromArgb("#F3F4F6");
            TextColor = Color.FromArgb("#111827");
            SecondaryText = Color.FromArgb("#6B7280");
            ButtonColor = Color.FromArgb("#E5E7EB");
            AccentColor = Color.FromArgb("#2563EB");
            SuccessColor = Color.FromArgb("#22C55E");
            WarningColor = Color.FromArgb("#F59E0B");
            DangerColor = Color.FromArgb("#DC2626");
            IsDark = false;
        }
    }
}