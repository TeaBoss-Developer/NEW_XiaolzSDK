using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XiaolzCSharp
{
	public class PInvoke
    {
		#region 全局变量
		public static string jsonstr;
		public static string plugin_key;
		public static bool PluginStatus;
		public static long RobotQQ=0;
		public static string MasterQQ ="";
		public static long FeedbackGroup =0;
	

		#endregion

		public static byte[] ReadBytes { get; set; }
		public static byte[] readbyte { get; set; }
		public static IntPtr pStruct { get; set; }
		public static GroupMemberInfo GMInfo { get; set; }

		#region 结构体
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct EventTypeBase
		{
			public long ThisQQ;
			public long SourceGroupQQ;
			public long OperateQQ;
			public long TriggerQQ;
			public long MessageSeq;
			public uint MessageTimestamp;
			[MarshalAs(UnmanagedType.LPStr)]			public string SourceGroupName;
			[MarshalAs(UnmanagedType.LPStr)]			public string OperateQQName;
			[MarshalAs(UnmanagedType.LPStr)]			public string TriggerQQName;
			[MarshalAs(UnmanagedType.LPStr)]			public string MessageContent;
			public EventTypeEnum EventType;
			public uint EventSubType;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct PrivateMessageEvent
		{
			public long SenderQQ;
			public long ThisQQ;
			public uint MessageReq;
			public long MessageSeq;
			public uint MessageReceiveTime;
			public long MessageGroupQQ;
			public uint MessageSendTime;
			public long MessageRandom;
			public uint MessageClip;
			public uint MessageClipCount;
			public long MessageClipID;
			[MarshalAs(UnmanagedType.LPStr)]
			public string MessageContent;
			public uint BubbleID;
			public MessageTypeEnum MessageType;
			public MessageSubTypeEnum MessageSubType;
			public MessageSubTypeEnum MessageSubTemporaryType;
			public uint RedEnvelopeType;
			[MarshalAs(UnmanagedType.LPStr)]
			public string SessionToken;
			public long SourceEventQQ;
			[MarshalAs(UnmanagedType.LPStr)]
			public string SourceEventQQName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string FileID;
			[MarshalAs(UnmanagedType.LPStr)]
			public string FileMD5;
			[MarshalAs(UnmanagedType.LPStr)]
			public string FileName;
			public long FileSize;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GroupMessageEvent
		{
			public long SenderQQ;
			public long ThisQQ;
			public uint MessageReq;
			public uint MessageReceiveTime;
			public long MessageGroupQQ;
			[MarshalAs(UnmanagedType.LPStr)]
			public string SourceGroupName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string SenderNickname;
			public uint MessageSendTime;
			public long MessageRandom;
			public uint MessageClip;
			public uint MessageClipCount;
			public long MessageClipID;
			public MessageTypeEnum MessageType;
			[MarshalAs(UnmanagedType.LPStr)]
			public string SenderTitle;
			[MarshalAs(UnmanagedType.LPStr)]
			public string MessageContent;
			[MarshalAs(UnmanagedType.LPStr)]
			public string ReplyMessageContent;
			public uint BubbleID;
			public uint ThisQQAnonymousID;
			public uint reserved_;
			[MarshalAs(UnmanagedType.LPStr)]
			public string FileID;
			[MarshalAs(UnmanagedType.LPStr)]
			public string FileMD5;
			[MarshalAs(UnmanagedType.LPStr)]
			public string FileName;
			public long FileSize;
			public uint MessageAppID;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct FriendDataList
		{
			public uint count;
			public FriendInfo pFriendInfo;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct ServiceInfo
		{
			public ServiceInformation ServiceList;
			public uint ServiceLevel;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GetFriendDataInfo //一维数组
		{
			public FriendInfo friendInfo;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct DataArray
		{
			public int index;
			public int Amount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024*100)]
			public byte[] pAddrList;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct FriendInfo
		{
			[MarshalAs(UnmanagedType.LPStr)]
			public string Email;
			public long QQNumber;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Name;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Note;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Status;
			public uint Likes;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Signature;
			public uint Gender;
			public uint Level;
			public uint Age;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Nation;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Province;
			[MarshalAs(UnmanagedType.LPStr)]
			public string City;
			public ServiceInfo serviceInfos;
			public uint ContinuousOnlineTime;
			[MarshalAs(UnmanagedType.LPStr)]
			public string QQTalent;
			public uint LikesToday;
			public uint LikesAvailableToday;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GroupMemberInfo
		{
			[MarshalAs(UnmanagedType.LPStr)]
			public string QQNumber;
			public uint Age;
			public uint Gender;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Name;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Email;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Nickname;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Note;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Title;
			[MarshalAs(UnmanagedType.LPStr)]
			public string Phone;
			public long TitleTimeout;
			public long ShutUpTimestamp;
			public long JoinTime;
			public long ChatTime;
			public long Level;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GroupInfo
		{
			public long GroupID;
			public long GroupQQ;
			public long CFlag;
			public long GroupInfoSeq;
			public long GroupFlagExt;
			public long GroupRankSeq;
			public long CertificationType;
			public long ShutUpTimestamp;
			public long ThisShutUpTimestamp;
			public long CmdUinUinFlag;
			public long AdditionalFlag;
			public long GroupTypeFlag;
			public long GroupSecType;
			public long GroupSecTypeInfo;
			public long GroupClassExt;
			public long AppPrivilegeFlag;
			public long SubscriptionUin;
			public long GroupMemberCount;
			public long MemberNumSeq;
			public long MemberCardSeq;
			public long GroupFlagExt3;
			public long GroupOwnerUin;
			public long IsConfGroup;
			public long IsModifyConfGroupFace;
			public long IsModifyConfGroupName;
			public long CmduinJoinTime;
			[MarshalAs(UnmanagedType.LPStr)]
			public string GroupName;
			[MarshalAs(UnmanagedType.LPStr)]
			public string GroupMemo;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct GroupCardInfoDatList
		{
			//public uint index; //数组索引
			//public uint Amount; //数组元素数量
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] public byte[] groupCardInfo;// 
			public GroupCardInfo groupCardInfo;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GroupCardInfo
		{			
			[MarshalAs(UnmanagedType.LPStr)]			public string GroupName;// 群名称																				
			[MarshalAs(UnmanagedType.LPStr)]			public string GroupLocation;// 群地点
			[MarshalAs(UnmanagedType.LPStr)]			public string GroupClassification;// 群分类																						  
			[MarshalAs(UnmanagedType.LPStr)]			public string GroupTags;// 群标签 以|分割
			[MarshalAs(UnmanagedType.LPStr)]			public string GroupDescription;// 群介绍
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GroupFileInfoDataList
		{
			public uint index; //数组索引
			public uint Amount; //数组元素数量
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]			public byte[] pAddrList; //每个元素的指针
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct OrderDetaildDataList
		{
			public OrderDetail orderDetail;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct OrderDetail
		{
			// 订单时间
				[MarshalAs(UnmanagedType.LPStr)]			public string OrderTime;
			// 订单说明			
			[MarshalAs(UnmanagedType.LPStr)]			public string OrderDescription;
			// 订单类名			
			[MarshalAs(UnmanagedType.LPStr)]			public string OrderClassification;
			// 订单类型			
			[MarshalAs(UnmanagedType.LPStr)]			public string OrderType;
			// 订单手续费			
			[MarshalAs(UnmanagedType.LPStr)]			public string OrderCommission;
			// 操作人QQ			
			[MarshalAs(UnmanagedType.LPStr)]			public string OperatorQQ;
			// 操作人昵称			
			[MarshalAs(UnmanagedType.LPStr)]			public string OperatorName;
			// 接收人QQ			
			[MarshalAs(UnmanagedType.LPStr)]			public string ReceiverQQ;
			// 接收人昵称			
			[MarshalAs(UnmanagedType.LPStr)]			public string ReceiverName;
			// 操作金额			
			[MarshalAs(UnmanagedType.LPStr)]			public string OperateAmount;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct RedEnvelopesDataList
		{
			//public int index;
			public NotReRedEnvelopes notReRedEnvelopes;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct NotReRedEnvelopes
		{
			public long SourceQQ;
			[MarshalAs(UnmanagedType.LPStr)]
			public string listid;
			[MarshalAs(UnmanagedType.LPStr)]
			public string authkey;
			[MarshalAs(UnmanagedType.LPStr)]
			public string title;
			[MarshalAs(UnmanagedType.LPStr)]
			public string channel;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GroupFileInformation
		{
			public IntPtr FileID; //文件夹fileid或者文件fileid
			public IntPtr FileName; // 文件夹名或文件名
			public long FileSize; // 文件大小，文件夹时表示有多少个文件
			public IntPtr FileMd5; // 文件md5，文件夹时为空，部分文件类型也可能是空
			public long FileFromUin; // 创建文件夹或上传文件的QQ
			public IntPtr FileFromNick; // 创建文件夹或上传文件的QQ
			public FiletypeEnum FileType; // 文件类型 1: 文件, 2: 文件夹
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GMBriefDataList
		{
			public GMBriefInfo groupMemberBriefInfo;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GMBriefInfo
		{
			public uint GroupMAax;
			public uint GruoupNum;
			public long GroupOwner;
			public IntPtr AdminiList;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct AdminListDataList
		{
			public int index;//数组索引
			public int Amount;//数组元素数量
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
			//public byte[] pdatalist;
			public long[] pdatalist;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public class GroupMemberBriefInfo
		{
			public uint GroupMAax;
			public uint GruoupNum;
			public long GroupOwner;
			public long[] AdminiList;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct GetCaptchaInfoDataList
		{
			//public int index;
			public CaptchaInformation CaptchaInfo;
		}
		/// <summary>
		/// 验证码信息
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
		public struct CaptchaInformation
		{
			// token_id			
			[MarshalAs(UnmanagedType.LPStr)]
			public string TokenID;
			// skey		
			[MarshalAs(UnmanagedType.LPStr)]
			public string SKey;
			// bank_type			
			[MarshalAs(UnmanagedType.LPStr)]
			public string BankType;
			// mobile			
			[MarshalAs(UnmanagedType.LPStr)]
			public string Mobile;
			// business_type			
			[MarshalAs(UnmanagedType.LPStr)]
			public string BusinessType;
			// random			
			public int Random;
			// transaction_id			
			[MarshalAs(UnmanagedType.LPStr)]
			public string TransactionID;
			// purchaser_id			
			[MarshalAs(UnmanagedType.LPStr)]
			public string PurchaserID;
			// token			
			[MarshalAs(UnmanagedType.LPStr)]
			public string Token;
			// auth_params			
			[MarshalAs(UnmanagedType.LPStr)]
			public string AuthParams;
		}
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct QQWalletDataList
		{
			public QQWalletInformation QQWalletInfo;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct QQWalletInformation
		{
			[MarshalAs(UnmanagedType.LPStr)]
			public string balance; // 余额
			[MarshalAs(UnmanagedType.LPStr)]
			public string id; // 身份证号
			[MarshalAs(UnmanagedType.LPStr)]
			public string realname; // 实名
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public CardListIntptr[] cardlist;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct CardListIntptr
		{
			public IntPtr addr; //数组指针
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct CardInformation
		{
			public int Serial; // 序列
			[MarshalAs(UnmanagedType.LPStr)]
			public string TailNumber; // 尾号
			[MarshalAs(UnmanagedType.LPStr)]
			public string Bank; // 银行
			[MarshalAs(UnmanagedType.LPStr)]
			public string BindPhone; // 绑定手机
			[MarshalAs(UnmanagedType.LPStr)]
			public string BindSerial;
			[MarshalAs(UnmanagedType.LPStr)]
			public string BankType; // bank_type
		}

		public struct AppInfo
		{
			public string sdkv;
			public string appname;
			public string author;
			public string describe;
			public string appv;
			public long groupmsaddres;
			public long friendmsaddres;
			public long eventmsaddres;
			public long unitproaddres;
			public long setproaddres;
			public long useproaddres;
			public long banproaddres;
			public dynamic data;
		}
		#endregion

		#region 枚举
		public enum FiletypeEnum
		{
			file = 1,
			folder = 2
		}

		public enum FriendVerificationOperateEnum
		{
			Agree = 1, //同意
			Deny = 2 //拒绝
		}
		public enum GroupVerificationOperateEnum
		{
			Agree = 11, //同意
			Deny = 12, //拒绝
			Ignore = 14 //忽略
		}
		public enum EventTypeEnum
		{
			Friend_Removed = 100,// 好友事件_被好友删除
			Friend_SignatureChanged = 101, // 好友事件_签名变更	
			Friend_NameChanged = 102,// 好友事件_昵称改变
			Friend_UserUndid = 103,// 好友事件_某人撤回事件
			Friend_NewFriend = 104,// 好友事件_有新好友
			Friend_FriendRequest = 105,// 好友事件_好友请求
			Friend_FriendRequestAccepted = 106, //好友事件_对方同意了您的好友请求
			Friend_FriendRequestRefused = 107,// 好友事件_对方拒绝了您的好友请求
			Friend_InformationLiked = 108,// 好友事件_资料卡点赞
			Friend_SignatureLiked = 109,// 好友事件_签名点赞
			Friend_SignatureReplied = 110,// 好友事件_签名回复
			Friend_TagLiked = 111,// 好友事件_个性标签点赞
			Friend_StickerLiked = 112,// 好友事件_随心贴回复
			Friend_StickerAdded = 113, // 好友事件_随心贴增添
			Friend_Blacklist = 64, // 好友事件_加入黑名单
			Group_Invited = 1,// 群事件_我被邀请加入群
			Group_MemberJoined = 2,// 群事件_某人加入了群
			Group_MemberVerifying = 3,// 群事件_某人申请加群
			Group_GroupDissolved = 4, // 群事件_群被解散
			Group_MemberQuit = 5,// 群事件_某人退出了群
			Group_MemberKicked = 6,// 群事件_某人被踢出群
			Group_MemberShutUp = 7,// 群事件_某人被禁言
			Group_MemberUndid = 8, // 群事件_某人撤回事件
			Group_AdministratorTook = 9, // 群事件_某人被取消管理
			Group_AdministratorGave = 10,// 群事件_某人被赋予管理
			Group_EnableAllShutUp = 11,// 群事件_开启全员禁言
			Group_DisableAllShutUp = 12,// 群事件_关闭全员禁言
			Group_EnableAnonymous = 13,// 群事件_开启匿名聊天
			Group_DisableAnonymous = 14,// 群事件_关闭匿名聊天			
			Group_EnableChatFrankly = 15,// 群事件_开启坦白说
			Group_DisableChatFrankly = 16, // 群事件_关闭坦白说
			Group_AllowGroupTemporary = 17,// 群事件_允许群临时会话
			Group_ForbidGroupTemporary = 18,// 群事件_禁止群临时会话
			Group_AllowCreateGroup = 19,// 群事件_允许发起新的群聊
			Group_ForbidCreateGroup = 20,// 群事件_允许发起新的群聊
			Group_AllowUploadFile = 21,// 群事件_允许上传群文件			
			Group_ForbidUploadFile = 22,// 群事件_禁止上传群文件			
			Group_AllowUploadPicture = 23,// 群事件_允许上传相册			
			Group_ForbidUploadPicture = 24,// 群事件_禁止上传相册			
			Group_MemberInvited = 25,// 群事件_某人被邀请入群			
			Group_ShowMemberTitle = 26,// 群事件_展示成员群头衔
			Group_HideMemberTitle = 27, // 群事件_隐藏成员群头衔
			Group_MemberNotShutUp = 28,// 群事件_某人被解除禁言
			QZone_Related = 29, // 空间事件_与我相关
			Group_MemberKickOut = 30,// 群事件_我被踢出
			This_SignInSuccess = 31,// 框架事件_登录成功
			Group_GroupNameUpdate = 32,// 群事件_群名变更
		}
		public enum PermissionEnum
		{
			OutputLog = 0, // 输出日志
			SendFriendMessage = 1, // 发送好友消息
			SendGroupMessage = 2, // 发送群消息
			SendGroupTemporaryMessage = 3, // 发送群临时消息
			AddFriend = 4, // 添加好友
			AddGroup = 5, // 添加群
			RemoveFriend = 6, // 删除好友！
			SetBlockFriend = 7, // 置屏蔽好友！
			SetSpecialFriend = 8, // 置特别关心好友
			SendFriendJSONMessage = 11, // 发送好友json消息
			SendGroupJSONMessage = 12, // 发送群json消息
			UploadFriendPicture = 13, // 上传好友图片
			UploadGroupPicture = 14, // 上传群图片
			UploadFriendAudio = 15, // 上传好友语音
			UploadGroupAudio = 16, // 上传群语音
			UploadAvatar = 17, // 上传头像！
			SetGroupMemberNickname = 18, // 设置群名片
			GetNameFromCache = 19, // 取昵称_从缓存
			GetNameForce = 20, // 强制取昵称
			GetSKey = 21, // 获取skey！
			GetPSKey = 22, // 获取pskey！
			GetClientKey = 23, // 获取clientkey！
			GetThisQQ = 24, // 取框架QQ
			GetFriendList = 25, // 取好友列表
			GetGroupList = 26, // 取群列表
			GetGroupMemberList = 27, // 取群成员列表
			SetAdministrator = 28, // 设置管理员
			GetAdministratorList = 29, // 取管理层列表
			GetGroupMemberNickname = 30, // 取群名片
			GetSignature = 31, // 取个性签名
			SetName = 32, // 修改昵称！
			SetSignature = 33, // 修改个性签名！
			KickGroupMember = 34, // 删除群成员
			BanGroupMember = 35, // 禁言群成员
			QuitGroup = 36, // 退群！
			DissolveGroup = 37, // 解散群！
			UploadGroupAvatar = 38, // 上传群头像
			BanAll = 39, // 全员禁言
			Group_Create = 40, // 群权限_发起新的群聊
			Group_CreateTemporary = 41, // 群权限_发起临时会话
			Group_UploadFile = 42, // 群权限_上传文件
			Group_UploadPicture = 43, // 群权限_上传相册
			Group_InviteFriend = 44, // 群权限_邀请好友加群
			Group_Anonymous = 45, // 群权限_匿名聊天
			Group_ChatFrankly = 46, // 群权限_坦白说
			Group_NewMemberReadChatHistory = 47, // 群权限_新成员查看历史消息
			Group_SetInviteMethod = 48, // 群权限_邀请方式设置
			Undo_Group = 49, // 撤回消息_群聊
			Undo_Private = 50, // 撤回消息_私聊本身
			SetLocationShare = 51, // 设置位置共享
			ReportCurrentLocation = 52, // 上报当前位置
			IsShutUp = 53, // 是否被禁言
			ProcessFriendVerification = 54, // 处理好友验证事件
			ProcessGroupVerification = 55, // 处理群验证事件
			ReadForwardedChatHistory = 56, // 查看转发聊天记录内容
			UploadGroupFile = 57, // 上传群文件
			CreateGroupFolder = 58, // 创建群文件夹
			SetStatus = 59, // 设置在线状态
			QQLike = 60, // QQ点赞！
			GetImageDownloadLink = 61, // 取图片下载地址
			QueryFriendInformation = 63, // 查询好友信息
			QueryGroupInformation = 64, // 查询群信息
			Reboot = 65, // 框架重启！
			GroupFileForwardToGroup = 66, // 群文件转发至群
			GroupFileForwardToFriend = 67, // 群文件转发至好友
			FriendFileForwardToFriend = 68, // 好友文件转发至好友
			SetGroupMessageReceive = 69, // 置群消息接收
			GetGroupNameFromCache = 70, // 取群名称_从缓存
			SendFreeGift = 71, // 发送免费礼物
			GetFriendStatus = 72, // 取好友在线状态
			GetQQWalletPersonalInformation = 73, // 取QQ钱包个人信息！
			GetOrderDetail = 74, // 获取订单详情
			SubmitPaymentCaptcha = 75, // 提交支付验证码
			ShareMusic = 77, // 分享音乐
			ModifyGroupMessageContent = 78, // 更改群聊消息内容！
			ModifyPrivateMessageContent = 79, // 更改私聊消息内容！
			GroupPasswordRedEnvelope = 80, // 群聊口令红包
			GroupRandomRedEnvelope = 81, // 群聊拼手气红包
			GroupNormalRedEnvelope = 82, // 群聊普通红包
			GroupDrawRedEnvelope = 83, // 群聊画图红包
			GroupAudioRedEnvelope = 84, // 群聊语音红包
			GroupFollowRedEnvelope = 85, // 群聊接龙红包
			GroupExclusiveRedEnvelope = 86, // 群聊专属红包
			FriendPasswordRedEnvelope = 87, // 好友口令红包
			FriendNormalRedEnvelope = 88, // 好友普通红包
			FriendDrawRedEnvelope = 89, // 好友画图红包
			FriendAudioRedEnvelope = 90, // 好友语音红包
			FriendFollowRedEnvelope = 91 // 好友接龙红包
		}
		public enum StatusOnlineTypeEnum
		{
			Normal = 0,
			Battery = 1000,
			WeakSignal = 1011,
			Sleeping = 1016,
			Gaming = 1017,
			Studying = 1018,
			Eating = 1019,
			WatchingTVSeries = 1021,
			OnVacation = 1022,
			OnlineStudying = 1024,
			TravelAtHome = 1025,
			TiMiing = 1027,
			ListeningToMusic = 1028,
			StayingUpLate = 1032,
			PlayingBall = 1050,
			FallInLove = 1051,
			ImOK = 1052
		}
		public enum StatusTypeEnum
		{
			Online = 11,
			Away = 31,
			Invisible = 41,
			Busy = 50,
			TalkToMe = 60,
			DoNotDisturb = 70
		}
		public enum MessageTypeEnum
		{
			Temporary = 141, // 临时会话
			FriendUsualMessage = 166, // 好友通常消息
			FriendFile = 529, // 好友文件
			FriendAudio = 208, // 好友语音
			GroupRedEnvelope = 78, // 群红包
			GroupUsualMessage = 134 // 群聊通常消息
		}
		public enum MessageSubTypeEnum
		{
			Temporary_Group = 0, // 临时会话_群
			Temporary_PublicAccount = 129, // 临时会话_公众号
			Temporary_WebQQConsultation = 201 // 临时会话_网页QQ咨询
		}
		public enum ServiceInformation
		{
			SVIP = 1, //SVIP
			VIDEO_VIP = 4, //视频会员
			MUSIC_PACK = 6, //音乐包
			STAR = 105, //star
			YELLOW_DIAMOND = 102, //黄钻
			GREEN_DIAMOND = 103, //绿钻
			RED_DIAMOND = 101, //红钻
			YELLOWLOVE = 104, //yellowlove
			SVIP_WITH_VIDEO = 107, //SVIP&视频会员
			SVIP_WITH_GREEN = 109, //SVIP&绿钻
			SVIP_WITH_MUSIC = 110 //SVIP&音乐包
		}
		public enum FreeGiftEnum
		{
			Gift_280 = 280, // 牵你的手
			Gift_281 = 281, // 可爱猫咪
			Gift_284 = 284, // 神秘面具
			Gift_285 = 285, // 甜wink
			Gift_286 = 286, // 我超忙的
			Gift_289 = 289, // 快乐肥宅水
			Gift_290 = 290, // 幸运手链
			Gift_299 = 299, // 卡布奇诺
			Gift_302 = 302, // 猫咪手表
			Gift_307 = 307, // 绒绒手套
			Gift_308 = 308, // 彩虹糖果
			Gift_312 = 312, // 爱心口罩
			Gift_313 = 313, // 坚强
			Gift_367 = 367 // 告白话筒
		}
		public enum EventProcessEnum
		{
			Block = 1,
			Ignore = 0
		}
		public enum AudioTypeEnum
		{
			Normal = 0, // 普通语音
			Change = 1, // 变声语音
			Text = 2, // 文字语音
			Match = 3 // (红包)匹配语音
		}
		public enum MusicAppTypeEnum
		{
			QQMusic = 0,
			XiaMiMusic = 1,
			KuWoMusic = 2,
			KuGouMusic = 3,
			WangYiMusic = 4
		}
		public enum MusicShare_Type
		{
			PrivateMsg = 0,
			GroupMsg = 1
		}
		public static Dictionary<string, string> PermiCon = new Dictionary<string, string>();
		public static void PermissonInit()
		{
			PermiCon.Add("API[0]", "输出日志");
			PermiCon.Add("API[1]", "发送好友消息");
			PermiCon.Add("API[2]", "发送群消息");
			PermiCon.Add("API[3]", "发送群临时消息");
			PermiCon.Add("API[4]", "添加好友");
			PermiCon.Add("API[5]", "添加群");
			PermiCon.Add("API[6]", "删除好友");
			PermiCon.Add("API[7]", "置屏蔽好友");
			PermiCon.Add("API[8]", "置特别关心好友");
			PermiCon.Add("API[9]", "发送好友xml消息");
			PermiCon.Add("API[10]", "发送群xml消息");
			PermiCon.Add("API[11]", "发送好友json消息");
			PermiCon.Add("API[12]", "发送群json消息");
			PermiCon.Add("API[13]", "上传好友图片");
			PermiCon.Add("API[14]", "上传群图片");
			PermiCon.Add("API[15]", "上传好友语音");
			PermiCon.Add("API[16]", "上传群语音");
			PermiCon.Add("API[17]", "上传头像");
			PermiCon.Add("API[18]", "设置群名片");
			PermiCon.Add("API[19]", "取昵称_从缓存");
			PermiCon.Add("API[20]", "强制取昵称");
			PermiCon.Add("API[21]", "获取skey");
			PermiCon.Add("API[22]", "获取pskey");
			PermiCon.Add("API[23]", "获取clientkey");
			PermiCon.Add("API[24]", "取框架QQ");
			PermiCon.Add("API[25]", "取好友列表");
			PermiCon.Add("API[26]", "取群列表");
			PermiCon.Add("API[27]", "取群成员列表");
			PermiCon.Add("API[28]", "设置管理员");
			PermiCon.Add("API[29]", "取管理层列表");
			PermiCon.Add("API[30]", "取群名片");
			PermiCon.Add("API[31]", "取个性签名");
			PermiCon.Add("API[32]", "修改昵称");
			PermiCon.Add("API[33]", "修改个性签名");
			PermiCon.Add("API[34]", "删除群成员");
			PermiCon.Add("API[35]", "禁言群成员");
			PermiCon.Add("API[36]", "退群");
			PermiCon.Add("API[37]", "解散群");
			PermiCon.Add("API[38]", "上传群头像");
			PermiCon.Add("API[39]", "全员禁言");
			PermiCon.Add("API[40]", "群权限_发起新的群聊");
			PermiCon.Add("API[41]", "群权限_发起临时会话");
			PermiCon.Add("API[42]", "群权限_上传文件");
			PermiCon.Add("API[43]", "群权限_上传相册");
			PermiCon.Add("API[44]", "群权限_邀请好友加群");
			PermiCon.Add("API[45]", "群权限_匿名聊天");
			PermiCon.Add("API[46]", "群权限_坦白说");
			PermiCon.Add("API[47]", "群权限_新成员查看历史消息");
			PermiCon.Add("API[48]", "群权限_邀请方式设置");
			PermiCon.Add("API[49]", "撤回消息_群聊");
			PermiCon.Add("API[50]", "撤回消息_私聊本身");
			PermiCon.Add("API[51]", "设置位置共享");
			PermiCon.Add("API[52]", "上报当前位置");
			PermiCon.Add("API[53]", "是否被禁言");
			PermiCon.Add("API[54]", "处理好友验证事件");
			PermiCon.Add("API[55]", "处理群验证事件");
			PermiCon.Add("API[56]", "查看转发聊天记录内容");
			PermiCon.Add("API[57]", "上传群文件");
			PermiCon.Add("API[58]", "创建群文件夹");
			PermiCon.Add("API[59]", "设置在线状态");
			PermiCon.Add("API[60]", "QQ点赞");
			PermiCon.Add("API[61]", "取图片下载地址");
			PermiCon.Add("API[63]", "查询好友信息");
			PermiCon.Add("API[64]", "查询群信息");
			PermiCon.Add("API[65]", "框架重启");
			PermiCon.Add("API[66]", "群文件转发至群");
			PermiCon.Add("API[67]", "群文件转发至好友");
			PermiCon.Add("API[68]", "好友文件转发至好友");
			PermiCon.Add("API[69]", "置群消息接收");
			PermiCon.Add("API[70]", "取群名称_从缓存");
			PermiCon.Add("API[71]", "发送免费礼物");
			PermiCon.Add("API[72]", "取好友在线状态");
			PermiCon.Add("API[73]", "取QQ钱包个人信息");
			PermiCon.Add("API[74]", "获取订单详情");
			PermiCon.Add("API[75]", "提交支付验证码");
			PermiCon.Add("API[77]", "分享音乐");
			PermiCon.Add("API[78]", "更改群聊消息内容");
			PermiCon.Add("API[79]", "更改私聊消息内容");
			PermiCon.Add("API[80]", "群聊口令红包");
			PermiCon.Add("API[81]", "群聊拼手气红包");
			PermiCon.Add("API[82]", "群聊普通红包");
			PermiCon.Add("API[83]", "群聊画图红包");
			PermiCon.Add("API[84]", "群聊语音红包");
			PermiCon.Add("API[85]", "群聊接龙红包");
			PermiCon.Add("API[86]", "群聊专属红包");
			PermiCon.Add("API[87]", "好友口令红包");
			PermiCon.Add("API[88]", "好友普通红包");
			PermiCon.Add("API[89]", "好友画图红包");
			PermiCon.Add("API[90]", "好友语音红包");
			PermiCon.Add("API[91]", "好友接龙红包");
			PermiCon.Add("API[92]", "重命名群文件夹");
			PermiCon.Add("API[93]", "删除群文件夹");
			PermiCon.Add("API[94]", "删除群文件");
			PermiCon.Add("API[95]", "保存文件到微云");
			PermiCon.Add("API[96]", "移动群文件");
			PermiCon.Add("API[97]", "取群文件列表");
			PermiCon.Add("API[98]", "设置专属头衔");
			PermiCon.Add("API[99]", "下线指定QQ");
			PermiCon.Add("API[100]", "登录指定QQ");
			PermiCon.Add("API[101]", "取群未领红包");
			PermiCon.Add("API[102]", "发送输入状态");
			PermiCon.Add("API[103]", "修改资料");
			PermiCon.Add("API[104]", "打好友电话");
			PermiCon.Add("API[105]", "取群文件下载地址");
			PermiCon.Add("API[106]", "头像双击_好友");
			PermiCon.Add("API[107]", "头像双击_群");
			PermiCon.Add("API[108]", "取群成员简略信息");
			PermiCon.Add("API[109]", "群聊置顶");
			PermiCon.Add("API[110]", "私聊置顶");
			PermiCon.Add("API[111]", "取加群链接");
			PermiCon.Add("API[112]", "设为精华");
			PermiCon.Add("API[113]", "群权限_设置群昵称规则");
			PermiCon.Add("API[114]", "群权限_设置群发言频率");
			PermiCon.Add("API[115]", "群权限_设置群查找方式");
			PermiCon.Add("API[116]", "邀请好友加群");
			PermiCon.Add("API[117]", "置群内消息通知");
			PermiCon.Add("API[118]", "修改群名称");
			PermiCon.Add("API[119]", "下线PCQQ");
			PermiCon.Add("API[120]", "登录网页取ck");
			PermiCon.Add("API[121]", "发送群公告");
			PermiCon.Add("API[122]", "取群成员信息");
			PermiCon.Add("API[123]", "发送邮件");
			PermiCon.Add("API[124]", "取钱包cookie");
			PermiCon.Add("API[125]", "取群网页cookie");
			PermiCon.Add("API[126]", "取手Q邮箱cookie");
			PermiCon.Add("API[127]", "转账");
			PermiCon.Add("API[128]", "余额提现");
			PermiCon.Add("API[129]", "取收款链接");
			PermiCon.Add("API[130]", "取群小视频下载地址");
			PermiCon.Add("API[131]", "取私聊小视频下载地址");
			PermiCon.Add("API[132]", "上传小视频");
			PermiCon.Add("API[133]", "取群成员概况");
			PermiCon.Add("API[134]", "添加好友_取验证类型");
			PermiCon.Add("API[135]", "群聊打卡");
			PermiCon.Add("API[136]", "群聊签到");
			PermiCon.Add("API[137]", "置群聊备注");
			PermiCon.Add("API[138]", "红包转发");
			PermiCon.Add("API[139]", "发送数据包");
			PermiCon.Add("API[140]", "请求ssoseq");
			PermiCon.Add("API[141]", "取sessionkey");
			PermiCon.Add("API[142]", "获取bkn_gtk");
			PermiCon.Add("API[143]", "置好友验证方式");
			PermiCon.Add("API[144]", "上传照片墙图片");
			PermiCon.Add("API[145]", "付款");
		}

		public enum ApiPermissionEnum //Api权限列表
		{
			[Description("输出日志")]
			ApiPermission0,
			[Description("发送好友消息")]
			ApiPermission1,
			[Description("发送群消息")]
			ApiPermission2,
			[Description("发送群临时消息")]
			ApiPermission3,
			[Description("添加好友")]
			ApiPermission4,
			[Description("添加群")]
			ApiPermission5,
			[Description("删除好友")]
			ApiPermission6,
			[Description("置屏蔽好友")]
			ApiPermission7,
			[Description("置特别关心好友")]
			ApiPermission8,
			[Description("发送好友json消息")]
			ApiPermission9,
			[Description("发送群json消息")]
			ApiPermission10,
			[Description("上传好友图片")]
			ApiPermission11,
			[Description("上传群图片")]
			ApiPermission12,
			[Description("上传好友语音")]
			ApiPermission13,
			[Description("上传群语音")]
			ApiPermission14,
			[Description("上传头像")]
			ApiPermission15,
			[Description("设置群名片")]
			ApiPermission16,
			[Description("取昵称_从缓存")]
			ApiPermission17,
			[Description("强制取昵称")]
			ApiPermission18,
			[Description("获取skey")]
			ApiPermission19,
			[Description("获取pskey")]
			ApiPermission20,
			[Description("获取clientkey")]
			ApiPermission21,
			[Description("取框架QQ")]
			ApiPermission22,
			[Description("取好友列表")]
			ApiPermission23,
			[Description("取群列表")]
			ApiPermission24,
			[Description("取群成员列表")]
			ApiPermission25,
			[Description("设置管理员")]
			ApiPermission26,
			[Description("取管理层列表")]
			ApiPermission27,
			[Description("取群名片")]
			ApiPermission28,
			[Description("取个性签名")]
			ApiPermission29,
			[Description("修改昵称")]
			ApiPermission30,
			[Description("修改个性签名")]
			ApiPermission31,
			[Description("删除群成员")]
			ApiPermission32,
			[Description("禁言群成员")]
			ApiPermission33,
			[Description("退群")]
			ApiPermission34,
			[Description("解散群")]
			ApiPermission35,
			[Description("上传群头像")]
			ApiPermission36,
			[Description("全员禁言")]
			ApiPermission37,
			[Description("群权限_发起新的群聊")]
			ApiPermission38,
			[Description("群权限_发起临时会话")]
			ApiPermission39,
			[Description("群权限_上传文件")]
			ApiPermission40,
			[Description("群权限_上传相册")]
			ApiPermission41,
			[Description("群权限_邀请好友加群")]
			ApiPermission42,
			[Description("群权限_匿名聊天")]
			ApiPermission43,
			[Description("群权限_坦白说")]
			ApiPermission44,
			[Description("群权限_新成员查看历史消息")]
			ApiPermission45,
			[Description("群权限_邀请方式设置")]
			ApiPermission46,
			[Description("撤回消息_群聊")]
			ApiPermission47,
			[Description("撤回消息_私聊本身")]
			ApiPermission48,
			[Description("设置位置共享")]
			ApiPermission49,
			[Description("上报当前位置")]
			ApiPermission50,
			[Description("是否被禁言")]
			ApiPermission51,
			[Description("处理群验证事件")]
			ApiPermission52,
			[Description("处理好友验证事件")]
			ApiPermission53,
			[Description("查看转发聊天记录内容")]
			ApiPermission54,
			[Description("上传群文件")]
			ApiPermission55,
			[Description("创建群文件夹")]
			ApiPermission56,
			[Description("设置在线状态")]
			ApiPermission57,
			[Description("api是否有权限")]
			ApiPermission58,
			[Description("重载自身")]
			ApiPermission59,
			[Description("取插件数据目录")]
			ApiPermission60,
			[Description("QQ点赞")]
			ApiPermission61,
			[Description("取图片下载地址")]
			ApiPermission62,
			[Description("查询好友信息")]
			ApiPermission63,
			[Description("查询群信息")]
			ApiPermission64,
			[Description("框架重启")]
			ApiPermission65,
			[Description("群文件转发至群")]
			ApiPermission66,
			[Description("群文件转发至好友")]
			ApiPermission67,
			[Description("好友文件转发至好友")]
			ApiPermission68,
			[Description("置群消息接收")]
			ApiPermission69,
			[Description("取群名称_从缓存")]
			ApiPermission70,
			[Description("发送免费礼物")]
			ApiPermission71,
			[Description("取好友在线状态")]
			ApiPermission72,
			[Description("取QQ钱包个人信息")]
			ApiPermission73,
			[Description("获取订单详情")]
			ApiPermission74,
			[Description("提交支付验证码")]
			ApiPermission75,
			[Description("分享音乐")]
			ApiPermission76,
			[Description("更改群聊消息内容")]
			ApiPermission77,
			[Description("更改私聊消息内容")]
			ApiPermission78,
			[Description("群聊口令红包")]
			ApiPermission79,
			[Description("群聊拼手气红包")]
			ApiPermission80,
			[Description("群聊普通红包")]
			ApiPermission81,
			[Description("群聊画图红包")]
			ApiPermission82,
			[Description("群聊语音红包")]
			ApiPermission83,
			[Description("群聊接龙红包")]
			ApiPermission84,
			[Description("群聊专属红包")]
			ApiPermission85,
			[Description("好友口令红包")]
			ApiPermission86,
			[Description("好友普通红包")]
			ApiPermission87,
			[Description("好友画图红包")]
			ApiPermission88,
			[Description("好友语音红包")]
			ApiPermission89,
			[Description("好友接龙红包")]
			ApiPermission90,
			[Description("重命名群文件夹")]
			ApiPermission91,
			[Description("删除群文件夹")]
			ApiPermission92,
			[Description("删除群文件")]
			ApiPermission93,
			[Description("保存文件到微云")]
			ApiPermission94,
			[Description("移动群文件")]
			ApiPermission95,
			[Description("取群文件列表")]
			ApiPermission96

		}


		#endregion
		public class MyData
		{
			public Needapilist PermissionList;
		}

		public class Needapilist
		{
			public string state { get; set; }
			public string safe { get; set; }
			public string desc { get; set; }

		}

		

		[DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		internal extern static IntPtr GetCurrentProcess();

		[DllImport("psapi.dll")]
		public extern static bool EmptyWorkingSet(IntPtr hProcess);
		[DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		internal extern static bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);



		public static string GetAt(long qq)
		{
			return $"[@{qq.ToString()}]";
		}
		public static string GetAtAll() // 艾特全体
		{
			return "[@all]";
		}
		public static string GetFace(int id, string name) // 表情
		{
			return $"[Face,Id={id},name={name}]";
		}
		public static string GetFace(int id, string name, string hash, string flag) // 大表情
		{
			return $"[bigFace,Id={id},name={name},hash={hash},flag={flag}]";
		}
		public static string GetSmallFace(int id, string name) // 小表情
		{
			return $"[smallFace,name={name},Id={id}]";
		}
		public static string GetShake(string name, int id, int type) // 抖一抖
		{
			return $"[Shake,name={name} ,Type={type},Id={id}]";
		}
		public static string GetLimiShow(string name, int id, int type, long QQ) // 厘米秀 目前不支持3D消息
		{
			return $"[limiShow,Id={id},name={name},type={type},object={QQ}]";
		}
		public static string GetFlashPicFile(string path) // 闪照_本地
		{
			return $"[flashPicFile,path={path}]";
		}
		public static string GetFlashWord(string desc, string resid, string prompt) // 闪字
		{
			return $"[flashWord,Desc={desc},Resid={resid},Prompt={prompt}]";
		}
		public static string GetHonest(long QQ, string name, string desc, string time, string Random, string backgroundtype) // 坦白说
		{
			return $"[Honest,ToUin={QQ},ToNick={name},Desc={desc},Time={time},Random={Random},Bgtype={backgroundtype}]";
		}
		public static string GetPicFile(string path) // 图片_本地
		{
			return $"[picFile,path={path}]";
		}
		public static string GetGraffiti(int id, string hash, string address) // 涂鸦
		{
			return $"[Graffiti,ModelId={id.ToString()},hash={hash},url={address}]";
		}
		public static string Getbq(int id) // 小黄豆表情
		{
			return $"[bq{id}]";
		}
		public static string GetLitleVideo(string linkParam, string hash1, string hash2) // 小视频
		{
			return $"[litleVideo,linkParam={linkParam},hash1={hash1},hash2={hash2}]";
		}
		public static string GetAudioFile(string path) // 语音_本地 必须是silk_v3编码的文件
		{
			return $"[AudioFile,path={path}]";
		}

		public struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;
			public IntPtr AllocationBase;
			public AllocationProtectEnum AllocationProtect;
			public IntPtr RegionSize;
			public StateEnum State;
			public AllocationProtectEnum Protect;
			public TypeEnum Type;
		}
		public enum AllocationProtectEnum : uint
		{
			PAGE_EXECUTE = 0x00000010,
			PAGE_EXECUTE_READ = 0x00000020,
			PAGE_EXECUTE_READWRITE = 0x00000040,
			PAGE_EXECUTE_WRITECOPY = 0x00000080,
			PAGE_NOACCESS = 0x00000001,
			PAGE_READONLY = 0x00000002,
			PAGE_READWRITE = 0x00000004,
			PAGE_WRITECOPY = 0x00000008,
			PAGE_GUARD = 0x00000100,
			PAGE_NOCACHE = 0x00000200,
			PAGE_WRITECOMBINE = 0x00000400
		}

		public enum StateEnum : uint
		{
			MEM_COMMIT = 0x1000,
			MEM_FREE = 0x10000,
			MEM_RESERVE = 0x2000
		}

		public enum TypeEnum : uint
		{
			MEM_IMAGE = 0x1000000,
			MEM_MAPPED = 0x40000,
			MEM_PRIVATE = 0x20000
		}
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpLibFileNmae);

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool GetModuleHandleEx(UInt32 dwFlags, string lpModuleName, out IntPtr phModule);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetModuleFileNameA(IntPtr hModule, StringBuilder lpFileName, int nSize);
		public const uint GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 0x00000004;
		public const uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002;
		[DllImport("kernel32.dll")]
		public static extern int VirtualQuery(	ref IntPtr lpAddress,	ref MEMORY_BASIC_INFORMATION lpBuffer,	int dwLength);
		[DllImport("Kernel32.dll")]
		public static extern void FreeLibraryAndExitThread(IntPtr hLibModule, int dwExitCode);
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32.dll")]
		public static extern Int32 WideCharToMultiByte(UInt32 CodePage, UInt32 dwFlags, [MarshalAs(UnmanagedType.LPWStr)] String lpWideCharStr, Int32 cchWideChar, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpMultiByteStr, Int32 cbMultiByte, IntPtr lpDefaultChar, IntPtr lpUsedDefaultChar);
	}
}
