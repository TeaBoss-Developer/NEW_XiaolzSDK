using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static XiaolzCSharp.PInvoke;

namespace XiaolzCSharp
{
	public class Main
	{
		public static CancellationTokenSource cts = new CancellationTokenSource();
		public static CancellationToken token = CancellationToken.None;
		private long RobotQQ = 1147930373;

		#region 收到私聊消息
		public static Delegate funRecvicePrivateMsg = new RecvicePrivateMsg(RecvicetPrivateMessage);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		public delegate int RecvicePrivateMsg(ref PrivateMessageEvent sMsg);
		public static int RecvicetPrivateMessage(ref PrivateMessageEvent sMsg)
		{
			PInvoke.RobotQQ = sMsg.ThisQQ;
			API.OutputLog(plugin_key, sMsg.MessageContent, 65280, 16711680);
			return 0;
		}
		#endregion
		#region 收到群聊消息
		public static RecviceGroupMsg funRecviceGroupMsg = new RecviceGroupMsg(RecvicetGroupMessage);
		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		public delegate int RecviceGroupMsg(ref GroupMessageEvent sMsg);
		public static int RecvicetGroupMessage(ref GroupMessageEvent sMsg)
		{
			PInvoke.RobotQQ = sMsg.ThisQQ;
			switch (sMsg.MessageGroupQQ)
			{
				case 1919810:
					if (sMsg.MessageContent == "测试")
					{
						API.SendGroupMsg(plugin_key, PInvoke.RobotQQ, sMsg.MessageGroupQQ, "签到成功啦~" + API.UploadGroupImage(plugin_key, PInvoke.RobotQQ, sMsg.MessageGroupQQ, false, File.ReadAllBytes("路径"), 1), false);
						break;
					}
					break;
			}
			return 0;
		}
		#endregion
	}

}
