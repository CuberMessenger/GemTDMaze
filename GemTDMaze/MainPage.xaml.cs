using System;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GemTDMaze {
    public enum BlockType { Normal, KeyPoint, Path, Unavailable };

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

        private BlockType[,] BlockTypes = new BlockType[37, 37];

        private short[,,,] Distances = new short[37,37,37,37];

        private (int, int) Cursor = (18, 18);


        public MainPage() {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = FixedWindowSize;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            InitializeMaze();
            InitializeAccelerator();
        }

        private void InitializeAdjacency() {
            for (int i = 0; i < 37; i++) {
                for (int j = 0; j < 37; j++) {
                    //Skip stones
                    if (Blocks[i,j].Background == TappedBrush) {
                        continue;
	                }

                    //Up
                    if (i > 0) {
                        if (Blocks[i-1,j].Background != TappedBrush) {
                            Distances[i,j,i-1,j] = 1;
	                    }
                        else {
                            Distances[i,j,i-1,j] = short.MaxValue;
                        }
                    }

                    //Down
                    if (i < 36) {
                        if (Blocks[i+1,j].Background != TappedBrush) {
                            Distances[i,j,i+1,j] = 1;
	                    }
                        else {
                            Distances[i,j,i+1,j] = short.MaxValue;
                        }
                    }

                    //Left
                    if (j > 0) {
                        if (Blocks[i,j-1].Background != TappedBrush) {
                            Distances[i,j,i,j-1] = 1;
	                    }
                        else {
                            Distances[i,j,i,j-1] = short.MaxValue;
                        }
                    }

                    //Right
                    if (j < 36) {
                        if (Blocks[i,j+1].Background != TappedBrush) {
                            Distances[i,j,i,j+1] = 1;
	                    }
                        else {
                            Distances[i,j,i,j+1] = short.MaxValue;
                        }
                    }
                }
            }
        }

        private void InitializeAccelerator() {
            var UpAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.Up };
            var DownAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.Down };
            var LeftAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.Left };
            var RightAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.Right };
            var PlaceAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.Q };
            var RemoveAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.W };

            UpAccelerator.Invoked += UpAccelerator_Invoked;
            DownAccelerator.Invoked += DownAccelerator_Invoked;
            LeftAccelerator.Invoked += LeftAccelerator_Invoked;
            RightAccelerator.Invoked += RightAccelerator_Invoked;
            PlaceAccelerator.Invoked += PlaceAccelerator_Invoked;
            RemoveAccelerator.Invoked += RemoveAccelerator_Invoked;

            滴汤Grid.KeyboardAccelerators.Add(UpAccelerator);
            滴汤Grid.KeyboardAccelerators.Add(DownAccelerator);
            滴汤Grid.KeyboardAccelerators.Add(LeftAccelerator);
            滴汤Grid.KeyboardAccelerators.Add(RightAccelerator);
            滴汤Grid.KeyboardAccelerators.Add(PlaceAccelerator);
            滴汤Grid.KeyboardAccelerators.Add(RemoveAccelerator);
        }

        private void InitializeMaze() {
            //Build base
            for (int i = 0; i < 37; i++) {
                for (int j = 0; j < 37; j++) {
                    Blocks[i, j] = new Border();
                    BlockTypes[i, j] = ((i <= 8 && j <= 8) || (i >= 28 && j >= 28)) ? BlockType.Unavailable : BlockType.Normal;
                    Blocks[i, j].Tag = (i, j);
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
                BlockTypes[pair.Item1, pair.Item2] = BlockType.KeyPoint;
                Blocks[pair.Item1, pair.Item2].Child = new TextBlock {
                    Text = pair.Item3,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            //Add color
            for (int i = 0; i < 37; i++) {
                for (int j = 0; j < 37; j++) {
                    switch (BlockTypes[i, j]) {
                        case BlockType.Normal:
                            Blocks[i, j].Background = NormalBackgroundBrush;
                            break;
                        case BlockType.KeyPoint:
                            Blocks[i, j].Background = KeyPointBrush;
                            break;
                        case BlockType.Unavailable:
                            Blocks[i, j].Background = UnavailableBrush;
                            break;
                        default:
                            break;
                    }
                }
            }

            //Paths
            for (int i = 0; i < 15; i++) {
                Blocks[i + 3, 4].Background = PathBrush;
            }
            for (int i = 0; i < 9; i++) {
                BlockTypes[i + 9, 4] = BlockType.Path;
            }

            for (int i = 0; i < 27; i++) {
                Blocks[18, i + 5].Background = PathBrush;
                BlockTypes[18, i + 5] = BlockType.Path;
            }

            for (int i = 0; i < 13; i++) {
                Blocks[i + 5, 32].Background = PathBrush;
                BlockTypes[i + 5, 32] = BlockType.Path;
            }

            for (int i = 0; i < 13; i++) {
                Blocks[4, i + 19].Background = PathBrush;
                BlockTypes[4, i + 19] = BlockType.Path;
            }

            for (int i = 0; i < 27; i++) {
                Blocks[i + 5, 18].Background = PathBrush;
                BlockTypes[i + 5, 18] = BlockType.Path;
            }

            for (int i = 0; i < 15; i++) {
                Blocks[32, i + 19].Background = PathBrush;
            }
            for (int i = 0; i < 9; i++) {
                BlockTypes[32, i + 19] = BlockType.Path;
            }

            //Cursor
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
        }

        private void BlockRightTapped(object sender, RightTappedRoutedEventArgs e) {
            var block = sender as Border;
            var tag = ((int, int))block.Tag;
            var type = BlockTypes[tag.Item1, tag.Item2];
            if (type == BlockType.Normal) {
                block.Background = NormalBackgroundBrush;
            }
            else if (type == BlockType.Path) {
                block.Background = PathBrush;
            }
        }

        private void BlockTapped(object sender, TappedRoutedEventArgs e) {
            var block = sender as Border;
            var tag = ((int, int))block.Tag;
            var type = BlockTypes[tag.Item1, tag.Item2];
            if (type == BlockType.Normal || type == BlockType.Path) {
                block.Background = TappedBrush;
            }
            else if (type == BlockType.KeyPoint || type == BlockType.Unavailable) {
                ShowMessageDialogAsync("不能在这里放置！");
            }

            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = BlockBorderBrush;
            Cursor = tag;
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
        }

        private async void ShowMessageDialogAsync(string message) {
            var messageDialog = new MessageDialog(message);
            messageDialog.Commands.Add(new UICommand("关闭"));
            await messageDialog.ShowAsync();
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e) => ApplicationView.GetForCurrentView().TryResizeView(FixedWindowSize);
    }
}
