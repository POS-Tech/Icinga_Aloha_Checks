using System;
using System.IO;


namespace check_aloha_dob
{
    internal class Program
    {
        // Define the path to the aloha.ini file using the 'iberdir' environment variable
        static string alohaIniPath = Path.Combine(Environment.GetEnvironmentVariable("iberdir"), "data\\aloha.ini");
        
        // Check if the script is running in debug mode
        static bool debugMode = true;  // Set to true to enable debug mode

        // Function to print the exit code or exit
        static void PrintOrExit(int exitCode, string message)
        {
            if (debugMode)
            {
                Console.WriteLine(message);
                Console.WriteLine($"Exit Code: {exitCode}");
            }
            else
            {
                Console.WriteLine(message);
                Environment.Exit(exitCode);
            }
        }


        static void Main(string[] args)
        {
            // Check if the file exists
            if (File.Exists(alohaIniPath))
            {
                // Read the content of the aloha.ini file
                string alohaIniContent;
                try
                {
                    alohaIniContent = File.ReadAllText(alohaIniPath);
                }
                catch (IOException)
                {
                    alohaIniContent = null;
                }

                // Extract the DOB value from the aloha.ini content
                string dobLine = null;
                if (alohaIniContent != null)
                {
                    foreach (string line in alohaIniContent.Split('\n'))
                    {
                        if (line.Contains("DOB="))
                        {
                            dobLine = line;
                            break;
                        }
                    }
                }

                if (dobLine != null)
                {
                    string dobValue = dobLine.Replace("DOB=", "");

                    // Parse the DOB value as a date
                    //DateTime dobDate = DateTime.ParseExact(dobValue, "MM dd yyyy", null);
                    DateTime dobDate = DateTime.ParseExact(dobValue.Replace("\r",""), "MM dd yyyy", null);
                    // Get today's date and current time
                    DateTime today = DateTime.Now;

                    // Check the current time of day
                    if (today.Hour < 5)
                    {
                        // It's before 5 AM, consider DOB to be 1 day older
                        today = today.AddDays(-1);
                    }

                    // Calculate the date difference
                    TimeSpan dateDifference = today - dobDate;

                    if (dateDifference.Days == 0)
                    {
                        // DOB matches today's date
                        PrintOrExit(0, $"DOB matches today: {dobValue}");
                    }
                    else if (dateDifference.Days == 1)
                    {
                        // DOB is 1 day old
                        PrintOrExit(2, $"DOB is 1 day old: {dobValue}");
                    }
                    else if (dateDifference.Days >= 2)
                    {
                        // DOB is older than 2 days
                        PrintOrExit(3, $"DOB is older than 2 days: {dobValue}");
                    }
                }
                else
                {
                    PrintOrExit(1, "DOB not found in aloha.ini");
                }
            }
            else
            {
                PrintOrExit(1, "aloha.ini file not found");
            }
            Console.ReadLine();
        }
    }
}
