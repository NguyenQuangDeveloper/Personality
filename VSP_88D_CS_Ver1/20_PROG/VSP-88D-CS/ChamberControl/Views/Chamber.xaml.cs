using System.Windows;
using System.Windows.Controls;

namespace ChamberControl.Views
{
    public partial class Chamber : UserControl
    {
        public Chamber()
        {
            InitializeComponent();
        }
        //public ChamberView() : this(new ChamberViewModels()) { }

        public ChamberViewModel ViewModel
        {
            get => (ChamberViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(ChamberViewModel),
                typeof(Chamber),
                new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Chamber view && e.NewValue is ChamberViewModel vm)
            {
                view.DataContext = vm;
            }
        }
    }
}
