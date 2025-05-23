﻿using Newsticker.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Newsticker.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string loadingState = "None")
        {
            if (loadingState.Equals("None"))
            {
                this.DataContext = new MainWindowViewModel(loadingState);
                InitializeComponent();
                ((MainWindowViewModel)this.DataContext).LoadAllComponents();
            }
            else
            {
                this.DataContext = new MainWindowViewModel(loadingState);
                InitializeComponent();
            }

        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        // TODO: Irgendwie ins ViewModel
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).LoadWeather(new object());
        }
    }
}
