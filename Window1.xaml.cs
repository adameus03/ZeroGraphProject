using System.Windows;
using System.Windows.Controls;


namespace ZeroGraphProject
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void console_TextChanged(object sender, TextChangedEventArgs e)
        {
            console.ScrollToEnd();
        }
    }
}
