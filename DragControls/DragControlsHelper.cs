using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;

namespace DragControls
{
	/// <summary>
	/// 控件拖动实现类
	/// </summary>
	public class DragControlsHelper
	{
		/// <summary>
		/// 数据字典
		/// UIElement：要拖动的控件
		/// AdornerLayer：装饰器
		/// DragControlsBase：装饰器实现类
		/// </summary>
		Dictionary<UIElement, Tuple<AdornerLayer, DragControlsBase>> DictionaryDataList = new Dictionary<UIElement, Tuple<AdornerLayer, DragControlsBase>>();
		/// <summary>
		/// 添加项
		/// </summary>
		/// <param name="Controls">控件</param>
		/// <param name="LlayoutContainer">窗体的布局容器：意思就是这个控件是被谁包这的就传它，我一般传窗体对象，窗体包着所有的控件，小范围拖动，自行建布局容器包着要拖动的控件 </param>
		public void Insert(UIElement Controls, FrameworkElement LlayoutContainer)
		{
			if (!DictionaryDataList.ContainsKey(Controls))
			{
				DragControlsBase dragControlsBase = new DragControlsBase(Controls, LlayoutContainer);
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Controls);
				adornerLayer.Add(dragControlsBase);
				Tuple<AdornerLayer, DragControlsBase> tuple = new Tuple<AdornerLayer, DragControlsBase>(adornerLayer, dragControlsBase);
				DictionaryDataList.Add(Controls, tuple);
			}
		}
		/// <summary>
		/// 移除拖动
		/// </summary>
		/// <param name="Controls">控件</param>
		public void Remove(UIElement Controls)
		{
			if (DictionaryDataList.ContainsKey(Controls))
			{
				DictionaryDataList[Controls].Item1.Remove(DictionaryDataList[Controls].Item2);  //移除此项属性
				Delete(Controls);   //在集合移除此项
			}
		}
		/// <summary>
		/// 删除此项
		/// </summary>
		/// <param name="Controls">控件</param>
		private void Delete(UIElement Controls)
		{
			DictionaryDataList.Remove(Controls);  //直接移除
		}
	}
}
