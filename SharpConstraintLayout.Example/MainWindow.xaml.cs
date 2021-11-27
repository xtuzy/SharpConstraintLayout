using androidx.constraintlayout.core.widgets;
using SharpConstraintLayout.Example.Reload;
using SharpConstraintLayout.Wpf;
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

namespace SharpConstraintLayout.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConstraintLayout page;

        public MainWindow()
        {
            InitializeComponent();
            page = new ConstraintLayout();
            page.Background = new SolidColorBrush(Colors.Cyan);
            this.RootWindow.Children.Add(page);

            new Reload_MainPage().Reload(this, page);

            App.ReloadClient.Reload += ReloadClient_Reload;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //App.ReloadClient.Reload += ReloadClient_Reload;
        }

        private void ReloadClient_Reload(object? sender, EventArgs e)
        {
            App.ReloadClient.ReloadType<Reload_MainPage>(this, page);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            //App.ReloadClient.Reload -= ReloadClient_Reload;
        }
    }
}
