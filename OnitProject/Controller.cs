using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnitProject
{
    class Controller
    {
        Model _model;
        public Controller()
        {
            _model = new Model();
        }

        internal void readData(string path)
        {
            _model.readData(path);
        }
    }
}
