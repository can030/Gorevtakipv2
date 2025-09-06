using CommunityToolkit.Maui.Markup;
using Gorevtakipv2.adminpncr;
using Microcharts;
using Microcharts.Maui;
using Microsoft.Maui.Controls;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gorevtakipv2.kullanicipncr;

namespace Gorevtakipv2.adminpencere
{
    public class IstatistikSayfasi : ContentView
    {
        public IstatistikSayfasi(ObservableCollection<GorevModel> gorevler)
        {
            gorevler ??= new ObservableCollection<GorevModel>();
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
                            new Label
                            {
                                Text = title,
                                FontSize = 14,
                                TextColor = tema.SecondaryText
                            },
                            new Label
                            {
                                Text = value,
                                FontSize = 22,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = color
                            }
                        }
                    }
                };
            }

            var toplamKpi = CreateKpiCard("Toplam Görev", gorevler.Count.ToString(), tema.TextColor);
            var tamamlananKpi = CreateKpiCard("Tamamlanan", gorevler.Count(g => g.Durum == "Tamamlandı").ToString(), tema.SuccessColor);
            var gecikenKpi = CreateKpiCard("Geç Kalan", gorevler.Count(g => g.KalanSure == "Süre doldu").ToString(), tema.DangerColor);

            var kpiGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star }
                },
                Margin = new Thickness(0, 10)
            };

            kpiGrid.Children.Add(toplamKpi); Grid.SetColumn(toplamKpi, 0);
            kpiGrid.Children.Add(tamamlananKpi); Grid.SetColumn(tamamlananKpi, 1);
            kpiGrid.Children.Add(gecikenKpi); Grid.SetColumn(gecikenKpi, 2);

            // --- Grafikler ---
            // Önemlilik dağılımı (Pie / Donut)
            var onemlilikData = gorevler
                .GroupBy(g => g.Onemlilik)
                .Select(gr => new Microcharts.ChartEntry(gr.Count())
                {
                    Label = gr.Key,
                    ValueLabel = gr.Count().ToString(),
                    Color = gr.Key switch
                    {
                        "Yüksek" => SKColor.Parse(tema.DangerColor.ToHex()),
                        "Orta" => SKColor.Parse(tema.WarningColor.ToHex()),
                        "Düşük" => SKColor.Parse(tema.SuccessColor.ToHex()),
                        _ => SKColor.Parse(tema.SecondaryText.ToHex())
                    }
                }).ToList();

            var onemlilikChart = new ChartView
            {
                HeightRequest = 200,
                Chart = new DonutChart
                {
                    Entries = onemlilikData,
                    BackgroundColor = SKColors.Transparent
                }
            };

            // Çalışana göre görev sayısı (Bar Chart)
            var calisanData = gorevler
                .GroupBy(g => g.Calisan)
                .Select(gr => new Microcharts.ChartEntry(gr.Count())
                {
                    Label = gr.Key,
                    ValueLabel = gr.Count().ToString(),
                    Color = SKColor.Parse(tema.AccentColor.ToHex())
                }).ToList();

            var calisanChart = new ChartView
            {
                HeightRequest = 200,
                Chart = new BarChart
                {
                    Entries = calisanData,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 18
                }
            };

            // Günlük tamamlanan görevler (Line Chart - Son 7 gün)
            var son7Gun = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .Select(tarih =>
                {
                    int count = gorevler.Count(g => g.Durum == "Tamamlandı" &&
                                                    g.BitisZamani.Date == tarih);
                    return new Microcharts.ChartEntry(count)
                    {
                        Label = tarih.ToString("dd.MM"),
                        ValueLabel = count.ToString(),
                        Color = SKColor.Parse(tema.AccentColor.ToHex())
                    };
                }).ToList();

            var gunlukChart = new ChartView
            {
                HeightRequest = 200,
                Chart = new LineChart
                {
                    Entries = son7Gun,
                    BackgroundColor = SKColors.Transparent,
                    LineMode = LineMode.Straight,
                    LineSize = 4,
                    PointMode = PointMode.Circle,
                    PointSize = 10
                }
            };

            // --- Scrollable içerik ---
            if (gorevler.Count == 0)
            {
                Content = new Grid
                {
                    BackgroundColor = tema.BackgroundColor,
                    Children =
            {
            new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 12,
                Children =
                {
                    
                    new Label
                    {
                        Text = "Sonuç bulunamadı",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = tema.SecondaryText,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Henüz görüntülenecek görev istatistiği yok.",
                        FontSize = 14,
                        TextColor = tema.TextColor,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        }
                };
            }
            else
            {
                Content = new ScrollView
                {
                    Content = new StackLayout
                    {
                        Padding = 16,
                        Spacing = 16,
                        Children =
            {
               
                new BoxView{ HeightRequest=2, Color=tema.ButtonColor },
                kpiGrid,

                new Label{ Text="Önemlilik Dağılımı", FontSize=20, TextColor=tema.TextColor },
                onemlilikChart,

                new Label{ Text="Çalışanlara Göre Görevler", FontSize=20, TextColor=tema.TextColor },
                calisanChart,

                new Label{ Text="Son 7 Gün Tamamlanan Görevler", FontSize=20, TextColor=tema.TextColor },
                gunlukChart
            }
                    }
                };
            }
        }
    }
}