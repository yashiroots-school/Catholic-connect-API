using System.Configuration;
namespace ChurchAPI
{


        public static class ErrorLogException
        {
            private static readonly object _lock = new object();

            public static async Task LogErrorAsync(Exception ex)
            {
                try
                {
                    string message = $"Time: {DateTime.Now:dd/MM/yyyy hh:mm:ss tt}{Environment.NewLine}";
                    message += "-----------------------------------------------------------" + Environment.NewLine;
                    message += $"Message: {ex.Message}{Environment.NewLine}";
                    message += $"StackTrace: {ex.StackTrace}{Environment.NewLine}";
                    message += $"Source: {ex.Source}{Environment.NewLine}";
                    message += $"TargetSite: {ex.TargetSite}{Environment.NewLine}";
                    message += "-----------------------------------------------------------" + Environment.NewLine;

                    string path = System.Configuration.ConfigurationManager.AppSettings["ErrorLogFilePath"];
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = AppDomain.CurrentDomain.BaseDirectory; // fallback to base directory
                    }

                    string filePath = Path.Combine(path, "ErrorLog.txt");

                    // Ensure thread-safe async writing
                    lock (_lock)
                    {
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine(message);
                        }
                    }
                }
                catch
                {
                    // If logging fails, swallow exception to avoid breaking API
                }
            }
        }

    }

