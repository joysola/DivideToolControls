using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace DivideToolControls.DeepZoomControls
{
	/// <summary>
	/// 虚拟化是指数据源没有完全加载，仅加载当前需要显示的数据呈现给用户
	/// </summary>
	public abstract class VirtualPanel : VirtualizingPanel
	{
		private class VirtualItemsList : IList, ICollection, IEnumerable
		{
			public IList Items
			{
				get;
				set;
			}

			public int Offset
			{
				get;
				set;
			}

			public int Count
			{
				get;
				set;
			}

			public object this[int index]
			{
				get
				{
					if (index < 0 || index >= Count)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					if (Items != null && index + Offset < Items.Count)
					{
						return Items[index + Offset];
					}
					return null;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public bool IsFixedSize => true;

			public bool IsReadOnly => true;

			public bool IsSynchronized => false;

			public object SyncRoot => null;

			public VirtualItemsList(IList items, int offset, int count)
			{
				Items = items;
				Offset = offset;
				Count = count;
			}

			public int IndexOf(object value)
			{
				if (Items != null)
				{
					for (int i = 0; i < Count && i + Offset < Items.Count; i++)
					{
						if (object.Equals(Items[i + Offset], value))
						{
							return i;
						}
					}
				}
				return -1;
			}

			public bool Contains(object value)
			{
				return IndexOf(value) >= 0;
			}

			public IEnumerator GetEnumerator()
			{
				if (Items != null)
				{
					for (int i = 0; i < Count && i + Offset < Items.Count; i++)
					{
						yield return Items[i + Offset];
					}
				}
			}

			public int Add(object value)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			public void Remove(object value)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public void CopyTo(Array array, int index)
			{
				throw new NotSupportedException();
			}
		}

		public new static readonly DependencyProperty IsVirtualizingProperty = VirtualizingStackPanel.IsVirtualizingProperty.AddOwner(typeof(VirtualPanel), new FrameworkPropertyMetadata(VirtualizingStackPanel.IsVirtualizingProperty.DefaultMetadata.DefaultValue, OnIsVirtualizingChanged));

		public static readonly DependencyProperty RealizationPriorityProperty = DependencyProperty.Register("RealizationPriority", typeof(DispatcherPriority), typeof(VirtualPanel), new FrameworkPropertyMetadata(DispatcherPriority.Normal));

		private static readonly DependencyProperty IndexForItemContainerProperty = DependencyProperty.RegisterAttached("IndexForItemContainer", typeof(int), typeof(VirtualPanel), new FrameworkPropertyMetadata(-1));

		public bool IsVirtualizing
		{
			get
			{
				return (bool)GetValue(IsVirtualizingProperty);
			}
			set
			{
				SetValue(IsVirtualizingProperty, value);
			}
		}

		public DispatcherPriority RealizationPriority
		{
			get
			{
				return (DispatcherPriority)GetValue(RealizationPriorityProperty);
			}
			set
			{
				SetValue(RealizationPriorityProperty, value);
			}
		}

		private DispatcherOperation RealizeOperation
		{
			get;
			set;
		}

		public ItemsControl ItemsOwner => ItemsControl.GetItemsOwner(this);

		private static void OnIsVirtualizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as VirtualPanel)?.OnIsVirtualizingChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		public int IndexFromContainer(UIElement container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator as ItemContainerGenerator;
			if (itemContainerGenerator != null && itemContainerGenerator.ItemFromContainer(container) != DependencyProperty.UnsetValue)
			{
				object obj = container.ReadLocalValue(IndexForItemContainerProperty);
				return (obj as int?) ?? (-1);
			}
			return -1;
		}

		public object ItemFromContainer(UIElement container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator as ItemContainerGenerator;
			if (itemContainerGenerator != null)
			{
				object obj = itemContainerGenerator.ItemFromContainer(container);
				if (obj == DependencyProperty.UnsetValue)
				{
					return null;
				}
				return obj;
			}
			return null;
		}

		public UIElement ContainerFromIndex(int itemIndex)
		{
			ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator as ItemContainerGenerator;
			if (itemContainerGenerator != null)
			{
				return itemContainerGenerator.ContainerFromIndex(itemIndex) as UIElement;
			}
			return null;
		}

		public UIElement ContainerFromItem(object item)
		{
			ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator as ItemContainerGenerator;
			if (itemContainerGenerator != null)
			{
				return itemContainerGenerator.ContainerFromItem(item) as UIElement;
			}
			return null;
		}

		public void InvalidateReality()
		{
			if (RealizeOperation != null)
			{
				RealizeOperation.Abort();
			}
			object state = null;
			Action action = null;
			action = delegate
			{
				RealizeOperation = null;
				state = RealizeCore(state);
				if (state != null && RealizeOperation == null)
				{
					RealizeOperation = base.Dispatcher.BeginInvoke(action, RealizationPriority);
				}
			};
			RealizeOperation = base.Dispatcher.BeginInvoke(action, RealizationPriority);
		}

		public void UpdateReality()
		{
			RealizeOperation.Abort();
			RealizeOperation = null;
			object obj = null;
			do
			{
				obj = RealizeCore(obj);
			}
			while (obj != null);
		}

		private object RealizeCore(object state)
		{
			if (base.IsItemsHost)
			{
				if (IsVirtualizing)
				{
					ItemsControl itemsOwner = ItemsOwner;
					if (itemsOwner != null)
					{
						return RealizeOverride(itemsOwner.ItemsSource ?? itemsOwner.Items, state);
					}
				}
				else if (base.InternalChildren.Count == 0)
				{
					IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
					using (itemContainerGenerator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
					{
						int num = 0;
						DependencyObject dependencyObject;
						while ((dependencyObject = itemContainerGenerator.GenerateNext()) != null)
						{
							UIElement uIElement = dependencyObject as UIElement;
							if (uIElement != null)
							{
								uIElement.SetValue(IndexForItemContainerProperty, num);
								AddInternalChild(uIElement);
								itemContainerGenerator.PrepareItemContainer(dependencyObject);
							}
							num++;
						}
					}
				}
			}
			return null;
		}

		protected abstract object RealizeOverride(IEnumerable items, object state);

		protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
		{
			base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
			InvalidateReality();
		}

		protected virtual void OnIsVirtualizingChanged(bool oldIsVirtualizing, bool newIsVirtualizing)
		{
			InvalidateReality();
		}

		protected sealed override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			ItemCollection itemCollection = ItemsOwner?.Items;
			NotifyCollectionChangedAction action = args.Action;
			if (action == NotifyCollectionChangedAction.Reset)
			{
				OnItemsChanged(itemCollection, new NotifyCollectionChangedEventArgs(action));
			}
			else if (itemCollection != null)
			{
				IItemContainerGenerator itemContainerGenerator = sender as IItemContainerGenerator;
				if (itemContainerGenerator != null)
				{
					switch (action)
					{
						case NotifyCollectionChangedAction.Add:
							{
								int num4 = itemContainerGenerator.IndexFromGeneratorPosition(args.Position);
								if (args.Position.Offset == 1)
								{
									num4 = itemCollection.Count - 1;
								}
								VirtualItemsList changedItems2 = new VirtualItemsList(itemCollection, num4, args.ItemCount);
								if (!IsVirtualizing)
								{
									for (int k = num4; k < num4 + args.ItemCount; k++)
									{
										RealizeItem(k);
									}
								}
								OnItemsChanged(itemCollection, new NotifyCollectionChangedEventArgs(action, changedItems2, num4));
								break;
							}
						case NotifyCollectionChangedAction.Remove:
							{
								int num5 = itemContainerGenerator.IndexFromGeneratorPosition(args.Position);
								ArrayList arrayList = new ArrayList(args.ItemCount);
								for (int l = 0; l < args.ItemUICount; l++)
								{
									UIElement uIElement = base.InternalChildren[args.Position.Index + l];
									arrayList.Add(ItemFromContainer(uIElement));
									if (num5 == -1)
									{
										num5 = (int)uIElement.ReadLocalValue(IndexForItemContainerProperty);
									}
									uIElement.ClearValue(IndexForItemContainerProperty);
								}
								if (args.ItemUICount > 0)
								{
									RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
								}
								OnItemsChanged(itemCollection, new NotifyCollectionChangedEventArgs(action, arrayList, num5));
								break;
							}
						case NotifyCollectionChangedAction.Move:
							{
								int num2 = -1;
								int num3 = itemContainerGenerator.IndexFromGeneratorPosition(args.Position);
								VirtualItemsList changedItems = new VirtualItemsList(itemCollection, num3, args.ItemCount);
								int itemUICount = args.ItemUICount;
								if (itemUICount > 0)
								{
									UIElement[] array = new UIElement[itemUICount];
									for (int i = 0; i < itemUICount; i++)
									{
										array[i] = base.InternalChildren[args.OldPosition.Index + i];
										if (num2 == -1)
										{
											num2 = (int)array[i].ReadLocalValue(IndexForItemContainerProperty);
										}
									}
									RemoveInternalChildRange(args.OldPosition.Index + Math.Min(args.OldPosition.Offset, 1), itemUICount);
									for (int j = 0; j < itemUICount; j++)
									{
										InsertInternalChild(args.Position.Index + j, array[j]);
									}
								}
								OnItemsChanged(itemCollection, new NotifyCollectionChangedEventArgs(action, changedItems, num3, num2));
								break;
							}
						case NotifyCollectionChangedAction.Replace:
							{
								int num = itemContainerGenerator.IndexFromGeneratorPosition(args.Position);
								VirtualItemsList newItems = new VirtualItemsList(itemCollection, num, args.ItemCount);
								VirtualItemsList oldItems = new VirtualItemsList(null, num, args.ItemCount);
								OnItemsChanged(itemCollection, new NotifyCollectionChangedEventArgs(action, newItems, oldItems, num));
								break;
							}
					}
				}
			}
			base.OnItemsChanged(sender, args);
			InvalidateReality();
		}

		protected virtual void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
		}
		/// <summary>
		/// 将瓦片图放入InternalChildren
		/// </summary>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		public UIElement RealizeItem(int itemIndex)
		{
			IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
			GeneratorPosition position = itemContainerGenerator.GeneratorPositionFromIndex(itemIndex);
			using (itemContainerGenerator.StartAt(position, GeneratorDirection.Forward, allowStartAtRealizedItem: true))
			{
				bool isNewlyRealized = false;
				UIElement uIElement = itemContainerGenerator.GenerateNext(out isNewlyRealized) as UIElement;
				if (position.Offset != 0 && uIElement != null && isNewlyRealized)
				{
					uIElement.SetValue(IndexForItemContainerProperty, itemIndex);
					InsertInternalChild(position.Index + 1, uIElement);
					itemContainerGenerator.PrepareItemContainer(uIElement);
				}
				return uIElement;
			}
		}

		public void VirtualizeItem(int itemIndex)
		{
			IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
			if (itemContainerGenerator != null)
			{
				GeneratorPosition position = itemContainerGenerator.GeneratorPositionFromIndex(itemIndex);
				if (position.Offset == 0)
				{
					itemContainerGenerator.Remove(position, 1);
					base.InternalChildren[position.Index].ClearValue(IndexForItemContainerProperty);
					RemoveInternalChildRange(position.Index, 1);
				}
			}
		}
	}
}
