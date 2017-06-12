using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TestListViewHeader
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TestHeader1 : Page
    {
        List<string> ItemSource;

        public TestHeader1()
        {
            this.InitializeComponent();
            ItemSource = new List<string>();
            for (int i = 1; i <= 20; i++)
                ItemSource.Add("Item" + i);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetZIndex(_listview.ItemsPanelRoot, -1);
            //原本ListViewBase里Header是在ItemsPanelRoot下方的，使用Canvans.SetZIndex把ItemsPanelRoot设置到下方

            var _scrollviewer = FindFirstChild<ScrollViewer>(_listview);
            var _headerVisual = ElementCompositionPreview.GetElementVisual(_header);
            var _manipulationPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollviewer);

            var _compositor = Window.Current.Compositor;
            var _headerAnimation = _compositor.CreateExpressionAnimation("_manipulationPropertySet.Translation.Y > -100f ? 0: -100f -_manipulationPropertySet.Translation.Y");
            //_manipulationPropertySet.Translation.Y是ScrollViewer滚动的数值，手指向上移动的时候，也就是可视部分向下移动的时候，Translation.Y是负数。

            _headerAnimation.SetReferenceParameter("_manipulationPropertySet", _manipulationPropertySet);

            _headerVisual.StartAnimation("Offset.Y", _headerAnimation);
        }

        static T FindFirstChild<T>(FrameworkElement element) where T : FrameworkElement
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            var children = new FrameworkElement[childrenCount];

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                children[i] = child;
                if (child is T)
                    return (T)child;
            }

            for (int i = 0; i < childrenCount; i++)
                if (children[i] != null)
                {
                    var subChild = FindFirstChild<T>(children[i]);
                    if (subChild != null)
                        return subChild;
                }

            return null;
        }
    }
}

