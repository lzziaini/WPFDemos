using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace DragControls
{
	/*
    注意：只要不带焦点的控件包括用户控件 都可以拖动与拖拽大小   【基类中的【公共参数】可以自行修改哦】
    使用方法[这是在一个窗体的后台代码]：
    //实例化对象
    public DragControlsHelper dragControlsHelper = new DragControlsHelper();

    //执行以下方法就可以拖拽了[this属于窗体的对象，小范围拖拽可以自建布局容器]
    dragControlsHelper.Insert(控件的对象或者控件的Name, this);

    //移除拖拽大小与移动也很简单
    dragControlsHelper.Remove(控件的对象或者控件的Name);

    //WPF中布局容器有6种如下：
    [Grid]网格布局，其中控件或容器需指定位置；
    [StackPanel]堆叠面板，其中的控件水平布局、竖直布局；
    [DockPanel]停靠面板，内部控件或容器可以放置在上、下、左、右；
    [WrapPanel]可以看作是具有自动换行功能的StackPanel容器。窗体太小时，其末尾的控件会自动换行，像Java中的流布局；
    [Canvas]坐标布局，基于坐标的布局，利用Canvas.Left,Canvas.Top,Canvas.Right,Canvas.Bottom这四个附加属性来定位控件坐标；
    [UniformGrid]指定行和列的数量, 均分有限的容器空间。
	*/

	/// <summary>
	/// 控件拖动基类
	/// </summary>
	public class DragControlsBase : Adorner
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="Controls">要拖动的控件</param>
		/// <param name="LlayoutContainer">窗体的布局容器</param>
		public DragControlsBase(UIElement Controls, FrameworkElement LlayoutContainer) : base(Controls)
		{
			this.Controls = Controls;
			this.LlayoutContainer = LlayoutContainer;

			InitDragDelta();  //初始化拖动大小

			InitMove();  //初始化移动
		}
		/// <summary>
		/// 容器边框颜色
		/// </summary>
		public SolidColorBrush BorderColor = new SolidColorBrush(Colors.Green);
		/// <summary>
		/// 容器边框线径
		/// </summary>
		public Thickness BorderWireDiameter = new Thickness(1);
		/// <summary>
		/// 容器边框透明度
		/// </summary>
		public double BorderOpacity = 0;
		/// <summary>
		/// 拖拽装饰器的内圈颜色
		/// </summary>
		public SolidColorBrush ThumbInnerColor = new SolidColorBrush(Colors.Red);
		/// <summary>
		/// 拖拽装饰器的外圈颜色
		/// </summary>
		public SolidColorBrush ThumbOuterColor = new SolidColorBrush(Colors.Red);
		/// <summary>
		/// 装饰器线径
		/// </summary>
		public double ThumbWireDiameter = 1;
		/// <summary>
		/// 装饰器透明度
		/// </summary>
		public double ThumbOpacity = 0.6;
		/// <summary>
		/// 拖拽最小的宽
		/// </summary>
		public double MinWidth = 100;
		/// <summary>
		/// 拖拽最小的高
		/// </summary>
		public double MinHeight = 100;
		/// <summary>
		/// 拖拽最大的宽
		/// </summary>
		public double MaxWidth = 500;
		/// <summary>
		/// 拖拽最大的高
		/// </summary>
		public double MaxHeight = 500;

		#region 私有字段
		/// <summary>
		/// 4条边
		/// </summary>
		Thumb LeftThumb, TopThumb, RightThumb, BottomThumb;
		/// <summary>
		/// 4个角
		/// </summary>
		Thumb LefTopThumb, RightTopThumb, RightBottomThumb, LeftbottomThumb;
		/// <summary>
		/// 中间  目前暂不使用
		/// </summary>
		Thumb CentreThumb;
		/// <summary>
		/// 布局容器，如果不使用布局容器，则需要给上述8个控件布局，实现和Grid布局定位是一样的，会比较繁琐且意义不大。
		/// </summary>
		Grid Llayout;
		/// <summary>
		/// 要拖动的控件
		/// </summary>
		UIElement Controls;
		/// <summary>
		/// 窗体的布局容器
		/// </summary>
		FrameworkElement LlayoutContainer;
		/// <summary>
		/// 鼠标是否按下
		/// </summary>
		bool IsMouseDown = false;
		/// <summary>
		/// 鼠标按下的位置
		/// </summary>
		Point MouseDownPosition;
		/// <summary>
		/// 鼠标按下控件的Margin
		/// </summary>
		Thickness MouseDownMargin;
		#endregion

		#region 重写方法
		protected override Visual GetVisualChild(int index)
		{
			return Llayout;
		}
		protected override int VisualChildrenCount
		{
			get
			{
				return 1;
			}
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			//直接给容器布局，容器内部的装饰器会自动布局。
			Llayout.Arrange(new Rect(new Point(-LeftThumb.Width / 2, -LeftThumb.Height / 2), new Size(finalSize.Width + LeftThumb.Width, finalSize.Height + LeftThumb.Height)));
			return finalSize;
		}
		#endregion

		#region 方法
		/// <summary>
		/// 初始化拖拽大小
		/// </summary>
		public void InitDragDelta()
		{
			//初始化装饰器
			LeftThumb = new Thumb();
			LeftThumb.HorizontalAlignment = HorizontalAlignment.Left;
			LeftThumb.VerticalAlignment = VerticalAlignment.Center;
			LeftThumb.Cursor = Cursors.SizeWE;
			TopThumb = new Thumb();
			TopThumb.HorizontalAlignment = HorizontalAlignment.Center;
			TopThumb.VerticalAlignment = VerticalAlignment.Top;
			TopThumb.Cursor = Cursors.SizeNS;
			RightThumb = new Thumb();
			RightThumb.HorizontalAlignment = HorizontalAlignment.Right;
			RightThumb.VerticalAlignment = VerticalAlignment.Center;
			RightThumb.Cursor = Cursors.SizeWE;
			BottomThumb = new Thumb();
			BottomThumb.HorizontalAlignment = HorizontalAlignment.Center;
			BottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
			BottomThumb.Cursor = Cursors.SizeNS;
			LefTopThumb = new Thumb();
			LefTopThumb.HorizontalAlignment = HorizontalAlignment.Left;
			LefTopThumb.VerticalAlignment = VerticalAlignment.Top;
			LefTopThumb.Cursor = Cursors.SizeNWSE;
			RightTopThumb = new Thumb();
			RightTopThumb.HorizontalAlignment = HorizontalAlignment.Right;
			RightTopThumb.VerticalAlignment = VerticalAlignment.Top;
			RightTopThumb.Cursor = Cursors.SizeNESW;
			RightBottomThumb = new Thumb();
			RightBottomThumb.HorizontalAlignment = HorizontalAlignment.Right;
			RightBottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
			RightBottomThumb.Cursor = Cursors.SizeNWSE;
			LeftbottomThumb = new Thumb();
			LeftbottomThumb.HorizontalAlignment = HorizontalAlignment.Left;
			LeftbottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
			LeftbottomThumb.Cursor = Cursors.SizeNESW;
			CentreThumb = new Thumb();
			CentreThumb.HorizontalAlignment = HorizontalAlignment.Center;
			CentreThumb.VerticalAlignment = VerticalAlignment.Center;
			CentreThumb.Cursor = Cursors.SizeAll;
			Llayout = new Grid();
			//给布局容器加个边框
			Border border = new Border();
			border.Margin = new Thickness(2);
			border.Opacity = BorderOpacity;
			border.BorderThickness = BorderWireDiameter;
			border.BorderBrush = BorderColor;
			Llayout.Children.Add(border);
			//给布局容器添加拖动大小装饰器
			Llayout.Children.Add(LeftThumb);
			Llayout.Children.Add(TopThumb);
			Llayout.Children.Add(RightThumb);
			Llayout.Children.Add(BottomThumb);
			Llayout.Children.Add(LefTopThumb);
			Llayout.Children.Add(RightTopThumb);
			Llayout.Children.Add(RightBottomThumb);
			Llayout.Children.Add(LeftbottomThumb);
			//Llayout.Children.Add(CentreThumb);   //中间的装饰器 暂不使用
			AddVisualChild(Llayout);
			foreach (var item in Llayout.Children)
			{
				if (item.GetType().Equals(typeof(Thumb)))
				{
					Thumb thumb = item as Thumb;
					thumb.Width = 5;   //设置圆圈的宽
					thumb.Height = 5;  //设置圆圈的高
					thumb.Opacity = ThumbOpacity;//透明度
					thumb.Template = new ControlTemplate(typeof(Thumb))   //模板
					{
						VisualTree = GetFactory(ThumbInnerColor, ThumbOuterColor, ThumbWireDiameter)
					};
					thumb.DragDelta += Control_DragDelta;
				}
			}
		}
		/// <summary>
		/// 装饰器样式
		/// </summary>
		/// <param name="InnerColor">内圈颜色</param>
		/// <param name="OuterColor">外圈颜色</param>
		/// <param name="WireDiameter">线径</param>
		/// <param name="Opacity">透明度</param>
		/// <returns></returns>
		FrameworkElementFactory GetFactory(Brush InnerColor, Brush OuterColor, double WireDiameter)
		{
			FrameworkElementFactory Element = new FrameworkElementFactory(typeof(Ellipse));  //绘制椭圆形元素
			Element.SetValue(Ellipse.FillProperty, InnerColor);  //内圈色
			Element.SetValue(Ellipse.StrokeProperty, OuterColor);  //外圈色
			Element.SetValue(Ellipse.StrokeThicknessProperty, WireDiameter);   //线径
			return Element;
		}

		/// <summary>
		/// 初始化移动
		/// </summary>
		public void InitMove()
		{
			//添加移动事件
			Controls.MouseLeftButtonDown += Control_MouseLeftButtonDown;   //鼠标左键按下
			Controls.MouseLeftButtonUp += Control_MouseLeftButtonUp;   //鼠标左键松开
			Controls.MouseMove += Control_MouseMove;   //鼠标移动
		}
		#endregion

		#region 事件
		//拖拽大小逻辑
		private void Control_DragDelta(object sender, DragDeltaEventArgs e)
		{
			FrameworkElement Control = Controls as FrameworkElement;  //要拖动的控件
			FrameworkElement Thumb = sender as FrameworkElement;  //哪个装饰被拖动
			double Left, Top, Right, Bottom, Width, Height;   //左，上，右，下，宽，高
			if (Thumb.HorizontalAlignment == HorizontalAlignment.Left)
			{
				Right = Control.Margin.Right;
				Left = Control.Margin.Left + e.HorizontalChange;
				Width = (double.IsNaN(Control.Width) ? Control.ActualWidth : Control.Width) - e.HorizontalChange;
			}
			else
			{
				Left = Control.Margin.Left;
				Right = Control.Margin.Right - e.HorizontalChange;
				Width = (double.IsNaN(Control.Width) ? Control.ActualWidth : Control.Width) + e.HorizontalChange;
			}
			if (Thumb.VerticalAlignment == VerticalAlignment.Top)
			{
				Bottom = Control.Margin.Bottom;
				Top = Control.Margin.Top + e.VerticalChange;
				Height = (double.IsNaN(Control.Height) ? Control.ActualHeight : Control.Height) - e.VerticalChange;
			}
			else
			{
				Top = Control.Margin.Top;
				Bottom = Control.Margin.Bottom - e.VerticalChange;
				Height = (double.IsNaN(Control.Height) ? Control.ActualHeight : Control.Height) + e.VerticalChange;
			}

			if (Thumb.HorizontalAlignment != HorizontalAlignment.Center)
			{
				if (Width >= 0)
				{
					if (Width >= MinWidth && Width <= MaxWidth)
					{
						Control.Margin = new Thickness(Left, Control.Margin.Top, Right, Control.Margin.Bottom);
						Control.Width = Width;
					}
				}
			}
			if (Thumb.VerticalAlignment != VerticalAlignment.Center)
			{
				if (Height >= 0)
				{
					if (Height >= MinHeight && Height <= MaxHeight)
					{
						Control.Margin = new Thickness(Control.Margin.Left, Top, Control.Margin.Right, Bottom);
						Control.Height = Height;
					}
				}
			}
		}

		//鼠标左键按下
		private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var c = sender as FrameworkElement;
			IsMouseDown = true;
			MouseDownPosition = e.GetPosition(LlayoutContainer);
			MouseDownMargin = c.Margin;
			c.CaptureMouse();
		}
		//鼠标左键松开
		private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var c = sender as FrameworkElement;
			IsMouseDown = false;
			c.ReleaseMouseCapture();
		}
		//鼠标移动
		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsMouseDown)
			{
				var c = sender as FrameworkElement;
				var pos = e.GetPosition(LlayoutContainer);
				var dp = pos - MouseDownPosition;
				double Left, Top, Right, Bottom;  //设置控件坐标
				Left = MouseDownMargin.Left + dp.X;
				Top = MouseDownMargin.Top + dp.Y;
				Right = MouseDownMargin.Right - dp.X;
				Bottom = MouseDownMargin.Bottom - dp.Y;
				c.Margin = new Thickness(Left, Top, Right, Bottom);

				//GeneralTransform generalTransform = c.TransformToAncestor(LlayoutContainer);
				//Point point = generalTransform.Transform(new Point(0, 0));
				////控件的  左上右下
				//double ControlLeft = c.Margin.Left;   //左
				//double ControlTop = c.Margin.Top;     //上
				//double ControlRight = point.X + c.Width;   //右
				//double ControlBottom = point.Y + c.Height;  //下
			}
		}
		#endregion
	}
}
