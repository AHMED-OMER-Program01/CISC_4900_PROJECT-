using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms4900.HelperFunctions
{
    public static class SuccessStrings
    {
        //------------------

        //-------------------------------------
        //Generic Success(s)
        public const string RESULT_SUCCESS = "[SUCCESS:IF THIS IS BEING GIVEN BACK, SUCCESS]";
        //-------------------------------------
        //File Success(s)
        public const string OUTPUT_OVERWRITE_FILE_SUCCESS = "[SUCCESS:Your output file was successfully overwritten!]";
        public const string OUTPUT_DESKTOP_FILE_CREATION_SUCCESS = "[SUCCESS:Your output file was successfully created to your desktop!]";
        //public const string OUTPUT_OVERWRITE_FILE_SUCCESS = "[SUCCESS:Your output file was successfully overwritten!]";
        //-------------------------------------
        //Functions
        public static string SUCCESS(string given)
        { return ("[SUCCESS: " + given + " ]"); }
        //-------------------------------------

    }
}
