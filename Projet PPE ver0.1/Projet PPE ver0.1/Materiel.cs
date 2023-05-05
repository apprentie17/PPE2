using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PPE_ver0._1
{
    internal class Materiel
    {
        
        public int ID;
        public string nom, adresse, mail, telephone;

        public Materiel(int id, string nom, string adresse, string mail, string telephone)
        {
            this.ID = id;
            this.nom = nom;
            this.adresse = adresse;
            this.mail = mail;
            this.telephone = telephone;
        }

        public override string ToString()
        {
            return this.nom;
        }
    }



}
