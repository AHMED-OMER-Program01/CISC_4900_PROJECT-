using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms4900.HelperFunctions
{



    public static class MessageStrings
    {

        private static string FormatS(string message)
        { return (("[ " + message + " ]\r\n")); }

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //ERRORS:
        public static string Error(int i)
        {
            string message = "";
            switch (i)
            {
                //File errors
                case 0:
                    message = FormatS(ErrorS("IF THIS IS BEING GIVEN BACK, ERROR"));
                    break;
                case 1:
                    message = FormatS(ErrorS("READING INPUT FILE"));
                    break;
                case 2:
                    message = FormatS(ErrorS("WRITING INPUT FILE"));//??? Why did I even???
                    break;
                case 3:
                    message = FormatS(ErrorS("READING OUTPUT FILE"));
                    break;
                case 4:
                    message = FormatS(ErrorS("WRITING OUTPUT FILE"));
                    break;
                case 5:
                    message = FormatS(ErrorS("ERROR IN FILE CREATION"));
                    break;
                //Empty Box Errors
                case 6:
                    message = FormatS(ErrorS("ENCRYPTION TEXTBOX EMPTY OR NULL"));
                    break;
                case 7:
                    message = FormatS(ErrorS("DECRYPTION TEXTBOX EMPTY OR NULL"));
                    break;
                //MISC ERRORS
                case 8:
                    message = FormatS(ErrorS("DECRYPTION INPUT ERROR"));
                    break;
                case 9:
                    message = FormatS(ErrorS("ENCRYPTION INPUT ERROR"));
                    break;
                case 10:
                    message = FormatS(ErrorS("GENERATION ERROR"));
                    break;
                case 11:
                    message = FormatS(ErrorS("SELECTION ERROR"));
                    break;
                case 12:
                    message = FormatS(ErrorS("DECRYPTION ERROR"));
                    break;
                default:
                    message = FormatS(ErrorS("ERROR"));
                    break;
            }
            return (message);
        }

        public static string ERROR(string given)
        { return (FormatS(ErrorS(given))); }
        private static string ErrorS(string message)
        { return (("ERROR : " + message)); }


        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------
        //SUCCCESS:
        public static string Success(int i)
        {
            string message = "";
            switch (i)
            {
                //File Successes
                case 0:
                    message = FormatS(SuccessS("SUCCESS STATE REACHED"));
                    break;
                case 1:
                    message = FormatS(SuccessS("READING INPUT FILE"));
                    break;
                case 2:
                    message = FormatS(SuccessS("WRITING INPUT FILE"));//??????????
                    break;
                case 3:
                    message = FormatS(SuccessS("READING OUTPUT FILE"));
                    break;
                case 4:
                    message = FormatS(SuccessS("WRITING OUTPUT FILE"));
                    break;
                case 5:
                    message = FormatS(SuccessS("FILE CREATION"));
                    break;
                //Box Successes
                case 6:
                    message = FormatS(SuccessS("ENCRYPTION TEXTBOX READ"));
                    break;
                case 7:
                    message = FormatS(SuccessS("DECRYPTION TEXTBOX READ"));
                    break;
                //MISC Successes
                case 8:
                    message = FormatS(SuccessS("ENCRYPTION"));
                    break;
                case 9:
                    message = FormatS(SuccessS("DECRYPTION"));
                    break;
                case 10:
                    message = FormatS(SuccessS("GENERATION"));
                    break;
                default:
                    message = FormatS(SuccessS("SUCCESS STATE REACHED"));
                    break;
            }
            return (message);
        }

        public static string SUCCCESS(string given)
        { return (FormatS(SuccessS(given))); }
        private static string SuccessS(string message)
        { return (("SUCCESS : " + message)); }
    }
}


