using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasurementsDiary
{
    class ReadWriteMPK : ReadWrite
    {
        public new string[] ReadFile(string filename)
        {
            string[] contents = { "", "" };

            // Try to read file contents
            try
            {
                contents[0] = File.ReadLines(filename).Skip(0).Take(1).First();
                contents[1] = File.ReadLines(filename).Skip(1).Take(1).First();
            }
            // If exception, tell user and return exception's message
            catch (Exception)
            {
                MessageBox.Show("Error at ReadWriteMPK.ReadFile()");
            }

            return contents;
        }

        public void WriteFile(string filename, bool append)
        {
            // Set txt and csv filenames by removing last 3 chars of (mpk) filename and adding txt or csv
            string filenameBase = filename.Substring(0, filename.Length - 3);
            string txtFilename = filenameBase + "txt";
            string csvFilename = filenameBase + "csv";
            // Make the string that will be the contents of the mpk file
            string mpkContents = txtFilename + "\n" + csvFilename;

            try
            {
                if (filename != null)
                {
                    // Save mpk file
                    File.WriteAllText(filename, mpkContents);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error at ReadWriteMPK.WriteFile()");
            }
        }

    }
}
