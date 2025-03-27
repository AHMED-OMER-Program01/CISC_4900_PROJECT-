using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsForms4900;
using WindowsForms4900.EncryptionAlgols;
using WindowsForms4900.HelperFunctions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

/*--------------------------------------------------------
//--------------------------------------------------------
TODO;;;

    -   UPDATE GITHUB!!

    -   Replace garfeild image with textbox explaining proper usage!!

    -   WORK ON KYBER ALREADY GWAD
    -   -   Use .Pqc.[something], ye?

    -   Have Errortextbox notify WHEN STUFF WORKS
        AND WHAT SPECIFICALLY "decrypted message in textbox 
        to encyption textbox/file created onto desktop/file given
        in decyption textbox successfully", etc, etc!


DO LATER, GOD; JUST DO KYBER ALREADY;;;;

(some saved) DONEActions; ;;

    -   Make the "[AES_ErrorTextbox]" textbox UNWRITABLE!
        Keep it *selectable*, though, obviously!

    -   Have it so, if the "READ AS PATHS" button
        Is triggered as true, then the text-boxes auto-remove
        quotation marks, if there are some on BOTH ends;
            THIS way, the user can paste copy-as-paths
            more easily!

    -   implement the "create new file" & "to desktop" stuff!

    -   Make Radio-Option click; to have the inputbox
    of each exncryption page OPTIONALLY *read from file
    based-off of the text given ( interperted as a PATH,
    I mean ),   OR,     just that text, straight out
        -   Have Same for decryptor ( Considering text may be large,
        Perhaps either hardline-make the one selection effect both,
        Or a drop-down, or...??


    - MAKE IT SO;;;
        If you ENCRYPT to path; create/overwrite file with .txt
        With encrypted version of inputfile.

        If you DECRYPT tp path;
        What currently happens; showcase decrypted thingy

    - When "Create to desktop" is clicked; .txt is named after time & date

    - Have textbox showcase errors when happen, Also, state WHEN IT WORKS

    --And all that, yeah???? Like; "lngtgre.txt" read to!
       "lfrejgvre.txt" was overwritten!
       The Encryption/Decryption file was added to the desktop!
        Etc

//--------------------------------------------------------
//--------------------------------------------------------
*/

namespace WindowsForms4900
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        //--------------------------------------------------------
        //--------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.ItemSize = new System.Drawing.Size(0, 1);//Hide Tab_control from user

            //label5.Text = "lmao";
            String tempstring = "\r\n\tSolar Data read from .txt;;;\r\n\r\n\t"
                + File.ReadAllText("solar.txt");//reads from project-folder -> bin -> Within debug folder
            textBox7.Text = tempstring;
        }



        public string AES_key = AES_StandardLibrary.Create256Key();//Start with Key
        public Stopwatch stopwatch = new Stopwatch();

        //--------------------------------------------------------
        //--------------------------------------------------------
        private void HOMEButton_Click(object sender, EventArgs e)
        {//HOME Button
            tabControl1.SelectTab(0);
        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        private void AESButton_Click(object sender, EventArgs e)
        {//STANDARD (AES) Button
            tabControl1.SelectTab(1);
        }
        //----------
        private void AES_EncryptButton_Click(object sender, EventArgs e)
        {//ENCRYPTION AES Button

            Boolean readAsPaths = (AES_PathOutput.Checked);
            Boolean desktopFile = (AES_DesktopAdd.Checked);

            string encryptBoxInput = @AES_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @AES_DECRYPT_TEXTBOX.Text;
            string result = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref decryptBoxInput, ref encryptBoxInput, AES_StatusTextbox)))
                        return;//Then, return.
                }
                //If Not returned;
                try//attempt to read non-empty text;
                {
                    encryptBoxInput = File.ReadAllText(encryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    AES_StatusTextbox.Text = ErrorStrings.INPUT_FILE_READ_ERROR;
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(encryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    AES_StatusTextbox.Text = ErrorStrings.ENCRYPTION_BOX_EMPTY_ERROR;
                    return;
                }
            }


            stopwatch.Start();//Start timer

            AES_key = AES_StandardLibrary.Create256Key();//create new key
            byte[] aes_encrypt_array = AES_StandardLibrary.AES_ENCRYPT_FILE(encryptBoxInput, AES_key);//encrypt
            string encrypt = Convert.ToBase64String(aes_encrypt_array);

            stopwatch.Stop();//End timer


            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + AES_key;
            AES_PasswordKeyTextbox.Text = AES_key;
            textBox7.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(decryptBoxInput, encrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = ErrorStrings.OUTPUT_FILE_WRITE_ERROR;
                    return;
                }
                AES_StatusTextbox.Text = SuccessStrings.OUTPUT_OVERWRITE_FILE_SUCCESS;
            }
            else if (readAsPaths && desktopFile)//If we're ARE going to be outputting a new file to desktop
            {//, But we're reading from a file-path
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = ErrorStrings.FILE_CREATION_ERROR;
                    return;
                }
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = ErrorStrings.FILE_CREATION_ERROR;
                    return;
                }
                AES_DECRYPT_TEXTBOX.Text = encrypt;
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nEncrypted Text:\t" + encrypt;
                AES_DECRYPT_TEXTBOX.Text = encrypt;
                textBox7.Text = result;
            }

            AES_StatusTextbox.Text = "";
        }
        //----------
        private void AES_DecryptButton_Click(object sender, EventArgs e)
        {//DECRYPTION AES Button

            Boolean readAsPaths = (AES_PathOutput.Checked);
            Boolean desktopFile = (AES_DesktopAdd.Checked);

            string encryptBoxInput = @AES_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @AES_DECRYPT_TEXTBOX.Text;
            string result = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref encryptBoxInput, ref decryptBoxInput, AES_StatusTextbox)))
                        return;//Then, return.
                }

                //If Not returned;
                try//attempt to read non-empty text;
                {
                    decryptBoxInput = File.ReadAllText(decryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    AES_StatusTextbox.Text = ErrorStrings.INPUT_FILE_READ_ERROR;
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(decryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    AES_StatusTextbox.Text = ErrorStrings.DECRYPTION_BOX_EMPTY_ERROR;
                    return;
                }
            }


            stopwatch.Start();//Start timer

            AES_key = AES_PasswordKeyTextbox.Text;//Read key from textbox

            string decrypt = "";

            try
            {
                decrypt = AES_StandardLibrary.AES_DECRYPT_FILE(Convert.FromBase64String(decryptBoxInput), AES_key).ToString();//decrypt
            }
            catch (System.FormatException)
            {
                AES_StatusTextbox.Text = ErrorStrings.OUTPUT_FILE_WRITE_ERROR;
                return;
            }

            stopwatch.Stop();//End timer


            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + AES_key;
            textBox7.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(encryptBoxInput, decrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = ErrorStrings.OUTPUT_FILE_WRITE_ERROR;
                    return;
                }
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(decrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = ErrorStrings.FILE_CREATION_ERROR;
                    return;
                }
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(decrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = ErrorStrings.FILE_CREATION_ERROR;
                    return;
                }
                AES_ENCRYPT_TEXTBOX.Text = decrypt;
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nDecrypted Text:\t" + decrypt;
                AES_ENCRYPT_TEXTBOX.Text = decrypt;
                textBox7.Text = result;
            }
            AES_StatusTextbox.Text = "";
        }
        //----------
        private void AES_ENCRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((AES_PathOutput.Checked) && (AES_ENCRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((AES_ENCRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (AES_ENCRYPT_TEXTBOX.Text.ElementAt(AES_ENCRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    AES_ENCRYPT_TEXTBOX.Text = AES_ENCRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //----------
        private void AES_DECRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((AES_PathOutput.Checked) && (AES_DECRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((AES_DECRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (AES_DECRYPT_TEXTBOX.Text.ElementAt(AES_DECRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    AES_DECRYPT_TEXTBOX.Text = AES_DECRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        private void button5_Click(object sender, EventArgs e)
        {//Encryption Type 2 Button
            tabControl1.SelectTab(2);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {


        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FileWork.help(AES_DECRYPT_TEXTBOX.Text);
        }






        //GET RID OF, LIKE; ALL OF THESE;;;;





        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        //--------------------------------------------------------
        //--------------------------------------------------------
    }
}
