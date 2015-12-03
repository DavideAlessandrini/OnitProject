using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnitProject
{
    class Locazione
    {
        public List<Sku> elementi { get; set; } //Lista degli elementi
        public Int32 priorità { get; set; }     //Somma delle priorità degli elementi 
        public Int32 capacità { get; set; }     //Dimensione della locazione
        public String nome { get; set; }        //Nome

        //Ritorna il massimo della posizione
        public Int32 getMaxPosition()
        {
            Int32 max = 0;
            foreach (var sel in elementi)
                if (sel.posizione > max)
                    max = sel.posizione;
            return max;
        }
    }
}
