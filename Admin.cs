using Gorevtakipv2.adminpencere;
using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp;
using Microcharts.Maui;

namespace Gorevtakipv2
{
    public class Admin : ContentPage
    {
        sqlbaglanti bgl = new sqlbaglanti();
        public Admin()
        {
            string adSoyad = Session.AdSoyad;
            string yetki = Session.Yetki;
            Title = "Admin Paneli";

            // === Kullanıcı bilgileri ===
            var profileImage = new Image
            {
                Source = "user.png",
                WidthRequest = 90,
                HeightRequest = 90,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20, 0, 10)
            };

            var nameLabel = new Label
            {
                Text = $"{adSoyad}",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };

            var roleLabel = new Label
            {
                Text = $"{yetki}",
                FontSize = 14,
                TextColor = Colors.LightGray,
                HorizontalOptions = LayoutOptions.Center
            };

            // === Menü butonları ===
            var gorevlerButton = CreateMenuButton("📝 Görevler");
            var aktifGorevlerButton = CreateMenuButton("⚡ Aktif Görevler");
            var istatistikButton = CreateMenuButton("📊 İstatistik");
            var gecmisButton = CreateMenuButton("📂 Geçmiş");
            var kayitButton = CreateMenuButton("➕ Kayıt İşlemleri");
            var ayarlarButton = CreateMenuButton("⚙️ Ayarlar");

            // Sol panel (sidebar)
            var leftPanel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromArgb("#23233B"),
                WidthRequest = 220,
                Padding = new Thickness(10, 20),
                Children =
                {
                    profileImage,
                    nameLabel,
                    roleLabel,
                    new BoxView { HeightRequest = 2, Color = Colors.Gray, Margin = new Thickness(0,10) },
                    gorevlerButton,
                    aktifGorevlerButton,
                    istatistikButton,
                    gecmisButton,
                    kayitButton,
                    ayarlarButton
                }
            };

            // === Sağ üst panel (header bar) ===
            var anaSayfaButton = new Button { Text = "🏠️", FontSize = 22, BackgroundColor = Colors.Transparent };
            var bildirimButton = new Button { Text = "🔔", FontSize = 22, BackgroundColor = Colors.Transparent };
            var cikisButton = new Button { Text = "⏻", FontSize = 22, BackgroundColor = Colors.Transparent };

            cikisButton.Clicked += async (s, e) =>
            {
                bool cevap = await DisplayAlert("Çıkış", "Çıkış yapmak istiyor musunuz?", "Evet", "Hayır");
                if (cevap)
                {
                    Session.AdSoyad = null;
                    Session.Yetki = null;
                    Application.Current.MainPage = new Login();
                }
            };

            var topPanel = new Grid
            {
                BackgroundColor = Color.FromArgb("#1E1E2F"),
                HeightRequest = 70,
                Padding = new Thickness(10),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            topPanel.Add(anaSayfaButton, 1, 0);
            topPanel.Add(bildirimButton, 2, 0);
            topPanel.Add(cikisButton, 3, 0);

            // === İçerik alanı ===
            var contentLabel = new Label
            {
                Text = "Admin Paneline Hoşgeldiniz! 🎉",
                FontSize = 26,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var contentArea = new ContentView
            {
                Content = contentLabel,
            };

            // Menü butonlarına tıklama -> içerik değiştir
            gorevlerButton.Clicked += async (s, e) =>
            {
                await AnimateButton(gorevlerButton);
                contentArea.Content = new adminpencere.gorevpncr();
            };

            aktifGorevlerButton.Clicked += async (s, e) =>
            {
                await AnimateButton(aktifGorevlerButton);
                contentArea.Content = new adminpencere.aktifgorevler();
            };

            istatistikButton.Clicked += async (s, e) =>
            {
                await AnimateButton(istatistikButton);
                {
                    await AnimateButton(istatistikButton);

                    // Veritabanından veya başka bir listeden görevleri al
                    var gorevlerListesi = new ObservableCollection<GorevModel>();

                    using (var conn = new sqlbaglanti().Connection())
                    {
                        await conn.OpenAsync();
                        string sql = @"SELECT g.baslik, g.aciklama, g.onemlilik, 
                              p.ad as calisan, g.baslangic_zamani, g.bitis_zamani 
                       FROM gorevler g 
                       JOIN personel_bilgi p ON g.calisan_id = p.id 
                       ORDER BY g.id DESC";

                        using (var cmd = new MySqlCommand(sql, conn))
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string onem = reader["onemlilik"].ToString();
                                Color renk = Colors.LightGray;
                                if (onem == "Yüksek") renk = Colors.OrangeRed;
                                else if (onem == "Orta") renk = Colors.Gold;
                                else if (onem == "Düşük") renk = Colors.LightGreen;

                                gorevlerListesi.Add(new GorevModel
                                {
                                    Baslik = reader["baslik"].ToString(),
                                    Calisan = reader["calisan"].ToString(),
                                    Zaman = $"{Convert.ToDateTime(reader["baslangic_zamani"]):dd.MM.yyyy HH:mm} - {Convert.ToDateTime(reader["bitis_zamani"]):dd.MM.yyyy HH:mm}",
                                    Onemlilik = onem,
                                    OnemlilikRenk = renk,
                                    Aciklama = reader["aciklama"].ToString(),
                                    BitisZamani = Convert.ToDateTime(reader["bitis_zamani"])
                                });
                            }
                        }
                    }

                    contentArea.Content = new adminpencere.IstatistikSayfasi(gorevlerListesi);
                }
                ;
            };

            gecmisButton.Clicked += async (s, e) =>
            {
                await AnimateButton(gecmisButton);
                contentArea.Content = new adminpencere.gecmis();
            };

            kayitButton.Clicked += async (s, e) =>
            {
                await AnimateButton(kayitButton);
                contentArea.Content = new adminpencere.Kayit();
            };

            ayarlarButton.Clicked += async (s, e) =>
            {
                await AnimateButton(ayarlarButton);
                contentArea.Content = new Label { Text = "⚙️ Ayarlar Sayfası", FontSize = 30, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            };

            // Sağ taraf için grid (üst panel + içerik)
            var rightSideGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = 70 },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            rightSideGrid.Add(topPanel, 0, 0);
            rightSideGrid.Add(contentArea, 0, 1);
            Grid.SetRow(contentArea, 1);

            // === Ana Grid (sol menü + sağ taraf) ===
            var mainGrid = new Grid
            {
                BackgroundColor = Color.FromArgb("#12121C"),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 220 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            mainGrid.Add(leftPanel, 0, 0);
            mainGrid.Add(rightSideGrid, 1, 0);

            Content = mainGrid;
        }

        // Ortak buton oluşturucu
        private Button CreateMenuButton(string text)
        {
            return new Button
            {
                Text = text,
                HeightRequest = 55,
                Margin = new Thickness(5, 8),
                FontSize = 16,
                BackgroundColor = Color.FromArgb("#343454"),
                TextColor = Colors.White,
                CornerRadius = 12
            };
        }

        // Buton animasyonu (tıklanınca büyüyüp geri küçülüyor)
        private async Task AnimateButton(Button button)
        {
            await button.ScaleTo(1.1, 100, Easing.CubicOut);
            await button.ScaleTo(1.0, 100, Easing.CubicIn);
        }
    }
}
