using System;
using System.IO;
namespace TMClient
{
    internal partial class Global
    {
        internal static string DataFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NoFruatINC", "TaskManager", "Client");
        internal static string SessionToken { get; set; } = string.Empty;
        internal static string GetSessionKeyFromFile()
        {
            if (DataFolder == null) return string.Empty;
            string sk = string.Empty;
            try
            {
                Directory.CreateDirectory(DataFolder);
                string file = Path.Combine(DataFolder, "session.key");
                using (var stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        sk = reader.ReadToEnd();
                        sk = sk.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read session key error: {ex}");
            }
            return sk;
        }
        internal static void SaveSessionKeyToFile(string sessionKey)
        {
            try
            {
                Directory.CreateDirectory(DataFolder);
                string file = Path.Combine(DataFolder, "session.key");
                using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(sessionKey ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Write session key error: {ex}");
            }
        }
        internal static bool RememberSession { get; set; }
    }
}
