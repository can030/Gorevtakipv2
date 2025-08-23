using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gorevtakipv2.adminpencere
{
    public class aktifgorevler : ContentView
    {
        private CollectionView gorevListView;
        private sqlbaglanti bgl = new sqlbaglanti();

        public aktifgorevler()
        {
            Color bgDark = Color.FromArgb("#1C1B29");
            Color cardBg = Color.FromArgb("#2A2937");
            Color textColor = Colors.WhiteSmoke;

            gorevListView = new CollectionView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var baslikLbl = new Label
                    {
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White
                    };
                    baslikLbl.SetBinding(Label.TextProperty, "Baslik");

                    var calisanLbl = new Label
                    {
                        FontSize = 14,
                        TextColor = Colors.LightGray
                    };
                    calisanLbl.SetBinding(Label.TextProperty, "Calisan");

                    var zamanLbl = new Label
                    {
                        FontSize = 14,
                        TextColor = Colors.LightGray
                    };
                    zamanLbl.SetBinding(Label.TextProperty, "Zaman");

                    var onemlilikLbl = new Label
                    {
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold
                    };
                    onemlilikLbl.SetBinding(Label.TextProperty, "Onemlilik");
                    onemlilikLbl.SetBinding(Label.TextColorProperty, "OnemlilikRenk");

                    var aciklamaLbl = new Label
                    {
                        FontSize = 13,
                        TextColor = Colors.WhiteSmoke,
                        LineBreakMode = LineBreakMode.TailTruncation
                    };
                    aciklamaLbl.SetBinding(Label.TextProperty, "Aciklama");

                    return new Frame
                    {
                        CornerRadius = 12,
                        BackgroundColor = cardBg,
                        Margin = new Thickness(10, 5),
                        Content = new StackLayout
                        {
                            Spacing = 6,
                            Children =
                            {
                                baslikLbl,
                                calisanLbl,
                                zamanLbl,
                                onemlilikLbl,
                                aciklamaLbl
                            }
                        }
                    };
                })
            };

            Content = new StackLayout
            {
                BackgroundColor = bgDark,
                Children =
                {
                    new Label
                    {
                        Text = "📋 Aktif Görevler",
                        FontSize = 22,
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(15,10,10,5)
                    },
                    gorevListView
                }
            };

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
                                   WHERE g.bitis_zamani >= NOW()
                                   ORDER BY g.id DESC"; // 📌 en yeni görev en üstte

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

                            liste.Add(new GorevModel
                            {
                                Baslik = reader["baslik"].ToString(),
                                Calisan = "👤 " + reader["calisan"].ToString(),
                                Zaman = $"⏳ {Convert.ToDateTime(reader["baslangic_zamani"]):dd.MM.yyyy HH:mm} - {Convert.ToDateTime(reader["bitis_zamani"]):dd.MM.yyyy HH:mm}",
                                Onemlilik = "⚡ " + onem,
                                OnemlilikRenk = renk,
                                Aciklama = reader["aciklama"].ToString()
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

    public class GorevModel
    {
        public string Baslik { get; set; }
        public string Calisan { get; set; }
        public string Zaman { get; set; }
        public string Onemlilik { get; set; }
        public Color OnemlilikRenk { get; set; }
        public string Aciklama { get; set; }
    }
}

