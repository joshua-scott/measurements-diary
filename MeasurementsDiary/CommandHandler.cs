using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace MeasurementsDiary
{
    class CommandHandler
    {
        private string defaultfileName = "Unsaved document";
        private string fileName;
        public MainWindow Window { get; set; }

        private void EnableEditing()
        {
            if (Window.text.IsEnabled == false)
            {
                Window.text.IsEnabled = true;
                Window.text.FontSize = 14;
            }
        }

        public void New()
        {
            // If textbox is enabled, ask if user wants to save before starting again
            if (Window.text.IsEnabled)
            {
                MessageBoxResult choice = MessageBox.Show("Save before clearing?", "Save?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (choice == MessageBoxResult.Yes)
                    Save(true);
            }

            EnableEditing();
            fileName = defaultfileName;
            Window.Title = fileName;
            Window.text.Document.Blocks.Clear();
        }

        public void Open()
        {
            // Open MPK/txt/csv files, get content
            ReadWriteDialog rwd = new ReadWriteDialog();
            List<string[]> csvContents;
            string txtContents = rwd.ReadFile(out fileName, out csvContents);

            // Set title
            if (fileName != null)
                Window.Title = fileName;

            // Add txtContents to textbox
            Window.text.Document.Blocks.Clear();
            if (txtContents != null)
                Window.text.Document.Blocks.Add(new Paragraph(new Run(txtContents)));
            EnableEditing();

            // Add csvContents to DataGrid
            ObservableCollection<DataItem> d = new ObservableCollection<DataItem>();
            if (csvContents != null)
            {
                foreach (var c in csvContents)
                {
                    try
                    {
                        uint date = Convert.ToUInt32(c[0]);
                        double temperature = Convert.ToDouble(c[1]);
                        int place = Convert.ToInt32(c[2]);
                        d.Add(new DataItem(date, temperature, place % 3 == 0 ? METERS.NAMES.AIRPORT : (place % 3 == 1 ? METERS.NAMES.CENTER : METERS.NAMES.RUISSALO)));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error reading data in file, check its contents.");
                        Window.Close();
                    }
                }
            }
            Window.Data = d;
        }

        public void Save(bool isname)
        {
            // In case user clicks Save straight away
            EnableEditing();

            // Change save to saveas if needed
            if (fileName == defaultfileName || fileName == null)
                isname = false;

            // Store current textCursor position
            TextPointer textCursorPosition = Window.text.Selection.Start;

            // Textbox data for txt file
            Window.text.SelectAll();
            string textToSave = Window.text.Selection.Text;

            // Measurement data for csv file
            List<string[]> dataToSave = new List<string[]>();
            foreach (var c in Window.Data)
            {
                double y = Math.Truncate(c.Y * 100) / 100;

                int place = 0;
                if (c.Meter.ToString() == "AIRPORT")
                    place = 0;
                else if (c.Meter.ToString() == "CENTER")
                    place = 1;
                else if (c.Meter.ToString() == "RUISSALO")
                    place = 2;

                string[] array = new string[3];
                array[0] = c.X.ToString();
                array[1] = y.ToString();
                array[2] = place.ToString();

                dataToSave.Add(array);
            }

            ReadWriteDialog rwd = new ReadWriteDialog();
            // If there isn't an existing name or user clicked saveAs, do saveAs
            if (isname == false)
            {
                fileName = rwd.WriteFileAs();
            }
            // Else just save with existing filename
            else
            {
                // Call save on mpk file
                ReadWriteMPK rwmpk = new ReadWriteMPK();
                rwmpk.WriteFile(fileName, false);
            }

            // Whether we've done Save or SaveAs, there should now be a working filename, and the MPK file is saved
            // All that's left is to save the txt and csv data to the relevant files
            if (fileName != null)
            {
                try
                {
                    // Set txt and csv filenames by removing last 3 chars of (mpk) filename and adding txt or csv
                    string filenameBase = fileName.Substring(0, fileName.Length - 3);
                    string txtFilename = filenameBase + "txt";
                    string csvFilename = filenameBase + "csv";

                    // Save txt file
                    ReadWrite rw = new ReadWrite();
                    rw.WriteFile(txtFilename, textToSave, false);
                    // Save csv file
                    ReadWriteCSV rwcsv = new ReadWriteCSV();
                    rwcsv.WriteFile(csvFilename, dataToSave, false);

                    // Set title
                    Window.Title = fileName;

                    // Put textCursor back in its previous position
                    Window.text.Selection.Select(textCursorPosition, textCursorPosition);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error saving to existing file, try SaveAs instead");
                }
            }
        }

        public void Close()
        {
            MessageBoxResult choice = MessageBox.Show("Save before closing?", "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (choice == MessageBoxResult.Yes)
            {
                Save(true);
                Window.Close();
            }
            else if (choice == MessageBoxResult.No)
            {
                Window.Close();
            }
        }
    }
}