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
            this.Width = 969;
            this.Height = 438;

            TABS.ItemSize = new System.Drawing.Size(0, 1);//Hide Tab_control from user

            //label5.Text = "lmao";
            //String tempstring = "\r\n\tSolar Data read from .txt;;;\r\n\r\n\t"
            //    + File.ReadAllText("solar.txt");//reads from project-folder -> bin -> Within debug folder
            //textBox7.Text = tempstring;
            
            tabPage2.AutoScroll = true;

            ML_DSA_SubtypeSelection.SelectedIndex = 0;
            PICNIC_SubtypeSelection.SelectedIndex = 0;

            hometextBox.Text = File.ReadAllText("EducationalTexts/home.txt");
            aestextBox.Text = File.ReadAllText("EducationalTexts/aes.txt");
            shifttextBox.Text = File.ReadAllText("EducationalTexts/shift.txt");
            mldsatextBox.Text = File.ReadAllText("EducationalTexts/lattice.txt");
            pcnctextBox.Text = File.ReadAllText("EducationalTexts/picnic.txt");
            vinegeretextBox.Text = File.ReadAllText("EducationalTexts/vine.txt");

        }



        public string AES_key = AES_StandardLibrary.Create256Key();//Start with Key
        public Stopwatch stopwatch = new Stopwatch();

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //HOME Tab
        private void HOMEButton_Click(object sender, EventArgs e)
        {//HOME Button
            TABS.SelectTab(0);
        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------





        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //AES Tab
        private void AESButton_Click(object sender, EventArgs e)
        {//STANDARD (AES) Button
            TABS.SelectTab(1);
        }
        //----------
        private void AES_EncryptButton_Click(object sender, EventArgs e)
        {//ENCRYPTION AES Button

            Boolean readAsPaths = (AES_PathOutput.Checked);
            Boolean desktopFile = (AES_DesktopAdd.Checked);

            string encryptBoxInput = @AES_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @AES_DECRYPT_TEXTBOX.Text;
            string result;

            AES_StatusTextbox.Text = "";

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
                    AES_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(encryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    AES_StatusTextbox.Text = MessageStrings.Error(6);
                    return;
                }
            }


            stopwatch.Start();//Start timer

            AES_key = AES_StandardLibrary.Create256Key();//create new key
            byte[] aes_encrypt_array = AES_StandardLibrary.AES_ENCRYPT_FILE(encryptBoxInput, AES_key);//encrypt
            string encrypt = Convert.ToBase64String(aes_encrypt_array);

            stopwatch.Stop();//End timer


            AES_StatusTextbox.Text = MessageStrings.Success(8);

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
                    AES_StatusTextbox.Text += MessageStrings.Error(4);
                    return;
                }
                AES_StatusTextbox.Text += MessageStrings.Success(4) + "\n";
            }
            else if (readAsPaths && desktopFile)//If we're ARE going to be outputting a new file to desktop
            {//, But we're reading from a file-path
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text += MessageStrings.Error(5); ;
                    return;
                }
                AES_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text += MessageStrings.Error(5);
                    return;
                }
                AES_DECRYPT_TEXTBOX.Text = encrypt;
                AES_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nEncrypted Text:\t" + encrypt;
                AES_DECRYPT_TEXTBOX.Text = encrypt;
                textBox7.Text = result;
            }

            AES_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void AES_DecryptButton_Click(object sender, EventArgs e)
        {//DECRYPTION AES Button

            Boolean readAsPaths = (AES_PathOutput.Checked);
            Boolean desktopFile = (AES_DesktopAdd.Checked);

            string encryptBoxInput = @AES_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @AES_DECRYPT_TEXTBOX.Text;
            string result;

            AES_StatusTextbox.Text = "";

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
                    AES_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(decryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    AES_StatusTextbox.Text = MessageStrings.Error(7);
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
                AES_StatusTextbox.Text = MessageStrings.Error(12);
                return;
            }

            stopwatch.Stop();//End timer


            AES_StatusTextbox.Text += MessageStrings.Success(9) + "\n";

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
                    AES_StatusTextbox.Text = MessageStrings.Error(4);
                    return;
                }
                AES_StatusTextbox.Text += MessageStrings.Success(4) + "\n";

            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(decrypt);
                }
                catch
                {
                    AES_StatusTextbox.Text = MessageStrings.Error(5);
                    return;
                }
                AES_ENCRYPT_TEXTBOX.Text = decrypt;
                AES_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nDecrypted Text:\t" + decrypt;
                AES_ENCRYPT_TEXTBOX.Text = decrypt;
                textBox7.Text = result;
            }
            AES_StatusTextbox.Text += MessageStrings.Success(0);
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



        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //ML-DSA TAB PAGE
        private void button6_Click_1(object sender, EventArgs e)
        {//ML-DSA TAB BUTTON
            TABS.SelectTab(3);
        }


        private void ML_DSA_GenerateButton_Click(object sender, EventArgs e)
        {//ML_DSA Generate Button

            bool temp;
            string result;
            ML_DSA_StatusTextbox.Text = "";

            stopwatch.Start();//Start timer

            temp = ML_DSA.ML_DSA_TEMPORARY_SUFFER(ref ML_DSA_PasswordKeyTextbox, ref ML_DSA_PrivateKeyTextbox,
                ref ML_DSA_PublicKeyTextbox, ref ML_DSA_StatusTextbox, ref ML_DSA_SubtypeSelection);

            stopwatch.Stop();//End timer

            if (temp)
            {
                ML_DSA_StatusTextbox.Text += MessageStrings.Success(10) + "\r\n";
                result = "\r\nTime elasped:\t";
                if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                    result += "Less than a Milisecond";
                else
                    result += stopwatch.ElapsedMilliseconds + " Milliseconds";
                ML_DSA_StatusTextbox.Text += result;
                stopwatch.Reset();//Let timer reset & rest

                if (ML_DSA_DesktopAdd.Checked)
                {//If writing to Desktop
                    string contents = ("Public:\r\n" + ML_DSA_PublicKeyTextbox.Text + "\r\n\r\nPrivate:\r\n" + ML_DSA_PrivateKeyTextbox.Text);
                    try
                    {
                        FileWork.CreateFile((contents));
                    }
                    catch
                    {
                        ML_DSA_StatusTextbox.Text += "\r\n" + MessageStrings.Error(5);
                        return;
                    }
                    ML_DSA_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
                }
                ML_DSA_StatusTextbox.Text += MessageStrings.Success(0);
            }
            else 
            {
                ML_DSA_StatusTextbox.Text += MessageStrings.Error(10);
            }
        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------






        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //PICNIC TAB PAGE
        private void PICNICButton_Click(object sender, EventArgs e)
        {//PICNIC TAB BUTTON
            TABS.SelectTab(4);
        }

        private void PICNIC_GenerateButton_Click(object sender, EventArgs e)
        {//PICNIC Generate Button
            bool temp;
            string result;
            PICNIC_StatusTextbox.Text = "";

            stopwatch.Start();//Start timer

            temp = PICNIC.PICNIC_TEMPORARY_SUFFER(ref PICNIC_PasswordKeyTextbox, ref PICNIC_PrivateKeyTextbox,
                ref PICNIC_PublicKeyTextbox, ref PICNIC_StatusTextbox, ref PICNIC_SubtypeSelection);

            stopwatch.Stop();//End timer

            if (temp)
            {
                PICNIC_StatusTextbox.Text += MessageStrings.Success(10) + "\r\n";
                result = "\r\nTime elasped:\t";
                if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                    result += "Less than a Milisecond";
                else
                    result += stopwatch.ElapsedMilliseconds + " Milliseconds";
                PICNIC_StatusTextbox.Text += result;
                stopwatch.Reset();//Let timer reset & rest

                if (PICNIC_DesktopAdd.Checked)
                {//If writing to Desktop
                    string contents = ("Public:\r\n" + PICNIC_PublicKeyTextbox.Text + "\r\n\r\nPrivate:\r\n" + PICNIC_PrivateKeyTextbox.Text);
                    try
                    {
                        FileWork.CreateFile((contents));
                    }
                    catch
                    {
                        PICNIC_StatusTextbox.Text += "\r\n" + MessageStrings.Error(5);
                        return;
                    }
                    PICNIC_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
                }
                PICNIC_StatusTextbox.Text += MessageStrings.Success(0);
            }
            else
            {
                PICNIC_StatusTextbox.Text += MessageStrings.Error(10);
            }

        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
















        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //SHIFT Tab Button
        private void SHIFTButton_Click(object sender, EventArgs e)
        {//SHIFT Button
            TABS.SelectTab(2);
        }

        //----------
        private void SHIFT_EncryptButton_Click(object sender, EventArgs e)
        {//ENCRYPTION SHIFT Button

            Boolean readAsPaths = (SHIFT_PathOutput.Checked);
            Boolean desktopFile = (SHIFT_DesktopAdd.Checked);

            string encryptBoxInput = @SHIFT_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @SHIFT_DECRYPT_TEXTBOX.Text;
            string result;

            SHIFT_StatusTextbox.Text = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref decryptBoxInput, ref encryptBoxInput, SHIFT_StatusTextbox)))
                        return;//Then, return.
                }
                //If Not returned;
                try//attempt to read non-empty text;
                {
                    encryptBoxInput = File.ReadAllText(encryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    SHIFT_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(encryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    SHIFT_StatusTextbox.Text = MessageStrings.Error(6);
                    return;
                }
            }

            int SHIFT_key = Convert.ToInt32(SHIFT_PasswordKey_NumBox.Value);
            if (SHIFT_key == 0)
            {
                SHIFT_StatusTextbox.Text += MessageStrings.ERROR("SHIFT cipher needs something to *shift* using!");
                return;
            }

            stopwatch.Start();//Start timer

            string encrypt = ShiftCipher.SHIFT_ENCRYPT_FILE(encryptBoxInput, SHIFT_key);

            stopwatch.Stop();//End timer


            SHIFT_StatusTextbox.Text += MessageStrings.Success(8);

            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + SHIFT_key;
            textBox9.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(decryptBoxInput, encrypt);
                }
                catch
                {
                    SHIFT_StatusTextbox.Text += MessageStrings.Error(4);
                    return;
                }
                SHIFT_StatusTextbox.Text += MessageStrings.Success(4) + "\n";
            }
            else if (readAsPaths && desktopFile)//If we're ARE going to be outputting a new file to desktop
            {//, But we're reading from a file-path
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    SHIFT_StatusTextbox.Text += MessageStrings.Error(5); ;
                    return;
                }
                SHIFT_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    SHIFT_StatusTextbox.Text += MessageStrings.Error(5);
                    return;
                }
                SHIFT_DECRYPT_TEXTBOX.Text += encrypt;
                SHIFT_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nEncrypted Text:\t" + encrypt;
                SHIFT_DECRYPT_TEXTBOX.Text = encrypt;
                textBox9.Text = result;
            }//7

            SHIFT_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void SHIFT_DecryptButton_Click(object sender, EventArgs e)
        {//DECRYPTION SHIFT_key Button

            Boolean readAsPaths = (SHIFT_PathOutput.Checked);
            Boolean desktopFile = (SHIFT_DesktopAdd.Checked);

            string encryptBoxInput = @SHIFT_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @SHIFT_DECRYPT_TEXTBOX.Text;
            string result;

            SHIFT_StatusTextbox.Text = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref encryptBoxInput, ref decryptBoxInput, SHIFT_StatusTextbox)))
                        return;//Then, return.
                }

                //If Not returned;
                try//attempt to read non-empty text;
                {
                    decryptBoxInput = File.ReadAllText(decryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    SHIFT_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(decryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    SHIFT_StatusTextbox.Text = MessageStrings.Error(7);
                    return;
                }
            }

            int SHIFT_key = Convert.ToInt32(SHIFT_PasswordKey_NumBox.Value);
            if (SHIFT_key == 0)
            {
                SHIFT_StatusTextbox.Text += MessageStrings.ERROR("SHIFT cipher needs something to *shift* using!");
                return;
            }

            string decrypt = "";

            stopwatch.Start();//Start timer

            try
            {
                decrypt = ShiftCipher.SHIFT_DECRYPT_FILE(decryptBoxInput, SHIFT_key); ;//decrypt
            }
            catch (System.FormatException)
            {
                SHIFT_StatusTextbox.Text = MessageStrings.Error(12);
                return;
            }

            stopwatch.Stop();//End timer

            SHIFT_StatusTextbox.Text += MessageStrings.Success(9) + "\n";

            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + SHIFT_key;
            textBox9.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(encryptBoxInput, decrypt);
                }
                catch
                {
                    SHIFT_StatusTextbox.Text = MessageStrings.Error(4);
                    return;
                }
                SHIFT_StatusTextbox.Text += MessageStrings.Success(4) + "\n";

            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(decrypt);
                }
                catch
                {
                    SHIFT_StatusTextbox.Text = MessageStrings.Error(5);
                    return;
                }
                SHIFT_ENCRYPT_TEXTBOX.Text = decrypt;
                SHIFT_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nDecrypted Text:\t" + decrypt;
                SHIFT_ENCRYPT_TEXTBOX.Text = decrypt;
                textBox9.Text = result;
            }
            SHIFT_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void SHIFT_ENCRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((SHIFT_PathOutput.Checked) && (SHIFT_ENCRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((SHIFT_ENCRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (SHIFT_ENCRYPT_TEXTBOX.Text.ElementAt(SHIFT_ENCRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    SHIFT_ENCRYPT_TEXTBOX.Text = SHIFT_ENCRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //----------
        private void SHIFT_DECRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((SHIFT_PathOutput.Checked) && (SHIFT_DECRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((SHIFT_DECRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (SHIFT_DECRYPT_TEXTBOX.Text.ElementAt(SHIFT_DECRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    SHIFT_DECRYPT_TEXTBOX.Text = SHIFT_DECRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------




















        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //VIGENERE_BEAUFORT Tab Button
        private void VIGENERE_BEAUFORTButton_Click(object sender, EventArgs e)
        {//VIGENERE_BEAUFORT Button
            TABS.SelectTab(5);
        }

        //----------
        private void VIGENERE_BEAUFORT_EncryptButton_Click(object sender, EventArgs e)
        {//ENCRYPTION VIGENERE_BEAUFORT Button

            Boolean readAsPaths = (VIGENERE_BEAUFORT_PathOutput.Checked);
            Boolean desktopFile = (VIGENERE_BEAUFORT_DesktopAdd.Checked);

            string encryptBoxInput = @VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text;
            string result;

            VIGENERE_BEAUFORT_StatusTextbox.Text = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref decryptBoxInput, ref encryptBoxInput, VIGENERE_BEAUFORT_StatusTextbox)))
                        return;//Then, return.
                }
                //If Not returned;
                try//attempt to read non-empty text;
                {
                    encryptBoxInput = File.ReadAllText(encryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(encryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(6);
                    return;
                }
            }

            string VIGENERE_BEAUFORT_key = VIGENERE_BEAUFORT_PasswordKeyTextbox.Text;
            if (VIGENERE_BEAUFORT_key == null || VIGENERE_BEAUFORT_key.Length == 0)
            {
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.ERROR("VIGENERE_BEAUFORT cipher needs a lettered key to encrypt with!");
                return;
            }

            stopwatch.Start();//Start timer

            string encrypt = VIGENERE_BEAUFORTCipher.VIGENERE_BEAUFORT_ENCRYPT_FILE(encryptBoxInput, VIGENERE_BEAUFORT_key);

            stopwatch.Stop();//End timer

            VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(8);

            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + VIGENERE_BEAUFORT_key;
            textBox13.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(decryptBoxInput, encrypt);
                }
                catch
                {
                    VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Error(4);
                    return;
                }
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(4) + "\n";
            }
            else if (readAsPaths && desktopFile)//If we're ARE going to be outputting a new file to desktop
            {//, But we're reading from a file-path
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Error(5); ;
                    return;
                }
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Error(5);
                    return;
                }
                VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text += encrypt;
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nEncrypted Text:\t" + encrypt;
                VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text = encrypt;
                textBox13.Text = result;
            }//7

            VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void VIGENERE_BEAUFORT_DecryptButton_Click(object sender, EventArgs e)
        {//DECRYPTION VIGENERE_BEAUFORT

            Boolean readAsPaths = (VIGENERE_BEAUFORT_PathOutput.Checked);
            Boolean desktopFile = (VIGENERE_BEAUFORT_DesktopAdd.Checked);

            string encryptBoxInput = @VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text;
            string result;

            VIGENERE_BEAUFORT_StatusTextbox.Text = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref encryptBoxInput, ref decryptBoxInput, VIGENERE_BEAUFORT_StatusTextbox)))
                        return;//Then, return.
                }

                //If Not returned;
                try//attempt to read non-empty text;
                {
                    decryptBoxInput = File.ReadAllText(decryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(decryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(7);
                    return;
                }
            }

            string VIGENERE_BEAUFORT_key = VIGENERE_BEAUFORT_PasswordKeyTextbox.Text;
            if (VIGENERE_BEAUFORT_key == null || VIGENERE_BEAUFORT_key.Length == 0)
            {
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.ERROR("VIGENERE_BEAUFORT cipher needs a lettered key to encrypt with!");
                return;
            }

            string decrypt = "";

            stopwatch.Start();//Start timer

            try
            {
                    decrypt = VIGENERE_BEAUFORTCipher.VIGENERE_BEAUFORT_DECRYPT_FILE(decryptBoxInput, VIGENERE_BEAUFORT_key); ;//decrypt
            }
            catch (System.FormatException)
            {
                VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(12);
                return;
            }

            stopwatch.Stop();//End timer


            VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(9) + "\n";

            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + VIGENERE_BEAUFORT_key;
            textBox13.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(encryptBoxInput, decrypt);
                }
                catch
                {
                    VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(4);
                    return;
                }
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(4) + "\n";

            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(decrypt);
                }
                catch
                {
                    VIGENERE_BEAUFORT_StatusTextbox.Text = MessageStrings.Error(5);
                    return;
                }
                VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text = decrypt;
                VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nDecrypted Text:\t" + decrypt;
                VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text = decrypt;
                textBox13.Text = result;
            }
            VIGENERE_BEAUFORT_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((VIGENERE_BEAUFORT_PathOutput.Checked) && (VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text.ElementAt(VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text = VIGENERE_BEAUFORT_ENCRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //----------
        private void VIGENERE_BEAUFORT_DECRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((VIGENERE_BEAUFORT_PathOutput.Checked) && (VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text.ElementAt(VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text = VIGENERE_BEAUFORT_DECRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel53_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel50_Paint(object sender, PaintEventArgs e)
        {

        }


        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------











        /*





        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //BLOWFISH Tab Button
        private void BLOWFISHButton_Click(object sender, EventArgs e)
        {//BLOWFISH Button
            TABS.SelectTab(6);
        }
        //----------
        private void BLOWFISH_EncryptButton_Click(object sender, EventArgs e)
        {//ENCRYPTION BLOWFISH Button

            Boolean readAsPaths = (BLOWFISH_PathOutput.Checked);
            Boolean desktopFile = (BLOWFISH_DesktopAdd.Checked);

            string encryptBoxInput = @BLOWFISH_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @BLOWFISH_DECRYPT_TEXTBOX.Text;
            string result;

            BLOWFISH_StatusTextbox.Text = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref decryptBoxInput, ref encryptBoxInput, BLOWFISH_StatusTextbox)))
                        return;//Then, return.
                }
                //If Not returned;
                try//attempt to read non-empty text;
                {
                    encryptBoxInput = File.ReadAllText(encryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    BLOWFISH_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(encryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    BLOWFISH_StatusTextbox.Text = MessageStrings.Error(6);
                    return;
                }
            }

            string BLOWFISH_key = BLOWFISH_PasswordKeyTextbox.Text;
            if (BLOWFISH_key == null || BLOWFISH_key.Length == 0)
            {//Generate one at random, instead??
                BLOWFISH_StatusTextbox.Text += MessageStrings.ERROR("BLOWFISH cipher needs a lettered key to encrypt with!");
                BLOWFISH_StatusTextbox.Text += MessageStrings.ERROR("Using string [ERROR]");
                BLOWFISH_key = "ERROR";
            }

            stopwatch.Start();//Start timer

            string encrypt = Blowfish.BLOWFISH_ENCRYPT_FILE(encryptBoxInput, BLOWFISH_key);

            stopwatch.Stop();//End timer

            BLOWFISH_StatusTextbox.Text += MessageStrings.Success(8);

            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + BLOWFISH_key;
            textBox20.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(decryptBoxInput, encrypt);
                }
                catch
                {
                    BLOWFISH_StatusTextbox.Text += MessageStrings.Error(4);
                    return;
                }
                BLOWFISH_StatusTextbox.Text += MessageStrings.Success(4) + "\n";
            }
            else if (readAsPaths && desktopFile)//If we're ARE going to be outputting a new file to desktop
            {//, But we're reading from a file-path
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    BLOWFISH_StatusTextbox.Text += MessageStrings.Error(5); ;
                    return;
                }
                BLOWFISH_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(encrypt);
                }
                catch
                {
                    BLOWFISH_StatusTextbox.Text += MessageStrings.Error(5);
                    return;
                }
                BLOWFISH_DECRYPT_TEXTBOX.Text += encrypt;
                BLOWFISH_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nEncrypted Text:\t" + encrypt;
                BLOWFISH_DECRYPT_TEXTBOX.Text = encrypt;
                textBox20.Text = result;
            }//7

            BLOWFISH_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void BLOWFISH_DecryptButton_Click(object sender, EventArgs e)
        {//DECRYPTION BLOWFISH

            Boolean readAsPaths = (BLOWFISH_PathOutput.Checked);
            Boolean desktopFile = (BLOWFISH_DesktopAdd.Checked);

            string encryptBoxInput = @BLOWFISH_ENCRYPT_TEXTBOX.Text;
            string decryptBoxInput = @BLOWFISH_DECRYPT_TEXTBOX.Text;
            string result;

            BLOWFISH_StatusTextbox.Text = "";

            if (readAsPaths)//If we're going to be reading & writing to file(s)
            {
                if (!(desktopFile))//If we're NOT going to be outputting a new file to desktop
                {
                    //Below;
                    //If input & output boxes are empty, If files could not be found, if files could not be read,
                    //Etc,
                    if (!(FileWork.CheckPathFiles(ref encryptBoxInput, ref decryptBoxInput, BLOWFISH_StatusTextbox)))
                        return;//Then, return.
                }

                //If Not returned;
                try//attempt to read non-empty text;
                {
                    decryptBoxInput = File.ReadAllText(decryptBoxInput);
                }
                catch
                {//File could NOT be read; showcase ERROR
                    BLOWFISH_StatusTextbox.Text = MessageStrings.Error(1);
                    return;
                }
            }
            else//Else, if we're reading as contents, towards the application;
            {
                if (String.IsNullOrEmpty(decryptBoxInput))//Is there not an input?
                {//If empty, showcase ERROR
                    BLOWFISH_StatusTextbox.Text = MessageStrings.Error(7);
                    return;
                }
            }

            string BLOWFISH_key = BLOWFISH_PasswordKeyTextbox.Text;
            if (BLOWFISH_key == null || BLOWFISH_key.Length == 0)
            {//ngksjngrg
                BLOWFISH_StatusTextbox.Text += MessageStrings.ERROR("BLOWFISH cipher needs a lettered key to encrypt with!");
                BLOWFISH_StatusTextbox.Text += MessageStrings.ERROR("Using string [ERROR]");
                BLOWFISH_key = "ERROR";
            }

            string decrypt = "";

            stopwatch.Start();//Start timer

            try
            {
                decrypt = Blowfish.BLOWFISH_DECRYPT_FILE(decryptBoxInput, BLOWFISH_key); ;//decrypt
            }
            catch (System.FormatException)
            {
                BLOWFISH_StatusTextbox.Text = MessageStrings.Error(12);
                return;
            }

            stopwatch.Stop();//End timer

            BLOWFISH_StatusTextbox.Text += MessageStrings.Success(9) + "\n";

            result = "Time elasped:\t";
            if (stopwatch.ElapsedMilliseconds <= 0)//check formatting for time-elasped
                result += "Less than a Milisecond";
            else
                result += stopwatch.ElapsedMilliseconds + " Milliseconds";

            result += "\r\nKey:\t\t" + BLOWFISH_key;
            textBox20.Text = result;

            stopwatch.Reset();//Let timer reset & rest

            if (readAsPaths && !(desktopFile))//If we're NOT going to be outputting a new file to desktop
            {//But instead, the outputfile!
                try
                {
                    File.WriteAllText(encryptBoxInput, decrypt);
                }
                catch
                {
                    BLOWFISH_StatusTextbox.Text = MessageStrings.Error(4);
                    return;
                }
                BLOWFISH_StatusTextbox.Text += MessageStrings.Success(4) + "\n";

            }
            else if (desktopFile)//If we ARE going to outputting a new file to Desktop;;;
            {//But the input isn't from a file, but the application's contents;;;
                try
                {
                    FileWork.CreateFile(decrypt);
                }
                catch
                {
                    BLOWFISH_StatusTextbox.Text = MessageStrings.Error(5);
                    return;
                }
                BLOWFISH_ENCRYPT_TEXTBOX.Text = decrypt;
                BLOWFISH_StatusTextbox.Text += MessageStrings.Success(5) + "\n";
            }
            else//Else, if we're reading and writing to, contents;
            {
                result += "\r\nDecrypted Text:\t" + decrypt;
                BLOWFISH_ENCRYPT_TEXTBOX.Text = decrypt;
                textBox20.Text = result;
            }
            BLOWFISH_StatusTextbox.Text += MessageStrings.Success(0);
        }
        //----------
        private void BLOWFISH_ENCRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((BLOWFISH_PathOutput.Checked) && (BLOWFISH_ENCRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((BLOWFISH_ENCRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (BLOWFISH_ENCRYPT_TEXTBOX.Text.ElementAt(BLOWFISH_ENCRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    BLOWFISH_ENCRYPT_TEXTBOX.Text = BLOWFISH_ENCRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //----------
        private void BLOWFISH_DECRYPT_TEXTBOX_TextChanged(object sender, EventArgs e)
        {//If we're reading as paths- auto-remove path-quotes for convienece of user
            if ((BLOWFISH_PathOutput.Checked) && (BLOWFISH_DECRYPT_TEXTBOX.Text.Length >= 3))
            {//If read as paths is true AND if the textbox's length is at least three
                if ((BLOWFISH_DECRYPT_TEXTBOX.Text.ElementAt(0) == '\"') && (BLOWFISH_DECRYPT_TEXTBOX.Text.ElementAt(BLOWFISH_DECRYPT_TEXTBOX.Text.Length - 1) == '\"'))
                {//If surrounded by quotes, remove those.
                    BLOWFISH_DECRYPT_TEXTBOX.Text = BLOWFISH_DECRYPT_TEXTBOX.Text.Replace('"', ' ').Trim();
                }
            }
        }
        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------

        */












        //GET RID OF, LIKE; ALL OF THESE;;;;









        //--------------------------------------------------------
        //--------------------------------------------------------
    }
}
