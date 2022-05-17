using System;
using System.Windows;
using System.Windows.Controls;

namespace wpf_application
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.ViewData(new MessageBoxErrorReporter(), new MessageBoxErrorReporter());
            CalculatingFunction.Text = "Cubic";
        }

        public class MessageBoxErrorReporter : ViewModel.IErrorReporter
        {
            public void ReportError(string message) => MessageBox.Show($"ERROR: {message}");

        }
    }

}