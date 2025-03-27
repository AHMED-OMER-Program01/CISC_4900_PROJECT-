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
//using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;//Dilithium stuffs, specifically
using Org.BouncyCastle.Pqc.Crypto.Saber;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crmf;
using Org.BouncyCastle.Asn1.Cms;//Saber stuffs, specifically
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


    public static class AES_StandardLibrary
    {
        private static string result_Error = HelperFunctions.ErrorStrings.RESULT_ERROR;
        private static int intLog = 8;//Assuming 256 key-length rn

        public static bool keyError = false;

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



            byte[] result = Encoding.ASCII.GetBytes(result_Error);


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

            string result = result_Error;

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
                        keyError = true;
                        result = "[ERROR:keyError]";
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
                    keyError = true;
                    result = "[ERROR:keyError]";
                }
            }
            return result;
        }//END OF [ AES_DECRYPT_FILE ] function

    }





    ///*

    //
    public class ML_DSA
    {
        private static string result_Error = HelperFunctions.ErrorStrings.RESULT_ERROR;
        private static SecureRandom randomGen;
        /*---------------------------------------------------
            hardlining it to the ML-DSA-65 param-set; sue me.
        Maybe make it like, a selection menu in the actual 
        form, and have a string be sent in, that hits a 
        switch-case or whatever; if you *really* wanted to 
        ---------------------------------------------------*/
        private static MLDsaParameters ml_dsa_param = MLDsaParameters.ml_dsa_65;


        public static bool keyError = false;

        public static AsymmetricCipherKeyPair Create_ML_DSA_KeyPair()
        {
            randomGen = new SecureRandom();
            var keyPairGenMLDsa = new MLDsaKeyPairGenerator();

            keyPairGenMLDsa.Init(new MLDsaKeyGenerationParameters(randomGen, ml_dsa_param));
            var keyPair = keyPairGenMLDsa.GenerateKeyPair();
            return keyPair;
        }

        public static byte[] ML_DSA_ENCRYPT_FILE(string plainText, AsymmetricCipherKeyPair key)
        {
            
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
            var signer = SignerUtilities.InitSigner("ML-DSA-65", forSigning: true, key.Private, randomGen);
            // Generate ML-DSA signature.
            signer.BlockUpdate(result, 0, result.Length);
            byte[] signature = signer.GenerateSignature();
            // Verify ML-DSA signature.
            var verifier = SignerUtilities.InitSigner("ML-DSA-65", forSigning: false, key.Public, random: null);
            verifier.BlockUpdate(result, 0, result.Length);
            //key.Private;

            if (verifier.VerifySignature(signature))
            {
                //Console.WriteLine("ML-DSA-65 signature created and verified successfully");
            }
            return result;

        }
    }



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


    public class SABER
    {

        public static byte[] SABER_ENCRYPT_FILE(string plainText, string key)
        {
            /*
            LEARN FROM;;;;;
            var random = new SecureRandom();

            //below; wr're hardling it to firesaber; so we REALLY want to do that???
            //Meh.
            var keyGenParameters = new SaberKeyGenerationParameters(random, SaberParameters.firesaberkem128r3);

            var SaberKeyPairGenerator = new SaberKeyPairGenerator();
            SaberKeyPairGenerator.Init(keyGenParameters);

            var aKeyPair = SaberKeyPairGenerator.GenerateKeyPair();

            var aPublic = (SaberPublicKeyParameters)aKeyPair.Public;
            var aPrivate = (SaberPrivateKeyParameters)aKeyPair.Private;


            var pubEncoded = aPublic.GetEncoded();
            var privateEncoded = aPrivate.GetEncoded();


            //var bobSaberKemGenerator = new SaberKemGenerator((SecureRandom)plainText);

            var bobSaberKemGenerator = new SaberKemGenerator(random);
            var encapsulatedSecret = bobSaberKemGenerator.GenerateEncapsulated(aPublic);
            var bobSecret = encapsulatedSecret.GetSecret();

            var cipherText = encapsulatedSecret.GetEncapsulation();
            // var ggg = encapsulatedSecret.

            var aliceSaberKemExtractor = new SaberKemExtractor(aPrivate);
            var aliceSecret = aliceSaberKemExtractor.ExtractSecret(cipherText);

            */

            return new byte[0];
        }
    }
    //*/











}

//Place below in FileOpen & Read & Write .cs LATER;;;


/*
            //---------------------------------------------------
            //INPUTFILE & READ ----------------------------------
            try
            {
                inputPath = Path.GetDirectoryName(inputPath);
                plainText = inputPath.ToString();
            }
            catch (FileNotFoundException)
            {//Inputfile Not found.
                return (result + ":[FileNotFoundException]");
            }
            catch (Exception)
            {//Another error occurred.
                return (result + ":[UNKNOWN ERROR]");
            }
 */