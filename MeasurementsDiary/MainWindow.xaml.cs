using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MeasurementsDiary
{
    public partial class MainWindow : Window
    {
        private CommandHandler cmhandler = new CommandHandler();

        private ObservableCollection<DataItem> data;
        public ObservableCollection<DataItem> Data { get { return data; } set { data = value; } }
        Coordinate2DDrawer drawer;
        private int min = -20;
        private int max = 20;

        public MainWindow()
        {
            InitializeComponent();
            cmhandler.Window = this; //Set the Window Property 
            DataContext = this;
            drawer = new Coordinate2DDrawer(canvas, 0, 31, min, max);
            drawer.DrawAxis();
            InitializeDatagrid();   
        }

        private void InitializeDatagrid()
        {
            data = new ObservableCollection<DataItem>();
            dataGrid1.ItemsSource = Data;
        }

        private void Draw()
        {
            drawer.Clear();
            Markers mark;
            SolidColorBrush color;
            foreach (var v in data)
            {
                switch (v.Meter)
                {
                    case METERS.NAMES.RUISSALO:
                        mark = Markers.CIRCLE; color = Brushes.Blue; break;
                    case METERS.NAMES.CENTER:
                        mark = Markers.CROSS; color = Brushes.Red; break;
                    case METERS.NAMES.AIRPORT:
                        mark = Markers.DIAMOND; color = Brushes.Green; break;
                    default:
                        mark = Markers.OTHER; color = Brushes.Orange; break;
                }
                drawer.DrawDataPoint(new Point(v.X, v.Y), color, mark);
            }
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            Draw();

            //if you want to verify which cell you selected and changed
            //if (dataGrid1.SelectedItem != null)
            //{
            //    object o = dataGrid1.CurrentItem;
            //    if (o is DataItem)
            //    {
            //        MessageBox.Show((o as DataItem).ToString());
            //    }
            //}
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(canvas);
            Point pp = drawer.Convert(p);
            point.Text = String.Format("(x, y) = ({0:N0}, {1:#0.0})", pp.X, pp.Y);
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            point.Text = "";
        }

        private void new_Click(object sender, RoutedEventArgs e)
        {
            cmhandler.New();
            drawer.Clear();
            data.Clear();
        }

        private void read_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            cmhandler.Open();
            dataGrid1.ItemsSource = Data;
            Draw();
        }

        // Allows menuitems and buttons to be visible all the time
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cmhandler.New();
            drawer.Clear();
            data.Clear();
        }

        private void Open_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            data.Clear();
            cmhandler.Open();
            dataGrid1.ItemsSource = Data;
            Draw();
        }

        private void Save_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cmhandler.Save(true);
        }

        private void SaveAs_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cmhandler.Save(false);
        }

        private void Close_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cmhandler.Close();
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
            Data = RandomData();
            dataGrid1.ItemsSource = Data;
            Draw();
        }

        // Generate random test data
        public ObservableCollection<DataItem> RandomData()
        {
            ObservableCollection<DataItem> d = new ObservableCollection<DataItem>();
            Random r = new Random();
            for (uint i = 0; i < 30; i++)
            {
                int place = r.Next(min, max);
                d.Add(new DataItem(i, r.NextDouble() * place,
                place % 3 == 0 ? METERS.NAMES.AIRPORT : (place % 3 == 1 ? METERS.NAMES.CENTER : METERS.NAMES.RUISSALO)));
            }
            return d;
        }
    }
}
