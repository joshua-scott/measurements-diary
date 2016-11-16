using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MeasurementsDiary
{
    class ReadWriteCSV : ReadWrite
    {
        public new List<string[]> ReadFile(string filename)
        {
            List<string[]> content = new List<string[]>();
            // Open file for reading 
            StreamReader reader = File.OpenText(filename);
            string row = "";
            string[] data;

            // Read each line until the end is reached  
            while (row != null)
            {
                row = reader.ReadLine();
                if (row != null && row.Length > 0)
                {
                    data = row.Split(',');
                    content.Add(data);
                }
            }
            reader.Close();

            return content;
        }

        public void WriteFile(string filename, List<string[]> data, bool append)
        {
            StreamWriter file = new StreamWriter(filename, append);
            // Using ensures flushing and closings 
            using (file)
            {
                int count = 0;
                StringBuilder sb = new StringBuilder();
                foreach (var group in data)
                {
                    foreach (var item in group)
                    {
                        if (item.Length > 0)
                            sb.Append(item);
                        if (count++ < group.Length - 1)
                            sb.Append(", ");
                    }
                    sb.AppendLine();
                    count = 0;
                }
                file.WriteLine(sb.ToString());
            }
        }
    }
}
