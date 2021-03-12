using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unplug.DataAccess
{
    class FileConfigToggler : DataAccessConfiguration
    {
        internal static Boolean FileConfigOpened = false;
        static System.IO.FileStream QSFileConfig;


        internal static void ToggleFileConfigOpen(FileConfigAction fileConfigAction)
        {
            if (fileConfigAction == FileConfigAction.Close)
            {
                //close file config
                if (FileConfigOpened == true && QSFileConfig != null)
                {
                    QSFileConfig.Close();
                    FileConfigOpened = false;
                }
            }
            else
            {
                //open file config
                QSFileConfig = System.IO.File.Open(DataAccessPath, System.IO.FileMode.Open);
                FileConfigOpened = true;
            }
        }
    }
}
