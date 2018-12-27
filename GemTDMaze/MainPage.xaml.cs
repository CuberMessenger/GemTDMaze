using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace GemTDMaze {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page {
        //Static Resource
        private static Size FixedWindowSize = new Size(925d, 965d);

        private static Brush BlockBorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);

        private static Brush CursorBorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);

        private static Brush PathBrush = new SolidColorBrush(Windows.UI.Colors.Silver);

        private static Brush UnavailableBrush = new SolidColorBrush(Windows.UI.Colors.Khaki);

        private static Brush NormalBackgroundBrush = new SolidColorBrush(Windows.UI.Colors.AliceBlue);

        private static Brush KeyPointBrush = new SolidColorBrush(Windows.UI.Colors.Chartreuse);

        private static Brush TappedBrush = new SolidColorBrush(Windows.UI.Colors.Black);

        private static (int, int, string)[] KeyPointsLookUp = {
            (2, 4, "S"),
            (4, 18, "4"),
            (4, 32, "3"),
            (18, 4, "1"),
            (18, 32, "2"),
            (32, 18, "5"),
            (32, 34, "E")
        };

        //
        private Border[,] Blocks = new Border[37, 37];

        private (int, int) Cursor = (18, 18);


        public MainPage() {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = FixedWindowSize;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            InitializeMaze();
        }

        private void InitializeMaze() {
            //Build base
            for (int i = 0; i < 37; i++) {
                for (int j = 0; j < 37; j++) {
                    Blocks[i, j] = new Border();
                    Blocks[i, j].Background = ((i <= 8 && j <= 8) || (i >= 28 && j >= 28)) ? UnavailableBrush : NormalBackgroundBrush;
                    Blocks[i, j].Tag = ((i <= 8 && j <= 8) || (i >= 28 && j >= 28)) ? "Unavailable" : "Normal";
                    Blocks[i, j].BorderBrush = BlockBorderBrush;
                    Blocks[i, j].BorderThickness = new Thickness(.7d);
                    Blocks[i, j].SetValue(Grid.RowProperty, i);
                    Blocks[i, j].SetValue(Grid.ColumnProperty, j);
                    Blocks[i, j].Tapped += BlockTapped;
                    Blocks[i, j].RightTapped += BlockRightTapped;
                    MazeGrid.Children.Add(Blocks[i, j]);
                }
            }

            //Key points
            foreach (var pair in KeyPointsLookUp) {
                Blocks[pair.Item1, pair.Item2].Background = KeyPointBrush;
                Blocks[pair.Item1, pair.Item2].Tag = "KeyPoint";
                Blocks[pair.Item1, pair.Item2].Child = new TextBlock {
                    Text = pair.Item3,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            //Paths
            for (int i = 0; i < 15; i++) {
                Blocks[i + 3, 4].Background = PathBrush;
            }
            for (int i = 0; i < 9; i++) {
                Blocks[i + 9, 4].Tag = "Path";
            }

            for (int i = 0; i < 27; i++) {
                Blocks[18, i + 5].Background = PathBrush;
                Blocks[18, i + 5].Tag = "Path";
            }

            for (int i = 0; i < 13; i++) {
                Blocks[i + 5, 32].Background = PathBrush;
                Blocks[i + 5, 32].Tag = "Path";
            }

            for (int i = 0; i < 13; i++) {
                Blocks[4, i + 19].Background = PathBrush;
                Blocks[4, i + 19].Tag = "Path";
            }

            for (int i = 0; i < 27; i++) {
                Blocks[i + 5, 18].Background = PathBrush;
                Blocks[i + 5, 18].Tag = "Path";
            }

            for (int i = 0; i < 15; i++) {
                Blocks[32, i + 19].Background = PathBrush;
            }
            for (int i = 0; i < 9; i++) {
                Blocks[32, i + 19].Tag = "Path";
            }

            //Cursor
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
        }

        private void BlockRightTapped(object sender, RightTappedRoutedEventArgs e) {
            var block = sender as Border;
            string tag = block.Tag as string;
            if (tag == "Normal") {
                block.Background = NormalBackgroundBrush;
            }
            else if (tag == "Path") {
                block.Background = PathBrush;
            }
        }

        private void BlockTapped(object sender, TappedRoutedEventArgs e) {
            var block = sender as Border;
            string tag = block.Tag as string;
            if (tag == "Normal" || tag == "Path") {
                block.Background = TappedBrush;
            }
            else if (tag == "KeyPoint" || tag == "Unavailable") {
                ShowMessageDialogAsync("不能在这里放置！");
            }
        }

        private async void ShowMessageDialogAsync(string message) {
            var messageDialog = new MessageDialog(message);
            messageDialog.Commands.Add(new UICommand("关闭"));
            await messageDialog.ShowAsync();
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e) => ApplicationView.GetForCurrentView().TryResizeView(FixedWindowSize);
    }
}
