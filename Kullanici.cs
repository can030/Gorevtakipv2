using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gorevtakipv2.adminpncr;
using Gorevtakipv2.kullanicipncr;



namespace Gorevtakipv2
{
    public class kullanici : ContentPage
    {
        sqlbaglanti bgl = new sqlbaglanti();
        private Grid mainGrid;
        private StackLayout leftPanel;
        private Grid topPanel;
        private ContentView contentArea;

        public kullanici()
        {
            string adSoyad = Session.AdSoyad;
            string yetki = Session.Yetki;
            Title = "Kullanıcı Paneli";

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

            // Menü butonları
            var klncaktifGorevlerButton = CreateMenuButton("📝 Görevler");
            var klncistatistikButton = CreateMenuButton("📊 İstatistik");
            var klncgecmisButton = CreateMenuButton("📂 Geçmiş");
            var klncayarlarButton = CreateMenuButton("⚙️ Ayarlar");

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

                    klncaktifGorevlerButton,
                    klncistatistikButton,
                    klncgecmisButton,
                    klncayarlarButton,
                }
            };
            // sol panel buton aktifleştirme 

            klncaktifGorevlerButton.Clicked += async (s, e) =>
            {
                await AnimateButton(klncaktifGorevlerButton);
                contentArea.Content = new kullanicipncr.Klncgorev();

            };
            klncistatistikButton.Clicked += async (s, e) =>
            {
                await AnimateButton(klncistatistikButton);
                contentArea.Content = new kullanicipncr.KlncIstatistik(Session.AdSoyad);

            };
            klncgecmisButton.Clicked += async (s, e) =>
            {
                await AnimateButton(klncgecmisButton);
                contentArea.Content = new kullanicipncr.KlncGecmis();

            };
            klncayarlarButton.Clicked += async (s, e) =>
            {
                await AnimateButton(klncayarlarButton);
                contentArea.Content = new kullanicipncr.klncayar();
            };
               

            



            // Üst panel butonları
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

            // İçerik alanı
            contentArea = new ContentView
            {
                BackgroundColor = tema.BackgroundColor,
                Content = new Label
                {
                    Text = "Kullanıcı Paneline Hoşgeldiniz! 🎉",
                    FontSize = 26,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = tema.TextColor,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            // === Ana Grid ===
            mainGrid = new Grid
            {
                BackgroundColor = tema.BackgroundColor,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(220) },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(70) },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            // Yerleşim
            mainGrid.Add(leftPanel, 0, 0);
            Grid.SetRowSpan(leftPanel, 2); // sol panel tam yükseklik

            mainGrid.Add(topPanel, 1, 0);
            mainGrid.Add(contentArea, 1, 1);

            Content = mainGrid;
        }

        // Menü düğmesi
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
                BorderColor = tema.IsDark ? Colors.White : Colors.Gray,
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

        // Üst düğme
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
