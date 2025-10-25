using System.Reflection;
using System.Text.Json;

namespace PaperTrails_ThomasAdams_c3429938.Services;

public static class SecretsReader
{
    private static IDictionary<string, string> secrets;

    public static string GetApiKey()
    {
        // Load secrets if not already loaded
        if (secrets == null)
        {
            LoadSecrets();
        }

        // Return the Google Books API key
        return secrets["GoogleBooksApiKey"];
    }

    private static void LoadSecrets()
    {
        // Read the embedded Secrets.json file
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "PaperTrails_ThomasAdams_c3429938.Secrets.json";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            // Deserialize the JSON content into the secrets dictionary
            string jsonContent = reader.ReadToEnd();
            secrets = JsonSerializer.Deserialize<IDictionary<string, string>>(jsonContent);
        }
    }
}