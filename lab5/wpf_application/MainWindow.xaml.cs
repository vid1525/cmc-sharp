using System;
using System.Collections.Generic;
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
using OxyPlot;

namespace wpf_application
{
    public partial class MainWindow : Window
    {
        private ViewData MainWindowInfo;
        public bool IsMeasured { get; set; } = false;

        public MainWindow()
        {
            MainWindowInfo = new ViewData();

            MainWindowInfo.Input = new InputChecker(Splines.SPf.CUBIC_FUNC, 3, 3, 0, 1, 0, 0, 0, 0);
            InitializeComponent();

            this.DataContext = MainWindowInfo;
            CalculatingFunction.Text = "Cubic";
        }

        private void MdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !MainWindowInfo.Input.ErrorMd;
        }

        private void MdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                MainWindowInfo.UpdateMeasuredData();
                IsMeasured = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception ocurred: {ex.Message}");
            }
        }

        private void SpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !MainWindowInfo.Input.ErrorSp && IsMeasured;
        }

        private void SpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try { 
                MainWindowInfo.UpdateSplineData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception ocurred: {ex.Message}");
            }
        }
    }

    public static class MenuCommands
    {
        public static readonly RoutedUICommand MeasuredData = new RoutedUICommand(
            "MeasuredData",
            "MeasuredData",
            typeof(MenuCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D1, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand Splines = new RoutedUICommand(
            "Splines",
            "Splines",
            typeof(MenuCommands),
            new InputGestureCollection()
            {
                 new KeyGesture(Key.D2, ModifierKeys.Control)
            }
        );
    }
}