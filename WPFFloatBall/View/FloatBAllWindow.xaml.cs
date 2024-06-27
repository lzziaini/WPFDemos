using System.Windows;
using System.Windows.Input;

namespace WPFFloatBall.View
{
    /// <summary>
    /// FloatAlarmWIndow.xaml 的交互逻辑
    /// </summary>
    public partial class FloatAlarmWIndow : Window
    {
        public FloatAlarmWIndow()
        {
            InitializeComponent();

        }

		private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

        }

		private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.DragMove();
			}
		}
	}
}
