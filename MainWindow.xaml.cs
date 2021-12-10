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
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Uwp.Notifications;


namespace OUILookup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Mutex mutex = null;
        private Oui oui = null;
        private string last_clip = null;
        private bool first_load = true;

        public MainWindow()
        {
            const string appName = "OUILookup";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);
           
            if (!createdNew) 
            {
                this.first_load = false;
            }
            else // first instance
            {
                ClipboardNotification.ClipboardUpdate += this.onClipboardChanged; // Add clipboard change event handler
            }

            this.oui = new Oui();
            InitializeComponent();
            this.textbox_mac.Focus();
        }


        /*
         * Checks for a valid MAC address on each clipboard change
         */
        private void onClipboardChanged(object sender, EventArgs e) {
            string test = Clipboard.GetText(TextDataFormat.Text);

            if (this.last_clip == test) {
                return;
            }

            Regex r = new Regex("^(?:[0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2}|(?:[0-9a-fA-F]{2}-){5}[0-9a-fA-F]{2}|(?:[0-9a-fA-F]{2}){5}[0-9a-fA-F]{2}$"); // 00:00:00:00:00:00 & 00-00-00-00-00-00
            if (r.IsMatch(test)) {
                string vendor = oui.find(test);

                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText(test)
                    .AddText(vendor)
                    .Show();
            }

            this.last_clip = test;
        }


        /*
         * Actively checks for a Vender each time the MAC TextBox Field is updated. 
         */
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = this.textbox_mac.Text;
            this.textbox_vendor.Text = oui.find(text);
        }


        /*
         * GUI Closing Event Handler 
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.first_load) // keeps OUILookup ToastNotifications running in background
            {
                e.Cancel = true;
                Hide();
            }

        }
    }
}
