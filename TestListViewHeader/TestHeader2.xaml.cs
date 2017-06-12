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
    class ListSourceModel : List<String>
    {
        public String Title { get; set; }
    }

    public sealed partial class TestHeader2 : Page
    {
        List<ListSourceModel> ItemSource;
        Windows.UI.Composition.Visual _headerVisual;
        public TestHeader2()
        {
            this.InitializeComponent();
            ItemSource = new List<ListSourceModel>();
            for (int j = 0; j < 3; j++)
            {
                var list = new ListSourceModel() { Title = "Item" + j };
                for (int i = 1; i <= 20; i++)
                    list.Add("Item" + i);
                ItemSource.Add(list);
            }
        }

        private void _pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _headerVisual?.StopAnimation("Offset.Y");
            UpdateAnimation();
        }

        void UpdateAnimation()
        {
            if (_pivot.SelectedIndex == -1) return;
            var SelectionItem = _pivot.ContainerFromIndex(_pivot.SelectedIndex) as PivotItem;
            if (SelectionItem == null) return;
            var _scrollviewer = FindFirstChild<ScrollViewer>(SelectionItem);
            if (_scrollviewer != null)
            {
                _headerVisual = ElementCompositionPreview.GetElementVisual(_header);
                var _manipulationPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollviewer);
                var _compositor = Window.Current.Compositor;

                var line = _compositor.CreateCubicBezierEasingFunction(new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(0.6f, 1));
                var MoveHeaderAnimation = _compositor.CreateScalarKeyFrameAnimation();
                MoveHeaderAnimation.InsertExpressionKeyFrame(0f, "_headerVisual.Offset.Y", line);
                MoveHeaderAnimation.InsertExpressionKeyFrame(1f, "_manipulationPropertySet.Translation.Y > -100f ? _manipulationPropertySet.Translation.Y: -100f", line);
                MoveHeaderAnimation.SetReferenceParameter("_headerVisual", _headerVisual);
                MoveHeaderAnimation.SetReferenceParameter("_manipulationPropertySet", _manipulationPropertySet);
                MoveHeaderAnimation.DelayTime = TimeSpan.FromSeconds(0.18d);
                MoveHeaderAnimation.Duration = TimeSpan.FromSeconds(0.1d);

                var Betch = _compositor.CreateScopedBatch(Windows.UI.Composition.CompositionBatchTypes.Animation);
                _headerVisual.StartAnimation("Offset.Y", MoveHeaderAnimation);
                Betch.Completed += (s, a) =>
                {
                    var _headerAnimation = _compositor.CreateExpressionAnimation("_manipulationPropertySet.Translation.Y > -100f ? (_manipulationPropertySet.Translation.Y == 0?This.CurrentValue :_manipulationPropertySet.Translation.Y) : -100f");
                    //_manipulationPropertySet.Translation.Y是ScrollViewer滚动的数值，手指向上移动的时候，也就是可视部分向下移动的时候，Translation.Y是负数。

                    _headerAnimation.SetReferenceParameter("_manipulationPropertySet", _manipulationPropertySet);
                    _headerVisual.StartAnimation("Offset.Y", _headerAnimation);
                };
                Betch.End();

            }
            else
                SelectionItem.Loaded += (s, a) =>
                {
                    UpdateAnimation();
                };
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

