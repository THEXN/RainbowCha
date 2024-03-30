using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RainbowChat;

[ApiVersion(2, 1)]
public class RainbowChat : TerrariaPlugin
{
	private Random _rand = new Random();

	private bool[] _rainbowChat = new bool[255];

	public override string Name => "Rainbow Chat";

	public override string Author => "Professor X制作,nnt升级/汉化,肝帝熙恩更新1449";

	public override string Description => "使玩家每次说话的颜色不一样.";

	public override Version Version => new Version(1, 0, 1);

	public RainbowChat(Main game)
		: base(game)
	{
	}

    public override void Initialize()
    {
        // 注册事件
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 注销事件
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
        }

        // 调用基类的Dispose方法
        base.Dispose(disposing);
    }



    private void OnInitialize(EventArgs args)
	{
		Commands.ChatCommands.Add(new Command("rainbowchat.use", RainbowChatCallback, "rainbowchat", "rc"));
	}

	private void OnChat(ServerChatEventArgs e)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEventArgs)(object)e).Handled)
		{
			return;
		}
		TSPlayer tSPlayer = TShock.Players[e.Who];
		if (tSPlayer != null && tSPlayer.HasPermission(Permissions.canchat) && !tSPlayer.mute && !e.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) && !e.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier) && _rainbowChat[tSPlayer.Index])
		{
			List<Color> colors = GetColors();
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = e.Text.Split(' ');
			string[] array2 = array;
			foreach (string text in array2)
			{
				stringBuilder.Append(TShock.Utils.ColorTag(text, colors[_rand.Next(colors.Count)]) + " ");
			}
			string msg = string.Format(TShock.Config.Settings.ChatFormat, tSPlayer.Group.Name, tSPlayer.Group.Prefix, tSPlayer.Name, tSPlayer.Group.Suffix, stringBuilder.ToString());
			TSPlayer.All.SendMessage(msg, tSPlayer.Group.R, tSPlayer.Group.G, tSPlayer.Group.B);
			TSPlayer.Server.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, tSPlayer.Group.Name, tSPlayer.Group.Prefix, tSPlayer.Name, tSPlayer.Group.Suffix, e.Text), tSPlayer.Group.R, tSPlayer.Group.G, tSPlayer.Group.B);
			((HandledEventArgs)(object)e).Handled = true;
		}
	}

	private void RainbowChatCallback(CommandArgs e)
	{
		_rainbowChat[e.Player.Index] = !_rainbowChat[e.Player.Index];
		if (e.Parameters.Count == 0)
		{
			e.Player.SendSuccessMessage(string.Format("你 {0} 彩虹聊天.", _rainbowChat[e.Player.Index] ? "已开启" : "已关闭"));
			return;
		}
		List<TSPlayer> list = TSPlayer.FindByNameOrID(e.Parameters[0]);
		if (list.Count == 0)
		{
			e.Player.SendErrorMessage("错误的玩家!");
		}
		else if (list.Count > 1)
		{
			e.Player.SendMultipleMatchError(list.Select((TSPlayer p) => p.Name));
		}
		else
		{
			_rainbowChat[list[0].Index] = !_rainbowChat[list[0].Index];
			e.Player.SendSuccessMessage(string.Format("{0} 已 {1} 彩虹聊天.", list[0].Name, _rainbowChat[list[0].Index] ? "已开启" : "已关闭"));
		}
	}

	private List<Color> GetColors()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		List<Color> list = new List<Color>();
		PropertyInfo[] properties = typeof(Color).GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			if (propertyInfo.PropertyType == typeof(Color))
			{
				list.Add((Color)propertyInfo.GetValue(null));
			}
		}
		return list;
	}
}
