using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;


namespace LoginApp
{
    class GirisYap
    {
        string connectionString = "Data Source=(local);Initial Catalog=FirmaVerileri;" + "Integrated Security=true";
        string queryString = "SELECT * FROM Kullanici";
        DataTable dtKullanici = new DataTable();
        SqlDataReader reader;
        string Joutput;
        JArray Jtable;

        public GirisYap(string KullaniciAdi, string Sifre)
        {
            
            string con = this.connectionString;
            SqlConnection connection = new SqlConnection(con);
            SqlCommand command = new SqlCommand(this.queryString, connection);
           

                this.dtKullanici.Columns.Add("KullaniciID", typeof(int));
                this.dtKullanici.Columns.Add("KullaniciAdi", typeof(string));
                this.dtKullanici.Columns.Add("Sifre", typeof(string));
                connection.Open();
                this.reader = command.ExecuteReader();
                while (this.reader.Read())
                {
                    DataRow dtrNew = dtKullanici.NewRow();
                    dtrNew["KullaniciID"] = reader["KullaniciID"];
                    dtrNew["KullaniciAdi"] = reader["KullaniciAdi"];
                    dtrNew["Sifre"] = reader["Sifre"];
                    this.dtKullanici.Rows.Add(dtrNew);
                }
                this.reader.Close();
                this.Joutput = JsonConvert.SerializeObject(dtKullanici);
                this.Jtable = JArray.Parse(Joutput);

                foreach (var item in Jtable)
                {
                    if (KullaniciAdi == (string)item["KullaniciAdi"] && Sifre == (string)item["Sifre"])
                    {
                        connection.Close();
                    }
                }
            }
           
        }
    }

