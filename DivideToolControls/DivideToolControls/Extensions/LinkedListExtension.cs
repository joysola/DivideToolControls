using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Extensions
{
	public static class LinkedListExtension
	{
		public static LinkedListNode<T> FindNext<T>(this LinkedList<T> list, LinkedListNode<T> node, T value)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (node == null)
			{
				return list.Find(value);
			}
			if (list != node.List)
			{
				throw new ArgumentException("The list does not contain the given node.");
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			node = node.Next;
			while (node != null)
			{
				try
				{
					if (value != null)
					{
						if (@default.Equals(node.Value, value))
						{
							return node;
						}
					}
					else if (node.Value == null)
					{
						return node;
					}
					node = node.Next;
				}
				catch
				{
				}
			}
			return null;
		}

		public static LinkedListNode<T> FindPrevious<T>(this LinkedList<T> list, LinkedListNode<T> node, T value)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (node == null)
			{
				return list.FindLast(value);
			}
			if (list != node.List)
			{
				throw new ArgumentException("The list does not contain the given node.");
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (node = node.Previous; node != null; node = node.Previous)
			{
				if (value != null)
				{
					if (@default.Equals(node.Value, value))
					{
						return node;
					}
				}
				else if (node.Value == null)
				{
					return node;
				}
			}
			return null;
		}
	}
}
