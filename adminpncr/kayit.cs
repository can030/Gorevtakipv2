using Gorevtakipv2.adminpncr;
using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gorevtakipv2.adminpencere
{
    public class Kayit : ContentView
    {
        private Entry isimEntry;
        private Entry soyadEntry;
        private Picker yetkiPicker;
        private Picker silPicker;
        private Button kaydetBtn;
        private Button silBtn;

        private sqlbaglanti bgl = new sqlbaglanti();
        private ObservableCollection<Kullanici> mevcutKullanicilar = new ObservableCollection<Kullanici>();

        public Kayit()
        {
            BackgroundColor = tema.BackgroundColor;

            // --- Kayıt girişleri ---
            isimEntry = new Entry
            {
                Placeholder = "İsim",
                PlaceholderColor = tema.TextColor,
                TextColor = tema.TextColor,
                BackgroundColor = tema.EntryBackground
            };

            soyadEntry = new Entry
            {
                Placeholder = "Soyad",
                PlaceholderColor = tema.TextColor,
                TextColor = tema.TextColor,
                BackgroundColor = tema.EntryBackground
            };

            yetkiPicker = new Picker
            {
                Title = "Yetki Seçin",
                TitleColor = tema.TextColor,
                TextColor = tema.TextColor,
                BackgroundColor = tema.EntryBackground
            };
            yetkiPicker.Items.Add("Kullanıcı");
            yetkiPicker.Items.Add("Admin");
            yetkiPicker.SelectedIndex = 0;

            kaydetBtn = new Button
            {
                Text = "Üye Kaydet",
                BackgroundColor = tema.SuccessColor,
                TextColor = Colors.White,
                CornerRadius = 12,
                BorderColor = tema.IsDark ? Colors.White : Color.FromArgb("#D1D5DB"),
                BorderWidth = 1,
                Shadow = tema.IsDark ? null : new Shadow
                {
                    Brush = Brush.Black,
                    Opacity = 0.05f,
                    Radius = 4,
                    Offset = new Point(2, 2)
                }
            };
            kaydetBtn.Clicked += KaydetBtn_Clicked;

            silPicker = new Picker
            {
                Title = "Silmek için kullanıcı seçin",
                TitleColor = tema.TextColor,
                TextColor = tema.TextColor,
                BackgroundColor = tema.EntryBackground,
                ItemsSource = mevcutKullanicilar,
                ItemDisplayBinding = new Binding("AdSoyad"),
                
            };

            silBtn = new Button
            {
                Text = "Üye Sil",
                BackgroundColor = tema.DangerColor,
                TextColor = Colors.White,
                CornerRadius = 12
            };
            silBtn.Clicked += SilBtn_Clicked;

            // --- Card ---
            var card = new Frame
            {
                CornerRadius = 16,
                Padding = 20,
                BackgroundColor = tema.CardColor,
                HasShadow = true,
                Content = new StackLayout
                {
                    Spacing = 14,
                    Children =
                    {
                        new Label
                        {
                            Text = "Üye Kayıt / Sil",
                            FontSize = 24,
                            TextColor = tema.TextColor,
                            FontAttributes = FontAttributes.Bold
                        },
                        isimEntry,
                        soyadEntry,
                        yetkiPicker,
                        kaydetBtn,
                        new BoxView { HeightRequest = 2, Color = tema.SecondaryText },
                        new Label 
                        { 
                            Text="Mevcut Kullanıcılar",
                            FontSize=18, 
                            TextColor=tema.TextColor },
                        silPicker,
                        silBtn
                    }
                }
            };

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 16,
                    Children = { card }
                }
            };

            LoadKullanicilar();
        }

        public class Kullanici
        {
            public int Id { get; set; }
            public string AdSoyad { get; set; }
        }

        private async void LoadKullanicilar()
        {
            mevcutKullanicilar.Clear();
            try
            {
                using var conn = bgl.Connection();
                await conn.OpenAsync();

                string sql = "SELECT id, ad, soyad FROM personel_bilgi ORDER BY ad";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    mevcutKullanicilar.Add(new Kullanici
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        AdSoyad = $"{reader["ad"]} {reader["soyad"]}"
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private async void KaydetBtn_Clicked(object sender, EventArgs e)
        {
            string isim = isimEntry.Text?.Trim();
            string soyad = soyadEntry.Text?.Trim();
            string yetki = yetkiPicker.SelectedItem?.ToString() ?? "Kullanıcı";

            if (string.IsNullOrWhiteSpace(isim) || string.IsNullOrWhiteSpace(soyad))
            {
                await DisplayAlert("Hata", "İsim ve Soyad boş olamaz!", "Tamam");
                return;
            }

            try
            {
                using var conn = bgl.Connection();
                await conn.OpenAsync();

                string sql = "INSERT INTO personel_bilgi (ad, soyad, yetki) VALUES (@isim, @soyad, @yetki)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@isim", isim);
                cmd.Parameters.AddWithValue("@soyad", soyad);
                cmd.Parameters.AddWithValue("@yetki", yetki);

                await cmd.ExecuteNonQueryAsync();

                await DisplayAlert("Başarılı", $"{yetki} yetkili üye kaydedildi.", "Tamam");

                isimEntry.Text = "";
                soyadEntry.Text = "";
                yetkiPicker.SelectedIndex = 0;

                LoadKullanicilar();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private async void SilBtn_Clicked(object sender, EventArgs e)
        {
            
            var secilenKullanici = silPicker.SelectedItem as Kullanici;
            if (secilenKullanici == null)
            {
                await DisplayAlert("Hata", "Lütfen silmek için bir kullanıcı seçin!", "Tamam");
                return;
            }

            try
            {
                using var conn = bgl.Connection();
                await conn.OpenAsync();

                string sql = "DELETE FROM personel_bilgi WHERE id=@id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", secilenKullanici.Id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                    await DisplayAlert("Başarılı", "Üye silindi.", "Tamam");
                else
                    await DisplayAlert("Bilgi", "Bu kullanıcı bulunamadı.", "Tamam");

                LoadKullanicilar();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }

        private Task DisplayAlert(string title, string message, string cancel)
        {
            if (Application.Current?.MainPage != null)
                return Application.Current.MainPage.DisplayAlert(title, message, cancel);

            return Task.CompletedTask;
        }
    }
}