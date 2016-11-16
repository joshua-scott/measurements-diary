using System;
using System.IO;
using System.Text;
using System.Windows;

namespace MeasurementsDiary
{
    public class ReadWrite : IReadWrite
    {
        public virtual string ReadFile(string filename)
        {
            StringBuilder contents = new StringBuilder("");

            // Try to read file contents
            try
            {
                StreamReader sr = new StreamReader(filename);
                contents.Append(sr.ReadToEnd());
            }
            // If exception, tell user and return exception's message
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error at ReadWrite.ReadFile()");
                return e.ToString();
            }

            // Must cast from StringBuilder to string
            return contents.ToString();
        }

        public virtual void WriteFile(string filename, string whatToSave, bool append)
        {
            try
            {
                if (filename != null)
                    File.WriteAllText(filename, whatToSave);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error at ReadWrite.WriteFile()");
            }
        }
    }
}
