using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace WindowsForms4900.HelperFunctions
{
    public static class FileWork
    {
        public static void CreateFile(string contents)
        {
            string name = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) +
                    @"\" +
                    DateTime.Now.ToString("MM_dd_yyyy-h_mm_ss tt") +
                    ".txt";

            StreamWriter file = new StreamWriter(name);
            file.WriteLine(contents);
            file.Close();
        }


        public static bool CheckPathFiles(ref string encryptBoxInput, ref string decryptBoxInput, TextBox statusTextbox)
        {

            if (String.IsNullOrEmpty(encryptBoxInput))//Is the outputfile-path given empty?
            {//If empty, showcase ERROR
                statusTextbox.Text += MessageStrings.Error(6) + "\n";
                return false;
            }

            if (!(File.Exists(decryptBoxInput)))//attempt to see if input file exists
            {//If it doesn't, showcase error, then return.
                statusTextbox.Text += MessageStrings.Error(1) + "\n";
                return false;
            }
            statusTextbox.Text += MessageStrings.Success(1) + "\n";

            if (!(File.Exists(encryptBoxInput)))//attempt to see if output file exists
            {//If it doesn't, showcase error, then return.
                statusTextbox.Text += MessageStrings.Error(3) + "\n";
                return false;
            }
            statusTextbox.Text += MessageStrings.Success(3) + "\n";

            return true;
        }

        public static void OpenAndReadFiles(ref string input, ref string output, string inputPATH, string outputPATH)
        {//Assumes above function called

           

        }

    }
}
