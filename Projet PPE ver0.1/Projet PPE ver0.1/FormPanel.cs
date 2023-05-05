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

namespace Projet_PPE_ver0._1
{
    public partial class FormPanel : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=.\sqlexpress;database=GM;Integrated Security=True");
        SqlCommand cmd;
        DataTable dtClient;
        DataTable dtMateriel;
        DataTable dtIntervention;
        SqlDataAdapter adptClient;
        SqlDataAdapter adptMateriel;
        SqlDataAdapter adptIntervention;
        int idClient;
        int idMateriel;
        int idIntervention; 
        public FormPanel()
        {
            InitializeComponent();
            MTBFVerif();
            showdataClient();
            showdataMateriel();
            showdataIntervention();
            fillCombo();
            fillComboInter();
            fillComboStatut();
            
        }
        private void FormPanel_Load(object sender, EventArgs e)
        {   
            FormLogin formLogin = new FormLogin();
            formLogin.Close();
        }


        /*  
            -----------------------------------------------------------------------------------------------
            ------------------------------------ START CLIENT PART ------------------------------------ 
            -----------------------------------------------------------------------------------------------
        */

        void MTBFVerif() // #MTBFVERIF
        {
            con.Open();
            cmd = new SqlCommand("SELECT m.Nom as NomMat, c.Nom as NomClient, m.Type, i.Site, " +
                "DATEDIFF(day, GETDATE(), DATEADD(month, m.MTBF, m.Date_Install)) as 'Jours_restants' " +
                "FROM MATERIEL m " +
                "JOIN CLIENT c ON m.ID_CLIENT = c.ID_CLIENT " +
                "LEFT JOIN INTERVENTION i ON m.ID_Materiel = i.ID_Materiel " +
                "WHERE m.Date_Install IS NOT NULL ", con);

            // SELECT m.Nom as NomMat, c.Nom as NomClient, m.Type as Type, i.Site as Site, DATEDIFF(day, GETDATE(),
            // DATEADD(month, m.MTBF, m.Date_Install)) as 'Jours_restants' FROM MATERIEL m JOIN CLIENT c ON m.ID_CLIENT = c.ID_CLIENT
            // LEFT JOIN INTERVENTION i ON m.ID_Materiel = i.ID_Materiel WHERE m.Date_Install IS NOT NULL
            
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string NomMat = reader["NomMat"].ToString();
                int JoursRestant = Convert.ToInt32(reader["Jours_restants"]);

                if (JoursRestant <= 15)
                {

                    string message = string.Format("Le matériel {0} est en panne depuis plus de {1} jours", NomMat, JoursRestant);
                    MessageBox.Show(message, "Alerte", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            con.Close();
        }
        
        public void showdataClient() // #SHOWDATACLIENT
        {
            con.Open();
            adptClient = new SqlDataAdapter("Select ID_CLIENT as ID,Nom,Adresse,Mail,Tel as Téléphone from CLIENT", con);
            dtClient = new DataTable();
            adptClient.Fill(dtClient);
            dataGridViewClients.DataSource = dtClient;
            con.Close();
        }


        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            showdataClient();
            showdataMateriel();
            showdataIntervention();
            comboBoxMat.Items.Clear();
            comboBoxInter.Items.Clear();
            comboBoxStatut.Items.Clear();
            fillCombo();
            fillComboInter();
            fillComboStatut();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) //#DATAGRIDCLIENT
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else
            {
                DataGridViewRow row = this.dataGridViewClients.Rows[e.RowIndex];
                if (row.Cells["ID"].Value.ToString() != "")
                {
                    idClient = Convert.ToInt32(row.Cells["ID"].Value.ToString());
                    textBoxNomClient.Text = row.Cells["Nom"].Value.ToString();
                    textBoxAdresseClient.Text = row.Cells["Adresse"].Value.ToString();
                    textBoxMailClient.Text = row.Cells["Mail"].Value.ToString();
                    textBoxTelClient.Text = row.Cells["Téléphone"].Value.ToString();
                    var NomClient = textBoxNomClient.Text;
                    var adresseClient = textBoxAdresseClient.Text;
                    var mailClient = textBoxMailClient.Text;
                    var telClient = textBoxTelClient.Text;
                }
                else
                {
                    textBoxNomClient.Text = "";
                    textBoxAdresseClient.Text = "";
                    textBoxMailClient.Text = "";
                    textBoxTelClient.Text = "";

                }
            }
        }
        private void buttonSupprimer_Click(object sender, EventArgs e) // #SUPPRIMERCLIENT
        {
                

            if (MessageBox.Show("Voulez vous vraiment supprimer ce client ?" + "\n" +
                "Attention, toutes les interventions et matériaux liées à ce client seront supprimées"
                , "Supprimer un client",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {   
                con.Open();
                //cmd = new SqlCommand("delete from MATERIEL where ID_CLIENT='" + idClient + "'", con);
                //cmd.ExecuteNonQuery();
                cmd = new SqlCommand("delete from CLIENT where ID_CLIENT='" + idClient + "'", con);
                cmd.ExecuteNonQuery();
                textBoxNomClient.Text = "";
                textBoxAdresseClient.Text = "";
                textBoxMailClient.Text = "";
                textBoxTelClient.Text = "";
                MessageBox.Show("Client supprimé");
                con.Close();
                showdataClient();
            }
        }

        private void buttonAjouter_Click(object sender, EventArgs e) // #AJOUTERCLIENT
        {
            // check if fields already exist
            
            con.Open();
            SqlCommand check_mail = new SqlCommand("SELECT COUNT(*) FROM CLIENT WHERE Mail = @Mail", con);
            check_mail.Parameters.AddWithValue("@Mail", textBoxMailClient.Text);
            int UserExist = (int)check_mail.ExecuteScalar();
            con.Close();
            if (UserExist > 0)
            {
                MessageBox.Show("Cette adresse email existe déjà");
            }
            else
            {
                if (textBoxNomClient.Text != "" && textBoxAdresseClient.Text != ""
                    && textBoxMailClient.Text != "" && textBoxTelClient.Text != "")
                {

                    try
                    {
                        Materiel selectedClient = (Materiel)comboBoxMat.SelectedItem;
                        con.Open();
                        cmd = new SqlCommand("insert into CLIENT(Nom,Adresse,Mail,Tel) values" +
                            "('" + textBoxNomClient.Text +
                            "','" + textBoxAdresseClient.Text +
                            "','" + textBoxMailClient.Text +
                            "','" + textBoxTelClient.Text + "')", con);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Client ajouté");
                        con.Close();
                        showdataClient();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
            }
        }

        private void buttonModifier_Click(object sender, EventArgs e) // #MODIFIERCLIENT
        {
            
                if (textBoxNomClient.Text != "" && textBoxAdresseClient.Text != "" 
                && textBoxMailClient.Text != "" && textBoxTelClient.Text != "")
                {
                try { 
                    con.Open();
                    cmd = new SqlCommand("update CLIENT set Nom='" + textBoxNomClient.Text + "',Adresse='" +
                        textBoxAdresseClient.Text + "',Mail='" + textBoxMailClient.Text + "',Tel='" +
                        textBoxTelClient.Text + "' where ID_CLIENT='" + idClient + "'", con);
                    
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Les informations ont bien été modifié");
                    con.Close();
                    showdataClient();
                    }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
            
        }
        /*  
            -----------------------------------------------------------------------------------------------
            ------------------------------------ END CLIENT PART ------------------------------------ 
            -----------------------------------------------------------------------------------------------
        */





        /*  
            -----------------------------------------------------------------------------------------------
            ------------------------------------ START MATERIEL PART ------------------------------------ 
            -----------------------------------------------------------------------------------------------
        */


        public void showComboBox()
        {
            // add client to combobox
            con.Open();
            cmd = new SqlCommand("Select Nom from CLIENT", con);
            SqlDataReader dr;
            dr = cmd.ExecuteReader();
            

        }
        private void buttonRefreshMat_Click(object sender, EventArgs e)
        {
            showdataClient();
            showdataMateriel();
            showdataIntervention();
            comboBoxMat.Items.Clear();
            comboBoxInter.Items.Clear();
            comboBoxStatut.Items.Clear();
            fillCombo();
            fillComboInter();
            fillComboStatut();
        }
        public void fillCombo()
        {
            con.Open();
            // StoredProcedure
            cmd = new SqlCommand("ClientsList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter dal = new SqlDataAdapter(cmd);

            dal.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["ID_CLIENT"].ToString());
                string nom = dr["Nom"].ToString();
                string adresse = dr["Adresse"].ToString();
                string mail = dr["Mail"].ToString();
                string tel = dr["Tel"].ToString();

                Materiel matos = new Materiel(id,nom,adresse,mail,tel);

                comboBoxMat.Items.Add(matos);


            }
            con.Close();
        }

        public void showdataMateriel()
        {
            con.Open();
            adptMateriel = new SqlDataAdapter("Select m.ID_MATERIEL as ID_MAT,m.Nom,m.NoSerie, " +
                "m.Date_Install,m.MTBF,m.Type,m.Marque,c.Nom as Client from " +
                "MATERIEL m join CLIENT c on m.ID_CLIENT = c.ID_CLIENT", con);  

            dtMateriel = new DataTable();
            adptMateriel.Fill(dtMateriel);
            dataGridViewMateriel.DataSource = dtMateriel;
            con.Close();
        }

        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else
            {
                DataGridViewRow row = this.dataGridViewMateriel.Rows[e.RowIndex];
                if (row.Cells["ID_MAT"].Value.ToString() != "")
                {
                    idMateriel = Convert.ToInt32(row.Cells["ID_MAT"].Value.ToString());
                    textBoxNomMateriel.Text = row.Cells["Nom"].Value.ToString();
                    textBoxNoSerieMateriel.Text = row.Cells["NoSerie"].Value.ToString();
                    dateTimePicker1.Value = DateTime.Parse(row.Cells["Date_Install"].Value.ToString());
                    textBoxMTBFMateriel.Text = row.Cells["MTBF"].Value.ToString();
                    textBoxTypeMateriel.Text = row.Cells["Type"].Value.ToString();
                    textBoxMarqueMateriel.Text = row.Cells["Marque"].Value.ToString();
                    comboBoxMat.Text = row.Cells["Client"].Value.ToString();
                }
                else
                {
                    textBoxNomMateriel.Text = "";
                    textBoxNoSerieMateriel.Text = "";
                    dateTimePicker1.Value = DateTime.Now;
                    textBoxMTBFMateriel.Text = "";
                    textBoxTypeMateriel.Text = "";
                    textBoxMarqueMateriel.Text = "";
                    comboBoxMat.SelectedItem = null;
                }
            }
        }

        private void buttonAjouterMateriel_Click(object sender, EventArgs e)
        {
            con.Open();
            SqlCommand check_NoSerie = new SqlCommand("SELECT COUNT(*) FROM MATERIEL WHERE NoSerie = @NoSerie", con);
            check_NoSerie.Parameters.AddWithValue("@NoSerie", textBoxNoSerieMateriel.Text);
            int UserExist = (int)check_NoSerie.ExecuteScalar();
            con.Close();

            
            if (UserExist > 0)
            {
                MessageBox.Show("Ce matériel existe déjà");
            }
            else
            {

                if (textBoxNomMateriel.Text != "" && textBoxNoSerieMateriel.Text != "" 
                    &&  textBoxMTBFMateriel.Text != "" && textBoxTypeMateriel.Text != "" 
                    && textBoxMarqueMateriel.Text != "" && comboBoxMat.SelectedItem != null)
                {

                    try
                    {
                        Materiel selectedClient = (Materiel)comboBoxMat.SelectedItem;
                        int idClient = selectedClient.ID;

                        con.Open();
                        cmd = new SqlCommand("insert into MATERIEL(Nom,NoSerie,Date_Install,MTBF,Type,Marque,ID_CLIENT) values('" +
                            textBoxNomMateriel.Text + "','" + textBoxNoSerieMateriel.Text + "','" +
                            dateTimePicker1.Value + "','" + textBoxMTBFMateriel.Text + "','" +
                            textBoxTypeMateriel.Text + "','" + textBoxMarqueMateriel.Text + "','" + idClient + "')", con);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Materiel ajouté");
                        con.Close();
                        showdataMateriel();
                    
                    }
                    catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
            }
        }

        private void buttonSupprimerMateriel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez vous vraiment supprimer ce matériel ?",
                "Supprimer un matériel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                con.Open();
                cmd = new SqlCommand("delete from MATERIEL where ID_MATERIEL='" + idMateriel + "'", con);
                cmd.ExecuteNonQuery();
                textBoxNomMateriel.Text = "";
                textBoxNoSerieMateriel.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                textBoxMTBFMateriel.Text = "";
                textBoxTypeMateriel.Text = "";
                textBoxMarqueMateriel.Text = "";
                comboBoxMat.SelectedItem = null;
                MessageBox.Show("Matériel supprimé");
                con.Close();
                showdataMateriel();
            }

            
        }

        private void buttonModifierMateriel_Click(object sender, EventArgs e)
        {
                if (textBoxNomMateriel.Text != "" && textBoxNoSerieMateriel.Text != "" 
                && textBoxMTBFMateriel.Text != "" && textBoxTypeMateriel.Text != ""
                && textBoxMarqueMateriel.Text != "" && comboBoxMat.SelectedItem != null)
                {
                try
                {
                    Materiel selectedClient = (Materiel)comboBoxMat.SelectedItem;
                    int idClient = selectedClient.ID;

                    con.Open();
                    cmd = new SqlCommand
                    ("update MATERIEL set Nom='" + textBoxNomMateriel.Text +
                    "',NoSerie='" + textBoxNoSerieMateriel.Text +
                    "',Date_Install='" + dateTimePicker1.Value +
                    "',MTBF='" + textBoxMTBFMateriel.Text +
                    "',Type='" + textBoxTypeMateriel.Text +
                    "',Marque='" + textBoxMarqueMateriel.Text +
                    "',ID_CLIENT='" + idClient +
                    "' where ID_MATERIEL='" + idMateriel + "'", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Matériel modifié");
                    con.Close();
                    showdataMateriel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
        }




        /*  
            -----------------------------------------------------------------------------------------------
            ------------------------------------ END MATERIEL PART ------------------------------------ 
            -----------------------------------------------------------------------------------------------
        */



        /*  
            -----------------------------------------------------------------------------------------------
            ------------------------------------ START INTERVENTION PART ------------------------------------ 
            -----------------------------------------------------------------------------------------------
        */


        public void showdataIntervention()
        {
            con.Open();
            adptIntervention = new SqlDataAdapter("Select i.ID_INTER as ID_INTER,i.Date_Inter," +
                "i.Commentaire,i.Technicien, i.Site, i.Statut," +
                "m.Nom as Materiel from INTERVENTION i join MATERIEL m on i.ID_MATERIEL = m.ID_MATERIEL", con);
            dtIntervention = new DataTable();
            adptIntervention.Fill(dtIntervention);
            dataGridViewIntervention.DataSource = dtIntervention;

            con.Close();
        }
        private void buttonRefreshInter_Click(object sender, EventArgs e)
        {
            showdataClient();
            showdataMateriel();
            showdataIntervention();
            comboBoxMat.Items.Clear();
            comboBoxInter.Items.Clear();
            comboBoxStatut.Items.Clear();
            fillCombo();
            fillComboInter();
            fillComboStatut();
        }

        private void dataGridViewIntervention_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else
            {
                DataGridViewRow row = this.dataGridViewIntervention.Rows[e.RowIndex];
                if (row.Cells["ID_INTER"].Value.ToString() != "")
                {
                    idIntervention = Convert.ToInt32(row.Cells["ID_INTER"].Value.ToString());
                    textBoxCommentaire.Text = row.Cells["Commentaire"].Value.ToString();
                    textBoxTechnicien.Text = row.Cells["Technicien"].Value.ToString();
                    dateTimePickerInter.Value = DateTime.Parse(row.Cells["Date_Inter"].Value.ToString());
                    comboBoxInter.Text = row.Cells["Materiel"].Value.ToString();
                    textBoxSite.Text = row.Cells["Site"].Value.ToString();
                    comboBoxStatut.Text = row.Cells["Statut"].Value.ToString();
                }
                else
                {
                    dateTimePickerInter.Value = DateTime.Now;
                    textBoxCommentaire.Text = "";
                    textBoxTechnicien.Text= "";
                    comboBoxInter.SelectedItem = null;
                    textBoxSite.Text = "";
                    comboBoxStatut.SelectedItem = null;
                }
            }
        }

        public void fillComboStatut()
        {
            comboBoxStatut.Items.Add("En attente");
            comboBoxStatut.Items.Add("En cours");
            comboBoxStatut.Items.Add("Terminée"); 
        }

        
        public void fillComboInter()
        {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM MATERIEL", con);
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter dal = new SqlDataAdapter(cmd);

            dal.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["ID_MATERIEL"].ToString());
                string nom = dr["Nom"].ToString();
                string noSerie = dr["NoSerie"].ToString();
                DateTime dateInstall = DateTime.Parse(dr["Date_Install"].ToString());
                int MTBF = Convert.ToInt32(dr["MTBF"].ToString());
                string type = dr["Type"].ToString();
                string marque = dr["Marque"].ToString();
                int idClient = Convert.ToInt32(dr["ID_CLIENT"].ToString());
                

                Inter intervention = new Inter(id, nom, noSerie, dateInstall, MTBF, type, marque, idClient);


                comboBoxInter.Items.Add(intervention);
            }
            con.Close();
        }

        private void buttonAjouterInter_Click(object sender, EventArgs e)
        {

            if (comboBoxInter.SelectedItem != null && textBoxCommentaire.Text != ""
                    && textBoxCommentaire.Text != "" && textBoxTechnicien.Text != "")
            {
                Inter selectedIntervention = (Inter)comboBoxInter.SelectedItem;
                int idMateriel = selectedIntervention.ID;

                con.Open();
                SqlCommand check_materiel = new SqlCommand("SELECT COUNT(*) FROM INTERVENTION WHERE ID_MATERIEL = @ID_MATERIEL", con);
                check_materiel.Parameters.AddWithValue("@ID_MATERIEL", idMateriel);
                int UserExist = (int)check_materiel.ExecuteScalar();

                con.Close();

                if (UserExist > 0)
                {
                    MessageBox.Show("Cette intervention est déjà en cours");
                }
                else
                {
                    try
                    {

                        con.Open();
                        cmd = new SqlCommand("insert into INTERVENTION(Date_Inter,Commentaire,Technicien,ID_MATERIEL,Site,Statut) values" +
                            "('" + dateTimePickerInter.Value +
                            "','" + textBoxCommentaire.Text +
                            "','" + textBoxTechnicien.Text +
                            "','" + idMateriel +
                            "','" + textBoxSite.Text +
                            "','" + comboBoxStatut.Text + "')", con);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Intervention ajoutée");
                        con.Close();
                        showdataIntervention();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez remplir tous les champs");
            }
            
        }
        


        private void buttonModifierInter_Click(object sender, EventArgs e)
        {
            if (comboBoxInter.SelectedItem != null && textBoxCommentaire.Text != ""
                && textBoxCommentaire.Text != "" && textBoxTechnicien.Text != "" 
                && textBoxSite.Text !="" && comboBoxStatut.SelectedItem != null)
            {
                try
                {
                    Inter selectedClient = (Inter)comboBoxInter.SelectedItem;
                    int idMateriel = selectedClient.ID;

                    string Statut = comboBoxStatut.SelectedItem.ToString();

                    
                    con.Open();
                    cmd = new SqlCommand("update INTERVENTION set Date_Inter='" + dateTimePickerInter.Value +
                        "',Commentaire='" + textBoxCommentaire.Text +
                        "',Technicien='" + textBoxTechnicien.Text +
                        "',ID_MATERIEL='" + idMateriel +
                        "',Site='" + textBoxSite.Text +
                        "',Statut='" + comboBoxStatut.SelectedItem +
                        "' where ID_INTER='" + idIntervention + "'", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Intervention modifiée");
                    con.Close();

                    if (Statut == "Terminée")
                    {
                        con.Open();
                        cmd = new SqlCommand("UPDATE MATERIEL SET Date_Install = GETDATE() WHERE ID_MATERIEL = @ID_MATERIEL ",con);
                        cmd.Parameters.AddWithValue("@ID_MATERIEL", idMateriel);

                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    
                    showdataIntervention();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Veuillez remplir tous les champs");
            }
        }

        private void buttonSupprimerInter_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new SqlCommand("delete from INTERVENTION where ID_INTER='" + idIntervention + "'", con);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Intervention supprimée");
            con.Close();
            showdataIntervention();
        }
        
    }

    /*  
        -----------------------------------------------------------------------------------------------
        ------------------------------------ END INTERVENTION PART ------------------------------------ 
        -----------------------------------------------------------------------------------------------
    */ 
}
