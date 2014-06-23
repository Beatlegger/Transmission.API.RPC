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
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;

namespace Transmission.API.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client _client = new Client();

        public MainWindow()
        {
            InitializeComponent();

            _client.Host = "http://192.168.1.50:9091/transmission/rpc";

            var allTorrents = _client.GetTorrents(TorrentFields.ALL_FIELDS);
            lvTorrents.ItemsSource = allTorrents.Torrents;
        }
    }
}
