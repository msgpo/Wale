﻿using System;
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
using System.Windows.Shapes;

namespace Wale.WPF
{
    /// <summary>
    /// Interaction logic for DeviceMap.xaml
    /// </summary>
    public partial class DeviceMap : Window
    {
        Properties.Settings settings = Properties.Settings.Default;
        private List<Wale.CoreAudio.DeviceData> data;

        public DeviceMap()
        {
            InitializeComponent();
        }
        public DeviceMap(List<Wale.CoreAudio.DeviceData> data)
        {
            InitializeComponent();
            this.data = data;
        }

        #region title panel control, location and size check events
        private Point titlePosition;
        private void titlePanel_MouseDown(object sender, MouseButtonEventArgs e) { titlePosition = e.GetPosition(this); }
        private void titlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point loc = PointToScreen(e.GetPosition(this));
                //MessageBox.Show($"L={Screen.PrimaryScreen.WorkingArea.Left} R={Screen.PrimaryScreen.WorkingArea.Right}, T={Screen.PrimaryScreen.WorkingArea.Top} B={Screen.PrimaryScreen.WorkingArea.Bottom}");
                //Console.WriteLine($"W:{Left},M:{loc.X},LM:{titlePosition.X},SW:{System.Windows.SystemParameters.PrimaryScreenWidth}");
                double x = loc.X - titlePosition.X;
                if (x + this.Width >= System.Windows.SystemParameters.WorkArea.Width) x = System.Windows.SystemParameters.WorkArea.Width - this.Width;
                else if (x <= 0) x = 0;

                double y = loc.Y - titlePosition.Y;
                if (y + this.Height >= System.Windows.SystemParameters.WorkArea.Height) y = System.Windows.SystemParameters.WorkArea.Height - this.Height;
                else if (y <= 0) y = 0;
                //MessageBox.Show($"x={x} y={y}");
                Left = x;
                Top = y;
            }
        }
        private void Window_LocationAndSizeChanged(object sender, EventArgs e)
        {
            if ((this.Left + this.Width) > System.Windows.SystemParameters.WorkArea.Width)
                this.Left = System.Windows.SystemParameters.WorkArea.Width - this.Width;

            if (this.Left < System.Windows.SystemParameters.WorkArea.Left)
                this.Left = System.Windows.SystemParameters.WorkArea.Left;


            if ((this.Top + this.Height) > System.Windows.SystemParameters.WorkArea.Height)
                this.Top = System.Windows.SystemParameters.WorkArea.Height - this.Height;

            if (this.Top < System.Windows.SystemParameters.WorkArea.Top)
                this.Top = System.Windows.SystemParameters.WorkArea.Top;
        }



        #endregion

        #region Data control
        private Task GetData()
        {
            return Task.Run(() => { data = new Wale.CoreAudio.Audio().GetDeviceList(); });
        }
        private void DrawMap()
        {
            if (data == null) { MessageBox.Show("There is no data."); return; }
            //View.ItemsSource = data;
            View.Items.Clear();
            View.Items.IsLiveSorting = true;
            foreach (Wale.CoreAudio.DeviceData dd in data)
            {
                TreeViewItem node = new TreeViewItem() { Header = dd.Name, ToolTip = dd.DeviceId };
                switch (dd.State)
                {
                    case CoreAudio.DeviceState.Active:
                        node.Background = new SolidColorBrush(ColorSet.MainColor);
                        break;
                    case CoreAudio.DeviceState.Disabled:
                        node.Background = new SolidColorBrush(ColorSet.AverageColor);
                        break;
                    case CoreAudio.DeviceState.UnPlugged:
                        node.Background = new SolidColorBrush(ColorSet.PeakColor);
                        break;
                    default:
                        break;
                }
                if (dd.Sessions != null)
                {
                    foreach (Wale.CoreAudio.SessionData ss in dd.Sessions)
                    {
                        TreeViewItem childNode = new TreeViewItem() { Header = ss.Name };
                        switch (ss.State)
                        {
                            case CoreAudio.SessionState.Active:
                                childNode.Background = new SolidColorBrush(ColorSet.MainColor);
                                break;
                            case CoreAudio.SessionState.Inactive:
                                childNode.Background = new SolidColorBrush(ColorSet.AverageColor);
                                break;
                            case CoreAudio.SessionState.Expired:
                                childNode.Background = new SolidColorBrush(ColorSet.PeakColor);
                                break;
                            default:
                                break;
                        }
                        childNode.IsExpanded = true;
                        node.Items.Add(childNode);
                    }
                }
                node.ContextMenu = new ContextMenu();

                MenuItem item1 = new MenuItem() { Header = "Enable", };
                item1.Click += OnEnableDevice;
                node.ContextMenu.Items.Add(item1);

                MenuItem item2 = new MenuItem() { Header = "Disable", };
                item2.Click += OnDisableDevice;
                node.ContextMenu.Items.Add(item2);

                MenuItem item3 = new MenuItem() { Header = "Re-Enable", };
                item3.Click += OnReEnableDevice;
                node.ContextMenu.Items.Add(item3);

                node.IsExpanded = true;
                View.Items.Add(node);
            }
        }
        private void OnEnableDevice(object sender, EventArgs e)
        {
            ///TreeNode node = sender as TreeNode;//data.Find(dd => dd.Name == node.Name);
            //Wale.CoreAudio.Audio audio = new CoreAudio.Audio();
            //audio.EnableAudioDevice(View.SelectedItem.ToolTipText);
        }
        private void OnDisableDevice(object sender, EventArgs e)
        {
            //TreeNode node = sender as TreeNode;//data.Find(dd => dd.Name == node.Name);
            //Wale.CoreAudio.Audio audio = new CoreAudio.Audio();
            //audio.DisableAudioDevice(View.SelectedNode.ToolTipText);
        }
        private void OnReEnableDevice(object sender, EventArgs e)
        {
            //TreeNode node = sender as TreeNode;//data.Find(dd => dd.Name == node.Name);
            //Wale.CoreAudio.Audio audio = new CoreAudio.Audio();
            //audio.DisableAudioDevice(View.SelectedNode.ToolTipText);
            //audio.EnableAudioDevice(View.SelectedNode.ToolTipText);
        }
        #endregion

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await GetData();
            DrawMap();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Update_Click(sender, new RoutedEventArgs());
        }
    }
}
