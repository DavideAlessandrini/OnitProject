/*
 * Davide Alessandrini
*/
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
                _pickupSeq[i].priorità = minpriority(_pickupSeq[i].sku, i);
            
        }

        /// <summary>
        /// find location number with SKU wich is closest to exit
        /// </summary>
        /// <param name="_sku">sku to find</param>
        /// <param name="locations">locations to find in</param>
        /// <returns>location number with closest SKU</returns>
        private Int32 findClosestSKU(Sku _sku, List<Locazione> locations)
        {
            Int32 minPos = Int32.MaxValue;
            Int32 minLocation = -1;
            Int32 currentPos = 0;
            for(Int32 locationNumber = 0; locationNumber < locations.Count; locationNumber++)
            {
                for (Int32 positionNumber = locations[locationNumber].elementi.Count - 1; positionNumber >= 0 ; positionNumber--)
                {
                    if (locations[locationNumber].elementi[positionNumber].sku == _sku.sku)
                    {
                        currentPos = locations[locationNumber].elementi.Count - positionNumber;
                        if (currentPos < minPos)
                        {
                            currentPos = minPos;
                            minLocation = locationNumber;
                            break;
                        }
                    }
                }
            }
            return minLocation;
        }

        /// <summary>
        /// gets SKU from location by moving previous SKUs to nearby locations
        /// </summary>
        /// <param name="_sku">SKU to find</param>
        /// <param name="locations">locations</param>
        /// <param name="locationNumber">location number to get SKU from</param>
        /// <returns>Number of steps needed to get SKU</returns>
        private Int32 getSKU(Sku _sku, List<Locazione> locations, Int32 locationNumber)
        {
            Int32 _ret = 0;
            Random myRandom = new Random();
            Sku skuGetted = locations[locationNumber].getSku();
            _ret++;
            while (skuGetted.sku != _sku.sku)
            {
                Int32 locationToPush = locationNumber + myRandom.Next(2) - 1; // move only to nearby locations
                locations[locationToPush].pushSku(skuGetted);
                skuGetted = locations[locationNumber].getSku();
                _ret++;
            }
            // here we assume that SKU is getted!
            return _ret;
        }

        /// <summary>
        /// Get solution cost for passed locations and order squence
        /// </summary>
        /// <param name="orders">order sequence</param>
        /// <param name="locations">locations</param>
        /// <returns>sloturion cost</returns>
        private Int32 getSolutionCost(List<Sku> orders, List<Locazione> locations)
        {
            
            Int32 _solutionCost = 0;
            for (int orderNumber = 0; orderNumber < orders.Count; orderNumber++)
            {
                Int32 locationNum = findClosestSKU(orders[orderNumber], locations);
                _solutionCost += getSKU(orders[orderNumber], locations, locationNum);
            }
            return _solutionCost;
        }

        public Int32 getCurrentSolutionCost()
        {
            return getSolutionCost(_pickupSeq, _locazioni);
        }

        //Verifica e impostazione della priorità corretta
        //1 -> più importante
        //99 -> meno importante
        private int minpriority(Int32 sku, Int32 count)
        {
            Int32 min = 1;
            for (int j = 0; j < count; j++)
            {
                if (_pickupSeq[j].sku == sku)
                {
                    if (_pickupSeq[j].priorità <= _pickupSeq[count].priorità)
                        min = _pickupSeq[j].priorità;
                    return _pickupSeq[j].priorità;
                }
                else
                {
                    min = _pickupSeq[count - 1].priorità + 1;
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
                string queryText = "select LOCAZIONE,POSIZIONE,sku from giacenza4I";
                _com.CommandText = queryText;//proprietà del commandtext che si aspetta una stringa
                IDataReader reader = _com.ExecuteReader();//mi permette di leggere i risultati -> risultato dell'esecuzione del comando -> executereader serve per le select
                while (reader.Read())//leggo 1 per 1 i record -> reader mi permette di vedere un record alla volta
                {
                    Console.WriteLine(reader["LOCAZIONE"] + " " + reader["POSIZIONE"] + " " + reader["sku"] + " \n");
                    var locazione = reader["LOCAZIONE"].ToString();
                    var posizione = Convert.ToInt32(reader["POSIZIONE"]);
                    var sku = Convert.ToInt32(reader["sku"]);

                    foreach (var sel in _locazioni)
                    {
                        //Seleziono la locazione
                        if (sel.nome == locazione)
                        {
                            Sku s = null;
                            Boolean trovato = false;
                            //Ricerco se sku è già in sequenza di pickup perchè ha caricata la priorità
                            foreach (var sk in _pickupSeq)
                            {
                                if (sk.sku == sku)
                                {
                                    s = sk;
                                    trovato = true;
                                    break;
                                }
                                //Altrimenti lo carico assegnandoli una priorità max
                            }
                            if (!trovato)
                                s = new Sku() { sku = sku, priorità = _maxPriority, posizione = posizione };
                             
                            sel.elementi.Add(s);
                            
                        }
                    }

                }
                reader.Close();
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[readTable] Errore: " + ex.Message + Environment.NewLine);
            }

            foreach (var sel in _locazioni)
                checkLocazione(sel);
        }

        //Check di verifica delle locazioni
        private void checkLocazione(Locazione l)
        {
            //Check sulla capacità della locazione
            if (l.capacità < l.getMaxPosition())
                l.capacità = l.getMaxPosition();
            //Ordino la sequenza nella locazione secondo la posizione
            var sortedList = l.elementi.OrderBy(x => x.posizione).ToList();
            l.elementi.Clear();
            l.elementi = sortedList;
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
                    Console.WriteLine(reader["LOCAZIONE"] + " " + reader["CAPACITA"] + " \n");
                    Locazione l = new Locazione() { nome = reader["LOCAZIONE"].ToString(), capacità = Convert.ToInt32(reader["CAPACITA"]) };
                    _locazioni.Add(l);
                }
                reader.Close();
                _conn.Close();
                foreach (var sel in _locazioni)
                    sel.elementi = new List<Sku>();
                    
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
