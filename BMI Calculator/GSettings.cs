using System.Configuration;
using System;
using System.Windows;

namespace BMI_Calculator
{
    // Class for reading and writing settings to App.config file

        class GSettings
        {
            public static void ReadAllSettings()
            {
                try
                {
                    var appSettings = ConfigurationManager.AppSettings;

                    if (appSettings.Count == 0)
                    {
                        Console.WriteLine("AppSettings is empty.");
                    }
                    else
                    {
                        foreach (var key in appSettings.AllKeys)
                        {
                            Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                        }
                    }
                }
                catch (ConfigurationErrorsException)
                {
                    Console.WriteLine("Error reading app settings");
                }
            }

            public static string ReadSetting(string key)
            {
                try
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    string result = appSettings[key] ?? "Not Found";
                return result;

                }
                catch (ConfigurationErrorsException)
                {
                    MessageBox.Show("Error reading app settings");
                }
            return "Something went wrong reading settings";
            }

            public static void AddUpdateAppSettings(string key, string value)
            {
                try
                {
                    var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = configFile.AppSettings.Settings;
                    if (settings[key] == null)
                    {
                        settings.Add(key, value);
                    }
                    else
                    {
                        settings[key].Value = value;
                    }
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                }
                catch (ConfigurationErrorsException)
                {
                    Console.WriteLine("Error writing app settings");
                }
            }
        };
    }

