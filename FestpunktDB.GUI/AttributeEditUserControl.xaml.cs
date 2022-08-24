using System.Windows;
using System.Windows.Controls;

namespace FestpunktDB.GUI
{
    /// <summary>
    /// Interaction logic for AttributeEditUserControl.xaml
    /// </summary>
    public partial class AttributeEditUserControl : UserControl
    {
        public AttributeEditUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The attribute name.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(AttributeEditUserControl), new PropertyMetadata(null));
    }
}
