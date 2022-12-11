using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClothesShopCursovaya;
using Npgsql;

namespace ClothesShopCursovaya.Test
{
    [TestClass]
    public class DbTests
    {
        string name = "";
        string adress = "";
        NpgsqlConnection connect = new NpgsqlConnection ("Host=localhost;Port=5432;Database=ClothesShop;Username=postgres;Password=123");
        
        [TestMethod]
        public void SupplierAddTest_NameAdress_1()
        {
            //arrange
            connect.Open();
            name = "biba1";
            adress = "bebs1";
            
            string expected = "1";
            //act
            new NpgsqlCommand($@"call Supplier_Insert('{name}','{adress}')", connect).ExecuteNonQuery();
            string actual = new NpgsqlCommand($"Select count(*) from Supplier where company_name='{name}' and legal_address='{adress}'", connect).ExecuteScalar().ToString();
            //assert
            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void SupplierUpdateTest_NameAdress_1()
        {
            connect.Open();
            name = "biba1";
            adress = "bebs1";
            int id = 0;

            NpgsqlDataReader dataReader = null;
            dataReader = new NpgsqlCommand($"Select ID_supplier from Supplier where company_name='{name}' and legal_address='{adress}'", connect).ExecuteReader();
            while (dataReader.Read())
                id = (int)dataReader["ID_supplier"];
            dataReader.Close();

            name = "biba228";
            adress = "bebs228";
            string expected = "1";
            new NpgsqlCommand($@"call Supplier_Update('{id}','{name}','{adress}')", connect).ExecuteNonQuery();
            string actual = new NpgsqlCommand($"Select count(*) from Supplier where company_name='{name}' and legal_address='{adress}'", connect).ExecuteScalar().ToString();
            //assert
            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void SupplierZeleteTest_NameAdress_0()
        {
            connect.Open();
            name = "biba228";
            adress = "bebs228";
            int id = 0;
           string expected = "0";

            NpgsqlDataReader dataReader = null;
            dataReader = new NpgsqlCommand($"Select ID_supplier from Supplier where company_name='{name}' and legal_address='{adress}'", connect).ExecuteReader();
            while (dataReader.Read())
                id = (int)dataReader["ID_supplier"];
            dataReader.Close();

            new NpgsqlCommand($@"call Supplier_Delete('{id}')", connect).ExecuteNonQuery();
            string actual = new NpgsqlCommand($"Select count(*) from Supplier where company_name='{name}' and legal_address='{adress}'", connect).ExecuteScalar().ToString();
            //assert
            Assert.AreEqual(expected, actual);

        }
    }
}
