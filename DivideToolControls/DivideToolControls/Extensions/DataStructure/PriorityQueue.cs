using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Extensions.DataStructure
{
	public class PriorityQueue<T, TPriority>
	{
		private readonly List<KeyValuePair<T, TPriority>> heap = new List<KeyValuePair<T, TPriority>>();

		private readonly Dictionary<T, int> indexes = new Dictionary<T, int>();

		private readonly IComparer<TPriority> comparer;

		private readonly bool invert;

		public TPriority this[T item]
		{
			get
			{
				return heap[indexes[item]].Value;
			}
			set
			{
				if (indexes.TryGetValue(item, out int value2))
				{
					int num = comparer.Compare(value, heap[value2].Value);
					if (num != 0)
					{
						if (invert)
						{
							num = ~num;
						}
						KeyValuePair<T, TPriority> element = new KeyValuePair<T, TPriority>(item, value);
						if (num < 0)
						{
							MoveUp(element, value2);
						}
						else
						{
							MoveDown(element, value2);
						}
					}
				}
				else
				{
					KeyValuePair<T, TPriority> keyValuePair = new KeyValuePair<T, TPriority>(item, value);
					heap.Add(keyValuePair);
					MoveUp(keyValuePair, Count);
				}
			}
		}

		public int Count => heap.Count - 1;

		public PriorityQueue()
			: this(invert: false)
		{
		}

		public PriorityQueue(bool invert)
			: this((IComparer<TPriority>)Comparer<TPriority>.Default)
		{
			this.invert = invert;
		}

		public PriorityQueue(IComparer<TPriority> comparer)
		{
			this.comparer = comparer;
			heap.Add(default(KeyValuePair<T, TPriority>));
		}

		public void Enqueue(T item, TPriority priority)
		{
			KeyValuePair<T, TPriority> keyValuePair = new KeyValuePair<T, TPriority>(item, priority);
			heap.Add(keyValuePair);
			MoveUp(keyValuePair, Count);
		}

		public KeyValuePair<T, TPriority> Dequeue()
		{
			int count = Count;
			if (count < 1)
			{
				throw new InvalidOperationException("Queue is empty.");
			}
			KeyValuePair<T, TPriority> result = heap[1];
			KeyValuePair<T, TPriority> element = heap[count];
			heap.RemoveAt(count);
			if (count > 1)
			{
				MoveDown(element, 1);
			}
			indexes.Remove(result.Key);
			return result;
		}

		public KeyValuePair<T, TPriority> Peek()
		{
			if (Count < 1)
			{
				throw new InvalidOperationException("Queue is empty.");
			}
			return heap[1];
		}

		public bool TryGetValue(T item, out TPriority priority)
		{
			if (indexes.TryGetValue(item, out int _))
			{
				priority = heap[indexes[item]].Value;
				return true;
			}
			priority = default(TPriority);
			return false;
		}

		private void MoveUp(KeyValuePair<T, TPriority> element, int index)
		{
			while (index > 1)
			{
				int num = index >> 1;
				if (IsPrior(heap[num], element))
				{
					break;
				}
				heap[index] = heap[num];
				indexes[heap[num].Key] = index;
				index = num;
			}
			heap[index] = element;
			indexes[element.Key] = index;
		}

		private void MoveDown(KeyValuePair<T, TPriority> element, int index)
		{
			int count = heap.Count;
			while (index << 1 < count)
			{
				int num = index << 1;
				int num2 = num | 1;
				if (num2 < count && IsPrior(heap[num2], heap[num]))
				{
					num = num2;
				}
				if (IsPrior(element, heap[num]))
				{
					break;
				}
				heap[index] = heap[num];
				indexes[heap[num].Key] = index;
				index = num;
			}
			heap[index] = element;
			indexes[element.Key] = index;
		}

		private bool IsPrior(KeyValuePair<T, TPriority> element1, KeyValuePair<T, TPriority> element2)
		{
			int num = comparer.Compare(element1.Value, element2.Value);
			if (invert)
			{
				num = ~num;
			}
			return num < 0;
		}
	}
}
