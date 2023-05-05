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

namespace Projet_PPE_ver0._1
{
    public partial class FormLogin : Form
    {

        public string strcon = @"Data Source=.\sqlexpress;database=GM;Integrated Security=True";
        public FormLogin()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

            SqlConnection sqlcon = new SqlConnection(this.strcon);
            sqlcon.Open();
            string sql = "SELECT * FROM Utilisateur WHERE Login = '" + userprompt.Text + "' AND Password = '" + passwordprompt.Text + "'";
            SqlCommand cmd = new SqlCommand(sql, sqlcon);
            SqlDataReader sqr = cmd.ExecuteReader();
            
            if (sqr.Read() == true)
            {
                FormPanel formPanel = new FormPanel();
                DialogResult dialogResult = formPanel.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login ou mot de passe incorrect");
            }

            sqr.Close();
            sqlcon.Close();
            

        }
    

    }
    
}
