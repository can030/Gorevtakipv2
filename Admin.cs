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
using Gorevtakipv2.adminpncr;

namespace Gorevtakipv2
{
    public class Admin : ContentPage
    {
        sqlbaglanti bgl = new sqlbaglanti();

        private Grid mainGrid;
        private StackLayout leftPanel;
        private Grid topPanel;
        private ContentView contentArea;

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
                TextColor = tema.TextColor,
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

            // === Sol panel (sidebar) ===
            leftPanel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = tema.CardColor,
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
                    ayarlarButton,
                }
            };

            // === Sağ üst panel (header bar) ===
            var anaSayfaButton = CreateTopButton("🏠️");
            var bildirimButton = CreateTopButton("🔔");
            var cikisButton = CreateTopButton("⏻");

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

            topPanel = new Grid
            {
                BackgroundColor = tema.CardColor,
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
            contentArea = new ContentView
            {
                Content = new Label
                {
                    Text = "Admin Paneline Hoşgeldiniz! 🎉",
                    FontSize = 26,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = tema.TextColor,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
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
                contentArea.Content = new adminpencere.Ayarlar();
            };

            // Sağ taraf grid
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

            // Ana Grid
            mainGrid = new Grid
            {
                BackgroundColor = tema.BackgroundColor,
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

        // Menü butonları
        private Button CreateMenuButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                HeightRequest = 55,
                Margin = new Thickness(5, 8),
                FontSize = 16,
                BackgroundColor = tema.CardColor,
                TextColor = tema.TextColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = tema.IsDark ? Colors.White : Colors.Gray, // kenarlık her temada var, sadece rengi değişiyor
                Padding = new Thickness(10, 0)
            };

            if (!tema.IsDark)
            {
                btn.Shadow = new Shadow
                {
                    Brush = Brush.Black,
                    Opacity = 0.08f,
                    Radius = 4,
                    Offset = new Point(2, 2)
                };
            }

            return btn;
        }

        // Üst panel butonları
        private Button CreateTopButton(string text)
        {
            return new Button
            {
                Text = text,
                FontSize = 22,
                BackgroundColor = Colors.Transparent,
                TextColor = tema.TextColor,
                WidthRequest = 50,
                HeightRequest = 50,
                CornerRadius = 25,
                BorderWidth = 1,
                BorderColor = tema.IsDark ? Colors.White : Colors.Gray
            };
        }

        // Buton animasyonu
        private async Task AnimateButton(Button button)
        {
            await button.ScaleTo(1.1, 100, Easing.CubicOut);
            await button.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        // Tema yenileme
        public void RefreshTheme()
        {
            mainGrid.BackgroundColor = tema.BackgroundColor;
            leftPanel.BackgroundColor = tema.CardColor;
            topPanel.BackgroundColor = tema.CardColor;
            contentArea.BackgroundColor = tema.BackgroundColor;

            foreach (var child in leftPanel.Children)
            {
                if (child is Label lbl)
                    lbl.TextColor = tema.TextColor;

                if (child is Button btn)
                {
                    btn.BackgroundColor = tema.CardColor;
                    btn.TextColor = tema.TextColor;
                    btn.BorderColor = tema.IsDark ? Colors.White : Colors.Gray;
                    btn.BorderWidth = 1;

                    if (!tema.IsDark)
                        btn.Shadow = new Shadow
                        {
                            Brush = Brush.Black,
                            Opacity = 0.08f,
                            Radius = 4,
                            Offset = new Point(2, 2)
                        };
                    else
                        btn.Shadow = null;
                }
            }

            foreach (var child in topPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.TextColor = tema.TextColor;
                    btn.BorderColor = tema.IsDark ? Colors.White : Colors.Gray;
                    btn.BorderWidth = 1;
                }
            }

            if (contentArea.Content is Label lblContent)
                lblContent.TextColor = tema.TextColor;

            if (contentArea.Content is ContentView cv)
                cv.BackgroundColor = tema.BackgroundColor;
        }
    }
}