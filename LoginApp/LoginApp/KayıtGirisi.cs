using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace LoginApp
{
    class KayıtGirisi
    {
        public string connectionString = "Data Source=(local);Initial Catalog=FirmaVerileri;" + "Integrated Security=true";
        public SqlConnection connection;
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataSet dsPersonel;
        public JArray jsonArray;
        public string output;

        public JArray JSONEkle(string Ad,string Soyad,string Adres,string Eposta,string Telefon,string Gorev) {

            this.jsonArray = new JArray();
            this.output = JsonConvert.SerializeObject(dsPersonel);
            JObject personel = JObject.Parse(output);
            this.jsonArray = (JArray)personel["Table"];

            JObject newObject = new JObject(
              new JProperty("PersonalID", jsonArray.Count + 1),
              new JProperty("Ad", Ad),
              new JProperty("Soyad", Soyad),
              new JProperty("Adres", Adres),
              new JProperty("Eposta", Eposta),
              new JProperty("Telefon", Telefon),
              new JProperty("Görev", Gorev));

            jsonArray.Add(newObject);
            return this.jsonArray;
        }

        public JArray JSONSil(string index) {
            this.jsonArray = new JArray();
            this.output = JsonConvert.SerializeObject(dsPersonel);
            JObject personel = JObject.Parse(output);
            this.jsonArray = (JArray)personel["Table"];

            int selectedIndex = Convert.ToInt32(index);
            this.jsonArray[selectedIndex].Remove();
            return this.jsonArray;
        }

        public JArray JSONDuzenle(DataGridViewRow dgvr,int index) {
            this.jsonArray = new JArray();
            this.output = JsonConvert.SerializeObject(dsPersonel);
            JObject personel = JObject.Parse(output);
            this.jsonArray = (JArray)personel["Table"];
          
            this.jsonArray[index]["Ad"] = (string)dgvr.Cells[1].Value;
            this.jsonArray[index]["Soyad"] = (string)dgvr.Cells[2].Value;
            this.jsonArray[index]["Adres"] = (string)dgvr.Cells[3].Value;
            this.jsonArray[index]["Telefon"] = (string)dgvr.Cells[4].Value;
            this.jsonArray[index]["Eposta"] = (string)dgvr.Cells[5].Value;
            this.jsonArray[index]["Görev"] = (string)dgvr.Cells[6].Value;
            return this.jsonArray;
        }

        public DataTable JSONDonustur(JArray array) {
            string json = JsonConvert.SerializeObject(array);
            var result = toDataTable(json);
            return result;
        }

        public DataTable KayitGetir(){      
            this.connection = new SqlConnection(this.connectionString);
            this.dsPersonel = new DataSet();
            this.command = new SqlCommand("SELECT * FROM Personel", connection);
            command.CommandType = CommandType.Text;
            this.adapter = new SqlDataAdapter(command);

            this.connection.Open();
            this.command.ExecuteNonQuery();
            this.adapter.Fill(dsPersonel);
            this.connection.Close();
            return dsPersonel.Tables[0];
        }
        
        public void SQLEkle(string Ad, string Soyad, string Adres, string Eposta, string Telefon, string Gorev)
        {
             this.connection = new SqlConnection(this.connectionString);
             this.command = new SqlCommand("INSERT INTO Personel(Ad,Soyad,Adres,Telefon,Eposta,Görev) VALUES (@Ad,@Soyad,@Adres,@Telefon,@Eposta,@Görev)", this.connection);
             this.command.CommandType = CommandType.Text;
             this.command.Parameters.AddWithValue("@Ad",Ad);
             this.command.Parameters.AddWithValue("@Soyad",Soyad);
             this.command.Parameters.AddWithValue("@Adres",Adres);
             this.command.Parameters.AddWithValue("@Telefon",Telefon);
             this.command.Parameters.AddWithValue("@Eposta",Eposta);
             this.command.Parameters.AddWithValue("@Görev",Gorev);
             this.connection.Open();
             int rowsaffected = this.command.ExecuteNonQuery();
             this.connection.Close();
            
        }
        
        public void SQLSil(string ID) {

              this.connection = new SqlConnection(this.connectionString);
              this.command = new SqlCommand("DELETE FROM Personel WHERE PersonalID = @ID", this.connection);
              this.command.CommandType = CommandType.Text;
              this.command.Parameters.AddWithValue("@ID", ID);
              this.connection.Open();
              int rowsaffected = this.command.ExecuteNonQuery();
              this.connection.Close();
        }

        public void SQLDuzenle(DataGridViewRow dgvr,int id) {
            this.connection = new SqlConnection(this.connectionString);
            this.connection.Open();
            this.command = new SqlCommand("UPDATE Personel SET Ad = @Ad, Soyad = @Soyad, Adres = @Adres, Telefon = @Telefon, Eposta = @Eposta, Görev = @Görev WHERE PersonalID = @PersonalID", this.connection);
                this.command.CommandType = CommandType.Text;
                this.command.Parameters.AddWithValue("@PersonalID", id);
                this.command.Parameters.AddWithValue("@Ad", dgvr.Cells[1].Value);
                this.command.Parameters.AddWithValue("@Soyad",dgvr.Cells[2].Value);
                this.command.Parameters.AddWithValue("@Adres",dgvr.Cells[3].Value);
                this.command.Parameters.AddWithValue("@Telefon",dgvr.Cells[4].Value);
                this.command.Parameters.AddWithValue("@Eposta",dgvr.Cells[5].Value);
                this.command.Parameters.AddWithValue("@Görev",dgvr.Cells[6].Value);
                int rowsAffected = this.command.ExecuteNonQuery();  
                this.connection.Close();

        }

        private static DataTable toDataTable(string json)
        {
            var result = new DataTable();
            var jArray = JArray.Parse(json);
            //Initialize the columns, If you know the row type, replace this   
            foreach (var row in jArray)
            {
                foreach (var jToken in row)
                {
                    var jproperty = jToken as JProperty;
                    if (jproperty == null) continue;
                    if (result.Columns[jproperty.Name] == null)
                        result.Columns.Add(jproperty.Name, typeof(string));
                }
            }
            foreach (var row in jArray)
            {
                var datarow = result.NewRow();
                foreach (var jToken in row)
                {
                    var jProperty = jToken as JProperty;
                    if (jProperty == null) continue;
                    datarow[jProperty.Name] = jProperty.Value.ToString();
                }
                result.Rows.Add(datarow);
            }

            return result;
        }
    }
}
