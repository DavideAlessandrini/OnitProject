using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SQLite;

namespace OnitProject
{
    public class Model
    {
        List<Sku> _pickupSeq;
        List<Locazione> _locazioni;
        String _connString;
        Int32 _maxPriority = 200;

        public Model()
        {
            _pickupSeq = new List<Sku>();
            _locazioni = new List<Locazione>();
            _connString = ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
        }

        internal void readData(string path)
        {
            _connString = _connString.Replace("PATH", path);

            readPickupSeq();        //Lettura della sequenza di Pickup
            setPriorityPickup();    //Assegna le priorità 
            readLocazioni();
            readGiacenza();
        }

        //Imposta la priorità della sequenza di Pickup
        private void setPriorityPickup()
        {
            for (int i = 0; i < _pickupSeq.Count; i++)
            {
                _pickupSeq[i].priorità = minpriority(_pickupSeq[i].sku, i);
            }
        }
        
        private int minpriority(Int32 sku, Int32 count)
        {
            Int32 min = 1;
            for (int j = 0; j < count; j++ )
            {
                if (_pickupSeq[j].sku == sku)
                {
                    if (_pickupSeq[j].priorità <= _pickupSeq[count].priorità)
                        min = _pickupSeq[j].priorità;
                        return _pickupSeq[j].priorità;
                }
                else
                {
                    min = _pickupSeq[count - 1].priorità+1;
                }
            }
            return min;

        }

        //Lettura della giacenza
        private void readGiacenza()
        {
            try
            {
                IDbConnection _conn = new SQLiteConnection(_connString);
                _conn.Open();//apre la connessione
                IDbCommand _com = _conn.CreateCommand();
                string queryText = "select LOCAZIONE,POSIZIONE,sku from capacita4I";
                _com.CommandText = queryText;//proprietà del commandtext che si aspetta una stringa
                IDataReader reader = _com.ExecuteReader();//mi permette di leggere i risultati -> risultato dell'esecuzione del comando -> executereader serve per le select
                while (reader.Read())//leggo 1 per 1 i record -> reader mi permette di vedere un record alla volta
                {
                    Console.WriteLine(reader["LOCAZIONE"] + " " + reader["POSIZIONE"] + " " + reader["sku"]+ " \n");
                    Locazione l = new Locazione() { nome = reader["LOCAZIONE"].ToString(), capacità = Convert.ToInt32(reader["CAPACITA"]) };
                    _locazioni.Add(l);
                }
                reader.Close();
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[readTable] Errore: " + ex.Message + Environment.NewLine);
            }
        }

        //Lettura delle locazioni del magazzino
        private void readLocazioni()
        {
            try
            {
                IDbConnection _conn = new SQLiteConnection(_connString);
                _conn.Open();//apre la connessione
                IDbCommand _com = _conn.CreateCommand();
                string queryText = "select LOCAZIONE,CAPACITA from capacita4I";
                _com.CommandText = queryText;//proprietà del commandtext che si aspetta una stringa
                IDataReader reader = _com.ExecuteReader();//mi permette di leggere i risultati -> risultato dell'esecuzione del comando -> executereader serve per le select
                while (reader.Read())//leggo 1 per 1 i record -> reader mi permette di vedere un record alla volta
                {
                    Console.WriteLine(reader["LOCAZIONE"] +" " + reader["CAPACITA"] +  " \n");
                    Locazione l = new Locazione() { nome = reader["LOCAZIONE"].ToString(), capacità = Convert.ToInt32(reader["CAPACITA"]) };
                    _locazioni.Add(l);
                }
                reader.Close();
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[readTable] Errore: " + ex.Message + Environment.NewLine);
            }
        }

        //Lettura della sequenza di Pickup
        private void readPickupSeq()
        {
            try
            {
                IDbConnection _conn = new SQLiteConnection(_connString);
                _conn.Open();//apre la connessione
                IDbCommand _com = _conn.CreateCommand();
                string queryText = "select Ordini from pickupSeq";
                _com.CommandText = queryText;//proprietà del commandtext che si aspetta una stringa
                IDataReader reader = _com.ExecuteReader();//mi permette di leggere i risultati -> risultato dell'esecuzione del comando -> executereader serve per le select
                Int32 count = 1;
                while (reader.Read())//leggo 1 per 1 i record -> reader mi permette di vedere un record alla volta
                {
                    Console.WriteLine(reader["Ordini"] + " \n");
                    Sku c = new Sku() { sku = Convert.ToInt32(reader["Ordini"]), priorità = count };
                    _pickupSeq.Add(c);
                    count++;
                }
                reader.Close();
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[readTable] Errore: " + ex.Message + Environment.NewLine);
            }
        }
    }
}
