using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Unplug.DataAccess
{
    class ApplicationSettingsHelper : DataAccessConfiguration
    {
        internal static T GetValue<T>(String appSettingKey)
        {
            try
            {
                //read app setting value
                String appSettingValue = Val(appSettingKey);

                if (String.IsNullOrWhiteSpace(appSettingValue))
                {
                    //specific app setting doesn't exist -> create app setting

                    UpdateAppSettings(appSettingKey, "");

                    //and return default value for type
                    return default;
                }
                else
                {
                    if(typeof(T) == typeof(Boolean))
                    {
                        return (T)Convert.ChangeType(Int32.Parse(appSettingValue), typeof(T));
                    }

                    return (T)Convert.ChangeType(appSettingValue, typeof(T));
                }
            }
            catch (ConfigurationErrorsException ce)
            {
                throw new Exception("File Config Read Error", ce);
            }
            catch (Exception ex)
            {
                throw new Exception("File Config Generic Error", ex);
            }
        }

        internal static void CreateAppSetting(String nameKey)
        {
            UpdateAppSettings(nameKey, "");
        }
        internal static void UpdateAppSettings(String nameKey, String InsertValue)
        {
            XDocument xmlDoc = XDocument.Load(DataAccessPath);

            var setting = (from item in xmlDoc.Descendants("Setting") select item).First();

            var element = setting.Elements().Where(a => a.Name == nameKey).FirstOrDefault();

            if(element != null)
                element.Remove();

            xmlDoc.Save(DataAccessPath);

            setting.Add(new XElement(nameKey, InsertValue));

            xmlDoc.Save(DataAccessPath);
        }

        private static String Val(String settingName)
        {
            DataSet ds = new DataSet();

            ds.ReadXml(DataAccessPath);

            if (ds.Tables[0].Columns.Contains(settingName) == false)
                return "";

            return (String)ds.Tables[0].Rows[0][settingName];
        }
    }
}
