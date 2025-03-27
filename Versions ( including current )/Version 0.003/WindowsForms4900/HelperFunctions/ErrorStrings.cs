using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsForms4900.EncryptionAlgols;
using WindowsForms4900.HelperFunctions;

namespace WindowsForms4900.HelperFunctions
{
    public static class ErrorStrings
    {
        //-------------------------------------
        //Generic Error(s)
        public const string RESULT_ERROR = "[ERROR:IF THIS IS BEING GIVEN BACK, ERROR]";
        //-------------------------------------
        //File errors
        public const string INPUT_FILE_READ_ERROR = "[ERROR: READING INPUT FILE]";
        public const string INPUT_FILE_WRITE_ERROR = "[ERROR: WRITING INPUT FILE]";
        //------------------
        public const string OUTPUT_FILE_READ_ERROR = "[ERROR: READING OUTPUT FILE]";
        public const string OUTPUT_FILE_WRITE_ERROR = "[ERROR: WRITING OUTPUT FILE]";
        //------------------
        public const string FILE_CREATION_ERROR = "[ERROR: ERROR IN FILE CREATION]";
        //-------------------------------------
        //Empty Box Errors
        public const string ENCRYPTION_BOX_EMPTY_ERROR = "[ERROR: ENCRYPTION TEXTBOX EMPTY OR NULL]";
        public const string DECRYPTION_BOX_EMPTY_ERROR = "[ERROR: DECRYPTION TEXTBOX EMPTY OR NULL]";
        //-------------------------------------
        //MISC ERRORS
        public const string DECRYPTION_BOX_INPUT_ERROR = "[ERROR: DECRYPTION INPUT ERROR]";
        //-------------------------------------
        //FUNCTIONS
        public static string ERROR(string given)
        { return ("[ERROR: " + given + " ]"); }
        //-------------------------------------
    }
}




//Old versions of stuff;;
/*-------------------------------------
 En & De crypt;


//-------------------------------------
private void AES_EncryptButton_Click(object sender, EventArgs e)
        {//ENCRYPTION AES Button

            Boolean Record = (AES_PathOutput.Checked);
            //-------------------------------------
            //If reading from a file, & outputting to a file
            //We do this check, & lock-in BEFORE calcutation;
            //Because at worst case, if it takes forever we don't want the user
            //To switch up by accident
            //-------------------------------------


Boolean desktopFile = (AES_DesktopAdd.Checked);//output to file created to Desktop

string inputText = AES_ENCRYPT_TEXTBOX.Text;
string outputText = "";
string result = "";

textBox7.Text = "";





if (String.IsNullOrEmpty(inputText))
{
    AES_ErrorTextbox.Text = ErrorStrings.ENCRYPTION_BOX_EMPTY_ERROR;
    return;
}

if (Record)//If reading from a file, & outputting to a file
{
    try
    {
        inputText = File.ReadAllText(inputText);
    }
    catch
    {
        AES_ErrorTextbox.Text = ErrorStrings.INPUT_FILE_READ_ERROR;
        return;
    }
    outputText = AES_DECRYPT_TEXTBOX.Text;
}


stopwatch.Start();//Start timer

AES_key = AES_StandardLibrary.Create256Key();//create new key
byte[] aes_encrypt_array = AES_StandardLibrary.AES_ENCRYPT_FILE(inputText, AES_key);//encrypt

string encrypt = Convert.ToBase64String(aes_encrypt_array);

//string encrypt = Encoding.UTF8.GetString(aes_encrypt_array);
//string decrypt = AES_StandardLibrary.AES_DECRYPT_FILE(aes_encrypt_array, AES_key).ToString();//decrypt

stopwatch.Stop();//End timer


result = "Time elasped:\t";
if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
    result += "Less than a Milisecond";
else
    result += stopwatch.ElapsedMilliseconds + " Milliseconds";

textBox7.Text = result;


stopwatch.Reset();//Let timer reset & rest

AES_PasswordKeyTextbox.Text = AES_key;
//result += "\r\nKey:\t\t" + AES_key;

if (Record && !(desktopFile))//If reading from a file, & outputting to a file
{
    //result += "\r\nDecrypted Text:\r\n\r\n" + decrypt;
    try
    {
        File.WriteAllText(outputText, encrypt);
    }
    catch
    {
        AES_ErrorTextbox.Text = ErrorStrings.OUTPUT_FILE_WRITE_ERROR;
    }
}
else if (desktopFile)//If we're making a new file
{
    FileWork.CreateFile(encrypt);
    //textBox7.Text = "\r\n\t[ERROR: WRITING TO OUTPUT FILE]";
    AES_DECRYPT_TEXTBOX.Text = encrypt;
}
else//If reading from the application, & outputting to the application
{
    result += "\r\nEncrypted Text:\t" + encrypt;
    result += "\r\nKey:\t\t" + AES_key;

    AES_DECRYPT_TEXTBOX.Text = encrypt;
}

textBox7.Text = result;

        }
//-------------------------------------
//-------------------------------------
 private void AES_DecryptButton_Click(object sender, EventArgs e)
        {//DECRYPTION AES Button

            Boolean Record = false;
            Boolean desktopFile = (AES_DesktopAdd.Checked);

            string inputText = AES_DECRYPT_TEXTBOX.Text;
            string outputText = "";
            string result = "";

            textBox7.Text = "";


            if (String.IsNullOrEmpty(inputText))
            {
                AES_ErrorTextbox.Text = ErrorStrings.DECRYPTION_BOX_EMPTY_ERROR;
                return;
            }

            if (Record)//If reading from a file, & outputting to a file
            {
                if (!(desktopFile))
                { 
                
                
                
                
                
                }


                try
                {
                    inputText = File.ReadAllText(inputText);
                }
                catch
                {
                    AES_ErrorTextbox.Text = ErrorStrings.INPUT_FILE_READ_ERROR;
                    return;
                }
                outputText = AES_DECRYPT_TEXTBOX.Text;
            }


          

            stopwatch.Start();//Start timer

            AES_key = AES_PasswordKeyTextbox.Text;//Read key from textbox
            //byte[] aes_encrypt_array = AES_StandardLibrary.AES_ENCRYPT_FILE(inputText, AES_key);//encrypt
            //string encrypt = Convert.ToBase64String(aes_encrypt_array);
            string decrypt = AES_StandardLibrary.AES_DECRYPT_FILE(Convert.FromBase64String(inputText), AES_key).ToString();//decrypt

            stopwatch.Stop();//End timer


            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            textBox7.Text = result;


            stopwatch.Reset();//Let timer reset & rest

            //result += "\r\nKey:\t\t" + AES_key;

            if (Record && !(desktopFile))//If reading from a file, & outputting to a file
            {
                //result += "\r\nDecrypted Text:\r\n\r\n" + decrypt;

                try
                {
                    File.WriteAllText(outputText, decrypt);
                }
                catch
                {
                    AES_ErrorTextbox.Text = ErrorStrings.OUTPUT_FILE_WRITE_ERROR;
                }
            }
            else if (desktopFile)//If we're making a new file
            {
                FileWork.CreateFile(decrypt);
                //textBox7.Text = "\r\n\t[ERROR: WRITING TO OUTPUT FILE]";
                AES_ENCRYPT_TEXTBOX.Text = decrypt;
            }
            else//If reading from the application, & outputting to the application
            {
                result += "\r\nDecrypted Text:\t" + decrypt;
                //result += "\r\nKey:\t\t" + AES_key;

                AES_ENCRYPT_TEXTBOX.Text = decrypt;
            }

            textBox7.Text = result;
        }
//-------------------------------------

 
 
 
 
-------------------------------------*/