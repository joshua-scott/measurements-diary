using System;
using Microsoft.Win32;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MeasurementsDiary
{
    class ReadWriteDialog
    {
        public string ReadFile(out string fileName, out List<string[]> csvContents)
        {
            // Initialise out parameters
            fileName = "";
            csvContents = null;
            string txtContents = "";
            // [0] will be txt filename, [1] will be mpk filename
            string[] txtCsvPaths = { "", "" };

            // Configure open file dialog box
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open an mpk file";
            dlg.DefaultExt = ".mpk";
            dlg.Filter = "MPK files (.mpk)|*.mpk|All files (*.*)|*.*";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                try
                {
                    // Get filenames from mpk document
                    fileName = dlg.FileName;
                    ReadWriteMPK rwmpk = new ReadWriteMPK();
                    txtCsvPaths = rwmpk.ReadFile(fileName);
                    
                    // Get text from txt file
                    ReadWrite rw = new ReadWrite();
                    txtContents = rw.ReadFile(txtCsvPaths[0]);

                    // Get content from csv file
                    ReadWriteCSV rwcsv = new ReadWriteCSV();
                    csvContents = rwcsv.ReadFile(txtCsvPaths[1]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error at ReadWriteDialog.ReadFile()");
                }
            }
            return txtContents;
        }

        public string WriteFileAs()
        {
            string filename = null;

            // Configure save file dialog box
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Measurements";
            dlg.DefaultExt = ".mpk";
            dlg.Filter = "MPK files (.mpk)|*.mpk";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                try
                {
                    // save document
                    ReadWriteMPK rwmpk = new ReadWriteMPK();
                    filename = dlg.FileName;
                    rwmpk.WriteFile(filename, false);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error at ReadWriteDialog.WriteFileAsCSV()");
                    return null;
                }
            }
            return filename;
        }
    }
}
