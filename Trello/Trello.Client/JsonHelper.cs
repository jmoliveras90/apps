using System.IO;
using System.Text.Json;
using System.Windows;

namespace Trello.Client
{
    public static class JsonHelper
    {
        public static Configuration? ReadConfiguration(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                Configuration? credentials = JsonSerializer.Deserialize<Configuration>(jsonString);
                
                return credentials;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error de configuración");
                return null;
            }
        }
    }
}
