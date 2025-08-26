using CommunityToolkit.Maui.Markup;
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

namespace Gorevtakipv2.adminpencere
{
    public class IstatistikSayfasi : ContentView
    {
        public IstatistikSayfasi(ObservableCollection<GorevModel> gorevler)
        {
            gorevler ??= new ObservableCollection<GorevModel>();
            BackgroundColor = Color.FromArgb("#12121C");

            // --- KPI Kartları ---
            Frame CreateKpiCard(string title, string value, Color color)
            {
                return new Frame
                {
                    CornerRadius = 14,
                    Padding = new Thickness(14),
                    Margin = new Thickness(6, 0),
                    BackgroundColor = Color.FromArgb("#1E1E2E"),
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
                                TextColor = Colors.Silver
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

            // --- KPI Değerleri ---
            var toplamKpi = CreateKpiCard("Toplam Görev", gorevler.Count.ToString(), Colors.White);
            var tamamlananKpi = CreateKpiCard("Tamamlanan", gorevler.Count(g => g.Durum == "Tamamlandı").ToString(), Colors.LightGreen);
            var gecikenKpi = CreateKpiCard("Geç Kalan", gorevler.Count(g => g.KalanSure == "Süre doldu").ToString(), Colors.OrangeRed);

            var kpiGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star }
                },
                Margin = new Thickness(0, 10, 0, 10)
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
                        "Yüksek" => SKColor.Parse("#FF4500"),
                        "Orta" => SKColor.Parse("#FFD700"),
                        "Düşük" => SKColor.Parse("#32CD32"),
                        _ => SKColor.Parse("#808080")
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
                    Color = SKColor.Parse("#1E90FF")
                }).ToList();

            var calisanChart = new ChartView
            {
                HeightRequest = 200,
                Chart = new BarChart
                {
                    Entries = calisanData,
                    BackgroundColor = SKColors.Transparent
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
                        Color = SKColor.Parse("#00CED1")
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
                    LineSize = 6,
                    PointMode = PointMode.Circle,
                    PointSize = 14
                }
            };

            // --- Scrollable içerik ---
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 16,
                    Spacing = 16,
                    Children =
                    {
                        new Label
                        {
                            Text = "📊 Görev İstatistikleri",
                            FontSize = 26,
                            TextColor = Color.FromArgb("#FFBF00"),
                            FontAttributes = FontAttributes.Bold
                        },
                        new BoxView{ HeightRequest=2, Color=Color.FromArgb("#2E2E3E") },
                        kpiGrid,

                        new Label{ Text="Önemlilik Dağılımı", FontSize=20, TextColor=Colors.White },
                        onemlilikChart,

                        new Label{ Text="Çalışanlara Göre Görevler", FontSize=20, TextColor=Colors.White },
                        calisanChart,

                        new Label{ Text="Son 7 Gün Tamamlanan Görevler", FontSize=20, TextColor=Colors.White },
                        gunlukChart
                    }
                }
            };
        }
    }
}