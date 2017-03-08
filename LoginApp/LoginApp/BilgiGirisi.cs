using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginApp
{
    public partial class BilgiGirisi : Form

    {
        KayıtGirisi kayitGirisi = new KayıtGirisi();
        public BilgiGirisi()
        {
            InitializeComponent();
        }

        private void BilgiGirisi_Load(object sender, EventArgs e)
        {
            lblBaslik.Font = new Font("Arial", 16, FontStyle.Bold);
            dgvListe.DataSource = kayitGirisi.KayitGetir();
          
        }
  
        private void btnEkle_Click(object sender, EventArgs e)
        {
            JArray eklenmis = kayitGirisi.JSONEkle(txtAd.Text, txtSoyad.Text, txtAdres.Text, txtEposta.Text, txtTelefon.Text, txtGorev.Text);
            kayitGirisi.SQLEkle(txtAd.Text, txtSoyad.Text, txtAdres.Text, txtEposta.Text, txtTelefon.Text, txtGorev.Text);
            dgvListe.DataSource = kayitGirisi.JSONDonustur(eklenmis);   
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
          
            string index = Convert.ToString(dgvListe.CurrentRow.Index);
            JArray silinmis = kayitGirisi.JSONSil(index);
            string ID = dgvListe.CurrentRow.Cells[0].Value.ToString();
            kayitGirisi.SQLSil(ID);
            dgvListe.DataSource = kayitGirisi.JSONDonustur(silinmis);
        }

        private void btnDuzenle_Click(object sender, EventArgs e)
        {
          
            int id =Convert.ToInt32( dgvListe.CurrentRow.Cells[0].Value.ToString());
            DataGridViewRow row = dgvListe.CurrentRow;
            int index = row.Index;
            JArray duzenlenmis = kayitGirisi.JSONDuzenle(row,index);
            kayitGirisi.SQLDuzenle(row,id);
            dgvListe.DataSource = kayitGirisi.JSONDonustur(duzenlenmis);
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

        public void btnYenile_Click(object sender, EventArgs e)
        {
            dgvListe.DataSource = kayitGirisi.KayitGetir();
        }
	}
        
  }

