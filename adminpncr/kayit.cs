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

        // ID tabanlı kullanıcı listesi
        private ObservableCollection<Kullanici> mevcutKullanicilar = new ObservableCollection<Kullanici>();

        public Kayit()
        {
            BackgroundColor = Color.FromArgb("#12121C");

            // --- Kayıt girişleri ---
            isimEntry = new Entry { Placeholder = "İsim", TextColor = Colors.White };
            soyadEntry = new Entry { Placeholder = "Soyad", TextColor = Colors.White };

            yetkiPicker = new Picker
            {
                Title = "Yetki Seçin",
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#1E1E2E")
            };
            yetkiPicker.Items.Add("Kullanıcı");
            yetkiPicker.Items.Add("Admin");
            yetkiPicker.SelectedIndex = 0;

            kaydetBtn = new Button
            {
                Text = "Üye Kaydet",
                BackgroundColor = Color.FromArgb("#28a745"),
                TextColor = Colors.White,
                CornerRadius = 12
            };
            kaydetBtn.Clicked += KaydetBtn_Clicked;

            // --- Mevcut kullanıcıları gösteren Picker ---
            silPicker = new Picker
            {
                Title = "Silmek için kullanıcı seçin",
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#1E1E2E"),
                ItemsSource = mevcutKullanicilar,
                ItemDisplayBinding = new Binding("AdSoyad") // Picker'da gösterilecek isim
            };

            silBtn = new Button
            {
                Text = "Üye Sil",
                BackgroundColor = Color.FromArgb("#dc3545"),
                TextColor = Colors.White,
                CornerRadius = 12
            };
            silBtn.Clicked += SilBtn_Clicked;

            // --- Card tasarım ---
            var card = new Frame
            {
                CornerRadius = 16,
                Padding = 20,
                BackgroundColor = Color.FromArgb("#1E1E2E"),
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
                            TextColor = Colors.White,
                            FontAttributes = FontAttributes.Bold
                        },
                        isimEntry,
                        soyadEntry,
                        yetkiPicker,
                        kaydetBtn,
                        new BoxView { HeightRequest = 2, Color = Color.FromArgb("#2E2E3E") },
                        new Label { Text="Mevcut Kullanıcılar", FontSize=18, TextColor=Colors.White },
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

            // Mevcut kullanıcıları yükle
            LoadKullanicilar();
        }

        // Kullanıcı sınıfı
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
                    var k = new Kullanici
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        AdSoyad = $"{reader["ad"]} {reader["soyad"]}"
                    };
                    mevcutKullanicilar.Add(k);
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

                // Listeyi güncelle
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

                // Listeyi güncelle
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