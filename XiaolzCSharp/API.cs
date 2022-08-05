using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;


namespace XiaolzCSharp
{
	public class API
	{
		public static Dictionary<long, Tuple<long, string, long, uint>> EventDics = new Dictionary<long, Tuple<long, string, long, uint>>();
		public static bool MsgRecod=false;

		#region 导出函数给框架并取到两个参数值
		[DllExport(CallingConvention = CallingConvention.StdCall)]
		public static IntPtr apprun([MarshalAs(UnmanagedType.LPStr)] string apidata, [MarshalAs(UnmanagedType.LPStr)] string pluginkey)
		{
			PInvoke.jsonstr = apidata;
			PInvoke.plugin_key = pluginkey;

			var json = "";
			Dictionary<string, string> JosnDict = (new JavaScriptSerializer()).Deserialize<Dictionary<string, string>>(apidata);
			foreach (KeyValuePair<string, string> KeyList in JosnDict)
			{
				json = AddPermission(KeyList.Key, json);
			}
			//json = AddPermission("输出日志", json)
			//json = AddPermission("发送好友消息", json)
			//json = AddPermission("查询好友信息", json)
			//json = AddPermission("发送群消息", json)
			//json = AddPermission("取图片下载地址", json)
			//json = AddPermission("取好友列表", json)
			//json = AddPermission("取群成员列表", json)
			//json = AddPermission("取群列表", json)
			//json = AddPermission("取框架QQ", json)
			//json = AddPermission("框架重启", json)

			object jsonkey = (new JavaScriptSerializer()).DeserializeObject(json);
			string resultJson = (new JavaScriptSerializer()).Serialize(new { needapilist = jsonkey });

			var App_Info = new PInvoke.AppInfo();
			App_Info.data = (new JavaScriptSerializer()).Deserialize<object>(resultJson);

			App_Info.sdkv = "2.8.7.5";
			App_Info.appname = "群管理";
			App_Info.author = "VAPE";
			App_Info.describe = "群管插件";
			App_Info.appv = "1.0.0";
			GC.KeepAlive(appEnableFunc);
			App_Info.useproaddres = Marshal.GetFunctionPointerForDelegate(appEnableFunc).ToInt64();
			GC.KeepAlive(AppDisabledEvent);
			App_Info.banproaddres = Marshal.GetFunctionPointerForDelegate(AppDisabledEvent).ToInt64();
			GC.KeepAlive(AppSettingEvent);
			App_Info.setproaddres = Marshal.GetFunctionPointerForDelegate(AppSettingEvent).ToInt64();
			GC.KeepAlive(AppUninstallEvent);
			App_Info.unitproaddres = Marshal.GetFunctionPointerForDelegate(AppUninstallEvent).ToInt64();
			GC.KeepAlive(Main.funRecvicePrivateMsg);
			App_Info.friendmsaddres = Marshal.GetFunctionPointerForDelegate(Main.funRecvicePrivateMsg).ToInt64();
			GC.KeepAlive(Main.funRecviceGroupMsg);
			App_Info.groupmsaddres = Marshal.GetFunctionPointerForDelegate(Main.funRecviceGroupMsg).ToInt64();
			GC.KeepAlive(funEvent);
			App_Info.banproaddres = Marshal.GetFunctionPointerForDelegate(AppDisabledEvent).ToInt64();
			string jsonstring = (new JavaScriptSerializer()).Serialize(App_Info);
			return Marshal.StringToHGlobalAnsi(jsonstring);

		}
		public static string AddPermission(string desc, string json)
		{
			var jsonstring = "";
			string SensitivePermissions = "";
			if (SensitivePermissions.Contains(desc))
			{
				var Permission = new PInvoke.MyData
                {
					PermissionList = new PInvoke.Needapilist
					{
						state = "0",
						safe = "0",
						desc = desc
					}
				};
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				jsonstring = serializer.Serialize(Permission).Replace("PermissionList", desc);
			}
			else
			{
				var Permission = new PInvoke.MyData
                {
					PermissionList = new PInvoke.Needapilist
					{
						state = "1",
						safe = "1",
						desc = desc
					}
				};
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				jsonstring = serializer.Serialize(Permission).Replace("PermissionList", desc);
			}
			if (json == "")
			{
				return jsonstring;
			}
			else
			{
				return (json + jsonstring).Replace("}{", ",");
			}
		}
		#endregion

		#region 插件启动	
		public static DelegateAppEnable appEnableFunc = new DelegateAppEnable(appEnable);
		public delegate int DelegateAppEnable();
		public static int appEnable()		
		{
			Application.ThreadException += Application_ThreadException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			InitFunction();
			//string res = CallGetLoginQQ();
			string sqlite3path = System.Environment.CurrentDirectory + "\\main\\corn\\sqlite3.dll";
			return 1;
		}
		#endregion	
		#region 插件卸载		
		public static DelegateAppUnInstall AppUninstallEvent = new DelegateAppUnInstall(AppUnInstall);
		public delegate int DelegateAppUnInstall();
		public static int AppUnInstall()
		{
			
			return 1;
		}

		#endregion
		#region 插件禁用
		public static DelegateAppDisabled AppDisabledEvent = new DelegateAppDisabled(appDisable);
		public delegate int DelegateAppDisabled();
		public static int appDisable()
		{
			return 1;
		}
		#endregion
		#region 取框架QQ
		public static string CallGetLoginQQ()
		{
			string RetJson =Marshal.PtrToStringAnsi( GetLoginQQ(PInvoke.plugin_key));
			try
			{
				dynamic root = new JavaScriptSerializer().Deserialize<Dictionary<string, Dictionary<string, object>>>(RetJson);
				var QQlist = root[root.Keys[0]];
				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());
			}		
			PInvoke.PluginStatus = true;
			return "";
		}
		#endregion
		#region 插件设置
		public static DelegateAppSetting AppSettingEvent = new DelegateAppSetting(AppSetting);
		public delegate void DelegateAppSetting();
		public static void AppSetting()
		{
			
		}
		#endregion
		#region 插件事件
		public static DelegatefunEvent funEvent = new DelegatefunEvent(OnEvent);
		public delegate void DelegatefunEvent(ref PInvoke.EventTypeBase EvenType);
		public static void OnEvent(ref PInvoke.EventTypeBase EvenType)
		{			
			switch (EvenType.EventType)
			{
				case PInvoke.EventTypeEnum.This_SignInSuccess:
					Console.WriteLine("登录成功");
					
					break;
				case PInvoke.EventTypeEnum.Friend_NewFriend:
					Console.WriteLine("有新好友");
					break;
				case PInvoke.EventTypeEnum.Friend_FriendRequest:
					Console.WriteLine("好友请求");					
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + EvenType.TriggerQQName + "(" + EvenType.TriggerQQ.ToString() + ")欲加机器人为好友,发送了这样的消息:" + EvenType.MessageContent + ",是否同意?", false);
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + Environment.NewLine + GetFriendData(EvenType.ThisQQ, EvenType.TriggerQQ), false);
					if (EventDics.ContainsKey(EvenType.TriggerQQ) == false)
						EventDics.Add(EvenType.TriggerQQ, new Tuple<long, string, long, uint>(EvenType.SourceGroupQQ, EvenType.TriggerQQName, EvenType.MessageSeq, EvenType.EventSubType));
					break;
				case PInvoke.EventTypeEnum.Friend_FriendRequestAccepted:
					Console.WriteLine("对方同意了您的好友请求");
					break;
				case PInvoke.EventTypeEnum.Friend_FriendRequestRefused:
					Console.WriteLine("对方拒绝了您的好友请求");
					break;
				case PInvoke.EventTypeEnum.Friend_Removed:
					Console.WriteLine("被好友删除");
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + EvenType.TriggerQQName + "(" + EvenType.TriggerQQ.ToString() + " ) 将机器人删除", false);
					break;
				case PInvoke.EventTypeEnum.Friend_Blacklist:
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + EvenType.TriggerQQName + "(" + EvenType.TriggerQQ.ToString() + " ) 将机器人加入黑名单", false);
					break;
				case PInvoke.EventTypeEnum.Group_MemberVerifying:
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + Environment.NewLine + EvenType.TriggerQQName + "(" + EvenType.TriggerQQ.ToString() + " ) 想加入群: " + EvenType.SourceGroupName + "(" + EvenType.SourceGroupQQ.ToString() + " )", false);
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + Environment.NewLine + GetFriendData(EvenType.ThisQQ, EvenType.TriggerQQ), false);
					if (EventDics.ContainsKey(EvenType.TriggerQQ) == false)
						EventDics.Add(EvenType.TriggerQQ, new Tuple<long, string, long, uint>(EvenType.SourceGroupQQ, EvenType.TriggerQQName, EvenType.MessageSeq, (uint)EvenType.EventType));
					break;
				case PInvoke.EventTypeEnum.Group_Invited:
					Console.WriteLine("我被邀请加入群");
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + Environment.NewLine + EvenType.OperateQQName+ "(" + EvenType.OperateQQ.ToString() + " ) 想邀请机器人进群: " + EvenType.SourceGroupName + "(" + EvenType.SourceGroupQQ.ToString() + " )", false);
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + Environment.NewLine + GetGroupData(EvenType.ThisQQ, EvenType.SourceGroupQQ), false);
					if (EventDics.ContainsKey(EvenType.TriggerQQ) == false)
						EventDics.Add(EvenType.SourceGroupQQ, new Tuple<long, string, long, uint>(EvenType.SourceGroupQQ, EvenType.OperateQQName, EvenType.MessageSeq, EvenType.EventSubType));
					break;
				case PInvoke.EventTypeEnum.Group_MemberJoined:
					Console.WriteLine("某人加入了群");
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, EvenType.SourceGroupQQ, "[@" + EvenType.TriggerQQ.ToString() + "]" + EvenType.TriggerQQName + ",欢迎你加入本群!", false);
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, PInvoke.FeedbackGroup, "[@"+ PInvoke.MasterQQ + "]" + Environment.NewLine + GetFriendData(EvenType.ThisQQ, EvenType.TriggerQQ), false);
					break;
				case PInvoke.EventTypeEnum.Group_MemberQuit:
					Console.WriteLine("某人退出了群");
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, EvenType.SourceGroupQQ, EvenType.TriggerQQName + "已退出本群!", false);
					break;
				case PInvoke.EventTypeEnum.Group_MemberUndid:
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, EvenType.SourceGroupQQ, EvenType.TriggerQQName + "(" + EvenType.TriggerQQ.ToString() + " ) 撤回了一条消息,内容如下:" + EvenType.MessageContent, false);
					break;
				case PInvoke.EventTypeEnum.Group_MemberInvited:
					Console.WriteLine("某人被邀请入群");

					break;
				case PInvoke.EventTypeEnum.Group_AllowUploadFile:
					Console.WriteLine("群事件_允许上传群文件");
					break;
				case PInvoke.EventTypeEnum.Group_ForbidUploadFile:
					Console.WriteLine("群事件_禁止上传群文件");
					break;
				case PInvoke.EventTypeEnum.Group_AllowUploadPicture:
					Console.WriteLine("群事件_允许上传相册");
					break;
				case PInvoke.EventTypeEnum.Group_ForbidUploadPicture:
					Console.WriteLine("群事件_禁止上传相册");
					break;
				case PInvoke.EventTypeEnum.Group_MemberKickOut:
					API.SendGroupMsg(PInvoke.plugin_key, EvenType.ThisQQ, EvenType.SourceGroupQQ, "你已被提出了群:" + EvenType.SourceGroupName + "(" + EvenType.SourceGroupQQ.ToString() + ")", false);
					break;
				default:
					Console.WriteLine(EvenType.EventType.ToString());
					break;
			}
		}
		#endregion
		#region 发送好友图片
		public string SendFriendImage(long thisQQ, long friendQQ, string picpath, bool is_flash)
		{
			Bitmap bitmap = new Bitmap(picpath);
			byte[] picture = GetByteArrayByImage(bitmap);
			IntPtr piccode = UploadFriendImage(PInvoke.plugin_key, thisQQ, friendQQ, is_flash, picture, picture.Length);
			long MessageRandom = 0;
			uint MessageReq = 0;
			IntPtr res= SendPrivateMsg(PInvoke.plugin_key, thisQQ, friendQQ, Marshal.PtrToStringAnsi(piccode), ref MessageRandom, ref MessageReq);
			return Marshal.PtrToStringAnsi(res);
		}
		public static byte[] GetByteArrayByImage(Bitmap bitmap)
		{
			byte[] result = null;
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Jpeg);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, (int)memoryStream.Length);
				memoryStream.Close();
				result = array;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		#endregion
		#region 发送群图片
		public string SendGroupImage(long thisQQ, long groupQQ, string picpath, bool is_flash)
		{
			Bitmap bitmap = new Bitmap(picpath);
			byte[] picture = GetByteArrayByImage(bitmap);
			IntPtr piccode = UploadGroupImage(PInvoke.plugin_key, thisQQ, groupQQ, is_flash, picture, picture.Length);
			IntPtr res=SendGroupMsg(PInvoke.plugin_key, thisQQ, groupQQ, Marshal.PtrToStringAnsi(piccode), false);
			return Marshal.PtrToStringAnsi(res);
		}
		#endregion
		#region 获取图片地址		
		public static string GetImageLink(long thisQQ, long sendQQ, long groupQQ, string ImgGuid)
		{
			var ImgUrl = GetImageDownloadLink(PInvoke.plugin_key, ImgGuid, thisQQ, groupQQ);
			if (groupQQ == 0)
			{
				long MessageRandom = 0;
				uint MessageReq = 0;
				SendPrivateMsg(PInvoke.plugin_key, thisQQ, sendQQ, "图片地址为:" + ImgUrl + "\r\n",ref MessageRandom,ref MessageReq);
			}
			else
			{
				SendGroupMsg(PInvoke.plugin_key, thisQQ, groupQQ, "图片地址为:" + ImgUrl + "\r\n",false);
			}
			return "";
		}
		#endregion
		#region 取好友列表		
		public static int GetFriendLists(long thisQQ, long sendQQ)
		{
			PInvoke.DataArray[] ptrArray = new PInvoke.DataArray[2];
			int count = GetFriendList(PInvoke.plugin_key, thisQQ, ref ptrArray);
			if (count > 0)
			{
				List<string> list = new List<string>();
				byte[] pAddrBytes = ptrArray[0].pAddrList;
				for (int i = 0; i < count; i++)
				{
					byte[] readByte = new byte[4];
					Array.Copy(pAddrBytes, i * 4, readByte, 0, readByte.Length);
					IntPtr StuctPtr = new IntPtr(BitConverter.ToInt32(readByte, 0));
					PInvoke.FriendInfo info = (PInvoke.FriendInfo)Marshal.PtrToStructure(StuctPtr, typeof(PInvoke.FriendInfo));
					list.Add(info.QQNumber.ToString() + "-" + info.Name);
				}
				long MessageRandom = 0;
				uint MessageReq = 0;
				SendPrivateMsg(PInvoke.plugin_key, thisQQ, sendQQ, "好友列表:" + "\r\n" + string.Join("\r\n", list), ref MessageRandom, ref MessageReq);
			}
			return count;
		}
		#endregion
		#region 查询好友信息
		public static string GetFriendData(long thisQQ, long otherQQ)
		{
			string res = "";
			PInvoke.GetFriendDataInfo[] pFriendInfo = new PInvoke.GetFriendDataInfo[2];
			if (GetFriendInfo(PInvoke.plugin_key, thisQQ, otherQQ, ref pFriendInfo) == true)
			{
				res= new JavaScriptSerializer().Serialize(pFriendInfo[0].friendInfo);
				dynamic result = new JavaScriptSerializer().DeserializeObject(res);
				string Gender = "";
				if (result["Gender"] == 1)
					Gender = "女";
				else if (result["Gender"] == 2)
					Gender = "男";
				else
					Gender = "未知";
				return "QQ资料信息: " + Environment.NewLine + "昵称: " + result["Name"] + Environment.NewLine + "性别: " + Gender + Environment.NewLine + "年龄: " + result["Age"] + Environment.NewLine + "等级: " + result["Level"] + Environment.NewLine + "国籍: " + result["Nation"] + Environment.NewLine + "签名: " + result["Signature"];
					
			}
			return res;
		}
		#endregion
		#region 取群成员列表
		public static int GetGroupMemberlists(long thisQQ, long groupQQ)
		{
			PInvoke.DataArray[] ptrArray = new PInvoke.DataArray[2];			
			int count = GetGroupMemberlist(PInvoke.plugin_key, thisQQ, groupQQ, ref ptrArray);
			if (count > 0)
			{
				List<string> list = new List<string>();
				{
					PInvoke.ReadBytes = new byte[ptrArray[0].pAddrList.Length];
					ptrArray[0].pAddrList.CopyTo(PInvoke.ReadBytes, 0);
					for (int i = 0; i < count; i++)
					{
						PInvoke.readbyte = new byte[4];
						Array.Copy(PInvoke.ReadBytes, i * 4, PInvoke.readbyte, 0, PInvoke.readbyte.Length);
						PInvoke.pStruct = new IntPtr(BitConverter.ToInt32(PInvoke.readbyte, 0));
						try
                        {
							PInvoke.GroupMemberInfo info = (PInvoke.GroupMemberInfo)Marshal.PtrToStructure(PInvoke.pStruct, typeof(PInvoke.GroupMemberInfo));
							list.Add(info.QQNumber + "-" + info.Name);
						}
						catch
                        {
							break;
                        }						
					}
				}				
				SendGroupMsg(PInvoke.plugin_key, thisQQ, groupQQ, "群成员列表:" + "\r\n" + string.Join("\r\n", list), false);	
			}
			return count;
		}
		#endregion
		#region 取群列表
		public static int GetGroupLists(long thisQQ, long groupQQ)
		{
			PInvoke.DataArray[] ptrArray = new PInvoke.DataArray[2];
			int count = GetGroupList(PInvoke.plugin_key, thisQQ, ref ptrArray);
			if (count > 0)
			{
				List<string> list = new List<string>();
				byte[] pAddrBytes = ptrArray[0].pAddrList;
				for (int i = 0; i < count; i++)
				{
					byte[] readByte = new byte[4];
					Array.Copy(pAddrBytes, i * 4, readByte, 0, readByte.Length);
					IntPtr StuctPtr = new IntPtr(BitConverter.ToInt32(readByte, 0));
					PInvoke.GroupInfo info = (PInvoke.GroupInfo)Marshal.PtrToStructure(StuctPtr, typeof(PInvoke.GroupInfo));
					list.Add(info.GroupID.ToString() + "-" + info.GroupName);
				}
				SendGroupMsg(PInvoke.plugin_key, thisQQ, groupQQ, "群列表:" + "\r\n" + string.Join("\r\n", list),false);
			}
			return count;
		}
		#endregion
		#region 取管理列表
		public static string[] GetAdministratorLists(long thisQQ, long gruopNumber)
		{		
			string ret =Marshal.PtrToStringAnsi(GetAdministratorList(PInvoke.plugin_key, thisQQ, gruopNumber));
			string[] adminlist = ret.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			Array.Resize(ref adminlist, adminlist.Length + 1);
			adminlist[adminlist.Length - 1] = PInvoke.MasterQQ;
			return adminlist;
		}
		#endregion
		#region 查询群信息
		public static string GetGroupData(long thisQQ, long otherGroupQQ)
		{
			string res = "";
			PInvoke.GroupCardInfoDatList[] pGroupInfo = new PInvoke.GroupCardInfoDatList[2];
			if (GetGroupInfo(PInvoke.plugin_key, thisQQ, otherGroupQQ, ref pGroupInfo))
			{
				PInvoke.GroupCardInfo groupinfo = pGroupInfo[0].groupCardInfo;
				return "该群信息: " + Environment.NewLine + "群名称: " + groupinfo.GroupName + Environment.NewLine + "群介绍: " + groupinfo.GroupDescription ;
			}
			return res;
		}
		#endregion
		#region 取群成员简略信息
		public static PInvoke.GroupMemberBriefInfo GetGroupMemberBriefInfoEvent(long thisQQ, long GroupQQ)
		{
            PInvoke.GMBriefDataList[] gMBriefDataLists = new PInvoke.GMBriefDataList[2];
			string ret = Marshal.PtrToStringAnsi(GetGroupMemberBriefInfo(PInvoke.plugin_key, thisQQ, GroupQQ,ref gMBriefDataLists));
            PInvoke.AdminListDataList adminList = (PInvoke.AdminListDataList)Marshal.PtrToStructure(gMBriefDataLists[0].groupMemberBriefInfo.AdminiList, typeof(PInvoke.AdminListDataList));
            PInvoke.GroupMemberBriefInfo groupMemberBriefInfo = new PInvoke.GroupMemberBriefInfo();
			groupMemberBriefInfo.GroupMAax = gMBriefDataLists[0].groupMemberBriefInfo.GroupMAax;
			groupMemberBriefInfo.GroupOwner = gMBriefDataLists[0].groupMemberBriefInfo.GroupOwner;
			groupMemberBriefInfo.GruoupNum = gMBriefDataLists[0].groupMemberBriefInfo.GruoupNum;
			return groupMemberBriefInfo;
		}
		#endregion
		#region 取QQ钱包个人信息
		public static string GetQQWalletPersonalInformationEvent(long thisQQ)
		{
			var ptr = Marshal.AllocHGlobal(4);
            PInvoke.CardInformation CardInfo = new PInvoke.CardInformation();
			Marshal.StructureToPtr(CardInfo, ptr, false);
            PInvoke.CardListIntptr[] ptrCardList = new PInvoke.CardListIntptr[1];
			ptrCardList[0].addr = ptr;

            PInvoke.QQWalletInformation QQWalletInfo = new PInvoke.QQWalletInformation();
			QQWalletInfo.balance = "";
			QQWalletInfo.realname = "";
			QQWalletInfo.id = "";
			QQWalletInfo.cardlist = ptrCardList;

            PInvoke.QQWalletDataList[] QQWallet = new PInvoke.QQWalletDataList[1];
			QQWallet[0].QQWalletInfo = QQWalletInfo;

			IntPtr ret = new IntPtr();
			try
			{
				ret = GetQQWalletPersonalInfo(PInvoke.plugin_key, thisQQ, ref QQWallet);
				Marshal.FreeHGlobal(ptr);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message.ToString());         
	     	}
            PInvoke.DataArray DataList = (PInvoke.DataArray)Marshal.PtrToStructure(QQWallet[0].QQWalletInfo.cardlist[0].addr, typeof(PInvoke.DataArray));
			List<PInvoke.CardInformation> list = new List<PInvoke.CardInformation>();
			if (DataList.Amount > 0)
			{
				byte[] pAddrBytes = DataList.pAddrList;
				for (int i = 0; i < DataList.Amount; i++)
				{
					byte[] readByte = new byte[4];
					Array.Copy(pAddrBytes, i * 4, readByte, 0, readByte.Length);
					IntPtr StuctPtr = new IntPtr(BitConverter.ToInt32(readByte, 0));
                    PInvoke.CardInformation info = (PInvoke.CardInformation)Marshal.PtrToStructure(StuctPtr, typeof(PInvoke.CardInformation));
					list.Add(info);
				}
			}
			//Return retQQWalletInformation
			return Marshal.PtrToStringAnsi(ret);
		}
		#endregion
		#region 取群文件列表	
		public delegate string GetGroupFileLists(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string folder, ref PInvoke.GroupFileInfoDataList[] groupFileInfoDataLists);
		public List<PInvoke.GroupFileInformation> GetGroupFileListEvent(long thisQQ, long groupQQ, string folder)
		{

			PInvoke.GroupFileInfoDataList[] pdatalist = new PInvoke.GroupFileInfoDataList[2];
			GetGroupFileList(PInvoke.plugin_key, thisQQ, groupQQ, folder, ref pdatalist);
			if (pdatalist[0].Amount > 0)
			{
				List<PInvoke.GroupFileInformation> list = new List<PInvoke.GroupFileInformation>();
				int i = 0;
				while (i < pdatalist[0].Amount)
				{
					byte[] recbyte = new byte[4];
					Array.Copy(pdatalist[0].pAddrList, i * 4, recbyte, 0, recbyte.Length);
					IntPtr pStruct = new IntPtr(BitConverter.ToInt32(recbyte, 0));
					PInvoke.GroupFileInformation gf = (PInvoke.GroupFileInformation)Marshal.PtrToStructure(pStruct, typeof(PInvoke.GroupFileInformation));
					list.Add(gf);
					i += 1;
				}
				return list;
			}
			return null;
		}
		#endregion

		#region 初始化传入的函数指针
		public static void InitFunction()
		{
			//Dictionary<String, int> jsonDic = new JavaScriptSerializer().Deserialize<Dictionary<String, int>> (jsonstr);
			dynamic json = new JavaScriptSerializer().DeserializeObject(PInvoke.jsonstr);
			RestartDelegate ReStartAPI = (RestartDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["框架重启"]), typeof(RestartDelegate));
			restart = ReStartAPI;
			GC.KeepAlive(restart);
			GetLoginQQDelegate GetLoginQQAPI = (GetLoginQQDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取框架QQ"]), typeof(GetLoginQQDelegate));
			GetLoginQQ = GetLoginQQAPI;
			GC.KeepAlive(GetLoginQQ);
			SendPrivateMsgDelegate SendPrivateMsgAPI = (SendPrivateMsgDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["发送好友消息"]), typeof(SendPrivateMsgDelegate));
			SendPrivateMsg = SendPrivateMsgAPI;
			GC.KeepAlive(SendPrivateMsg);
			SendGroupMsgDelegate SendGroupMsgAPI = (SendGroupMsgDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["发送群消息"]), typeof(SendGroupMsgDelegate));
			SendGroupMsg = SendGroupMsgAPI;
			GC.KeepAlive(SendGroupMsg);
			FriendverificationEventDelegate FriendverificationEventAPI = (FriendverificationEventDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["处理好友验证事件"]), typeof(FriendverificationEventDelegate));
			FriendverificationEvent = FriendverificationEventAPI;
			GC.KeepAlive(FriendverificationEvent);
			GroupVerificationEventDelegate GroupVerificationEventAPI = (GroupVerificationEventDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["处理群验证事件"]), typeof(GroupVerificationEventDelegate));
			GroupVerificationEvent = GroupVerificationEventAPI;
			GC.KeepAlive(GroupVerificationEvent);
			UploadFriendImageDelegate UploadFriendImageAPI = (UploadFriendImageDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["上传好友图片"]), typeof(UploadFriendImageDelegate));
			UploadFriendImage = UploadFriendImageAPI;
			GC.KeepAlive(UploadFriendImage);
			UploadGroupImageDelegate UploadGroupImageAPI = (UploadGroupImageDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["上传群图片"]), typeof(UploadGroupImageDelegate));
			UploadGroupImage = UploadGroupImageAPI;
			GC.KeepAlive(UploadGroupImage);
			UploadFriendAudioDelegate UploadFriendAudioAPI = (UploadFriendAudioDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["上传好友语音"]), typeof(UploadFriendAudioDelegate));
			UploadFriendAudio = UploadFriendAudioAPI;
			GC.KeepAlive(UploadFriendAudio);
			UploadGroupAudioDelegate UploadGroupAudioAPI = (UploadGroupAudioDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["上传群语音"]), typeof(UploadGroupAudioDelegate));
			UploadGroupAudio = UploadGroupAudioAPI;
			GC.KeepAlive(UploadGroupAudio);
			GetImageDownloadLinkDelegate GetImageDownloadLinkAPI = (GetImageDownloadLinkDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取图片下载地址"]), typeof(GetImageDownloadLinkDelegate));
			GetImageDownloadLink = GetImageDownloadLinkAPI;
			GC.KeepAlive(GetImageDownloadLink);
			GetFriendListDelegate GetFriendListAPI = (GetFriendListDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取好友列表"]), typeof(GetFriendListDelegate));
			GetFriendList = GetFriendListAPI;
			GC.KeepAlive(GetFriendList);
			GetFriendInfoDelegate GetFriendInfoAPI = (GetFriendInfoDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["查询好友信息"]), typeof(GetFriendInfoDelegate));
			GetFriendInfo = GetFriendInfoAPI;
			GC.KeepAlive(GetFriendInfo);
			GetGroupMemberlistDelegate GetGroupMemberlistAPI = (GetGroupMemberlistDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取群成员列表"]), typeof(GetGroupMemberlistDelegate));
			GetGroupMemberlist = GetGroupMemberlistAPI;
			GC.KeepAlive(GetGroupMemberlist);
			GetGroupListDelegate GetGroupListAPI = (GetGroupListDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取群列表"]), typeof(GetGroupListDelegate));
			GetGroupList = GetGroupListAPI;
			GC.KeepAlive(GetGroupList);
			GetGroupInfoDelegate GetGroupInfoAPI = (GetGroupInfoDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["查询群信息"]), typeof(GetGroupInfoDelegate));
			GetGroupInfo = GetGroupInfoAPI;
			GC.KeepAlive(GetGroupInfo);
			PrivateUndoDelegate UndoPrivateAPI = (PrivateUndoDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["撤回消息_私聊本身"]), typeof(PrivateUndoDelegate));
			Undo_PrivateEvent = UndoPrivateAPI;
			GC.KeepAlive(Undo_PrivateEvent);
			UndoGroupDelegate UndoGroupApi = (UndoGroupDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["撤回消息_群聊"]), typeof(UndoGroupDelegate));
			Undo_GroupEvent = UndoGroupApi;
			GC.KeepAlive(Undo_GroupEvent);
			GetAdministratorListDelegate GetAdministratorListAPI = (GetAdministratorListDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取管理层列表"]), typeof(GetAdministratorListDelegate));
			GetAdministratorList = GetAdministratorListAPI;
			GC.KeepAlive(GetAdministratorList);
			SendFriendJSONMessageDelegate SendFriendJSONMessageAPI = (SendFriendJSONMessageDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["发送好友json消息"]), typeof(SendFriendJSONMessageDelegate));
			SendFriendJSONMessage = SendFriendJSONMessageAPI;
			GC.KeepAlive(SendFriendJSONMessage);
			SendGroupJSONMessageDelegate SendGroupJSONMessageAPI = (SendGroupJSONMessageDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["发送群json消息"]), typeof(SendGroupJSONMessageDelegate));
			SendGroupJSONMessage = SendGroupJSONMessageAPI;
			GC.KeepAlive(SendGroupJSONMessage);
			SaveFileToWeiYunDelegate SaveFileToWeiYunAPI = (SaveFileToWeiYunDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["保存文件到微云"]), typeof(SaveFileToWeiYunDelegate));
			SaveFileToWeiYun = SaveFileToWeiYunAPI;
			GC.KeepAlive(SaveFileToWeiYun);
			ReadForwardedChatHistoryDelegate ReadForwardedChatHistoryAPI = (ReadForwardedChatHistoryDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["查看转发聊天记录内容"]), typeof(ReadForwardedChatHistoryDelegate));
			ReadForwardedChatHistory = ReadForwardedChatHistoryAPI;
			GC.KeepAlive(ReadForwardedChatHistory);
			UploadGroupFileDelegate UploadGroupFileAPI = (UploadGroupFileDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["上传群文件"]), typeof(UploadGroupFileDelegate));
			UploadGroupFile = UploadGroupFileAPI;
			GC.KeepAlive(UploadGroupFile);
			GetGroupFileListDelegate GetGroupFileListAPI = (GetGroupFileListDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取群文件列表"]), typeof(GetGroupFileListDelegate));
			GetGroupFileList = GetGroupFileListAPI;
			GC.KeepAlive(GetGroupFileList);
			DeleteGroupMemberDelegate DeleteGroupMemberAPI = (DeleteGroupMemberDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["删除群成员"]), typeof(DeleteGroupMemberDelegate));
			DeleteGroupMember = DeleteGroupMemberAPI;
			GC.KeepAlive(DeleteGroupMember);
			DeleteFriendDelegate DeleteFriendAPI = (DeleteFriendDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["删除好友"]), typeof(DeleteFriendDelegate));
			DeleteFriend = DeleteFriendAPI;
			GC.KeepAlive(DeleteFriend);
			MuteGroupMemberDelegate MuteGroupMemberAPI = (MuteGroupMemberDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["禁言群成员"]), typeof(MuteGroupMemberDelegate));
			MuteGroupMember = MuteGroupMemberAPI;
			GC.KeepAlive(MuteGroupMember);
			MuteGroupAllDelegate MuteGroupAllAPI = (MuteGroupAllDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["全员禁言"]), typeof(MuteGroupAllDelegate));
			MuteGroupAll = MuteGroupAllAPI;
			GC.KeepAlive(MuteGroupAll);
			SetupAdministratorDelegate SetupAdministratorAPI = (SetupAdministratorDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["设置管理员"]), typeof(SetupAdministratorDelegate));
			SetupAdministrator = SetupAdministratorAPI;
			GC.KeepAlive(SetupAdministrator);
			ShareMusicDelegate ShareMusicAPI = (ShareMusicDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["分享音乐"]), typeof(ShareMusicDelegate));
			ShareMusic = ShareMusicAPI;
			GC.KeepAlive(ShareMusic);
			GetQQWalletPersonalInformation GetQQWalletPersonalInformationAPI = (GetQQWalletPersonalInformation)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取QQ钱包个人信息"]), typeof(GetQQWalletPersonalInformation));
			GetQQWalletPersonalInfo = GetQQWalletPersonalInformationAPI;
			GC.KeepAlive(GetQQWalletPersonalInfo);
			GetMoneyCookieDelegate GetMoneyCookieAPI = (GetMoneyCookieDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取钱包cookie"]), typeof(GetMoneyCookieDelegate));
			GetMoneyCookie = GetMoneyCookieAPI;
			GC.KeepAlive(GetMoneyCookie);
			GetClientKeyDelegate GetClientKeyAPI = (GetClientKeyDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["获取clientkey"]), typeof(GetClientKeyDelegate));
			GetClientKey = GetClientKeyAPI;
			GC.KeepAlive(GetClientKey);
			GetPSKeyDelegate GetPSKeyAPI = (GetPSKeyDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["获取pskey"]), typeof(GetPSKeyDelegate));
			GetPSKey = GetPSKeyAPI;
			GC.KeepAlive(GetPSKey);
			GetSKeyDelegate GetSKeyAPI = (GetSKeyDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["获取skey"]), typeof(GetSKeyDelegate));
			GetSKey = GetSKeyAPI;
			GC.KeepAlive(GetSKey);
			GetGroupMemberBriefInfoDelegate GetGroupMemberBriefInfoAPI = (GetGroupMemberBriefInfoDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(json["取群成员简略信息"]), typeof(GetGroupMemberBriefInfoDelegate));
			GetGroupMemberBriefInfo = GetGroupMemberBriefInfoAPI;
			GC.KeepAlive(GetGroupMemberBriefInfo);
		}
		#endregion
		#region 函数委托指针
		//输出日志
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr OutputLog(string pkey, [MarshalAs(UnmanagedType.LPStr)] string message, int text_color, int background_color);
		//发送好友消息
		public static SendPrivateMsgDelegate SendPrivateMsg = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SendPrivateMsgDelegate(string pkey, long ThisQQ, long SenderQQ, [MarshalAs(UnmanagedType.LPStr)] string MessageContent, ref long MessageRandom, ref uint MessageReq);
		//发送群消息
		public static SendGroupMsgDelegate SendGroupMsg = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SendGroupMsgDelegate(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string msgcontent, bool anonymous);
		//撤回私人消息
		public static PrivateUndoDelegate Undo_PrivateEvent = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool PrivateUndoDelegate(string pkey, long thisQQ, long otherQQ, long message_random, int message_req, int time);
		//撤回群消息
		public static UndoGroupDelegate Undo_GroupEvent = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool UndoGroupDelegate(string pkey, long thisQQ, long groupQQ, long message_random, int message_req);
		//收到网络图片
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr RecviceimageDelegate(string pkey, string guid, long thisQQ, long groupQQ);
		//获取好友列表
		public static GetFriendListDelegate GetFriendList = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate int GetFriendListDelegate(string pkey, long thisQQ, ref PInvoke.DataArray[] DataInfo);
		//获取群列表
		public static GetGroupListDelegate GetGroupList = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate int GetGroupListDelegate(string pkey, long thisQQ, ref PInvoke.DataArray[] DataInfo);
		//获取群会员列表
		public static GetGroupMemberlistDelegate GetGroupMemberlist = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate int GetGroupMemberlistDelegate(string pkey, long thisQQ, long groupQQ, ref PInvoke.DataArray[] DataInfo);
		//获取管理员列表
		public static GetAdministratorListDelegate GetAdministratorList = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetAdministratorListDelegate(string pkey, long thisQQ, long gruopQQ);
		//设置管理员
		public static SetupAdministratorDelegate SetupAdministrator = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SetupAdministratorDelegate(string pkey, long thisQQ, long gruopQQ, long memberQQ, bool SetupOrCancel);
		//重启框架
		public static RestartDelegate restart = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate void RestartDelegate(string pkey);
		//获取框架QQ
		public static GetLoginQQDelegate GetLoginQQ = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetLoginQQDelegate(string pkey);
		//处理好友验证事件
		public static FriendverificationEventDelegate FriendverificationEvent = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate void FriendverificationEventDelegate(string pkey, long thisQQ, long triggerQQ, long message_seq, PInvoke.FriendVerificationOperateEnum operate_type);
		//处理群验证事件
		public static GroupVerificationEventDelegate GroupVerificationEvent = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GroupVerificationEventDelegate(string pkey, long thisQQ, long source_groupQQ, long triggerQQ, long message_seq, PInvoke.GroupVerificationOperateEnum operate_type, PInvoke.EventTypeEnum event_type, [MarshalAs(UnmanagedType.LPStr)] string refuse_reason);
		//获取图片下载链接
		public static GetImageDownloadLinkDelegate GetImageDownloadLink = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetImageDownloadLinkDelegate(string pkey, string guid, long thisQQ, long groupQQ);
		//查询好友信息
		public static GetFriendInfoDelegate GetFriendInfo = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GetFriendInfoDelegate(string pkey, long thisQQ, long otherQQ, ref PInvoke.GetFriendDataInfo[] friendInfos);
		//查询群信息
		public static GetGroupInfoDelegate GetGroupInfo = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GetGroupInfoDelegate(string pkey, long thisQQ, long otherGroupQQ, ref PInvoke.GroupCardInfoDatList[] GroupInfos);
		//取群名片
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GetGroupCardInfoDelegate(string pkey, long thisQQ, long otherGroupQQ, ref PInvoke.GroupCardInfoDatList[] groupCardInfo);
		//设置群名片
		public static SetupGroupCardInfoDelegate SetupGroupCardInfo = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetupGroupCardInfoDelegate(string pkey, long thisQQ, long otherGroupQQ, long memberQQ, [MarshalAs(UnmanagedType.LPStr)] string newCard );
		//创建群文件夹
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr CreateGroupFolderDelegate(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string folder);
		//发送好友json消息
		public static SendFriendJSONMessageDelegate SendFriendJSONMessage = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SendFriendJSONMessageDelegate(string pkey, long thisQQ, long friendQQ, [MarshalAs(UnmanagedType.LPStr)] string json_content);
		//发送群json消息
		public static SendGroupJSONMessageDelegate SendGroupJSONMessage = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SendGroupJSONMessageDelegate(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string json_content, bool anonymous);
		//发送免费礼物
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SendFreeGiftDelegate(string pkey, long thisQQ, long groupQQ, long otherQQ, int gift);
		//删除群成员
		public static DeleteGroupMemberDelegate DeleteGroupMember = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool DeleteGroupMemberDelegate(string pkey, long thisQQ, long groupQQ, long memberQQ, bool ifAddAgain);
		//删除好友
		public static DeleteFriendDelegate DeleteFriend = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool DeleteFriendDelegate(string pkey, long thisQQ,long friendQQ);
		//发送临时消息
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SendGroupTemporaryMessage(string pkey, long thisQQ, long groupQQ, long otherQQ, [MarshalAs(UnmanagedType.LPStr)] string content, ref long random, ref int req);
		//查看转发聊天记录内容
		public static ReadForwardedChatHistoryDelegate ReadForwardedChatHistory = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate void ReadForwardedChatHistoryDelegate(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string resID, [MarshalAs(UnmanagedType.LPStr)] ref string retPtr);
		//分享音乐
		public static ShareMusicDelegate ShareMusic = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool ShareMusicDelegate(string pkey, long thisQQ, long otherQQ, [MarshalAs(UnmanagedType.LPStr)] string music_name, [MarshalAs(UnmanagedType.LPStr)] string artist_name, [MarshalAs(UnmanagedType.LPStr)] string redirect_link, [MarshalAs(UnmanagedType.LPStr)] string cover_link, [MarshalAs(UnmanagedType.LPStr)] string file_path, PInvoke.MusicAppTypeEnum app_type, PInvoke.MusicShare_Type share_type);
		//更改群聊消息内容
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool ModifyGroupMessageContent(string pkey, [MarshalAs(UnmanagedType.SysInt)] int data_pointer, [MarshalAs(UnmanagedType.LPStr)] string new_message_content);
		//更改私聊消息内容
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool ModifyPrivateMessageContent(string pkey, [MarshalAs(UnmanagedType.SysInt)] int data_pointer, [MarshalAs(UnmanagedType.LPStr)] string new_message_content);
		//群聊画图红包
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GroupDrawRedEnvelope(string pkey, long thisQQ, int total_number, int total_amount, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string question, [MarshalAs(UnmanagedType.LPStr)] string payment_password, int card_serial, ref PInvoke.GetCaptchaInfoDataList[] captchaInfo);
		//好友普通红包
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr FriendNormalRedEnvelope(string pkey, long thisQQ, int total_number, int total_amount, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string question, int skinID, [MarshalAs(UnmanagedType.LPStr)] string payment_password, int card_serial, ref PInvoke.GetCaptchaInfoDataList[] ciDataLists);
		// 好友文件转发至好友
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool FriendFileToFriend(string pkey, long thisQQ, long sourceQQ, long targetQQ, [MarshalAs(UnmanagedType.LPStr)] string fileID, [MarshalAs(UnmanagedType.LPStr)] string file_name, long file_size, ref int msgReq, ref long Random, ref int time);
		// 获取插件目录
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetPluginDataDirectory(string pkey);
		// 获取ClientKey
		public static GetClientKeyDelegate GetClientKey = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetClientKeyDelegate(string pkey, long thisQQ);
		// 获取PSKey
		public static GetPSKeyDelegate GetPSKey = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetPSKeyDelegate(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string domain);
		// 获取SKey
		public static GetSKeyDelegate GetSKey = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetSKeyDelegate(string pkey, long thisQQ);
		// 获取订单信息
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetOrderDetail(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string orderID, ref PInvoke.OrderDetaildDataList[] data);
		// 解散群
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool DissolveGroup(string pkey, long thisQQ, long gruopNumber);
		// 强制取昵称
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetNameForce(string pkey, long thisQQ, long otherQQ);
		
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetOrderDetailDegelte(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string orderID, ref PInvoke.OrderDetaildDataList[] data);
		// 取QQ钱包个人信息
		public static GetQQWalletPersonalInformation GetQQWalletPersonalInfo = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetQQWalletPersonalInformation(string pkey, long thisQQ, ref PInvoke.QQWalletDataList[] qQWalletInfoDataLists);
		// 取钱包cookie
		public static GetMoneyCookieDelegate GetMoneyCookie = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetMoneyCookieDelegate(string pkey, long thisQQ);
		// 从缓存获取昵称
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetNameFromCache(string pkey, long otherQQ);
		// 取群名片
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetGroupNickname(string pkey, long thisQQ, long groupQQ, long otherQQ);
		//获取群文件列表
		public static GetGroupFileListDelegate GetGroupFileList = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetGroupFileListDelegate(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string folder, ref PInvoke.GroupFileInfoDataList[] groupFileInfoDataLists);
		// 群权限_新成员查看历史消息
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GroupPermission_SetInviteMethod(string pkey, long thisQQ, long groupQQ, int method);
		// 转发群文件至好友
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool ForwardGroupFileToFriend(string pkey, long thisQQ, long source_groupQQ, long target_groupQQ, [MarshalAs(UnmanagedType.LPStr)] string fileID, [MarshalAs(UnmanagedType.LPStr)] string filename, long filesize, ref int msgReq, ref long Random, ref int time);
		// 群文件转发至群
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool ForwardGroupFileToGroup(string pkey, long thisQQ, long source_groupQQ, long target_groupQQ, [MarshalAs(UnmanagedType.LPStr)] string fileID);
		// 删除群文件
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr DeleteGroupFile(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string file_id, [MarshalAs(UnmanagedType.LPStr)] string folder);
		// 删除群文件夹
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr DeleteGroupFolder(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string folder);
		// 重命名群文件夹
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr RenameGroupFolder(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string old_folder, [MarshalAs(UnmanagedType.LPStr)] string new_folder);
		// 上传群文件
		public static UploadGroupFileDelegate UploadGroupFile = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr UploadGroupFileDelegate(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string path, [MarshalAs(UnmanagedType.LPStr)] string folder);
		// 移动群文件
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr MoveGroupFile(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string file_id, [MarshalAs(UnmanagedType.LPStr)] string old_folder, [MarshalAs(UnmanagedType.LPStr)] string new_folder);
		// 上传好友图片
		public static UploadFriendImageDelegate UploadFriendImage = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr UploadFriendImageDelegate(string pkey, long thisQQ, long friendQQ, bool is_flash, [MarshalAs(UnmanagedType.LPArray)] byte[] pic, int picsize);
		// 上传群图片
		public static UploadGroupImageDelegate UploadGroupImage = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr UploadGroupImageDelegate(string pkey, long thisQQ, long friendQQ, bool is_flash, [MarshalAs(UnmanagedType.LPArray)] byte[] pic, int picsize);
		// 上传群头像
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool UploadGroupAvatar(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPArray)] byte[] pic, int picsize);
		// 上传头像
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr UploadAvatar(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPArray)] byte[] pic, int picsize);
		// 上传好友语音
		public static UploadFriendAudioDelegate UploadFriendAudio = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr UploadFriendAudioDelegate(string pkey, long thisQQ, long friendQQ, int audio_type, [MarshalAs(UnmanagedType.LPStr)] string audio_text, [MarshalAs(UnmanagedType.LPArray)] byte[] audio, int audiosize);
		// 上传群语音
		public static UploadGroupAudioDelegate UploadGroupAudio = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr UploadGroupAudioDelegate(string pkey, long thisQQ, long friendQQ, int audio_type, [MarshalAs(UnmanagedType.LPStr)] string audio_text, [MarshalAs(UnmanagedType.LPArray)] byte[] audio, int audiosize);
		// 保存文件到微云
		public static SaveFileToWeiYunDelegate SaveFileToWeiYun = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SaveFileToWeiYunDelegate(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string file_id);
		//禁言群成员
		public static MuteGroupMemberDelegate MuteGroupMember = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool MuteGroupMemberDelegate(string pkey, long thisQQ, long groupQQ, long memberQQ,uint muteTime);
		//全员禁言
		public static MuteGroupAllDelegate MuteGroupAll = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool MuteGroupAllDelegate(string pkey, long thisQQ, long groupQQ, bool ifOpen);
		//是否被禁言
		public static IfMutedDelegate IfMuted = null;
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool IfMutedDelegate(string pkey, long thisQQ, long groupQQ);
		// 上报当前位置
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool ReportCurrent(string pkey, long thisQQ, long groupQQ, double Longitude, double Latitude);
		// 设置群名片
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SetGroupNickname(string pkey, long thisQQ, long groupQQ, long otherQQ, [MarshalAs(UnmanagedType.LPStr)] string nickname);
		// 设置位置共享
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetLocationShare(string pkey, long thisQQ, long groupQQ, double Longitude, double Latitude, bool is_enabled);
		// 设置在线状态
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetStatus(string pkey, long thisQQ, int main, int sun, int battery);
		// 设置专属头衔
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool Setexclusivetitle(string pkey, long thisQQ, long groupQQ, long otherQQ, [MarshalAs(UnmanagedType.LPStr)] string name);
		// 添加好友
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr AddFriend(string pkey, long thisQQ, long otherQQ, [MarshalAs(UnmanagedType.LPStr)] string verification, [MarshalAs(UnmanagedType.LPStr)] string comment);
		// 添加群
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr AddGroup(string pkey, long thisQQ, long groupQQ, [MarshalAs(UnmanagedType.LPStr)] string verification);
		// 退群
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool QuitGroup(string pkey, long thisQQ, long groupQQ);
		// 修改个性签名
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetSignature(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string signature, [MarshalAs(UnmanagedType.LPStr)] string location);
		// 修改昵称
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetName(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string name);
		// 置屏蔽好友
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SetBlockFriend(string pkey, long thisQQ, long otherQQ, bool is_blocked);
		// 置群消息接收
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetGroupMessageReceive(string pkey, long thisQQ, long groupQQ, int set_type);
		// 置特别关心好友
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SetSpecialFriend(string pkey, long thisQQ, long otherQQ, bool is_special);
		// 提交支付验证码
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr SubmitPaymentCaptcha(string pkey, long thisQQ, IntPtr captcha_information, [MarshalAs(UnmanagedType.LPStr)] string captcha, [MarshalAs(UnmanagedType.LPStr)] string payment_password);
		// 登录指定QQ
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool LoginSpecifyQQ(string pkey, long otherQQ);
		// 发送输入状态
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SendIMEStatus(string pkey, long thisQQ, long ohterQQ, int iMEStatus);
		// api是否有权限
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool CheckPermission(string pkey, int permission);
		// QQ点赞
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr QQLike(string pkey, long thisQQ, long otherQQ);
		// 修改资料
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool Modifyinformation(string pkey, long thisQQ, [MarshalAs(UnmanagedType.LPStr)] string json);
		// 取群未领红包
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetRedEnvelope(string pkey, long thisQQ, long GroupQQ, ref PInvoke.RedEnvelopesDataList[] reDataList);
		// 打好友电话
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate void CallPhone(string pkey, long thisQQ, long otherQQ);
		// 取群文件下载地址
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GroupFileDownloadLink(string pkey, long thisQQ, long GroupQQ, [MarshalAs(UnmanagedType.LPStr)] string FileID, [MarshalAs(UnmanagedType.LPStr)] string FileName);
		// 头像双击_群
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool DoubleclickGroupFace(string pkey, long thisQQ, long otherQQ, long groupQQ);
		// 群聊置顶
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GroupTop(string pkey, long thisQQ, long GroupQQ, bool istop);
		// 设为精华
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetEssence(string pkey, long thisQQ, long groupQQ, int message_req, long message_random);
		// 群权限_设置群昵称规则
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetGroupNickRules(string pkey, long thisQQ, long GroupQQ, [MarshalAs(UnmanagedType.LPWStr)] string rules);
		// 群权限_设置群发言频率
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool SetGroupLimitNumber(string pkey, long thisQQ, long GroupQQ, int LimitNumber);
		// 群权限_设置群查找方式
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool FriendjoinGroup(string pkey, long thisQQ, long GroupQQ, long otherQQ, long otherGroupQQ);
		// 置群内消息通知
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool GroupNoticeMethod(string pkey, long thisQQ, long GroupQQ, long otherQQ, int metohd);
		//取群成员简略信息
		public static GetGroupMemberBriefInfoDelegate GetGroupMemberBriefInfo = null;

          [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate IntPtr GetGroupMemberBriefInfoDelegate(string pkey, long thisQQ, long GroupQQ, ref PInvoke.GMBriefDataList[] gMBriefDataLists);
		//修改群名称
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		public delegate bool UpdataGroupNameDelegate(string pkey, long thisQQ, long GroupQQ, [MarshalAs(UnmanagedType.LPStr)] string NewGroupName);

		#endregion

		#region 全局异常
		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Exception ex = e.Exception;
			MessageBox.Show(string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace));
		}
		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			MessageBox.Show(string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}\r\nCLR即将退出：{3}", ex.GetType(), ex.Message, ex.StackTrace, e.IsTerminating));
		}
		#endregion
	}
}
