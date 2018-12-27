using System;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GemTDMaze {
    public sealed partial class MainPage : Page {
        private void RemoveAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            if (args.Handled) {
                return;
            }
            BlockRightTapped(Blocks[Cursor.Item1, Cursor.Item2], null);
            args.Handled = true;
        }

        private void PlaceAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            if (args.Handled) {
                return;
            }
            BlockTapped(Blocks[Cursor.Item1, Cursor.Item2], null);
            args.Handled = true;
        }

        private void RightAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            if (args.Handled) {
                return;
            }
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = BlockBorderBrush;
            Cursor.Item2 = Math.Clamp(Cursor.Item2 + 1, 0, 36);
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
            args.Handled = true;
        }

        private void LeftAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            if (args.Handled) {
                return;
            }
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = BlockBorderBrush;
            Cursor.Item2 = Math.Clamp(Cursor.Item2 - 1, 0, 36);
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
            args.Handled = true;
        }

        private void DownAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            if (args.Handled) {
                return;
            }
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = BlockBorderBrush;
            Cursor.Item1 = Math.Clamp(Cursor.Item1 + 1, 0, 36);
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
            args.Handled = true;
        }

        private void UpAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
            if (args.Handled) {
                return;
            }
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = BlockBorderBrush;
            Cursor.Item1 = Math.Clamp(Cursor.Item1 - 1, 0, 36);
            Blocks[Cursor.Item1, Cursor.Item2].BorderBrush = CursorBorderBrush;
            args.Handled = true;
        }
    }
}