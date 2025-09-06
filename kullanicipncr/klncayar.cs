using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gorevtakipv2.kullanicipncr;
using Gorevtakipv2.adminpncr;
using Microsoft.Maui.Graphics.Text;

namespace Gorevtakipv2.kullanicipncr
{
    public class klncayar : ContentView
    {
        private Picker temaPicker;
        private Switch bildirimSwitch;
        private Button parolaBtn;
        private Frame card;
        private StackLayout cardStack;

        public klncayar()
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
            {
                temaPicker.TextColor = tema.TextColor;
            }
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
                BorderWidth = 1
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
            // Seçilen temayı uygula
            switch (temaPicker.SelectedItem?.ToString())
            {
                case "Açık":
                    
                    tema.AcikTema();
                    break;
                case "Koyu":
                    tema.KoyuTema();
                    break;
            }

            // Sayfayı tamamen yenile
            sayfayenile();
        }

        private async void ParolaBtn_Clicked(object sender, EventArgs e)
        {
            // Mevcut parolayı al
            string mevcut = await Application.Current.MainPage.DisplayPromptAsync(
                "Parola Doğrulama", "Mevcut parolanızı girin:",
                "Devam", "İptal", placeholder: "Mevcut Parola",
                maxLength: 20, keyboard: Keyboard.Text);

            if (string.IsNullOrWhiteSpace(mevcut)) return;

            try
            {
                using var conn = new sqlbaglanti().Connection();
                await conn.OpenAsync();

                // Mevcut parola kontrolü
                string kontrolSql = "SELECT COUNT(*) FROM personel_bilgi WHERE ID=@id AND parola=@parola";
                using var kontrolCmd = new MySqlCommand(kontrolSql, conn);
                kontrolCmd.Parameters.AddWithValue("@id", Session.ID);
                kontrolCmd.Parameters.AddWithValue("@parola", mevcut);

                var sonuc = Convert.ToInt32(await kontrolCmd.ExecuteScalarAsync());
                if (sonuc == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Mevcut parolanız yanlış.", "Tamam");
                    return;
                }

                // Yeni parolayı al
                string yeni1 = await Application.Current.MainPage.DisplayPromptAsync(
                    "Yeni Parola", "Yeni parolanızı girin:",
                    "Devam", "İptal", placeholder: "Yeni Parola",
                    maxLength: 20, keyboard: Keyboard.Text);

                if (string.IsNullOrWhiteSpace(yeni1)) return;

                string yeni2 = await Application.Current.MainPage.DisplayPromptAsync(
                    "Parola Onay", "Yeni parolayı tekrar girin:",
                    "Kaydet", "İptal", placeholder: "Yeni Parola Tekrar",
                    maxLength: 20, keyboard: Keyboard.Text);

                if (yeni1 != yeni2)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Yeni parolalar eşleşmiyor.", "Tamam");
                    return;
                }

                // Güncelle
                string updateSql = "UPDATE personel_bilgi SET parola=@parola WHERE ID=@id";
                using var updateCmd = new MySqlCommand(updateSql, conn);
                updateCmd.Parameters.AddWithValue("@parola", yeni1);
                updateCmd.Parameters.AddWithValue("@id", Session.ID);
                await updateCmd.ExecuteNonQueryAsync();

                await Application.Current.MainPage.DisplayAlert("Başarılı", "Parolanız değiştirildi.", "Tamam");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private void sayfayenile()
        {
            // Arka plan ve kart renklerini güncelle
            BackgroundColor = tema.BackgroundColor;
            card.BackgroundColor = tema.CardColor;
            temaPicker.BackgroundColor = tema.EntryBackground;

            // Parola butonu
            parolaBtn.BackgroundColor = tema.ButtonColor;
            parolaBtn.TextColor = tema.TextColor;
            parolaBtn.BorderColor = tema.IsDark ? Colors.White : Color.FromArgb("#D1D5DB");

            // Card içindeki tüm elementler
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
                            b.BackgroundColor = tema.ButtonColor;
                            b.TextColor = tema.TextColor;
                            b.BorderColor = tema.IsDark ? Colors.White : Color.FromArgb("#D1D5DB");
                        }
                    }
                }
            }
        }
    }
}
