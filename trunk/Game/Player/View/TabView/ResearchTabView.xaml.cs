using SharedData.Types;
using SharedLogic.Production;
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

namespace Player.View.TabView
{
    /// <summary>
    /// Interaction logic for ResearchTabView.xaml
    /// </summary>
    public partial class ResearchTabView : UserControl
    {
        public ResearchTabView()
        {
            InitializeComponent();
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                string value = ((sender as StackPanel).DataContext as DoubleTagContainor).RealName;

                data.SetData("Object", value);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                 
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            // If the DataObject contains string data, extract it. 
            if (e.Data.GetDataPresent("Object"))
            {
                
                string data = (string)e.Data.GetData("Object");
                ((sender as Image).DataContext as ResearchItem).Name = data; 
                // Set Effects to notify the drag source what effect 
                // the drag-and-drop operation had. 
                // (Copy if CTRL is pressed; otherwise, move.) 
                e.Effects = DragDropEffects.Move;

            }
            e.Handled = true;
        }

        private void Image_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            // If the DataObject contains string data, extract it. 
            if (e.Data.GetDataPresent("Object"))
            {
               e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }
    }
}
