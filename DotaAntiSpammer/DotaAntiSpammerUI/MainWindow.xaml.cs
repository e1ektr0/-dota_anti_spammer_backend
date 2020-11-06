using System.Windows;

namespace DotaAntiSpammerNet
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
                     

            Match.Ini(DotaAntiSpammerCommon.Models.Match.Sample());

        }
        
    }
}