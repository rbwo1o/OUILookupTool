using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Net;


namespace OUILookup
{
    class Oui
    {
        /*
         * Finds a vender based on a given MAC OUI
         * Attempts to download 'oui.txt' on failed StreamReader
         */
        public string find(string test)
        {
            try
            {
                test = test.Replace(":", "");
                test = test.Replace("-", "");
                StreamReader sr = new StreamReader("oui.txt");
                string line, mac;
                while ((line = sr.ReadLine()) != null)
                {
                    mac = line.Substring(0, 6);
                    if (test.ToUpper().Contains(mac.ToUpper()))
                    {
                        return line.Substring(7).Trim();
                    }
                }
            }
            catch (Exception e)
            {
                try // attempt to download oui.txt
                {
                    WebClient client = new WebClient();
                    client.DownloadFile("https://aquarrii.com/oui.txt", "oui.txt");
                }
                catch (Exception b) 
                {
                    return "Could not open or download oui.txt. Please check internet connection.";
                }

            }

            return "Unknown";
        }

    }
}
