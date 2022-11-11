using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClothesShopCursovaya.Extensions
{
    class Helper
    {
        public string connect = "Host=localhost;Port=5432;Database=ClothesShop;Username=postgres;Password=123";
        public string Encrypt(string decrypted)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(decrypted);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider();
            tripDes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("123"));
            tripDes.Mode = CipherMode.ECB;
            ICryptoTransform transform = tripDes.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(result);
        }
        public string Decrypt(string encrypted)
        {
            byte[] data = Convert.FromBase64String(encrypted);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider();
            tripDes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("123"));
            tripDes.Mode = CipherMode.ECB;
            ICryptoTransform transform = tripDes.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
            return UTF8Encoding.UTF8.GetString(result);
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                MessageBox.Show("Неверный формат почты", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }
        public bool IsNull(string text)
        {
            if(text.Replace(" ","")=="")
            {
                MessageBox.Show("Заполните данные","", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        public bool IsValidPassword(string pswd)
        {
           
            bool IsSmallLetter(char c)
            {
                if (Char.IsLetter(c))
                {
                    return c.ToString().ToLower() == c.ToString();
                }

                else return false;
            }

            bool b1 = pswd.Length >= 8;

            bool b2 = false;
            foreach (char c in pswd)
            {
                if (IsSmallLetter(c))
                {
                    b2 = true;
                    break;
                }
            }

            bool b3 = false;
            foreach (char c in pswd)
            {
                if (Char.IsDigit(c))
                {
                    b3 = true;
                    break;
                }
            }

            bool b4 = false;
            foreach (char c in pswd)
            {
                if (c == '!' || c == '@' || c == '#' || c == '$' || c == '%' || c == '^')
                {
                    b4 = true;
                    break;
                }
            }

            return b1 && b2 && b3 && b4;
        }

        public bool IsAngl(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'Z')
                return true;
            if (c >= '!' && c <= '=')
                return true;
            if (c >= '_' && c <= '_')
                return true;
            if (c >= '@' && c <= '@')
                return true;
            return false;
        }

        public bool IsRus(char c)
        {
            if (c >= 'а' && c <= 'я')
                return true;
            if (c >= 'А' && c <= 'Я')
                return true;
            if (c >= 'a' && c <= 'z')
                return true;
            if (c >= 'A' && c <= 'Z')
                return true;
            return false;
        }
        public bool Isnum(char c)
        {
            if (c >= '0' && c <= '9')
                return true;

            return false;
        }
    }
}
