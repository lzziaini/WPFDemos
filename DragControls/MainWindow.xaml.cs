using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DragControls
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// 三合一
		/// 单个窗体中只能定义一个布局容器，这个布局容器，不能设置Margin，不能设置固定宽高
		/// </summary>
		DragControlsAnimate dragControlsAnimate;
		public MainWindow()
		{
			InitializeComponent();
			dragControlsAnimate = new DragControlsAnimate(this, Pane);   //你得定义一个容器传容器对象或者Name
			dragControlsAnimate.Insert(ConShow1);
			dragControlsAnimate.Insert(ConShow2);
			dragControlsAnimate.Insert(ConShow3);
			dragControlsAnimate.MessageEvenTrigger += MessageEvenTrigger;
			dragControlsAnimate.DragEvenTrigger += DragEvenTrigger;
		}
		/// <summary>
		/// 消息
		/// </summary>
		/// <param name="Message">消息</param>
		/// <param name="element">哪个控件显示的消息</param>
		public void MessageEvenTrigger(string Message, FrameworkElement element)
		{
			Console.WriteLine($"控件Name:{element.Name}->抛出消息：{Message}");
		}
		/// <summary>
		/// 提醒拖拽事件开始了，请传需要拖动的按钮对象
		/// </summary>
		/// <param name="element">在哪个控件上触发了拖拽</param>
		/// <returns>返回已经创建了新的控件对象  -   是否需要拖拽大小</returns>
		public (FrameworkElement NewControl, bool IsDragAndDragSize) DragEvenTrigger(FrameworkElement ShowControl)
		{
			FrameworkElement NewControl = new FrameworkElement();
			bool IsDragAndDragSize = false;
			switch (ShowControl.Name)
			{
				case "ConShow1":
					NewControl = InitControls(0);
					IsDragAndDragSize = false;
					break;
				case "ConShow2":
					NewControl = InitControls(1);
					IsDragAndDragSize = true;
					break;
				case "ConShow3":
					NewControl = InitControls(1);
					IsDragAndDragSize = true;
					break;
			}
			return (NewControl, IsDragAndDragSize);
		}
		/// <summary>
		/// 创建图标
		/// </summary>
		/// <param name="dashboardDataMode">图标类型</param>
		private Label InitControls(int A)
		{
			return new Label() { Background = new SolidColorBrush(A == 0 ? Colors.AliceBlue : Colors.AntiqueWhite), Width = 100, Height = 100, Content = "自定义控件" };
		}

	}
}
