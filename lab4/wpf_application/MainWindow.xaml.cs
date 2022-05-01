using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace wpf_application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewData MainWindowInfo;

        public MainWindow()
        {
            InitializeComponent();

            MainWindowInfo = new ViewData();

            this.DataContext = MainWindowInfo;
        }

        private void CloseMainWindow(object sender, CancelEventArgs e)
        {
            CheckDataSaved();
        }

        private void CheckDataSaved()
        {
            if (MainWindowInfo.DataSaved)
            {
                return;
            }

            var res = System.Windows.MessageBox.Show("Do you want to save changes? Otherwise it will be lost.", "Data is not saved!", System.Windows.MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                SaveFile(null, null);
            }
        }

        public void MakeNewBenchmark(object sender, RoutedEventArgs e)
        {
            CheckDataSaved();

            MainWindowInfo.Benchmark = new calculations.VMBenchmark();
            MainWindowInfo.DataSaved = true;
        }

        public void OpenFile(object sender, RoutedEventArgs e)
        {
            CheckDataSaved();

            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "benchmarks";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == false)
            {
                System.Windows.MessageBox.Show($"File {dlg.FileName} was not uploaded.");
                return;
            }

            try
            {
                MainWindowInfo.Load(dlg.FileName);
            } catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"File {dlg.FileName} was not uploaded.\n{ex}");
                return;
            }
            System.Windows.MessageBox.Show($"File {dlg.FileName} successfully uploaded.");
        }

        public void SaveFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "benchmarks";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == false)
            {
                System.Windows.MessageBox.Show($"File {dlg.FileName} was not saved.");
                return;
            }

            try
            {
                MainWindowInfo.Save(dlg.FileName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"File {dlg.FileName} was not saved.\n{ex}");
                return;
            }
                
            System.Windows.MessageBox.Show($"File {dlg.FileName} successfully saved.");
            MainWindowInfo.DataSaved = true;
        }

        public void AddVmTime(object sender, RoutedEventArgs e)
        {
            var vmGrid = GetGridFromUI();

            if (vmGrid is null)
            {
                return;
            }

            try
            {
                MainWindowInfo.AddVMTime(vmGrid);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Some errors occured during execution:\n{ex.Message}");
            }
        }

        public void AddVmAccuracy(object sender, RoutedEventArgs e)
        {
            var vmGrid = GetGridFromUI();

            if (vmGrid is null)
            {
                return;
            }

            try
            {
                MainWindowInfo.AddVMAccuracy(vmGrid);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Some errors occured during execution:\n{ex.Message}");
            }
        }

        private static calculations.VMf ConvertStringToVMf(string value)
        {
            if (value == "vmsExp")
            {
                return calculations.VMf.VMS_EXP;
            }
            else if (value == "vmdExp")
            {
                return calculations.VMf.VMD_EXP;
            }
            else if (value == "vmsErf")
            {
                return calculations.VMf.VMS_ERF;
            }
            else if (value == "vmdErf")
            {
                return calculations.VMf.VMD_ERF;
            }
            throw new InvalidEnumArgumentException();
        }

        private calculations.VMGrid GetGridFromUI()
        {
            calculations.VMf vmfFunction;
            try
            {
                vmfFunction = ConvertStringToVMf(CalculatingFunction.Text);
            }
            catch (InvalidEnumArgumentException)
            {
                System.Windows.MessageBox.Show("Invalid function type value");
                return null;
            }

            int argsVectorLength;
            try
            {
                argsVectorLength = Int32.Parse(GridPointsCount.Text);
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Invalid points count value - must be int");
                return null;
            }
            if (argsVectorLength < 2)
            {
                System.Windows.MessageBox.Show("Invalid points count value - must be >= 2");
                return null;
            }

            float leftBound;
            try
            {
                leftBound = Single.Parse(LeftBoundaryOfRange.Text, new CultureInfo(""));
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Invalid left bound value - must be float");
                return null;
            }

            float rightBound;
            try
            {
                rightBound = Single.Parse(RightBoundaryOfRange.Text, new CultureInfo(""));
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Invalid right bound value - must be float");
                return null;
            }

            if (leftBound > rightBound)
            {
                System.Windows.MessageBox.Show("Invalid range - right bound must be greater than left bound");
                return null;
            }

            return new calculations.VMGrid(argsVectorLength, leftBound, rightBound, vmfFunction);
        }
    }
}
