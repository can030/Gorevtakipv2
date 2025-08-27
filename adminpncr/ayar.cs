using Gorevtakipv2.adminpncr;
using Microsoft.Maui.Graphics.Text;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace Gorevtakipv2.adminpencere
{
    public class Ayarlar : ContentView
    {
        private Picker temaPicker;
        private Switch bildirimSwitch;
        private Button parolaBtn;
        private Frame card;
        private StackLayout cardStack;

        public Ayarlar()
        {
            BackgroundColor = tema.BackgroundColor;

            // --- Tema Seçimi ---
            temaPicker = new Picker
            {
                
                TextColor = tema.TextColor,
                BackgroundColor = tema.EntryBackground
            };
            temaPicker.Items.Add("Açık");
            {
                temaPicker.TextColor = tema.TextColor;
            }
            temaPicker.Items.Add("Koyu");
            temaPicker.SelectedIndex = tema.IsDark ? 1 : 0;
            temaPicker.SelectedIndexChanged += TemaPicker_SelectedIndexChanged;

            // --- Bildirim Switch ---
            bildirimSwitch = new Switch
            {
                IsToggled = true,
                OnColor = tema.AccentColor,
                ThumbColor = tema.IsDark ? Colors.White : Colors.Gray
            };

            // --- Parola Butonu ---
            parolaBtn = new Button
            {
                Text = "Parola Değiştir",
                BackgroundColor = tema.ButtonColor,
                TextColor = tema.TextColor,
                CornerRadius = 12,
                BorderColor = tema.IsDark ? Colors.White : Color.FromArgb("#D1D5DB"),
                BorderWidth = 1,
                Shadow = new Shadow
                {
                    Brush = Brush.Black,
                    Opacity = tema.IsDark ? 0 : 0.05f,
                    Radius = 4,
                    Offset = new Point(2, 2)
                }
            };
            parolaBtn.Clicked += ParolaBtn_Clicked;

            // --- Kart ---
            cardStack = new StackLayout
            {
                Spacing = 20,
                Children =
                {
                    new Label
                    {
                        Text = "Ayarlar",
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = tema.TextColor
                    },
                    new StackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new Label{ Text="Tema Seçimi", TextColor=tema.TextColor },
                            temaPicker
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 10,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label{ Text="Bildirimler", TextColor=tema.TextColor, VerticalOptions=LayoutOptions.Center },
                            bildirimSwitch
                        }
                    },
                    parolaBtn
                }
            };

            card = new Frame
            {
                CornerRadius = 16,
                Padding = 20,
                BackgroundColor = tema.CardColor,
                HasShadow = true,
                Content = cardStack
            };

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 16,
                    Children = { card }
                }
            };
        }

        private void TemaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (temaPicker.SelectedItem?.ToString())
            {
                case "Açık":
                    tema.garatema();
                    break;
                case "Koyu":
                    tema.beyaztema();
                    break;
            }

            // Ana sayfayı güncelle
            (Application.Current.MainPage as Admin)?.RefreshTheme();

            // Ayarlar sayfasını anlık güncelle
            BackgroundColor = tema.BackgroundColor;
            card.BackgroundColor = tema.CardColor;
            temaPicker.BackgroundColor = tema.EntryBackground;

            parolaBtn.BackgroundColor = tema.ButtonColor;
            parolaBtn.TextColor = tema.TextColor;
            parolaBtn.BorderColor = tema.IsDark ? Colors.White : Color.FromArgb("#D1D5DB");
            parolaBtn.Shadow = tema.IsDark ? null : new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.05f,
                Radius = 4,
                Offset = new Point(2, 2)
            };

            foreach (var child in cardStack.Children)
            {
                if (child is Label lbl) lbl.TextColor = tema.TextColor;
                if (child is StackLayout sl)
                {
                    foreach (var inner in sl.Children)
                    {
                        if (inner is Label l) l.TextColor = tema.TextColor;
                        else if (inner is Button b)
                        {
                            b.BackgroundColor = tema.ButtonColor;
                            b.TextColor = tema.TextColor;
                            b.BorderColor = tema.IsDark ? Colors.White : Color.FromArgb("#D1D5DB");
                        }
                    }
                }
            }
        }

        private async void ParolaBtn_Clicked(object sender, EventArgs e)
        {
            bool kendiMi = await Application.Current.MainPage.DisplayAlert(
                "Parola Değiştir",
                "Kendi parolanızı mı değiştirmek istiyorsunuz?",
                "Evet", "Hayır");

            if (kendiMi)
            {
                await DegistirDialog("Kendi parolanızı değiştirin");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Bilgi",
                    "Başka kullanıcı için parola değiştirme özelliği henüz geliştirilmekte.",
                    "Tamam");
            }
        }

        private async Task DegistirDialog(string title)
        {
            string yeniParola = await Application.Current.MainPage.DisplayPromptAsync(
                title,
                "Yeni parolayı girin:",
                "Tamam", "İptal", placeholder: "Parola", maxLength: 20, keyboard: Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(yeniParola))
            {
                try
                {
                    using var conn = new sqlbaglanti().Connection();
                    await conn.OpenAsync();
                    string sql = "UPDATE personel_bilgi SET parola=@parola WHERE id=@id";
                    using var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@parola", yeniParola);
                    cmd.Parameters.AddWithValue("@id", 1); // Admin ID
                    await cmd.ExecuteNonQueryAsync();

                    await Application.Current.MainPage.DisplayAlert("Başarılı", "Parola değiştirildi.", "Tamam");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
                }
            }
        }
    }
}
