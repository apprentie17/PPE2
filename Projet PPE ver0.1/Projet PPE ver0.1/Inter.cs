using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PPE_ver0._1
{

    internal class Inter
    {
        public int ID, ID_Client, MTBF;
        public string nom, NoSerie, type, marque;
        public DateTime dateInstall;


        public Inter(int id, string nom, string NoSerie, DateTime dateInstall, int MTBF, string type, string marque, int ID_Client)
        {
            this.ID = id;
            this.nom = nom;
            this.NoSerie = NoSerie;
            this.dateInstall = dateInstall;
            this.MTBF = MTBF;
            this.type = type;
            this.marque = marque;
            this.ID_Client = ID_Client;
        }

        public override string ToString()
        {
            return this.nom;
        }
    }
}
