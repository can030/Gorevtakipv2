using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gorevtakipv2
{
    public class Login : ContentPage
    {
       public Login()
        {
            sqlbaglanti bgl = new sqlbaglanti();

            Image userIcon = new Image
            {
                Source = "user.png",
                WidthRequest = 150,
                HeightRequest = 150,
                VerticalOptions = LayoutOptions.Center
            };

            var emailEntry = new Entry
            {
                Placeholder = "Email",
                WidthRequest = 300,
                HeightRequest = 40,
                Margin = new Thickness(0, 10)
            };

            var passwordEntry = new Entry
            {
                Placeholder = "Şifre",
                IsPassword = true,
                WidthRequest = 300,
                HeightRequest = 40,
                Margin = new Thickness(0, 10)
            };

            var loginButton = new Button
            {
                Text = "Giriş Yap",
                WidthRequest = 200,
                HeightRequest = 60,
                FontSize = 20,
                Margin = new Thickness(0, 20)
            };

            loginButton.Clicked += async (s, e) =>
            {
                string email = emailEntry.Text?.Trim();
                string parola = passwordEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(parola))
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Email veya şifre boş olamaz!", "Tamam");
                    return;
                }

                try
                {
                    using (MySqlConnection conn = bgl.Connection())
                    {
                        await conn.OpenAsync();

                        string sql = "SELECT yetki, ad, soyad FROM personel_bilgi WHERE email=@pEmail AND parola=@pParola;";
                        using (MySqlCommand command = new MySqlCommand(sql, conn))
                        {
                            command.Parameters.AddWithValue("@pEmail", email);
                            command.Parameters.AddWithValue("@pParola", parola);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    string yetki = reader["yetki"].ToString();
                                    string ad = reader["ad"].ToString();
                                    string soyad = reader["soyad"].ToString();

                                    // 🔹 Session bilgilerini kaydet
                                    Session.AdSoyad = ad + " " + soyad;
                                    Session.Yetki = yetki;

                                    if (yetki == "admin")
                                        Application.Current.MainPage = new Admin();
                                    else
                                    {
                                        // Application.Current.MainPage = new KullaniciPage();

                                    }

                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert("Hata", "Email veya parola yanlış!", "Tamam");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", ex.Message, "Tamam");
                }
            };

            Content = new StackLayout
            {
                Padding = 10,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children = { userIcon, emailEntry, passwordEntry, loginButton }
            };


        }
    }
}

