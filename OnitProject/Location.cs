using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnitProject
{
    class Location
    {
        public List<Sku> listOfElement { get; set; } //Lista degli elementi
        public Int32 priority { get; set; }     //Somma delle priorità degli elementi 
        public Int32 capacity { get; set; }     //Dimensione della locazione
        public String name { get; set; }        //Nome

        //Ritorna il massimo della posizione
        public Int32 getMaxPosition()
        {
            Int32 max = 0;
            foreach (var sel in listOfElement)
                if (sel.position > max)
                    max = sel.position;
            return max;
        }
    }
}
