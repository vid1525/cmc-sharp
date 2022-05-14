using System;
using System.Windows;
using System.Windows.Input;

namespace wpf_application
{
    public partial class MainWindow : Window
    {
        private ViewModel.ViewData MainWindowInfo;
        public bool IsMeasured { get; set; } = false;

        public MainWindow()
        {
            MainWindowInfo = new ViewModel.ViewData();

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
                MainWindowInfo.DrawMeasuredData();
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
            try
            {
                MainWindowInfo.UpdateSplineData();
                MainWindowInfo.DrawSplinesData();
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