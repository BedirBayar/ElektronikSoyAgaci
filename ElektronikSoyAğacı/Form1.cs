using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ElektronikSoyAğacı
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
     
            loadTree();
            Soy.ExpandAll();
        }

        private void loadTree()
        {
            SqlConnection con = new SqlConnection(GlobalVariables.local);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "select * from BIREY where IsFromTree = 'True' order by ID asc";
            con.Open();
            var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                comboParents.Items.Add(reader.GetInt32(4).ToString()+ " " +reader.GetString(1) +" "+ reader.GetString(2));
                    if (reader.GetInt32(4) / 10 == 0)
                    {
                        Soy.Nodes.Add(reader.GetString(1));
                    }
                    else if (reader.GetInt32(4)/100==0) {
                        Soy.Nodes[reader.GetInt32(4) / 10 - 1].Nodes.Add(reader.GetString(1));
                    }
                    else if (reader.GetInt32(4) / 1000 == 0)
                    {
                        Soy.Nodes[reader.GetInt32(4) / 100 - 1].Nodes[(reader.GetInt32(4)%100) / 10 - 1].Nodes.Add(reader.GetString(1));
                    }
                    else if(reader.GetInt32(4)/10000==0)
                    {
                        Soy.Nodes[reader.GetInt32(4) / 1000 - 1].Nodes[(reader.GetInt32(4)%1000) / 100 - 1].Nodes[((reader.GetInt32(4) % 1000)%100)/10 - 1].Nodes.Add(reader.GetString(1));
                    }
                else // (reader.GetInt32(4) / 100000 == 0)
                {
                    Soy.Nodes[reader.GetInt32(4) / 10000 - 1].Nodes[(reader.GetInt32(4) % 10000) / 1000 - 1].Nodes[((reader.GetInt32(4) % 10000) % 1000) / 100 - 1].Nodes[((reader.GetInt32(4) % 10000) % 1000) % 100/10 - 1].Nodes.Add(reader.GetString(1));
                }

            }
            con.Close();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            String name = txt_name.Text;
            String surname = txt_surname.Text;
            Boolean gender = checkMale.Checked;
            DateTime birth = Birthdate.Value.Date;
            int parentno = Int32.Parse(Regex.Match(comboParents.SelectedItem.ToString(), @"\d+").Value);
            int neworder = findNewOrder(parentno)+1;
            int generation = (int)Math.Log10(neworder)+1;


            SqlConnection con = new SqlConnection(GlobalVariables.local);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "insert into BIREY (Name, Surname, Gender, Generation, OrderNo, BirthDate, IsFromTree) values (@name, @surname, @gender, @generation, @orderno, @birth, 'True')";
            cmd.Parameters.AddWithValue(@"name", name);
            cmd.Parameters.AddWithValue(@"surname", surname);
            cmd.Parameters.AddWithValue(@"gender", gender);
            cmd.Parameters.AddWithValue(@"generation", generation);
            cmd.Parameters.AddWithValue(@"orderno", neworder);
            cmd.Parameters.AddWithValue(@"birth", birth);

            con.Open();
            try { 
            var reader = cmd.ExecuteNonQuery();
                lbl_add_status.ForeColor = System.Drawing.Color.Green;
                lbl_add_status.Text = "Yeni üye başarıyla eklendi.";
            }catch(Exception ex) {
                lbl_add_status.ForeColor = System.Drawing.Color.Red;
                lbl_add_status.Text =ex.Message.ToString();
            }
            con.Close();
            Soy.Nodes.Clear();
            loadTree();

        }
        private int findNewOrder(int no)
        {
            int result = 0;
            int limit = (no+1) * 10;
            SqlConnection con = new SqlConnection(GlobalVariables.local);
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "select MAX (OrderNo) from BIREY where OrderNo<@limit";
            cmd.Parameters.AddWithValue( @"limit", limit);
            con.Open();
            var reader = cmd.ExecuteReader();
            result = reader.Read() ? reader.GetInt32(0) : 0;
            con.Close();
            return result < no * 10 ? no * 10 : result;
        }
    }
}
