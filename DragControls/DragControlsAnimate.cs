using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace DragControls
{
	/// <summary>
	/// 拖拽控件动画
	/// </summary>
	public class DragControlsAnimate
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="Windows">窗体</param>
		/// <param name="LlayoutContainer">容器：让控件在这里面拖动</param>
		public DragControlsAnimate(FrameworkElement Windows, object LlayoutContainer)
		{
			this.Windows = Windows;
			this.LlayoutContainer = LlayoutContainer;
			Windows.SizeChanged += Windows_SizeChanged;
		}
		#region 私有字段
		/// <summary>
		/// 界面上已经生成的控件，也就是从哪个控件上拖动的集合
		/// </summary>
		List<FrameworkElement> ShowControlsList = new List<FrameworkElement>();
		/// <summary>
		/// 窗体
		/// </summary>
		FrameworkElement Windows;
		/// <summary>
		/// 容器：让控件在这里面拖动
		/// </summary>
		object LlayoutContainer;
		/// <summary>
		/// 鼠标是否按下
		/// </summary>
		bool IsMouseDown = false;
		/// <summary>
		/// 实时需要拖动的控件
		/// </summary>
		FrameworkElement ControlsObj;
		/// <summary>
		/// 拖拽大小与移动
		/// </summary>
		DragControlsHelper dragControlsHelper = new DragControlsHelper();
		#endregion

		#region 方法

		/// <summary>
		/// 添加拖拽大小与移动
		/// </summary>
		public void DragSizeInsert(FrameworkElement Controls, FrameworkElement Window)
		{
			//创建拖动与拖拽大小
			dragControlsHelper.Insert(Controls, Window);
		}

		/// <summary>
		/// 移除拖拽大小与移动
		/// </summary>
		public void DragSizeRemove(FrameworkElement Controls)
		{
			//创建拖动与拖拽大小
			dragControlsHelper.Remove(Controls);
		}
		/// <summary>
		///  添加需要拖动的组件
		/// </summary>
		/// <param name="ControlsShow">界面上已经生成的控件</param>
		public void Insert(FrameworkElement ControlsShow)
		{
			if (!ShowControlsList.Contains(ControlsShow))  //不存在则添加
			{
				InsertEven(ControlsShow);
				ShowControlsList.Add(ControlsShow);
			}
		}
		/// <summary>
		/// 移除拖动
		/// </summary>
		/// <param name="ControlsShow">界面上已经生成的控件</param>
		public void Remove(FrameworkElement ControlsShow)
		{
			if (ShowControlsList.Contains(ControlsShow))
			{
				RemoveEven(ControlsShow);
				ShowControlsList.Remove(ControlsShow);  //直接移除
			}
		}
		/// <summary>
		/// 创建事件
		/// </summary>
		/// <param name="ControlsShow">界面上已经生成的控件</param>
		public void InsertEven(FrameworkElement ControlsShow)
		{
			//ControlsShow.PreviewMouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e) { ControlsShow_PreviewMouseLeftButtonDown(sender, e, ControlsObj); };
			//ControlsShow.PreviewMouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e) { ControlsShow_PreviewMouseLeftButtonUp(sender, e, ControlsObj); };
			//ControlsShow.PreviewMouseMove += delegate (object sender, MouseEventArgs e) { ControlsShow_PreviewMouseMove(sender, e, ControlsObj); };

			ControlsShow.PreviewMouseLeftButtonDown += ControlsShow_PreviewMouseLeftButtonDown;
			ControlsShow.PreviewMouseLeftButtonUp += ControlsShow_PreviewMouseLeftButtonUp;
			ControlsShow.PreviewMouseMove += ControlsShow_PreviewMouseMove;
		}
		/// <summary>
		/// 移除事件
		/// </summary>
		/// <param name="ControlsShow">界面上已经生成的控件</param>
		public void RemoveEven(FrameworkElement ControlsShow)
		{
			ControlsShow.PreviewMouseLeftButtonDown -= ControlsShow_PreviewMouseLeftButtonDown;
			ControlsShow.PreviewMouseLeftButtonUp -= ControlsShow_PreviewMouseLeftButtonUp;
			ControlsShow.PreviewMouseMove -= ControlsShow_PreviewMouseMove;
		}

		#endregion

		#region 委托回调事件

		/// <summary>
		/// 定义委托 提醒拖拽事件开始了，请传需要拖动的按钮对象
		/// </summary>
		/// <param name="ShowControl">在哪个控件上触发了拖拽</param>
		/// <returns>返回已经创建了新的控件对象  -   是否需要拖拽大小</returns>
		public delegate (FrameworkElement NewControl, bool IsDragAndDragSize) dragEvenTrigger(FrameworkElement ShowControl);
		/// <summary>
		/// 实现委托
		/// </summary>
		public dragEvenTrigger DragEvenTrigger;

		/// <summary>
		/// 消息委托
		/// </summary>
		/// <param name="Message">消息</param>
		/// <param name="element">哪个控件显示的消息</param>
		public delegate void messageEvenTrigger(string Message, FrameworkElement element);
		/// <summary>
		/// 实现委托
		/// </summary>
		public messageEvenTrigger MessageEvenTrigger;
		#endregion

		#region 执行事件

		//移动位置
		private void ControlsShow_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (ControlsObj == null) return;
			if (IsMouseDown)
			{
				if (LlayoutContainer.GetType().Equals(typeof(Canvas)))
				{
					Point pos = e.GetPosition(Windows);
					Canvas.SetLeft(ControlsObj, pos.X - ControlsObj.Width / 2);
					Canvas.SetTop(ControlsObj, pos.Y - ControlsObj.Height / 2);
				}
				else if (LlayoutContainer.GetType().Equals(typeof(Grid)))
				{
					Point pos = e.GetPosition(Windows);
					double Left = pos.X - ControlsObj.Width / 2;
					double Top = pos.Y - ControlsObj.Height / 2;
					double Right = Windows.ActualWidth - Left - ControlsObj.Width;
					double Bottom = Windows.ActualHeight - Top - ControlsObj.Height;
					ControlsObj.Margin = new Thickness(Left, Top, Right, Bottom);
				}
			}
		}

		//当在已显示的控件左键点松开后
		private void ControlsShow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			IsMouseDown = false;
			if (ControlsObj == null) return;
			ControlsObj.Opacity = 1;
			ControlsObj = null;
		}


		//当在已显示的控件左键点击后
		private void ControlsShow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (LlayoutContainer.GetType().Equals(typeof(Canvas)))
			{
				Canvas layout = LlayoutContainer as Canvas;
				(FrameworkElement element, bool IsDragAndDragSize) Data = DragEvenTrigger(sender as FrameworkElement);
				ControlsObj = Data.element;
				if (!layout.Children.Contains(ControlsObj))
				{
					IsMouseDown = true;
					Point Position = e.GetPosition(Windows);
					ControlsObj.Opacity = 0.5;
					Canvas.SetLeft(ControlsObj, Position.X - ControlsObj.Width / 2);
					Canvas.SetTop(ControlsObj, Position.Y - ControlsObj.Height / 2);
					layout.Children.Add(ControlsObj);
					if (Data.IsDragAndDragSize)
					{
						//添加拖拽大小与移动
						DragSizeInsert(ControlsObj, Windows);
					}
				}
				else
				{
					MessageEvenTrigger("此控件已在布局中存在", sender as FrameworkElement);
					ControlsObj = null;
				}
			}
			else if (LlayoutContainer.GetType().Equals(typeof(Grid)))
			{
				Grid layout = LlayoutContainer as Grid;
				(FrameworkElement element, bool IsDragAndDragSize) Data = DragEvenTrigger(sender as FrameworkElement);
				ControlsObj = Data.element;
				if (!layout.Children.Contains(ControlsObj))
				{
					IsMouseDown = true;
					Point Position = e.GetPosition(Windows);
					ControlsObj.Opacity = 0.5;

					double Left = Position.X - ControlsObj.Width / 2;
					double Top = Position.Y - ControlsObj.Height / 2;
					double Right = Windows.ActualWidth - Left - ControlsObj.Width;
					double Bottom = Windows.ActualHeight - Top - ControlsObj.Height;


					ControlsObj.Margin = new Thickness(Left, Top, Right, Bottom);


					layout.Children.Add(ControlsObj);
					if (Data.IsDragAndDragSize)
					{
						//添加拖拽大小与移动
						DragSizeInsert(ControlsObj, Windows);
					}
				}
				else
				{
					MessageEvenTrigger("此控件已在布局中存在", sender as FrameworkElement);
					ControlsObj = null;
				}
			}
		}


		//当窗体大小改变，布局容器也要跟着改变大小
		private void Windows_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			FrameworkElement window = sender as FrameworkElement;
			if (LlayoutContainer.GetType().Equals(typeof(Canvas)))
			{
				Canvas layout = LlayoutContainer as Canvas;
				layout.Width = window.ActualWidth;
				layout.Height = window.ActualHeight;
			}
			else if (LlayoutContainer.GetType().Equals(typeof(Grid)))
			{
				Grid layout = LlayoutContainer as Grid;
				layout.Width = window.ActualWidth;
				layout.Height = window.ActualHeight;
			}
		}
		#endregion

	}
}
