using Microcharts;
using Microcharts.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MySql.Data.MySqlClient;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Gorevtakipv2.adminpncr;
using Gorevtakipv2.adminpencere;




namespace Gorevtakipv2.kullanicipncr
{
    public class KlncIstatistik : ContentView
    {
        private ObservableCollection<GorevModel> gorevler;

        public KlncIstatistik(string kullaniciAdi)
        {
            gorevler = new ObservableCollection<GorevModel>();

            LoadGorevler(kullaniciAdi);

            BackgroundColor = tema.BackgroundColor;

            // --- KPI Kartları ---
            Frame CreateKpiCard(string title, string value, Color color)
            {
                return new Frame
                {
                    CornerRadius = 16,
                    Padding = new Thickness(16),
                    Margin = new Thickness(6, 0),
                    BackgroundColor = tema.CardColor,
                    HasShadow = true,
                    Content = new StackLayout
                    {
                        Spacing = 6,
                        Children =
                        {
                            new Label { Text = title, FontSize = 14, TextColor = tema.SecondaryText },
                            new Label { Text = value, FontSize = 22, FontAttributes = FontAttributes.Bold, TextColor = color }
                        }
                    }
                };
            }

            var toplamKpi = CreateKpiCard("Toplam Görev", gorevler.Count.ToString(), tema.TextColor);
            var tamamlananKpi = CreateKpiCard("Tamamlanan", gorevler.Count(g => g.Durum == "Tamamlandı").ToString(), tema.SuccessColor);
            var gecikenKpi = CreateKpiCard("Geç Kalan", gorevler.Count(g => (g.BitisZamani - DateTime.Now).TotalSeconds <= 0).ToString(), tema.DangerColor);

            var kpiGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                Margin = new Thickness(0, 10)
            };
            kpiGrid.Children.Add(toplamKpi); Grid.SetColumn(toplamKpi, 0);
            kpiGrid.Children.Add(tamamlananKpi); Grid.SetColumn(tamamlananKpi, 1);
            kpiGrid.Children.Add(gecikenKpi); Grid.SetColumn(gecikenKpi, 2);

            // --- Görev Listesi ---
            var gorevListView = new CollectionView
            {
                ItemsSource = gorevler,
                ItemTemplate = new DataTemplate(() =>
                {
                    var importanceDot = new BoxView { WidthRequest = 12, HeightRequest = 12, CornerRadius = 6, Margin = new Thickness(0, 4, 6, 0) };
                    importanceDot.SetBinding(BoxView.ColorProperty, "OnemlilikRenk");

                    var baslikLbl = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = tema.TextColor };
                    baslikLbl.SetBinding(Label.TextProperty, "Baslik");

                    var zamanLbl = new Label { FontSize = 14, TextColor = tema.SecondaryText };
                    zamanLbl.SetBinding(Label.TextProperty, "Zaman");

                    var onemLbl = new Label { FontSize = 14, FontAttributes = FontAttributes.Bold };
                    onemLbl.SetBinding(Label.TextProperty, "Onemlilik");
                    onemLbl.SetBinding(Label.TextColorProperty, "OnemlilikRenk");

                    var aciklamaLbl = new Label { FontSize = 14, TextColor = tema.SecondaryText, LineBreakMode = LineBreakMode.WordWrap, MaxLines = 3 };
                    aciklamaLbl.SetBinding(Label.TextProperty, "Aciklama");

                    var contentStack = new StackLayout
                    {
                        Spacing = 6,
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children = { importanceDot, baslikLbl }
                            },
                            zamanLbl,
                            onemLbl,
                            aciklamaLbl
                        }
                    };

                    return new Frame
                    {
                        CornerRadius = 12,
                        HasShadow = true,
                        Padding = 12,
                        Margin = new Thickness(12, 8),
                        BackgroundColor = tema.CardColor,
                        Content = contentStack
                    };
                }),
                EmptyView = new Label
                {
                    Text = "Henüz görev bulunamadı.",
                    FontSize = 16,
                    TextColor = tema.SecondaryText,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                }
            };

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 16,
                    Spacing = 16,
                    Children =
                    {
                        new Label { Text="Görev İstatistikleri", FontSize=26, FontAttributes=FontAttributes.Bold, TextColor=tema.AccentColor },
                        kpiGrid,
                        new Label { Text="Görevler", FontSize=20, TextColor=tema.TextColor },
                        gorevListView
                    }
                }
            };
        }

        private void LoadGorevler(string kullaniciAdi)
        {
            gorevler.Clear();

            using var conn = new sqlbaglanti().Connection();
            conn.Open();
            string sql = @"SELECT g.baslik, g.aciklama, g.onemlilik, g.durum, g.baslangic_zamani, g.bitis_zamani
                           FROM gorevler g
                           JOIN personel_bilgi p ON g.calisan_id = p.id
                           WHERE p.ad = @ad
                           ORDER BY g.id DESC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ad", kullaniciAdi);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string onem = reader["onemlilik"].ToString();
                Color renk = onem switch
                {
                    "Yüksek" => Colors.OrangeRed,
                    "Orta" => Colors.Gold,
                    "Düşük" => Colors.LightGreen,
                    _ => Colors.LightGray
                };

                DateTime baslangic = Convert.ToDateTime(reader["baslangic_zamani"]);
                DateTime bitis = Convert.ToDateTime(reader["bitis_zamani"]);

                gorevler.Add(new GorevModel
                {
                    Baslik = reader["baslik"].ToString(),
                    Aciklama = reader["aciklama"].ToString(),
                    Onemlilik = onem,
                    OnemlilikRenk = renk,
                    Durum = reader["durum"].ToString(),
                    BaslangicZamani = baslangic,
                    BitisZamani = bitis,
                    Zaman = $"{baslangic:dd.MM.yyyy HH:mm} - {bitis:dd.MM.yyyy HH:mm}"
                });
            }
        }
    }
}