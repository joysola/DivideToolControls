using DivideToolControls.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DivideToolControls.Controls
{
    /// <summary>
    /// MessageWind.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWind : Window
    {
        public bool IsWithButtons
        {
            get;
            set;
        }

        public bool DoNotShowMessageAgain => _DoNotShowAgain.IsChecked == true;

        public Visibility DoNotShowMessageAgainVisibility
        {
            get
            {
                return _DoNotShowAgain.Visibility;
            }
            set
            {
                _DoNotShowAgain.Visibility = value;
            }
        }

        public string Message
        {
            get
            {
                return _Message.Text;
            }
            set
            {
                _Message.Text = value;
            }
        }

        public bool? MessageResult
        {
            get;
            set;
        }

        public MessageWind()
        {
            InitializeComponent();
        }

        private ImageSource GetIconSrc(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Hand:
                    return SystemIcons.Error.ToImageSource();
                case MessageBoxIcon.Question:
                    return SystemIcons.Question.ToImageSource();
                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation.ToImageSource();
                case MessageBoxIcon.Asterisk:
                    return SystemIcons.Information.ToImageSource();
                default:
                    return null;
            }
        }

        public MessageWind(MessageBoxButton buttons, Window parent, string message, string caption, MessageBoxIcon icon, bool isError, bool isWithButtons = true)
        {
            InitializeComponent();
            Message = message;
            _Message.TextWrapping = TextWrapping.Wrap;
            _Message.Foreground = (isError ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Black);
            IsWithButtons = isWithButtons;
            if (isWithButtons)
            {
                switch (buttons)
                {
                    case MessageBoxButton.OK:
                        _NoButton.Visibility = Visibility.Collapsed;
                        _CancelButton.Visibility = Visibility.Collapsed;
                        break;
                    case MessageBoxButton.OKCancel:
                        _NoButton.Visibility = Visibility.Collapsed;
                        break;
                    case MessageBoxButton.YesNo:
                        _CancelButton.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        throw new ArgumentException($"Invalid argument: MessageBoxButton - {buttons}");
                    case MessageBoxButton.YesNoCancel:
                        break;
                }
            }
            else
            {
                _OKButton.Visibility = Visibility.Collapsed;
                _NoButton.Visibility = Visibility.Collapsed;
                _CancelButton.Visibility = Visibility.Collapsed;
            }
            base.Owner = parent;
            base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            IcoImage.Source = GetIconSrc(icon);
            _Title.Content = caption;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void _OKButton_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = true;
            MessageResult = true;
        }

        private void _NoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageResult = null;
            Close();
        }

        private void _CancelButton_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
            MessageResult = false;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsWithButtons)
            {
                base.DialogResult = false;
            }
        }
    }
}
