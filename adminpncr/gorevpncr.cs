using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MySql.Data.MySqlClient;
using System;
using System.Data;



namespace Gorevtakipv2.adminpencere
{
    public class gorevpncr : ContentView
    {
        private Picker calisanPicker;
        private sqlbaglanti bgl = new sqlbaglanti();

        private Entry baslikEntry;
        private Editor aciklamaEditor;
        private Picker onemlilikPicker;
        private DatePicker baslangicTarihiPicker;
        private TimePicker baslangicSaatPicker;
        private DatePicker bitisTarihiPicker;
        private TimePicker bitisSaatPicker;

        public gorevpncr()
        {
            // === Tema renkleri ===
            Color bgDark = Color.FromArgb("#1C1B29");   // koyu arka plan
            Color cardBg = Color.FromArgb("#2A2937");  // kart rengi
            Color textColor = Colors.WhiteSmoke;       // yazı rengi
            Color accent = Color.FromArgb("#0078D7");  // buton rengi
            Color border = Color.FromArgb("#444455");

            // === Görev başlığı ===
            baslikEntry = new Entry
            {
                Placeholder = "📝 Görev Başlığı",
                BackgroundColor = Colors.Transparent,
                TextColor = textColor,
                PlaceholderColor = Colors.Gray,
                FontSize = 16
            };

            // === Başlangıç Tarih + Saat ===
            baslangicTarihiPicker = new DatePicker
            {
                Format = "dd.MM.yyyy",
                MinimumDate = DateTime.Today,
                TextColor = textColor,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            baslangicSaatPicker = new TimePicker
            {
                Time = DateTime.Now.TimeOfDay,
                TextColor = textColor,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var baslangicFrame = CreateCard("📅 Başlangıç",
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new Frame { Content = baslangicTarihiPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = bgDark, BorderColor = border, HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand },
                        new Frame { Content = baslangicSaatPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = bgDark, BorderColor = border, HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand }
                    }
                }, cardBg, border);

            // === Bitiş Tarih + Saat ===
            bitisTarihiPicker = new DatePicker
            {
                Format = "dd.MM.yyyy",
                MinimumDate = DateTime.Today,
                TextColor = textColor,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            bitisSaatPicker = new TimePicker
            {
                Time = DateTime.Now.TimeOfDay,
                TextColor = textColor,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var bitisFrame = CreateCard("⏰ Bitiş",
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new Frame { Content = bitisTarihiPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = bgDark, BorderColor = border, HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand },
                        new Frame { Content = bitisSaatPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = bgDark, BorderColor = border, HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand }
                    }
                }, cardBg, border);

            var tarihLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 20,
                Children = { baslangicFrame, bitisFrame }
            };

            // === Çalışan seçimi ===
            calisanPicker = new Picker
            {
                Title = "👤 Çalışan Seç",
                TextColor = textColor,
                TitleColor = Colors.Gray,
                BackgroundColor = Colors.Transparent
            };

            // === Sınıf seçimi ===
            var sinifPicker = new Picker
            {
                Title = "🏷️ Sınıf Seç",
                ItemsSource = new string[] { "A", "B", "C" },
                TextColor = textColor,
                TitleColor = Colors.Gray,
                BackgroundColor = Colors.Transparent
            };

            // === Önemlilik ===
            onemlilikPicker = new Picker
            {
                Title = "⭐ Önemlilik",
                ItemsSource = new string[] { "Düşük", "Orta", "Yüksek" },
                TextColor = textColor,
                TitleColor = Colors.Gray,
                BackgroundColor = Colors.Transparent
            };

            // === Açıklama ===
            aciklamaEditor = new Editor
            {
                Placeholder = "🖊️ Görev detaylarını yazınız...",
                AutoSize = EditorAutoSizeOption.TextChanges,
                HeightRequest = 120,
                BackgroundColor = Colors.Transparent,
                TextColor = textColor,
                PlaceholderColor = Colors.Gray
            };

            // === Gönder Butonu ===
            var gonderBtn = new Button
            {
                Text = "📌 Gönder",
                BackgroundColor = accent,
                TextColor = Colors.White,
                CornerRadius = 12,
                HeightRequest = 55,
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                Margin = new Thickness(0, 15, 0, 0)
            };

            gonderBtn.Clicked += async (s, e) =>
            {
                await AnimateButton(gonderBtn);
                await KaydetGorev();
            };

            // === Layout ===
            Content = new ScrollView
            {
                BackgroundColor = bgDark,
                Content = new StackLayout
                {
                    Padding = 20,
                    Spacing = 12,
                    Children =
                    {
                        CreateCard("📝 Başlık", baslikEntry, cardBg, border),
                        tarihLayout,
                        CreateCard("👤 Çalışan", calisanPicker, cardBg, border),
                        CreateCard("🏷️ Sınıf", sinifPicker, cardBg, border),
                        CreateCard("⭐ Önemlilik", onemlilikPicker, cardBg, border),
                        CreateCard("📖 Açıklama", aciklamaEditor, cardBg, border),
                        gonderBtn
                    }
                }
            };

            // Çalışanları DB’den çek
            LoadCalisanlar();
        }

        // === Ortak Kart Component ===
        private Frame CreateCard(string title, View content, Color bg, Color border)
        {
            return new Frame
            {
                Content = new StackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        new Label { Text = title, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold, FontSize = 14 },
                        content
                    }
                },
                Padding = new Thickness(12),
                CornerRadius = 12,
                BackgroundColor = bg,
                BorderColor = border,
                HasShadow = true,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        // === Buton animasyonu ===
        private async Task AnimateButton(Button button)
        {
            await button.ScaleTo(1.1, 100, Easing.CubicOut);
            await button.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        // === DB'den çalışanları yükle ===
        private async void LoadCalisanlar()
        {
            try
            {
                using (var conn = bgl.Connection())
                {
                    await conn.OpenAsync();
                    string sql = "SELECT id, ad FROM personel_bilgi";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32("id");
                            string ad = reader.GetString("ad");
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                calisanPicker.Items.Add($"{id}-{ad}");
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
                });
            }
        }

        // === Kaydetme işlemi ===
        private async Task KaydetGorev()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baslikEntry.Text) ||
                    calisanPicker.SelectedIndex < 0 ||
                    onemlilikPicker.SelectedIndex < 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Uyarı", "Lütfen tüm alanları doldurun!", "Tamam");
                    return;
                }

                string baslik = baslikEntry.Text;
                string aciklama = aciklamaEditor.Text ?? "";
                string onemlilik = onemlilikPicker.SelectedItem.ToString();

                string calisanSecim = calisanPicker.SelectedItem.ToString();
                int calisanId = int.Parse(calisanSecim.Split('-')[0]);

                DateTime baslangic = baslangicTarihiPicker.Date + baslangicSaatPicker.Time;
                DateTime bitis = bitisTarihiPicker.Date + bitisSaatPicker.Time;

                using (var conn = bgl.Connection())
                {
                    await conn.OpenAsync();
                    string sql = @"INSERT INTO gorevler 
                                  (baslik, aciklama, onemlilik, calisan_id, baslangic_zamani, bitis_zamani) 
                                  VALUES (@baslik, @aciklama, @onemlilik, @calisan_id, @baslangic, @bitis)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@baslik", baslik);
                        cmd.Parameters.AddWithValue("@aciklama", aciklama);
                        cmd.Parameters.AddWithValue("@onemlilik", onemlilik);
                        cmd.Parameters.AddWithValue("@calisan_id", calisanId);
                        cmd.Parameters.AddWithValue("@baslangic", baslangic);
                        cmd.Parameters.AddWithValue("@bitis", bitis);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Başarılı", "Görev başarıyla eklendi ✅", "Tamam");
                MessagingCenter.Send(this, "GorevEklendi");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}



