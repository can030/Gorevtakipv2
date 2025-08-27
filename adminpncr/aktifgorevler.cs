using CommunityToolkit.Maui.Markup;
using Gorevtakipv2.adminpncr;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;



namespace Gorevtakipv2.adminpencere
{
    public class aktifgorevler : ContentView
    {
        private CollectionView gorevListView;
        private sqlbaglanti bgl = new sqlbaglanti();
        private System.Timers.Timer timer;
        private List<GorevModel> gorevler = new List<GorevModel>();

        public aktifgorevler()
        {
            // --- CollectionView tanımı ---
            gorevListView = new CollectionView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var importanceDot = new BoxView
                    {
                        WidthRequest = 12,
                        HeightRequest = 12,
                        CornerRadius = 6,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(0, 4, 6, 0)
                    };
                    importanceDot.SetBinding(BoxView.ColorProperty, "OnemlilikRenk");

                    var baslikLbl = new Label
                    {
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = tema.TextColor
                    };
                    baslikLbl.SetBinding(Label.TextProperty, "Baslik");

                    var calisanLbl = new Label { FontSize = 15, TextColor = tema.SecondaryText };
                    calisanLbl.SetBinding(Label.TextProperty, "Calisan");

                    var zamanLbl = new Label { FontSize = 15, TextColor = tema.SecondaryText };
                    zamanLbl.SetBinding(Label.TextProperty, "Zaman");

                    var onemLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
                    onemLbl.SetBinding(Label.TextProperty, "Onemlilik");
                    onemLbl.SetBinding(Label.TextColorProperty, "OnemlilikRenk");

                    var infoRow = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 18,
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label{Text="👤", TextColor = tema.SecondaryText}, calisanLbl }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label{Text="⏳", TextColor = tema.SecondaryText}, zamanLbl }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label{Text="⚡", TextColor = tema.SecondaryText}, onemLbl }
                            }
                        }
                    };

                    var aciklamaLbl = new Label
                    {
                        FontSize = 15,
                        TextColor = tema.TextColor,
                        LineBreakMode = LineBreakMode.WordWrap,
                        MaxLines = 3
                    };
                    aciklamaLbl.SetBinding(Label.TextProperty, "Aciklama");

                    var aciklamaFrame = new Frame
                    {
                        Padding = new Thickness(10, 6),
                        BackgroundColor = tema.CardColor,
                        CornerRadius = 10,
                        HasShadow = false,
                        Content = aciklamaLbl
                    };

                    var kalanSureLbl = new Label
                    {
                        FontSize = 14,
                        TextColor = tema.WarningColor,
                        FontAttributes = FontAttributes.Bold
                    };
                    kalanSureLbl.SetBinding(Label.TextProperty, "KalanSure");

                    var contentStack = new StackLayout
                    {
                        Spacing = 8,
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children = { importanceDot, baslikLbl }
                            },
                            infoRow,
                            aciklamaFrame,
                            kalanSureLbl
                        }
                    };

                    return new Frame
                    {
                        CornerRadius = 16,
                        HasShadow = true,
                        Padding = new Thickness(14),
                        Margin = new Thickness(14, 10),
                        BackgroundColor = tema.CardColor,
                        BorderColor = tema.ButtonColor,
                        Content = contentStack
                    };
                })
            };

            // --- Ana Grid: başlık + liste ---
            var mainGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                BackgroundColor = tema.BackgroundColor
            };

            var lblTitle = new Label
            {
                Text = "📋 Aktif Görevler",
                FontSize = 24,
                TextColor = tema.AccentColor,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(20, 15, 10, 10)
            };
            Grid.SetRow(lblTitle, 0);
            Grid.SetRow(gorevListView, 1);

            mainGrid.Children.Add(lblTitle);
            mainGrid.Children.Add(gorevListView);

            Content = mainGrid;

            LoadGorevler();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (gorevler.Count == 0) return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var g in gorevler)
                {
                    g.OnPropertyChanged(nameof(g.KalanSure));
                }
            });
        }

        private async void LoadGorevler()
        {
            try
            {
                gorevler.Clear();
                using (var conn = bgl.Connection())
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT g.baslik, g.aciklama, g.onemlilik, 
                                          p.ad as calisan, g.baslangic_zamani, g.bitis_zamani 
                                   FROM gorevler g 
                                   JOIN personel_bilgi p ON g.calisan_id = p.id 
                                   WHERE g.bitis_zamani >= NOW()
                                   ORDER BY g.id DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string onem = reader["onemlilik"].ToString();
                            Color renk = onem switch
                            {
                                "Yüksek" => tema.DangerColor,
                                "Orta" => tema.WarningColor,
                                "Düşük" => tema.SuccessColor,
                                _ => tema.SecondaryText
                            };

                            gorevler.Add(new GorevModel
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
                gorevListView.ItemsSource = gorevler;
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }

    public class GorevModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime bitisZamani;
        public DateTime BitisZamani
        {
            get => bitisZamani;
            set
            {
                bitisZamani = value;
                OnPropertyChanged(nameof(KalanSure));
            }
        }

        public string Baslik { get; set; }
        public string Calisan { get; set; }
        public string Zaman { get; set; }
        public string Onemlilik { get; set; }
        public Color OnemlilikRenk { get; set; }
        public string Aciklama { get; set; }
        public string Durum { get; set; }

        public string KalanSure
        {
            get
            {
                var kalan = BitisZamani - DateTime.Now;
                if (kalan.TotalSeconds <= 0)
                    return "Süre doldu";
                if (kalan.TotalDays >= 1)
                    return $"{(int)kalan.TotalDays} gün {(int)kalan.Hours} saat kaldı";
                if (kalan.TotalHours >= 1)
                    return $"{(int)kalan.TotalHours} saat {(int)kalan.Minutes} dakika kaldı";
                return $"{(int)kalan.Minutes} dakika {(int)kalan.Seconds} saniye kaldı";
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}




