using Gorevtakipv2.adminpncr;
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
        private Frame card; // kartı sınıf düzeyine taşıyoruz
        private StackLayout cardStack; // içindeki içerik

        public Ayarlar()
        {
            BackgroundColor = tema.BackgroundColor;

            // Tema Seçimi
            temaPicker = new Picker
            {
                Title = "Tema Seçimi",
                TextColor = tema.TextColor,
                BackgroundColor = tema.CardColor
            };
            
            temaPicker.Items.Add("Açık");
            temaPicker.Items.Add("Koyu");
            temaPicker.SelectedIndex = 0;

            temaPicker.SelectedIndexChanged += TemaPicker_SelectedIndexChanged;

            // Bildirim
            bildirimSwitch = new Switch
            {
                IsToggled = true,
                OnColor = Colors.White,
                ThumbColor = Colors.Gray
            };

            // Parola değiştir
            parolaBtn = new Button
            {
                Text = "Parola Değiştir",
                BackgroundColor = tema.CardColor,
                TextColor = tema.TextColor,
                CornerRadius = 12
            };
            parolaBtn.Clicked += ParolaBtn_Clicked;

            // Kart tasarımı
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
                        Spacing = 8,
                        Children =
                        {
                            new Label{ Text="Dil Seçimi (Geliştirilmekte)", TextColor=tema.TextColor }
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
                case "Sistem":
                case "Açık":
                    tema.beyaztema();
                    break;
                case "Koyu":
                    tema.garatema();
                    break;
            }

            // Admin ana sayfayı güncelle
            (Application.Current.MainPage as Admin)?.RefreshTheme();

            // Ayarlar sayfasındaki anlık değişiklik
            BackgroundColor = tema.BackgroundColor;
            card.BackgroundColor = tema.CardColor;
            temaPicker.BackgroundColor = tema.CardColor;

            foreach (var child in cardStack.Children)
            {
                if (child is Label lbl)
                    lbl.TextColor = tema.TextColor;

                if (child is StackLayout sl)
                {
                    foreach (var inner in sl.Children)
                    {
                        if (inner is Label l)
                            l.TextColor = tema.TextColor;
                        else if (inner is Button b)
                        {
                            b.BackgroundColor = tema.CardColor;
                            b.TextColor = tema.TextColor;
                        }
                    }
                }
            }

            // Buton rengini ayrıca ayarla
            parolaBtn.BackgroundColor = tema.CardColor;
            parolaBtn.TextColor = tema.TextColor;
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
