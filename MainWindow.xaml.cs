using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Calculate calculate = null!;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try { 
            double x = Convert.ToDouble(xBox.Text);
            double n = Convert.ToDouble(nBox.Text);
            calculate = new Calculate(x, n);
            calculate.CalculateStatusChanged += StatusUpdated;
            calculate.CalculationsEnded += CalculationsEnded;
            runButton.IsEnabled = false;
            cancelButton.IsEnabled = true;
            calculate.RunCalculations();
            }catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            pauseButton.IsEnabled = true;
        }

        private void CalculationsEnded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                runButton.IsEnabled = true;
                cancelButton.IsEnabled = false;
                pauseButton.IsEnabled = false;
                pauseButton.Content = "Преостановить";
            });
        }

        private void StatusUpdated(object? sender, CalculateEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                statusLabel.Content =
                "Status:\n" +
                "Current N:" + e.currentN + "\n" +
                "Current Sum:" + e.currentSum;
            });
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            calculate.CancelCalculations();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculate.isPaused)
            {
                calculate.ContinueCalculations();
                pauseButton.Content = "Преостановить";
            }
            else
            {
                calculate.PauseCalculations();
                pauseButton.Content = "Возобновить";
            }
        }
    }
}
