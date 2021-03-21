﻿using CardMarket_Web_Core.ApiQueryLogic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace CardMarket_Web_Core.DbCode
{
    public class DAOMySQL : IDAO
    {
        private string myConnectionString;
        MySqlConnection conn;

        public DAOMySQL(string _s)
        {
            myConnectionString = _s;
        }

        public void AddProduct(ProductObj productobj)
        {
            string sqlStatement = "insert into Product (cardname, productid, metaproductid, expansionname) " +
                "values(@cardname, @productid, @metaproductid,@expansionname)";

            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                foreach (Product p in productobj.product)
                {
                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                    command.Parameters.AddWithValue("@cardname", p.enName);
                    command.Parameters.AddWithValue("@productid", p.idProduct);
                    command.Parameters.AddWithValue("@metaproductid", p.idMetaproduct);
                    command.Parameters.AddWithValue("@expansionname", p.expansionName);

                    command.ExecuteNonQuery();
                    Console.WriteLine("try addproduct ");
                }
            }
        }

        public bool HasProductInfo(string cardName, out ProductObj retProducObj)
        {
            //Console.WriteLine("HasProductInfo: " + cardName);
            //Console.WriteLine(config.GetConnectionString("DefaultConnection"));

            string sqlStatement = "select * from Product where cardname = @cardname";
            retProducObj = new ProductObj();

            using (conn = new MySqlConnection(myConnectionString))
            {
                conn.Open();

                MySqlCommand myCommand = new MySqlCommand(sqlStatement, conn);

                myCommand.Parameters.AddWithValue("@cardname", cardName);

                MySqlDataReader myReader;

                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    //Console.WriteLine(myReader["cardname"]);

                    Product p = new Product()
                    {
                        enName = myReader["cardname"].ToString().Trim(),
                        idProduct = (int)myReader["productid"],
                        idMetaproduct = (int)myReader["metaproductid"],
                        expansionName = myReader["expansionname"].ToString().Trim()
                    };

                    retProducObj.product.Add(p);
                }

                if (retProducObj != null && retProducObj.product.Count > 0)
                    return true;
                else return false;
            }
        }
    }
}