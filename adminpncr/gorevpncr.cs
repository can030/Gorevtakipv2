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
            Color bgDark = Color.FromArgb("#1C1B29");   // koyu arka plan
            Color cardBg = Color.FromArgb("#2A2937");  // kart rengi
            Color textColor = Colors.WhiteSmoke;       // yazı rengi
            Color accent = Color.FromArgb("#0078D7");  // buton rengi

            baslikEntry = new Entry
            {
                Placeholder = "Görev Başlığı",
                BackgroundColor = Colors.Transparent,
                TextColor = textColor,
                PlaceholderColor = Colors.Gray
            };

            // Başlangıç Tarihi + Saati
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

            var baslangicLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 8,
                Children =
                {
                    new Label { Text = "Başlangıç Tarihi", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 16, HorizontalOptions = LayoutOptions.Center },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 8,
                        Children =
                        {
                            new Frame { Content = baslangicTarihiPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = Color.FromArgb("#1E1D2D"), BorderColor = Color.FromArgb("#444455"), HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand },
                            new Frame { Content = baslangicSaatPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = Color.FromArgb("#1E1D2D"), BorderColor = Color.FromArgb("#444455"), HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand }
                        }
                    }
                }
            };

            var baslangicFrame = new Frame
            {
                Content = baslangicLayout,
                Padding = new Thickness(12),
                CornerRadius = 12,
                BackgroundColor = cardBg,
                BorderColor = Color.FromArgb("#333344"),
                HasShadow = true,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Bitiş Tarihi + Saati
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

            var bitisLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 8,
                Children =
                {
                    new Label { Text = "Bitiş Tarihi ", TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 16, HorizontalOptions = LayoutOptions.Center },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 8,
                        Children =
                        {
                            new Frame { Content = bitisTarihiPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = Color.FromArgb("#1E1D2D"), BorderColor = Color.FromArgb("#444455"), HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand },
                            new Frame { Content = bitisSaatPicker, Padding = new Thickness(5), CornerRadius = 8, BackgroundColor = Color.FromArgb("#1E1D2D"), BorderColor = Color.FromArgb("#444455"), HasShadow = false, HorizontalOptions = LayoutOptions.FillAndExpand }
                        }
                    }
                }
            };

            var bitisFrame = new Frame
            {
                Content = bitisLayout,
                Padding = new Thickness(12),
                CornerRadius = 12,
                BackgroundColor = cardBg,
                BorderColor = Color.FromArgb("#333344"),
                HasShadow = true,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Başlangıç ve Bitiş yan yana
            var tarihLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 20,
                Children =
                {
                    baslangicFrame,
                    bitisFrame
                }
            };

            calisanPicker = new Picker
            {
                Title = "Çalışan Seçimi",
                TextColor = textColor,
                TitleColor = Colors.Gray,
                BackgroundColor = Colors.Transparent
            };

            var sinifPicker = new Picker
            {
                Title = "Sınıf Seçimi",
                ItemsSource = new string[] { "A", "B", "C" },
                TextColor = textColor,
                TitleColor = Colors.Gray,
                BackgroundColor = Colors.Transparent
            };

            onemlilikPicker = new Picker
            {
                Title = "Önemlilik",
                ItemsSource = new string[] { "Düşük", "Orta", "Yüksek" },
                TextColor = textColor,
                TitleColor = Colors.Gray,
                BackgroundColor = Colors.Transparent
            };

            aciklamaEditor = new Editor
            {
                Placeholder = "Görev detaylarını yazınız...",
                AutoSize = EditorAutoSizeOption.TextChanges,
                HeightRequest = 120,
                BackgroundColor = Colors.Transparent,
                TextColor = textColor,
                PlaceholderColor = Colors.Gray
            };

            Frame WrapView(View v) => new Frame
            {
                Content = v,
                Padding = new Thickness(10),
                CornerRadius = 10,
                BackgroundColor = cardBg,
                BorderColor = Color.FromArgb("#333344"),
                HasShadow = false,
                Margin = new Thickness(0, 8)
            };

            var gonderBtn = new Button
            {
                Text = "Gönder",
                BackgroundColor = accent,
                TextColor = Colors.White,
                CornerRadius = 12,
                HeightRequest = 50,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 15, 0, 0)
            };

            gonderBtn.Clicked += async (s, e) => await KaydetGorev();

            Content = new ScrollView
            {
                BackgroundColor = bgDark,
                Content = new StackLayout
                {
                    Padding = 20,
                    Children =
                    {
                        WrapView(baslikEntry),
                        tarihLayout,   // Başlangıç ve Bitiş yan yana paneller
                        WrapView(calisanPicker),
                        WrapView(sinifPicker),
                        WrapView(onemlilikPicker),
                        WrapView(aciklamaEditor),
                        gonderBtn
                    }
                }
            };

            // Çalışanları veritabanından yükle
            LoadCalisanlar();
        }

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

                            // PickerItem model olarak tutulabilir
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

                // Çalışan id’yi picker item’den çekiyoruz ("id-ad" formatında eklemiştik)
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


