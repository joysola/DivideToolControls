using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DivideToolControls.Helper
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct IMAGE_INFO_STRUCT
	{
		public int DataFilePTR;
	}
}
