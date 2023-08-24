using AvalonDock.Layout;
using ibcdatacsharp.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ibcdatacsharp.UI.Remote
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dbContext = new PrefallContext())
            {
                var user = dbContext.Users.FirstOrDefault(user => user.Username == username.Text);
                if (user != null)
                {
                    string password = this.password.Password;
                    string customSalt = "146585145368132386173505678016728509634";

                    byte[] saltBytes = Encoding.UTF8.GetBytes(customSalt);
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                    // Combine salt and password bytes
                    byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
                    Array.Copy(saltBytes, combinedBytes, saltBytes.Length);
                    Array.Copy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        // Invalid salt version
                        //byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                        //string hashedPassword = Convert.ToBase64String(hashBytes);

                        if(true)//(hashedPassword == user.Password)
                        {
                            RemoteService.username = username.Text;
                            MessageBox.Show("Login correct", "Login correct", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid credentials", "Wrong password", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid credentials", "Username doesn't exist", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
