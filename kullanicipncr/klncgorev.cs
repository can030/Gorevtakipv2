using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;
using Gorevtakipv2.adminpencere;


using System;
using System.Collections.ObjectModel;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Gorevtakipv2.kullanicipncr
{
    public class Klncgorev : ContentView
    {
        private CollectionView gorevListView;
        private ObservableCollection<GorevModel> gorevler = new ObservableCollection<GorevModel>();
        private System.Timers.Timer timer;

        public Klncgorev()
        {
            BackgroundColor = adminpncr.tema.BackgroundColor;

            // --- CollectionView tanımı ---
            gorevListView = new CollectionView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    Padding = 15;
                    var importanceDot = new BoxView
                    {
                        WidthRequest = 12,
                        HeightRequest = 12,
                        CornerRadius = 10,
                        
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Start,
                        Margin = new Thickness(0, 4, 6, 0)
                        
                    };
                    importanceDot.SetBinding(BoxView.ColorProperty, "OnemlilikRenk");

                    var baslikLbl = new Label
                    {
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = adminpncr.tema.TextColor
                    };
                    baslikLbl.SetBinding(Label.TextProperty, "Baslik");

                    var zamanLbl = new Label { FontSize = 15, TextColor = adminpncr.tema.SecondaryText };
                    zamanLbl.SetBinding(Label.TextProperty, "Zaman");

                    var onemLbl = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
                    onemLbl.SetBinding(Label.TextProperty, "Onemlilik");
                    onemLbl.SetBinding(Label.TextColorProperty, "OnemlilikRenk");

                    var infoRow = new StackLayout
                    {
                        
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 40,
                       
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label { Text="⏳", TextColor=adminpncr.tema.SecondaryText }, zamanLbl }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label { Text="⚡", TextColor=adminpncr.tema.SecondaryText }, onemLbl }
                            }
                        }
                    };

                    var aciklamaLbl = new Label
                    {
                        FontSize = 15,
                        TextColor = adminpncr.tema.TextColor,
                        LineBreakMode = LineBreakMode.WordWrap,
                        MaxLines = 3
                    };
                    aciklamaLbl.SetBinding(Label.TextProperty, "Aciklama");

                    var aciklamaFrame = new Frame
                    {
                        Padding = new Thickness(10, 6),
                        BackgroundColor = adminpncr.tema.CardColor,
                        CornerRadius = 10,
                        HasShadow = false,
                        Content = aciklamaLbl
                    };

                    var kalanSureLbl = new Label
                    {
                        FontSize = 14,
                        TextColor = adminpncr.tema.WarningColor,
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
                        BackgroundColor = adminpncr.tema.CardColor,
                        BorderColor = adminpncr.tema.ButtonColor,
                        Content = contentStack
                    };
                }),

                EmptyView = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 12,
                    Children =
                    {
                        new Image
                        {
                            Source = "empty.png",
                            HeightRequest = 120,
                            Opacity = 0.6
                        },
                        new Label
                        {
                            Text = "Henüz görev bulunamadı",
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = adminpncr.tema.SecondaryText,
                            HorizontalTextAlignment = TextAlignment.Center
                        },
                        new Label
                        {
                            Text = "Sana atanmış görev yok.",
                            FontSize = 14,
                            TextColor = adminpncr.tema.TextColor,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    }
                }
            };

            Content = gorevListView;

            LoadGorevler();

            // Kalan süreyi her saniye güncelle
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
                    g.OnPropertyChanged(nameof(g.KalanSure));
            });
        }

        private async void LoadGorevler()
        {
            try
            {
                gorevler.Clear();
                using (var conn = new sqlbaglanti().Connection())
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT g.baslik, g.aciklama, g.onemlilik, 
                                          g.baslangic_zamani, g.bitis_zamani
                                   FROM gorevler g
                                   WHERE g.calisan_id = @kullaniciId
                                   ORDER BY g.baslangic_zamani DESC";

                    using var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@kullaniciId", Session.ID);

                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        string onem = reader["onemlilik"].ToString();
                        Color renk = onem switch
                        {
                            "Yüksek" => adminpncr.tema.DangerColor,
                            "Orta" => adminpncr.tema.WarningColor,
                            "Düşük" => adminpncr.tema.SuccessColor,
                            _ => adminpncr.tema.SecondaryText
                        };

                        gorevler.Add(new GorevModel
                        {
                            Baslik = reader["baslik"].ToString(),
                            Zaman = $"{Convert.ToDateTime(reader["baslangic_zamani"]):dd.MM.yyyy HH:mm} - {Convert.ToDateTime(reader["bitis_zamani"]):dd.MM.yyyy HH:mm}",
                            Onemlilik = onem,
                            OnemlilikRenk = renk,
                            Aciklama = reader["aciklama"].ToString(),
                            BaslangicZamani = Convert.ToDateTime(reader["baslangic_zamani"]),
                            BitisZamani = Convert.ToDateTime(reader["bitis_zamani"])
                        });
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

        public DateTime BaslangicZamani { get; set; }
        public string Baslik { get; set; }
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