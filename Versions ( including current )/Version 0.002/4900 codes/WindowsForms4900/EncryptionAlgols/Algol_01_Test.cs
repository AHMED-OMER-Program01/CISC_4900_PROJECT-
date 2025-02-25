using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

/*
 Okay, so;

AES allows only 128, 192, or 256 bit keys
    ( 16, 24, & 32 length chars );

Let's regulate it to a strict 16 length,
and have the windows-form allow an optional 

 
 
 */



namespace WindowsForms4900.EncryptionAlgols
{
    internal class Algol_01_Test
    {
       
    }

    public static class AES_StandardLibrary
    {




        public static string AES_ENCRYPT_FILE(string inputPath, string outputPath, string key)
        {
            bool fileOpenError = false;
            bool fileReadError = false;
            bool fileWriteError = false;
            bool fileCloseError = false;

            string result = "";



            using (Aes myAes = Aes.Create())
            {

            }


            return result;
        }






        public static string AES_DECRYPT_FILE(string inputPath, string outputPath, string key)
        {
            string result = "";


            return result;
        }





    }



}
