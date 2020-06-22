﻿using Common;
using Common.Wrappers;
using TwitchChat.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TwitchChat
{
	public class TwitchChat : MonoBehaviour
	{
		private TwitchIRC irc;

		private static int guiX = Screen.width / 2 + Screen.width / 4;
		private static int guiWidth = Screen.width - guiX;
		private static string textFieldString;
		private static string twitchChat = "";

		private GUIStyle settingsWindowStyle;
		private GUIStyle settingsToggleStyle;
		private GUIStyle settingsButtonStyle;
		private GUIStyle settingsTextAreaStyle;
		private GUIStyle settingsTextFieldStyle;
		private GUIStyle settingsLabelStyle;
		private GUIStyle settingsBoxStyle;
		private GUIStyle settingsHorizontalSliderStyle;
		private GUIStyle settingsHorizontalSliderThumbStyle;
		private Vector2 settingsScrollPosition;
		private Vector2 scrollPosition;
		GUIStyle twitchStyle;
		private float update;
		private float textHeight;
		private bool textChanged;

		private Config config;
		

		#region Unity Methods
		void Start()
		{
			config = Config.LoadConfig();
			update = 0.0f;

			irc = new TwitchIRC(
				"irc.chat.twitch.tv",
				6667,
				config.TwitchChannel
			);

			irc.Initilize();
			irc.MessageReceived += OnMessageRecieved;
			textFieldString = irc.channel;
		}

		void OnGUI()
		{
			if (config.Enabled)
			{
				twitchStyle.fontSize = config.FontSize;
				textHeight = twitchStyle.CalcHeight(new GUIContent(twitchChat), guiWidth-20.0f);
				if (textChanged)
				{
					scrollPosition = new Vector2(0, textHeight);
					textChanged = false;
				}
				GUI.BeginGroup(new Rect(guiX, 0, guiWidth, Screen.height));
				GUI.Box(new Rect(0, 0, guiWidth, Screen.height), "Twitch Chat");
				scrollPosition = GUI.BeginScrollView(new Rect(10, 20, guiWidth-20, Screen.height-40), scrollPosition, new Rect(0, 0, guiWidth-40, textHeight), false, true);
				GUI.Label(new Rect(0, 0, guiWidth - 40, Screen.height - 80), twitchChat, twitchStyle);
				GUI.EndScrollView();
				GUI.EndGroup();
			}
			
			if (settingsWindowStyle is null)
			{
				settingsWindowStyle = new GUIStyle(GUI.skin.window);
				settingsToggleStyle = new GUIStyle(GUI.skin.toggle);
				settingsButtonStyle = new GUIStyle(GUI.skin.button);
				settingsTextAreaStyle = new GUIStyle(GUI.skin.textArea);
				settingsTextFieldStyle = new GUIStyle(GUI.skin.textField);
				settingsLabelStyle = new GUIStyle(GUI.skin.label);
				settingsBoxStyle = new GUIStyle(GUI.skin.box);
				settingsHorizontalSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
				settingsHorizontalSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);

				twitchStyle = new GUIStyle();
				twitchStyle.wordWrap = true;
				twitchStyle.richText = true;
			}
			if (config.ConfigWindowEnabled)
			{
				config.DrawLabelWindows();
				var outputRect = GUILayout.Window(187001001, new Rect(config.ConfigX, config.ConfigY, 320.0f, 500.0f), OnWindow, new GUIContent("Twitch Chat Settings"), settingsWindowStyle);
				config.ConfigX = outputRect.x;
				config.ConfigY = outputRect.y;
			}
		}
		void LateUpdate()
		{
			update += Time.deltaTime;
			if (update > config.UpdateSpeed)
			{
				update = 0.0f;
				if (irc.currentChannel != config.TwitchChannel)
				{
					irc.channel = config.TwitchChannel;
					twitchChat = "";
				}
				irc.Update();
				
			}
			config.HandleInput();
		}

		#endregion
		void OnMessageRecieved(object sender, MessageRecievedArgs e)
		{
			Dictionary<string, string> messageDict = e.MessageDictionary;
			twitchChat += $"<b><color={messageDict["color"]}FF>{messageDict["username"]}</color></b><color=#FFFFFFF>: {messageDict["message"]}</color>\n";
			textChanged = true;
		}
		private void OnWindow(int id)
		{
			var largeLabelStyle = new GUIStyle
			{
				fontSize = 20,
				alignment = TextAnchor.UpperLeft,
				fontStyle = FontStyle.Bold,
				normal = new GUIStyleState
				{
					textColor = Color.white,
				}
			};
			var smallLabelStyle = new GUIStyle
			{
				fontSize = 14,
				alignment = TextAnchor.UpperLeft,
				normal = new GUIStyleState
				{
					textColor = Color.white,
				}
			};
			settingsScrollPosition = GUILayout.BeginScrollView(settingsScrollPosition);
			config.ConfigureGUI(new Common.Settings.GUIConfigurationStyles
			{
				LargeLabel = largeLabelStyle,
				SmallLabel = smallLabelStyle,
				Window = settingsWindowStyle,
				Toggle = settingsToggleStyle,
				Button = settingsButtonStyle,
				TextArea = settingsTextAreaStyle,
				TextField = settingsTextFieldStyle,
				Label = settingsLabelStyle,
				Box = settingsBoxStyle,
				HorizontalSlider = settingsHorizontalSliderStyle,
				HorizontalSliderThumb = settingsHorizontalSliderThumbStyle
			});
			GUILayout.Space(25.0f);

			GUILayout.Label($"Twitch Chat v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
			GUILayout.Label("Tweak by Joosthoi1");
			GUILayout.EndScrollView();
			GUI.DragWindow();
		}


	}
}