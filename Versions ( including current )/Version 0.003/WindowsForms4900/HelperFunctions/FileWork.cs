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

        public static void help(object text)
        {
            text = "WOAH";
        }


        public static bool CheckPathFiles(ref string encryptBoxInput, ref string decryptBoxInput, TextBox errorTextbox)
        {

            if (String.IsNullOrEmpty(encryptBoxInput))//Is the outputfile-path given empty?
            {//If empty, showcase ERROR
                errorTextbox.Text = ErrorStrings.ENCRYPTION_BOX_EMPTY_ERROR;
                return false;
            }

            if (!(File.Exists(decryptBoxInput)))//attempt to see if input file exists
            {//If it doesn't, showcase error, then return.
                errorTextbox.Text = ErrorStrings.INPUT_FILE_READ_ERROR;
                return false;
            }

            if (!(File.Exists(encryptBoxInput)))//attempt to see if output file exists
            {//If it doesn't, showcase error, then return.
                errorTextbox.Text = ErrorStrings.OUTPUT_FILE_READ_ERROR;
                return false;
            }

            return true;
        }

        public static void OpenAndReadFiles(ref string input, ref string output, string inputPATH, string outputPATH)
        {//Assumes above function called

           

        }

    }
}
