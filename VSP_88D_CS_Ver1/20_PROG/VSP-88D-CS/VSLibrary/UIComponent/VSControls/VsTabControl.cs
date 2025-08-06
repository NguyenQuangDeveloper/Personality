using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VSLibrary.Common.MVVM.Core;

namespace VSLibrary.UIComponent.VSControls
{
    public class VsTabControl : TabControl
    {
        static VsTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VsTabControl),
                new FrameworkPropertyMetadata(typeof(VsTabControl)));

            var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsTabControlStyle.xaml", UriKind.RelativeOrAbsolute);

            if (Application.Current != null)
            {
                bool alreadyAdded = Application.Current.Resources.MergedDictionaries
                    .OfType<ResourceDictionary>()
                    .Any(x => x.Source != null && x.Source.Equals(uri));

                if (!alreadyAdded)
                {
                    var dict = new ResourceDictionary { Source = uri };
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
            }
        }

        public VsTabControl()
        {
            this.PreviewMouseDoubleClick += OnTabItemDoubleClick;

            CloseTabCommand = new RelayCommand<TabItem>(tab =>
            {
                if (tab != null && Items.Contains(tab))
                {
                    Items.Remove(tab);
                }
            });
            
            Loaded += (s, e) =>
            {
                foreach (var tab in Items.OfType<VsTabItem>())
                {
                    AttachContextMenu(tab);
                }

                ItemContainerGenerator.ItemsChanged += (s2, e2) =>
                {
                    foreach (var item in Items.OfType<VsTabItem>())
                    {
                        if (item.ContextMenu == null)
                            AttachContextMenu(item);
                    }
                };
            };
        }

        #region Private Field
        private void OnTabItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tab = FindParent<VsTabItem>(e.OriginalSource as DependencyObject);
            if (tab != null && tab.IsSelected)
            {
                DetachTab(tab);
                e.Handled = true;
            }
        }

        private void AttachContextMenu(VsTabItem tab)
        {
            var menu = new ContextMenu();
            
            var detach = new MenuItem { Header = "이 탭 분리하기" };
            detach.Click += (s, e) => DetachTab(tab);
            
            var close = new MenuItem { Header = "이 탭 닫기" };
            close.Click += (s, e) => RemoveTab(tab);
            
            menu.Items.Add(detach);
            menu.Items.Add(close);
            
            tab.ContextMenu = menu;
        }
       
        private void OnCloseTabClick(object sender, RoutedEventArgs e)
        {
            if (TryGetTabItemFromContextMenu(sender, out var tab))
            {
                RemoveTab(tab);

                if (tab.Content is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                tab.Content = null;
            }
        }

        public void RemoveTab(TabItem tab)
        {
            if (tab == null) return;

            if (Items.Contains(tab))
            {
                Items.Remove(tab);
            }
        }
        
        private void OnDetachTabClick(object sender, RoutedEventArgs e)
        {
            if (TryGetTabItemFromContextMenu(sender, out var tab))
            {
                DetachTab(tab);
            }
        }

        private bool TryGetTabItemFromContextMenu(object sender, out VsTabItem tab)
        {
            tab = null;

            if (sender is MenuItem menuItem &&
                menuItem.Parent is ContextMenu ctx &&
                ctx.PlacementTarget is DependencyObject target)
            {
                tab = FindParent<VsTabItem>(target);
                return tab != null;
            }

            return false;
        }

        // 시각 트리 상위 요소에서 TabItem 찾기
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        // 탭 분리 동작
        private void DetachTab(VsTabItem tab)
        {
            if (tab?.Content is not UIElement content)
                return;

            var header = tab.Header;
            var icon = tab.Icon;
            var isClosable = tab.IsClosable;

            var floatWindow = new Window
            {
                Title = header?.ToString(),
                Content = content,
                Width = 800,
                Height = 600,
                Icon = icon,
            };

            floatWindow.Closed += (s, e) =>
            {
                // 창 닫힐 때 다시 탭으로 복원
                AddTab(header?.ToString(), content, icon, isClosable);
            };

            Items.Remove(tab);
            floatWindow.Show();
        }
        #endregion

        public void AddTab(string title, UIElement content, ImageSource? icon = null, bool isClosable = true)
        {
            var newTab = new VsTabItem
            {
                Header = title,
                Content = content,
                Icon = icon,
                IsClosable = isClosable,
            };

            Items.Add(newTab);
            SelectedItem = newTab;
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register(nameof(CloseTabCommand), typeof(ICommand), typeof(VsTabControl), new PropertyMetadata(null));
        public ICommand CloseTabCommand
        {
            get => (ICommand)GetValue(CloseTabCommandProperty);
            set => SetValue(CloseTabCommandProperty, value);
        }
    }

    /// <summary>
    /// 확장 기능 추가를 위한 커스텀 TabItem
    /// </summary>
    public class VsTabItem : TabItem
    {
        public static readonly DependencyProperty IsClosableProperty =
            DependencyProperty.Register(nameof(IsClosable), typeof(bool), typeof(VsTabItem), new PropertyMetadata(true));
        public bool IsClosable
        {
            get => (bool)GetValue(IsClosableProperty);
            set => SetValue(IsClosableProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(VsTabItem), new PropertyMetadata(null));
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        #region Design Option
        public static readonly DependencyProperty TabHeaderBackgroundProperty =
           DependencyProperty.Register(nameof(TabHeaderBackground), typeof(Brush), typeof(VsTabItem), new PropertyMetadata(Brushes.LightGray));
        public Brush TabHeaderBackground
        {
            get => (Brush)GetValue(TabHeaderBackgroundProperty);
            set => SetValue(TabHeaderBackgroundProperty, value);
        }

        public static readonly DependencyProperty TabHeaderSelectedBackgroundProperty =
            DependencyProperty.Register(nameof(TabHeaderSelectedBackground), typeof(Brush), typeof(VsTabItem), new PropertyMetadata(Brushes.White));
        public Brush TabHeaderSelectedBackground
        {
            get => (Brush)GetValue(TabHeaderSelectedBackgroundProperty);
            set => SetValue(TabHeaderSelectedBackgroundProperty, value);
        }

        public static readonly DependencyProperty TabCornerRadiusProperty =
            DependencyProperty.Register(nameof(TabCornerRadius), typeof(CornerRadius), typeof(VsTabItem), new PropertyMetadata(new CornerRadius(4)));
        public CornerRadius TabCornerRadius
        {
            get => (CornerRadius)GetValue(TabCornerRadiusProperty);
            set => SetValue(TabCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty TabBorderThicknessProperty =
            DependencyProperty.Register(nameof(TabBorderThickness), typeof(Thickness), typeof(VsTabItem), new PropertyMetadata(new Thickness(1)));
        public Thickness TabBorderThickness
        {
            get => (Thickness)GetValue(TabBorderThicknessProperty);
            set => SetValue(TabBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty TabHeaderFontSizeProperty =
            DependencyProperty.Register(nameof(TabHeaderFontSize), typeof(double), typeof(VsTabItem), new PropertyMetadata(12.0));
        public double TabHeaderFontSize
        {
            get => (double)GetValue(TabHeaderFontSizeProperty);
            set => SetValue(TabHeaderFontSizeProperty, value);
        }
        #endregion
        static VsTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VsTabItem),
                new FrameworkPropertyMetadata(typeof(VsTabItem)));
        }
    }
}

