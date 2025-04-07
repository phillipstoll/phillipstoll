using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProgressionGeneratorUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        InitializeComponent();
    }
    private MainWindowViewModel mainWindowViewModel;

    private void listBoxHeights_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        // TODO: use binding to handle event
        if (sender is ListBox listBoxHeights)
        {
            if (ItemsControl.ContainerFromElement(listBoxHeights, e.OriginalSource as DependencyObject) is ListBoxItem item)
            {
                if (item.IsSelected)
                {
                    listBoxHeights.UnselectAll();
                    e.Handled = true;
                }
            }
        }
    }
}