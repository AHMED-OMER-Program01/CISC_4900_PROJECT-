using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;
using System.Security;

//--------------------------------------------------------
//--------------------------------------------------------
//BOUNCY CASTLE BS LET GOOOOOOOOO
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities.Encoders;
//using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;//Dilithium stuffs, specifically
using Org.BouncyCastle.Pqc.Crypto.Saber;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crmf;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Pqc.Crypto.Picnic;
using System.Windows.Forms.VisualStyles;//Saber stuffs, specifically
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Drawing;
using System.Xml.Linq;
//using Org.BouncyCastle.Crypto.Generators;

/*
 Okay, so;

AES allows only 128, 192, or 256 bit keys
    ( 16, 24, & 32 length chars );

Let's regulate it to a strict 16 length,
and have the windows-form allow an optional 
    256 IS CONSIDERED QUANTUM RESISTANT; REMEMBER THAT!!!
 
 TO DO;;

 - Instead of throwing error exceptions
    have errors flip boolean
    To which prints specified message
    To WinFroms/The Screen?
 */

namespace WindowsForms4900.EncryptionAlgols
{
    internal class Algol_01_Test
    {
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public static class AES_StandardLibrary
    {
        //private static string result_Error = HelperFunctions.MessageStrings.Error(0);
        private static readonly int intLog = 8;//Assuming 256 key-length rn

        //public static bool keyError = false;//dumb.

        public static string Create256Key()
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.KeySize = 256;//MAKE CHANGEBALE, LATER?
                //BASED OFF OF RADIO-BUTTON PROMPT, IN WinForm???

                myAes.GenerateKey();
                return Convert.ToBase64String(myAes.Key);
            }
        }

        //=================================================================
        //ENCRYPTION FUNCTION==============================================
        public static byte[] AES_ENCRYPT_FILE(string plainText, string key)
        {//CURRENTLY ASSUMES 256 KEY USED

            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));

            byte[] result = Encoding.ASCII.GetBytes(HelperFunctions.MessageStrings.Error(0));

            //---------------------------------------------------
            //ACTUAL ENCRYPTION ---------------------------------
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = Convert.FromBase64String(key);

                //Replace 8 with log2(keysize)
                //( 8 is for 256, since 2^8 = 256 )
                myAes.IV = new byte[myAes.BlockSize / intLog];

                myAes.Mode = CipherMode.CBC;
                myAes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = myAes.CreateEncryptor(myAes.Key, myAes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    result = msEncrypt.ToArray();
                }
            }
            return result;
        }//END OF [ AES_ENCRYPT_FILE ] function

        //=================================================================
        //DECRYPTION FUNCTION==============================================
        public static string AES_DECRYPT_FILE(byte[] encryptedTextByteArr, string key)
        {//CURRENTLY ASSUMES 256 KEY USED

            string result = HelperFunctions.MessageStrings.Error(0);

            //---------------------------------------------------
            //ACTUAL DECRYPTION ---------------------------------
            using (Aes myAes = Aes.Create())
            {
                //catch(System.FormatException)
                //catch (System.Security.Cryptography.CryptographicException)
                try
                {
                    myAes.Key = Convert.FromBase64String(key);
                }
                catch (Exception)
                {
                    try
                    {
                        myAes.Key = Encoding.ASCII.GetBytes(key);
                    }
                    catch (Exception)
                    {
                        //keyError = true;//dumb.
                        return result;
                    }
                }

                //Replace 8 with log2(keysize)
                //( 8 is for 256, since 2^8 = 256 )
                myAes.IV = new byte[myAes.BlockSize / intLog];

                myAes.Mode = CipherMode.CBC;
                myAes.Padding = PaddingMode.PKCS7;


                ICryptoTransform decryptor = myAes.CreateDecryptor(myAes.Key, myAes.IV);

                try
                {
                    using (MemoryStream msDecrypt = new MemoryStream(encryptedTextByteArr))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        result = srDecrypt.ReadToEnd();
                    }
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    //keyError = true;//dumb.
                    result = HelperFunctions.MessageStrings.Error(0);
                }
            }
            return result;
        }//END OF [ AES_DECRYPT_FILE ] function
    }
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------


    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public class ML_DSA
    {
        private static readonly string result_Error = HelperFunctions.MessageStrings.Error(0);
        private static readonly string result_Success = HelperFunctions.MessageStrings.Success(0);
        /*---------------------------------------------------
            hardlining it to the ML-DSA-65 param-set; sue me.
        Maybe make it like, a selection menu in the actual 
        form, and have a string be sent in, that hits a 
        switch-case or whatever; if you *really* wanted to 
        ---------------------------------------------------*/
        //private static MLDsaParameters ml_dsa_param = MLDsaParameters.ml_dsa_65;

        //public static bool keyError = false;
        public static bool ML_DSA_TEMPORARY_SUFFER(ref System.Windows.Forms.TextBox givenPasswordScale, ref System.Windows.Forms.TextBox privateKeyBox,
            ref System.Windows.Forms.TextBox publicKeyBox, ref System.Windows.Forms.TextBox statusBox, ref System.Windows.Forms.ComboBox selectionBox)
        {
            string plainText = result_Error;
            if (!(givenPasswordScale.Text == null || givenPasswordScale.Text.Length <= 0))
            {
                plainText = givenPasswordScale.Text;
                //throw new ArgumentNullException(nameof(plainText));
            }

            MLDsaParameters ml_dsa_param;
            String parameterText;
            switch (selectionBox.SelectedIndex)
            {
                case 0:// "ML - DSA 44(128 bit)":
                    ml_dsa_param = MLDsaParameters.ml_dsa_44;
                    parameterText = "ML-DSA-44";
                    break;
                case 1:// "ML - DSA 65(192 bit)":
                    ml_dsa_param = MLDsaParameters.ml_dsa_65;
                    parameterText = "ML-DSA-65";
                    break;
                case 2:// "ML - DSA 87(256 bit)":
                    ml_dsa_param = MLDsaParameters.ml_dsa_87;
                    parameterText = "ML-DSA-87";
                    break;
                default:
                    ml_dsa_param = MLDsaParameters.ml_dsa_44;
                    parameterText = "ML-DSA-44";
                    statusBox.Text += HelperFunctions.MessageStrings.Error(11) + " : DEFAULTING TO ml_dsa_44\n";
                    break;
            }

            SecureRandom randomGen = new SecureRandom();

            //else if (randomGen == null)
            //    throw new ArgumentNullException(nameof(randomGen));

            randomGen = new SecureRandom();
            var keyPairGenMLDsa = new MLDsaKeyPairGenerator();

            keyPairGenMLDsa.Init(new MLDsaKeyGenerationParameters(randomGen, ml_dsa_param));
            var key = keyPairGenMLDsa.GenerateKeyPair();

            //byte[] result = Encoding.ASCII.GetBytes(result_Error);
            byte[] givenQuoteUnquoteMessage = Encoding.ASCII.GetBytes(plainText);
            //X509Certificate2Enumerator.ReferenceEquals(result, key);//Why the HELL did I include this??? 

            publicKeyBox.Text = Convert.ToBase64String(((MLDsaPublicKeyParameters)key.Public).GetEncoded());
            privateKeyBox.Text = Convert.ToBase64String(((MLDsaPrivateKeyParameters)key.Private).GetEncoded());

            // Create ML-DSA signer.
            var ML_DSA_sign_maker = SignerUtilities.InitSigner(parameterText, forSigning: true, key.Private, randomGen);

            // Generate ML-DSA signature.
            ML_DSA_sign_maker.BlockUpdate(givenQuoteUnquoteMessage, 0, givenQuoteUnquoteMessage.Length);
            byte[] ML_DSA_signature = ML_DSA_sign_maker.GenerateSignature();

            // Verify ML-DSA signature.
            var ML_DSA_verifer = SignerUtilities.InitSigner(parameterText, forSigning: false, key.Public, random: null);
            ML_DSA_verifer.BlockUpdate(givenQuoteUnquoteMessage, 0, givenQuoteUnquoteMessage.Length);
            //key.Private;

            if (ML_DSA_verifer.VerifySignature(ML_DSA_signature))
            {//If true, as far as [ ML_DSA_verifer ] is aware, we have +
                //result = ML_DSA_signature;//Console.WriteLine("ML-DSA-65 signature created and verified successfully");
                statusBox.Text = result_Success;
                return true;
            }

            statusBox.Text = result_Error;
            return false;

        }


        /*
         
        public static AsymmetricCipherKeyPair Create_ML_DSA_KeyPair()
        {
            ///*---------------------------------------------------
            //    hardlining it to the ML-DSA-65 param-set; sue me.
            //Maybe make it like, a selection menu in the actual 
            //form, and have a string be sent in, that hits a 
            //switch-case or whatever; if you *really* wanted to 
            //---------------------------------------------------
            SecureRandom randomGen = new SecureRandom();
            MLDsaParameters ml_dsa_param = MLDsaParameters.ml_dsa_65;
            randomGen = new SecureRandom();
            var keyPairGenMLDsa = new MLDsaKeyPairGenerator();

            keyPairGenMLDsa.Init(new MLDsaKeyGenerationParameters(randomGen, ml_dsa_param));
            var keyPair = keyPairGenMLDsa.GenerateKeyPair();
            return keyPair;
        

        public static byte[] ML_DSA_CREATE_AND_CHECK_KEYS(string plainText, ref AsymmetricCipherKeyPair key)
        {//NOTE; [ key is passed in as a ref!!! ]

            SecureRandom randomGen = new SecureRandom();

            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            else if (key == null)
                throw new ArgumentNullException(nameof(key));
            else if (randomGen == null)
                throw new ArgumentNullException(nameof(randomGen));


            //byte[] result = Encoding.ASCII.GetBytes(result_Error);
            byte[] result = Encoding.ASCII.GetBytes(plainText);
            //X509Certificate2Enumerator.ReferenceEquals(result, key);//Why the HELL did I include this??? 

            // Create ML-DSA signer.
            var ML_DSA_sign_maker = SignerUtilities.InitSigner("ML-DSA-65", forSigning: true, key.Private, randomGen);

            // Generate ML-DSA signature.
            ML_DSA_sign_maker.BlockUpdate(result, 0, result.Length);
            byte[] ML_DSA_signature = ML_DSA_sign_maker.GenerateSignature();

            // Verify ML-DSA signature.
            var ML_DSA_verifer = SignerUtilities.InitSigner("ML-DSA-65", forSigning: false, key.Public, random: null);
            ML_DSA_verifer.BlockUpdate(result, 0, result.Length);
            //key.Private;

            if (ML_DSA_verifer.VerifySignature(ML_DSA_signature))
            {//If true, as far as [ ML_DSA_verifer ] is aware, we have +
                //result = ML_DSA_signature;//Console.WriteLine("ML-DSA-65 signature created and verified successfully");
            }
            else
            {
                result = Convert.FromBase64String(result_Error);
            }
            return result;

        }

        public static byte[] ML_DSA_CHECK_KEYS(string publicKey, string privateKey)
        {//NOTE; [ key is passed in as a ref!!! ]

            //AsymmetricCipherKeyPair
            //&privK. = Encoding.ASCII.GetBytes(publicKey);
            //byte[] result = Encoding.ASCII.GetBytes(result_Error);
            //byte[] result = Encoding.ASCII.GetBytes(plainText);
            //X509Certificate2Enumerator.ReferenceEquals(result, key);//Why the HELL did I include this??? 

            //
            //byte[] result = new byte[32];

            //// Create ML-DSA signer.
            //var ML_DSA_sign_maker = SignerUtilities.InitSigner("ML-DSA-65", forSigning: true, key.Private, randomGen);

            //// Generate ML-DSA signature.
            //ML_DSA_sign_maker.BlockUpdate(result, 0, result.Length);
            //byte[] ML_DSA_signature = ML_DSA_sign_maker.GenerateSignature();

            //// Verify ML-DSA signature.
            //var ML_DSA_verifer = SignerUtilities.InitSigner("ML-DSA-65", forSigning: false, key.Public, random: null);
            //ML_DSA_verifer.BlockUpdate(result, 0, result.Length);
            ////key.Private;

            //if (ML_DSA_verifer.VerifySignature(ML_DSA_signature))
            //{//If true, as far as [ ML_DSA_verifer ] is aware, we have +
            //    //result = ML_DSA_signature;//Console.WriteLine("ML-DSA-65 signature created and verified successfully");
            //}
            //else
            //{
            //    result = Convert.FromBase64String(result_Error);
            //}
            //return result;
            //
            return (new Byte[0]);
        }
        */
    }
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------





    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public class PICNIC
    {
        private static readonly string result_Error = HelperFunctions.MessageStrings.Error(0);
        private static readonly string result_Success = HelperFunctions.MessageStrings.Success(0);

        public static bool PICNIC_TEMPORARY_SUFFER(ref System.Windows.Forms.TextBox givenPasswordScale, ref System.Windows.Forms.TextBox privateKeyBox,
            ref System.Windows.Forms.TextBox publicKeyBox, ref System.Windows.Forms.TextBox statusBox, ref System.Windows.Forms.ComboBox selectionBox)
        {
            //Minor check;;;
            string plainText = result_Error;
            if (!(givenPasswordScale.Text == null || givenPasswordScale.Text.Length <= 0))
            {
                plainText = givenPasswordScale.Text;
                //throw new ArgumentNullException(nameof(plainText));
            }

            SecureRandom randomGen = new SecureRandom();

            //Generation type selection;;;;;;;
            //MLDsaParameters ml_dsa_param;
            var keyGenParameters = new PicnicKeyGenerationParameters(randomGen, PicnicParameters.picnic3l1);
            switch (selectionBox.SelectedIndex)
            {//Proooob should have this measure against text, not index position, but; counterpoint;;;
                //I'm sleepy ( LOL )
                case 0:// "picnic3l1":
                    keyGenParameters = new PicnicKeyGenerationParameters(randomGen, PicnicParameters.picnic3l1);
                    break;
                case 1:// "picnic3l3":
                    keyGenParameters = new PicnicKeyGenerationParameters(randomGen, PicnicParameters.picnic3l3);
                    break;
                case 2:// "picnic3l4":
                    keyGenParameters = new PicnicKeyGenerationParameters(randomGen, PicnicParameters.picnic3l5);
                    break;
                default:
                    keyGenParameters = new PicnicKeyGenerationParameters(randomGen, PicnicParameters.picnic3l1);
                    statusBox.Text += HelperFunctions.MessageStrings.Error(11) + " : DEFAULTING TO picnic3l1\n";
                    break;
            }

            //Generation of key based off of selection;;;;;;;
            var keyPairGenPicnic = new PicnicKeyPairGenerator();
            keyPairGenPicnic.Init(keyGenParameters);
            var key = keyPairGenPicnic.GenerateKeyPair();

            //Update boxes
            publicKeyBox.Text = Convert.ToBase64String(((PicnicPublicKeyParameters)key.Public).GetEncoded());
            privateKeyBox.Text = Convert.ToBase64String(((PicnicPrivateKeyParameters)key.Private).GetEncoded());

            // Generate Picnic signer & signature.
            var Picnic_sign_maker = new PicnicSigner();
            Picnic_sign_maker.Init(true, key.Private);
            byte[] Picnic_signature = Picnic_sign_maker.GenerateSignature(System.Text.Encoding.UTF8.GetBytes(plainText));

            // Verify Picnic signature.
            var Picnic_verifer = new PicnicSigner();
            Picnic_verifer.Init(false, key.Public);
            //key.Private;

            if (Picnic_verifer.VerifySignature(System.Text.Encoding.UTF8.GetBytes(plainText), Picnic_signature))
            {
                statusBox.Text = result_Success;
                return true;
            }

            statusBox.Text = result_Error;
            return false;

        }
    }
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------





    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------

    /*
        public class KYBER_Lattice
    {
        private static string result_Error = HelperFunctions.ErrorStrings.RESULT_ERROR;
        private static int intLog = 8;//Assuming 256 key-length rn

        public static bool keyError = false;

        public static string CreateDilithThreeKey()
        {

            var randomGen = new SecureRandom();
            var parametersForDilithium = new DilithiumKeyGenerationParameters(randomGen, DilithiumParameters.Dilithium3);


            using (Aes myAes = Aes.Create())
            {
                myAes.KeySize = 256;//MAKE CHANGEBALE, LATER?
                //BASED OFF OF RADIO-BUTTON PROMPT, IN WinForm???

                myAes.GenerateKey();
                return Convert.ToBase64String(myAes.Key);
            }
        }
        public static byte[] KYBER_Lattice_ENCRYPT_FILE(string plainText, string key)
        {

            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));


            byte[] result = Encoding.ASCII.GetBytes(result_Error);


            //---------------------------------------------------
            //ACTUAL ENCRYPTION ---------------------------------
            var randomGen = new SecureRandom();
            var parametersForDilithium = new DilithiumKeyGenerationParameters(randomGen, DilithiumParameters.Dilithium3);









            using (Aes myAes = Aes.Create())
            {
                myAes.Key = Convert.FromBase64String(key);

                //Replace 8 with log2(keysize)
                //( 8 is for 256, since 2^8 = 256 )
                myAes.IV = new byte[myAes.BlockSize / intLog];

                myAes.Mode = CipherMode.CBC;
                myAes.Padding = PaddingMode.PKCS7;


                ICryptoTransform encryptor = myAes.CreateEncryptor(myAes.Key, myAes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    result = msEncrypt.ToArray();
                }
            }
            return result;
        }

    }
    */

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------





    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    /*
    //public class SABER
    //{

        //public static byte[] SABER_ENCRYPT_FILE(string plainText, string key)
        //{
            //
            //LEARN FROM;;;;;
            //var random = new SecureRandom();

            ////below; wr're hardling it to firesaber; so we REALLY want to do that???
            ////Meh.
            //var keyGenParameters = new SaberKeyGenerationParameters(random, SaberParameters.firesaberkem128r3);

            //var SaberKeyPairGenerator = new SaberKeyPairGenerator();
            //SaberKeyPairGenerator.Init(keyGenParameters);

            //var aKeyPair = SaberKeyPairGenerator.GenerateKeyPair();

            //var aPublic = (SaberPublicKeyParameters)aKeyPair.Public;
            //var aPrivate = (SaberPrivateKeyParameters)aKeyPair.Private;


            //var pubEncoded = aPublic.GetEncoded();
            //var privateEncoded = aPrivate.GetEncoded();


            ////var bobSaberKemGenerator = new SaberKemGenerator((SecureRandom)plainText);

            //var bobSaberKemGenerator = new SaberKemGenerator(random);
            //var encapsulatedSecret = bobSaberKemGenerator.GenerateEncapsulated(aPublic);
            //var bobSecret = encapsulatedSecret.GetSecret();

            //var cipherText = encapsulatedSecret.GetEncapsulation();
            //// var ggg = encapsulatedSecret.

            //var aliceSaberKemExtractor = new SaberKemExtractor(aPrivate);
            //var aliceSecret = aliceSaberKemExtractor.ExtractSecret(cipherText);

            //

            //return new byte[0];
        //}
    //}
    */

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------





    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public static class ShiftCipher
    {
        //=================================================================
        //ENCRYPTION FUNCTION==============================================
        public static string SHIFT_ENCRYPT_FILE(string plainText, int key)
        {
            if (plainText == null || plainText.Length <= 0)
                return plainText;
            if (key == 0)
                return plainText;

            char[] chars = plainText.ToCharArray();
            int tempTextVal;
            int tempKeyVal;

            for (int i = 0; i < chars.Length; i++)
            {
                tempTextVal = (int)chars[i];
                if((tempTextVal >= 32) && (tempTextVal <= 126))
                {
                    tempKeyVal = tempTextVal + key;
                    while ((tempKeyVal) > 126)
                    {
                        tempKeyVal -= 95;
                    }

                    chars[i] = ((char)(tempKeyVal));

                }
            }
            return(new string(chars));
        }//END OF [ SHIFT_ENCRYPT_FILE ] function

        //=================================================================
        //DECRYPTION FUNCTION==============================================
        public static string SHIFT_DECRYPT_FILE(string encryptedText, int key)
        {
            if (encryptedText == null || encryptedText.Length <= 0)
                return encryptedText;
            if (key == 0)
                return encryptedText;

            char[] chars = encryptedText.ToCharArray();
            int tempTextVal;
            int tempKeyVal;

            for (int i = 0; i < chars.Length; i++)
            {
                tempTextVal = (int)chars[i];
                if ((tempTextVal >= 32) && (tempTextVal <= 126))
                {
                    tempKeyVal = tempTextVal - key;
                    while ((tempKeyVal) < 32)
                    {
                        tempKeyVal += 95;
                    }

                    chars[i] = ((char)(tempKeyVal));

                }
            }
            return (new string(chars));
        }//END OF [ SHIFT_DECRYPT_FILE ] function
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------





    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    public static class VIGENERE_BEAUFORTCipher
    {
        //I could've made this all one function.
        //Hell, I could've done ALL of this, better.
        //I'm like. Half asleep rn though, and I have
        //other assignments due soon though ( lmao )
        //Wish me luck!

        private static readonly Dictionary<char, int> alpha_D = new Dictionary<char, int>()
        {   
            { 'A', 0 }, { 'B', 1 }, { 'C', 2 }, { 'D', 3 }, { 'E', 4 }, { 'F', 5 },
            { 'G', 6 }, { 'H', 7 }, { 'I', 8 }, { 'J', 9 }, { 'K', 10 },{ 'L', 11 },
            { 'M', 12 },{ 'N', 13 },{ 'O', 14 },{ 'P', 15 },{ 'Q', 16 },{ 'R', 17 },
            { 'S', 18 },{ 'T', 19 },{ 'U', 20 },{ 'V', 21 },{ 'W', 22 },{ 'X', 23 },
            { 'Y', 24 },{ 'Z', 25 } 
        };

        private static readonly string alpha_C = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; 

        //=================================================================
        //ENCRYPTION FUNCTION==============================================
        public static string VIGENERE_BEAUFORT_ENCRYPT_FILE(string plainText, string key)
        {
            if (plainText == null || plainText.Length <= 0)
                return plainText;
            if ( key == null || key.Length == 0)
                return plainText;

            string result = HelperFunctions.MessageStrings.Error(0);
            char[] message = (plainText.ToUpper()).ToCharArray();
            char[] password = (key.ToUpper()).ToCharArray();
            char temp;
            int[] plainTextNumbers = new int[message.Length];
            int[] keyNumbers = new int[password.Length];
            int modifier;


            for(int i = 0; i < message.Length; i++)
            { 
                temp = message[i];
                if (Char.IsLetter(temp))
                    plainTextNumbers[i] = alpha_D[temp];
                else
                    plainTextNumbers[i] = -1;
            }

            for (int i = 0; i < password.Length; i++)
            {
                temp = password[i];
                if (Char.IsLetter(temp))
                    keyNumbers[i] = alpha_D[temp];
                else
                    keyNumbers[i] = -1;
            }

            //Actual encryption;;;;
            for (int i = 0, j = 0; i < message.Length; i++)
            {
                if (plainTextNumbers[i] != -1)
                {
                    modifier = plainTextNumbers[i] - keyNumbers[j];
                    if (modifier < 0)
                        modifier += 26;

                    message[i] = (char)alpha_C[modifier];

                    j++;
                    if (j >= (password.Length))
                        j = 0;
                }
            }

            return (new string(message));
        }//END OF [ VIGENERE_BEAUFORT_ENCRYPT_FILE ] function

        //=================================================================
        //DECRYPTION FUNCTION==============================================
        public static string VIGENERE_BEAUFORT_DECRYPT_FILE(string encryptedText, string key)
        {
            if (encryptedText == null || encryptedText.Length <= 0)
                return encryptedText;
            if (key == null || key.Length == 0)
                return encryptedText;

            string result = HelperFunctions.MessageStrings.Error(0);
            char[] message = (encryptedText.ToUpper()).ToCharArray();
            char[] password = (key.ToUpper()).ToCharArray();
            char temp;
            int[] plainTextNumbers = new int[message.Length];
            int[] keyNumbers = new int[password.Length];
            int modifier;

            for (int i = 0; i < message.Length; i++)
            {
                temp = message[i];
                if (Char.IsLetter(temp))
                    plainTextNumbers[i] = alpha_D[temp];
                else
                    plainTextNumbers[i] = -1;
            }

            for (int i = 0; i < password.Length; i++)
            {
                temp = password[i];
                if (Char.IsLetter(temp))
                    keyNumbers[i] = alpha_D[temp];
                else
                    keyNumbers[i] = -1;
            }

            //Actual encryption;;;;
            for (int i = 0, j = 0; i < message.Length; i++)
            {
                if (plainTextNumbers[i] != -1)
                {
                    modifier = plainTextNumbers[i] + keyNumbers[j];
                    if (modifier > 25)
                        modifier -= 26;

                    message[i] = (char)alpha_C[modifier];

                    j++;
                    if (j == (password.Length))
                        j = 0;
                }
            }

            return (new string(message));
        }//END OF [ VIGENERE_BEAUFORT_DECRYPT_FILE ] function
    }
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------






    /*
    
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------

    //-----------------------------------------------------------------
    //Broke it, can't figure out how to fix it, kill me, ugghhhhhhhhh
    //-----------------------------------------------------------------

    //Blowfish was made by Bruce Schneier in 1993,
    //and it's public domain!
    //Oh- and bouncycastle appearently aready... has it... Hm.
    public static class Blowfish
    {
        //=================================================================
        //ENCRYPTION FUNCTION==============================================
        public static string BLOWFISH_ENCRYPT_FILE(string plainText, string key)
        {
            if (plainText == null || plainText.Length <= 0)
                return plainText;
            if (key == null || key.Length == 0)
                return plainText;

            string message =plainText;
            string result = HelperFunctions.MessageStrings.Error(0);
            
            int parameter;
            //
            //34
            //64
            //128
            //256
            //
            //switch (selectionBox.SelectedIndex)
            //{
            //    case 0://34
            //        parameter = 34;
            //        break;
            //    case 1://64
            //        parameter = 64;
            //        break;
            //    case 2://128
            //        parameter = 128;
            //        break;
            //    case 32://256
            //        parameter = 256;
            //        break;
            //    default:
            //        parameter = 34;
            //        statusBox.Text += HelperFunctions.MessageStrings.Error(11) + " : DEFAULTING TO 34 bits\n";
            //        break;
            //}
            
            //rename inArr & outArr to arr1 & arr2 later!
            BlowfishEngine fish = new BlowfishEngine();
            PaddedBufferedBlockCipher blowfishCiper = new PaddedBufferedBlockCipher(fish, new Pkcs7Padding());//UGH
            //It SHOULD work WITHOUT specifying the Pkcs7Padding; I CHECKED!
            //AHHHHHHHHHHHHHHHHH whatever it's the same result anyway- when not specified,
            //That's the default, so;
            //I guess I shouldn't care...????

            blowfishCiper.Init(true, new KeyParameter(Encoding.UTF8.GetBytes(key)));

            byte[] inArr = Encoding.UTF8.GetBytes(message); 
            byte[] outArr = new byte[blowfishCiper.GetOutputSize(inArr.Length)];

            blowfishCiper.DoFinal(outArr, blowfishCiper.ProcessBytes(inArr, 0, inArr.Length, outArr, 0));

            result = Encoding.UTF8.GetString(outArr).Replace("-", "");

            return(result);

        }//END OF [ BLOWFISH_ENCRYPT_FILE ] function

        //=================================================================
        //DECRYPTION FUNCTION==============================================
        public static string BLOWFISH_DECRYPT_FILE(string encryptedText, string key)
        {
            string result = HelperFunctions.MessageStrings.Error(0);
            string message = encryptedText;

            BlowfishEngine fish = new BlowfishEngine();
            PaddedBufferedBlockCipher blowfishCiper = new PaddedBufferedBlockCipher(fish, new Pkcs7Padding());
            //Still hate that.

            blowfishCiper.Init(true, new KeyParameter(Encoding.UTF8.GetBytes((key))));

            byte[] arr1 = Encoding.UTF8.GetBytes(message);
            byte[] arr2 = new byte[blowfishCiper.GetOutputSize(arr1.Length)];

            blowfishCiper.DoFinal(arr2, blowfishCiper.ProcessBytes(arr1, 0, arr1.Length, arr2, 0));

            result = Encoding.UTF8.GetString(arr2);

            return(result);

        }//END OF [ BLOWFISH_DECRYPT_FILE ] function
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------

    */

}