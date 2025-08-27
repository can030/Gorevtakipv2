using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;
using Gorevtakipv2.adminpncr;

namespace Gorevtakipv2.adminpencere
{
    public class gecmis : ContentView
    {
        private CollectionView gorevListView;
        private sqlbaglanti bgl = new sqlbaglanti();

        public gecmis()
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

                    var calisanLbl = new Label
                    {
                        FontSize = 15,
                        TextColor = tema.SecondaryText
                    };
                    calisanLbl.SetBinding(Label.TextProperty, "Calisan");

                    var zamanLbl = new Label
                    {
                        FontSize = 15,
                        TextColor = tema.SecondaryText
                    };
                    zamanLbl.SetBinding(Label.TextProperty, "Zaman");

                    var onemLbl = new Label
                    {
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold
                    };
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
                                Children = { new Label{Text="👤", TextColor = tema.TextColor}, calisanLbl }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label{Text="⏳", TextColor = tema.TextColor}, zamanLbl }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 4,
                                Children = { new Label{Text="⚡", TextColor = tema.TextColor}, onemLbl }
                            }
                        }
                    };

                    var aciklamaLbl = new Label
                    {
                        FontSize = 15,
                        TextColor = tema.SecondaryText,
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

                    var bittiLbl = new Label
                    {
                        FontSize = 14,
                        TextColor = tema.SuccessColor,
                        FontAttributes = FontAttributes.Bold,
                        Text = "✔ Görev tamamlandı"
                    };

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
                            bittiLbl
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
                Text = "📜 Geçmiş Görevler",
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
        }

        private async void LoadGorevler()
        {
            try
            {
                var liste = new List<GorevModel>();
                using (var conn = bgl.Connection())
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT g.baslik, g.aciklama, g.onemlilik, 
                                          p.ad as calisan, g.baslangic_zamani, g.bitis_zamani 
                                   FROM gorevler g 
                                   JOIN personel_bilgi p ON g.calisan_id = p.id 
                                   WHERE g.bitis_zamani < NOW()
                                   ORDER BY g.bitis_zamani DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string onem = reader["onemlilik"].ToString();
                            Color renk = Colors.LightGray;
                            if (onem == "Yüksek") renk = tema.DangerColor;
                            else if (onem == "Orta") renk = tema.WarningColor;
                            else if (onem == "Düşük") renk = tema.SuccessColor;

                            liste.Add(new GorevModel
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
                gorevListView.ItemsSource = liste;
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}