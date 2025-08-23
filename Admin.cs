using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

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
                WidthRequest = 80,
                HeightRequest = 80,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 10, 0, 5)
            };

            var nameLabel = new Label
            {
                Text = $"{adSoyad}",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 5, 0, 3)
            };

            var roleLabel = new Label
            {
                Text = $"{yetki}",
                FontSize = 14,
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 3, 0, 10)
            };

            // === Menü butonları ===
            var gorevlerButton = new Button { Text = "Görevler", HeightRequest = 70 };
            var aktifGorevlerButton = new Button { Text = "Aktif Görevler", HeightRequest = 70 }; // ✅ Yeni buton
            var istatistikButton = new Button { Text = "İstatistik", HeightRequest = 70 };
            var gecmisButton = new Button { Text = "Geçmiş", HeightRequest = 70 };
            var kayitButton = new Button { Text = "Kayıt İşlemleri", HeightRequest = 70 };
            var ayarlarButton = new Button { Text = "Ayarlar", HeightRequest = 70 };

            // Sol panel (sidebar)
            var leftPanel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromArgb("#2D2D44"),
                WidthRequest = 200,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    profileImage,
                    nameLabel,
                    roleLabel,
                    gorevlerButton,
                    aktifGorevlerButton, // ✅ Menüye ekledik
                    istatistikButton,
                    gecmisButton,
                    kayitButton,
                    ayarlarButton
                }
            };

            // === Sağ üst panel (header bar) ===
            var anaSayfaButton = new Button { Text = "🏠️" };
            var bildirimButton = new Button { Text = "🔔" };
            var cikisButton = new Button { Text = "⏻" };
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
                HeightRequest = 80,
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
                Text = "Admin Paneline Hoşgeldiniz!",
                FontSize = 24,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var contentArea = new ContentView
            {
                Content = contentLabel,
            };

            // Menü butonlarına tıklama -> içerik değiştir
            gorevlerButton.Clicked += (s, e) =>
            {
                contentArea.Content = new adminpencere.gorevpncr();
            };

            aktifGorevlerButton.Clicked += (s, e) =>  // ✅ Aktif Görevler butonuna tıklandığında
            {
                contentArea.Content = new adminpencere.aktifgorevler();
            };

            istatistikButton.Clicked += (s, e) =>
                contentArea.Content = new adminpencere.istatik();

            gecmisButton.Clicked += (s, e) =>
                contentArea.Content = new adminpencere.gecmis();

            kayitButton.Clicked += (s, e) =>
                contentArea.Content = new Label { Text = "Kayıt İşlemleri Sayfası", FontSize = 40, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            ayarlarButton.Clicked += (s, e) =>
                contentArea.Content = new Label { Text = "Ayarlar Sayfası", FontSize = 40, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            // Sağ taraf için grid (üst panel + içerik)
            var rightSideGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = 80 },
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
                    new ColumnDefinition { Width = 200 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            mainGrid.Add(leftPanel, 0, 0);
            mainGrid.Add(rightSideGrid, 1, 0);

            Content = mainGrid;
        }
    }
}

