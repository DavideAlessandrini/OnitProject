using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnitProject
{
    public class Utility
    {
        public static string openFile(string fileType)
        {
            string res = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "";
            openFileDialog1.Filter = fileType + " files (*." + fileType + ")|*." + fileType + "|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                res = openFileDialog1.FileName;

            return res;
        }

    }
}
