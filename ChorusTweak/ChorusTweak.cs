using System;
using Common;
using Common.Wrappers;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Common.Settings;
using System.Threading;
using System.Collections;
using static UnityEngine.GUI;

namespace ChorusTweak
{
	class ChorusTweak : MonoBehaviour
	{
		private bool sceneChanged = true;
		public bool showArchiveError = false;
		public bool downloadedSong = false;
		public string archiveLocation;

		private bool scanning;
		private string mainText;
		private string folderText;
		private string countText;
		private string errorText;
		private string badSongsText;

		private GameObject BackgroundImg;
		private GameObject headerBackgroundImg;
		private GameObject headerForegroundImg;
		private GameObject footerForegroundImg;
		private GameObject chorusText;
		private GameObject advButton;
		private GameObject randButton;

		private KeyBind chorusKeybind = new KeyBind()
		{
			Key = KeyCode.C,
			Ctrl = true,
			Alt = false,
			Shift = true
		};
		private bool chorusEnabled;
		private Songs chorusResults;

		private GUIStyle textFieldStyle;

		private string textFieldString;
		private Vector2 scrollPosition = Vector2.zero;

		private List<GameObject> contentImage = new List<GameObject>();
		private List<GameObject> contentDownload = new List<GameObject>();
		private List<GameObject> songName = new List<GameObject>();
		private List<GameObject> songArtist = new List<GameObject>();
		private List<GameObject> songAlbum = new List<GameObject>();
		private List<GameObject> songTime = new List<GameObject>();

		private Font uiFont;
		private Transform canvasTransform;
        #region Unity Functions
        void Start()
		{
			SceneManager.activeSceneChanged += delegate (Scene _, Scene __)
			{
				sceneChanged = true;
			};
		}
		void LateUpdate()
		{
			string sceneName = SceneManager.GetActiveScene().name;
			if (uiFont is null && sceneName == "Main Menu")
			{
				uiFont = GameObject.Find("Profile Title").GetComponent<Text>().font;
			}
			if (this.sceneChanged)
			{
				this.sceneChanged = false;
				if (sceneName == "Main Menu")
				{
					canvasTransform = GameObject.Find("FadeCanvas").transform;
					canvasTransform.gameObject.AddComponent<GraphicRaycaster>();

					Debug.Log(canvasTransform.name);

					//img = CreateImage(canvasTransform, "image232323", );
					//x = CreateGameplayLabel(canvasTransform, "namme", uiFont);
				}
				else if (chorusEnabled)
				{
					chorusEnabled = false;
					DestroyChorus();
					if (downloadedSong)
					{
						downloadedSong = false;
						//var cache = new CacheWrapper();
						//cache.ScanSongsFast();
					}
				}
			}
			if (sceneName == "Main Menu" && chorusEnabled)
			{
				UpdateChorus();
			}
			HandleInput();
		}

		void OnGUI()
		{
			if (chorusEnabled)
			{
				if (textFieldStyle == null)
				{
					textFieldStyle = new GUIStyle(GUI.skin.textField);
				}
				if (showArchiveError)
				{
					GUI.Box(new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 4, Screen.height / 5), "Warning!");
					GUI.Label(new Rect(Screen.width / 3, (float)Screen.height *(3.0f/8.0f), Screen.width / 4, Screen.height / 4), $"Archive files not currently supported, please unpack it manually at {archiveLocation}");
					if (GUI.Button(new Rect(Screen.width / 3+ Screen.width / 8, (float)Screen.height /2, 40, 20), "Ok"))
					{
						showArchiveError = false;
					}
				}
				textFieldStyle.fontSize = Screen.height / 54;
				//UpdateButton(new Rect((float)Screen.width/1.47f,(float)Screen.height*(1.0f/8.0f)+ (float)Screen.height * (1.0f / 20.0f)-15, 120,30), "Advanced Search", () => x(2));
				//UpdateButton(new Rect((float)Screen.width/1.30f,(float)Screen.height*(1.0f/8.0f)+ (float)Screen.height * (1.0f / 20.0f)-15, 120,30), "Randomizer", () => x(2));
				GUI.SetNextControlName("TextField");
				textFieldString = GUI.TextField(new Rect((float)Screen.width / 3, (float)Screen.height * (1.0f / 8.0f) + (float)Screen.height * (1.0f / 20.0f) - Screen.height / 72, (float)Screen.width / 3.0f, Screen.height / 36), textFieldString, textFieldStyle);
				if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "TextField")
				{
					DestroyScrollContent();
					chorusResults = ChorusAPI.CSearch(search: textFieldString);
					createScrollContent();
				}
				//Debug.Log(GUI.GetNameOfFocusedControl());
				scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 4 * 2, Screen.height / 4 * 2), scrollPosition, new Rect(0, 0, Screen.width / 4 * 2 - 20, Screen.height / 10 * chorusResults.songs.Count), false, true);
				GUI.EndScrollView();
			}
		}
        #endregion
        #region button methods
		void Randomizer()
		{
			DestroyScrollContent();
			chorusResults = ChorusAPI.CSearch(type: 2);
			createScrollContent();
		}
        #endregion
        #region create update structures
        void CreateChorus()
		{
			chorusResults = ChorusAPI.CSearch(type: 1);
			BackgroundImg = CreateImage(canvasTransform, "background", 0);
			headerBackgroundImg = CreateImage(canvasTransform, "headerbackground", 2);
			footerForegroundImg = CreateImage(canvasTransform, "footerForeground", 2);
			headerForegroundImg = CreateImage(canvasTransform, "headerForeground", 4);
			chorusText = CreateLabel(canvasTransform, "chorusLabel", uiFont, 5);
			chorusText.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
			//CreateButton();
			advButton = CreateTextButton(headerForegroundImg.transform, "advSearch", uiFont, 0, new Color32(204, 144, 144, 255), new Color32(204, 144, 144, 255));
			randButton = CreateTextButton(headerForegroundImg.transform, "randomizer", uiFont, 0, new Color32(204, 144, 144, 255), new Color32(204, 144, 144, 255));
			advButton.GetComponent<Button>().onClick.AddListener(() => Randomizer());
			randButton.GetComponent<Button>().onClick.AddListener(() => Randomizer());
			createScrollContent();
		}
		void UpdateChorus()
		{
			UpdateImage(BackgroundImg, new Color32(44, 62, 80, 255), new Vector2((float)Screen.width * (3.0f / 4.0f), (float)Screen.height * (3.0f / 4.0f)));
			UpdateImage(headerBackgroundImg, new Color32(44, 62, 80, 255), new Vector2((float)Screen.width * (3.0f / 4.0f), (float)Screen.height * (1.0f / 8.0f)));
			UpdateImage(footerForegroundImg, new Color32(44, 62, 80, 255), new Vector2((float)Screen.width * (3.0f / 4.0f), (float)Screen.height * (1.0f / 8.0f)));
			UpdateImage(headerForegroundImg, new Color32(52, 73, 94, 255), new Vector2((float)Screen.width * (3.0f / 4.0f), (float)Screen.height * (1.0f / 10.0f)));
			UpdateLabel(chorusText, "chorus", (int)Math.Floor((float)Screen.height / 12.705f), new Color32(243, 172, 7, 255));
			UpdateTextButton(advButton, "Advanced Search", (float)Screen.height / 54.0f, new Color32(255, 179, 0, 255));
			UpdateTextButton(randButton, "Randomizer", (float)Screen.height / 54.0f, new Color32(255, 179, 0, 255));
			advButton.transform.localPosition = new Vector2((float)Screen.width * (7.0f / 32.0f), 0);
			randButton.transform.localPosition = new Vector2((float)Screen.width * (5.0f / 16.0f), 0);

			headerBackgroundImg.transform.localPosition = new Vector3(0, (float)Screen.height * (3.0f / 8.0f) - (float)Screen.height * (1.0f / 16.0f));
			footerForegroundImg.transform.localPosition = new Vector3(0, -(float)Screen.height * (3.0f / 8.0f) + (float)Screen.height * (1.0f / 16.0f));
			headerForegroundImg.transform.localPosition = new Vector3(0, (float)Screen.height * (3.0f / 8.0f) - (float)Screen.height * (1.0f / 20.0f));
			chorusText.transform.localPosition = new Vector3(-(float)Screen.width / 3, (float)Screen.height * (3.0f / 8.0f) - (float)Screen.height * (1.0f / 20.0f));
			updateScrollContent();
		}
		void createScrollContent()
		{
			for (int i = 0; i < chorusResults.songs.Count; i++)
			{
				int index = i;
				contentImage.Add(CreateImage(canvasTransform, "contentImage", 1));
				contentDownload.Add(CreateTextButton(contentImage[i].transform, "downloadButton", uiFont, 0, new Color32(204, 144, 144, 255), new Color32(204, 144, 144, 255)));
				contentDownload[i].GetComponent<Button>().onClick.AddListener(() => ChorusAPI.DownloadFile(chorusResults.songs[index].directLinks, chorusResults.songs[index].name, this));
				songName.Add(CreateLabel(contentImage[i].transform, "songName", uiFont, 0));
				songAlbum.Add(CreateLabel(contentImage[i].transform, "songAlbum", uiFont, 0));
				songArtist.Add(CreateLabel(contentImage[i].transform, "songArtist", uiFont, 0));
				songTime.Add(CreateLabel(contentImage[i].transform, "songTime", uiFont, 0));
			}
		}
		void updateScrollContent()
		{
			bool dark = true;
			Color color;
			for (int i = 0; i < chorusResults.songs.Count; i++)
			{
				color = dark ? new Color32(58, 75, 92, 255) : new Color32(72, 88, 103, 255);
				dark = !dark;
				UpdateImage(contentImage[i], color, new Vector2(Screen.width / 2, Screen.height / 10));
				Vector2 pos = new Vector2(0, Screen.height / 5 + scrollPosition.y - (Screen.height / 10) * i);
				if (pos.y > Screen.height / 5 + Screen.height / 10 || pos.y < -(Screen.height / 5 + Screen.height / 10))
				{
					contentImage[i].SetActive(false);
				}
				else
				{
					contentImage[i].SetActive(true);
				}
				contentImage[i].transform.localPosition = pos;

				TimeSpan time = TimeSpan.FromSeconds(chorusResults.songs[i].length);
				TimeSpan effectiveTime = TimeSpan.FromSeconds(chorusResults.songs[i].effectivelength);

				string timeStr = time.ToString(chorusResults.songs[i].length > 3600 ? @"hh\:mm\:ss" : @"mm\:ss");
				string effectiveTimeStr = effectiveTime.ToString(chorusResults.songs[i].effectivelength > 3600 ? @"hh\:mm\:ss" : @"mm\:ss");

				UpdateLabel(songName[i], chorusResults.songs[i].name, Screen.height / 55, Color.white);
				UpdateLabel(songAlbum[i], String.IsNullOrEmpty(chorusResults.songs[i].album) ? "Unkown album" : chorusResults.songs[i].album, Screen.height / 60, new Color32(182, 188, 194, 255));
				UpdateLabel(songArtist[i], $"<b>{chorusResults.songs[i].artist}</b>", Screen.height / 50, Color.white);
				UpdateLabel(songTime[i], chorusResults.songs[i].length > 0 ? $"{timeStr} ({effectiveTimeStr})" : "", Screen.height / 60, new Color32(182, 188, 194, 255));
				UpdateTextButton(contentDownload[i], "Download", (float)Screen.height / 35, new Color32(255, 179, 0, 255));
				int width = Screen.width / 2;
				int height = Screen.height / 10;

				songName[i].transform.localPosition = new Vector2(-width / 2, height / 2 - Screen.height / 50);
				songAlbum[i].transform.localPosition = new Vector2(-width / 2, height / 2 - Screen.height / 55 - Screen.height / 50);
				songTime[i].transform.localPosition = new Vector2(-width / 2, height / 2 - Screen.height / 60 - Screen.height / 55 - Screen.height / 50);
				songArtist[i].transform.localPosition = new Vector2(-width / 2, height / 2);
				contentDownload[i].transform.localPosition = new Vector2((float)Screen.width * (2.0f / 12.0f), 0);

				//Debug.Log(scrollPosition.y);
			}
		}
        #endregion
        #region destroy structures
		private void DestroyScrollContent()
		{
			if (contentImage != null)
			{
				for (int i = 0; i < contentImage.Count; i++)
				{
					if (contentImage[i] != null) Destroy(contentImage[i]);
				}
			}
			contentImage = new List<GameObject>();
			contentDownload = new List<GameObject>();
			songName = new List<GameObject>();
			songArtist = new List<GameObject>();
			songAlbum = new List<GameObject>();
			songTime = new List<GameObject>();
			Vector2 scrollPosition = Vector2.zero;
		}
		private void DestroyChorus()
		{
			if (BackgroundImg != null) Destroy(BackgroundImg);
			if (headerBackgroundImg != null) Destroy(headerBackgroundImg);
			if (headerForegroundImg != null) Destroy(headerForegroundImg);
			if (footerForegroundImg != null) Destroy(footerForegroundImg);
			if (chorusText != null) Destroy(chorusText);
			if (advButton != null) Destroy(advButton);
			if (randButton != null) Destroy(randButton);
			BackgroundImg = null;
			headerBackgroundImg = null;
			headerForegroundImg = null;
			footerForegroundImg = null;
			chorusText = null;
			advButton = null;
			randButton = null;
			textFieldStyle = null;
			textFieldString = null;
			scrollPosition = Vector2.zero;

			DestroyScrollContent();
		}
        #endregion
        #region create update components

        private GameObject CreateLabel(Transform canvasTransform, string labelName, Font uiFont, int height)
		{
			var o = new GameObject(labelName, new Type[] {
						typeof(Text)
					});
			o.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);
			o.layer = LayerMask.NameToLayer("UI");
			o.transform.SetParent(canvasTransform);
			o.transform.SetSiblingIndex(height);
			o.transform.localEulerAngles = new Vector3();
			o.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			Text text = o.GetComponent<Text>();
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			text.font = uiFont;
			text.alignment = TextAnchor.UpperLeft;
			text.alignByGeometry = true;
			return o;
		}
		private void UpdateLabel(GameObject o, string content, int fontSize, Color color)
		{
			o.transform.localPosition = new Vector3(0 , 0, 0);
			var text = o.GetComponent<Text>();
			text.enabled = true;
			text.fontSize = fontSize;
			text.fontStyle = FontStyle.Normal;
			text.text = content;
			text.color = color;
		}
		private GameObject CreateImage(Transform canvasTransform, string labelName, int height)
		{
			var o = new GameObject(labelName, new Type[] {
						typeof(Image)
					});
			o.transform.SetParent(canvasTransform);
			o.transform.SetSiblingIndex(height);
			o.transform.localPosition = new Vector3(0, 0);

			return o;
		}
		void UpdateImage(GameObject o, Color color, Vector2 size)
		{
			Image image = o.GetComponent<Image>();
			image.color = color;
			image.rectTransform.sizeDelta = size;
		}
		void UpdateButton(Rect position, string text, Action method)
		{
			if(GUI.Button(position, text))
			{
				method();
			}
		}
		
		
		private GameObject CreateTextButton(Transform canvasTransform, string buttonName, Font uiFont, int height, Color highlightColor, Color pressedColor)
		{
			GameObject o = new GameObject("buttonName");
			o.transform.SetParent(canvasTransform);
			o.transform.SetSiblingIndex(height);
			o.AddComponent<RectTransform>();
			o.AddComponent<Text>();
			o.AddComponent<Button>();
			RectTransform rectTransform = o.GetComponent<RectTransform>();
			Text text = o.GetComponent<Text>();
			Button button = o.GetComponent<Button>();

			text.font = uiFont;
			text.alignment = TextAnchor.MiddleCenter;
			text.horizontalOverflow = HorizontalWrapMode.Overflow;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			text.alignByGeometry = true;
			text.color = Color.white;
			
			ColorBlock colorVar = button.colors;
			colorVar.highlightedColor = highlightColor;
			colorVar.pressedColor = pressedColor;
			button.colors = colorVar;
			Navigation nav = button.navigation;
			nav.mode = Navigation.Mode.None;
			button.navigation = nav;

			return o;
		}
		private void UpdateTextButton(GameObject o, string content, float fontSize, Color color)
		{
			Text text = o.GetComponent<Text>();
			text.text = content;
			text.color = color;
			text.fontSize = (int)fontSize;
			o.GetComponent<RectTransform>().sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
		}
		private GameObject CreateImageButton(Transform canvasTransform, string buttonName, Font uiFont, int height)
		{
			GameObject o = new GameObject("Button");
			o.transform.SetParent(canvasTransform);
			o.transform.SetSiblingIndex(height);
			o.AddComponent<RectTransform>();
			o.AddComponent<Image>();
			o.AddComponent<Button>();

			GameObject text = new GameObject("text");
			text.transform.SetParent(o.transform);
			text.AddComponent<Text>();

			o.transform.localPosition = new Vector2(0,0);
			text.transform.localPosition = new Vector2(0,0);

			text.GetComponent<Text>().font = uiFont;
			text.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
			text.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
			text.GetComponent<Text>().color = Color.black;
			text.GetComponent<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
			text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

			o.GetComponent<Button>().transform.localPosition = new Vector2(0, 0);
			//button.GetComponent<Button>().
			return o;
		}
		private void UpdateImageButton(GameObject o, string content, float fontSize, Color imageColor, Color textColor)
		{
			Text text = o.GetComponentInChildren<Text>();
			text.text = content;
			text.fontSize = (int)fontSize;
			text.color = textColor;

			o.GetComponent<RectTransform>().sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
			o.GetComponent<Image>().color = imageColor;
		}
        #endregion
        #region utility
        public void HandleInput()
		{
			if (chorusKeybind.IsPressed && !chorusKeybind.JustSet)
			{
				chorusEnabled = !chorusEnabled;
				if (chorusEnabled)
				{
					CreateChorus();
				}
				else
				{
					DestroyChorus();
					StartCoroutine(ScanSongs());
				}
			}
			chorusKeybind.JustSet = false;
		}
		private IEnumerator ScanSongs()
		{
			scanning = true;
			Debug.Log("test0");
			var cache = new CacheWrapper();
			Debug.Log("test1");
			var thread = new Thread(cache.ScanSongsFull);
			Debug.Log("test2");
			thread.Start();
			while (thread.IsAlive)
			{
				mainText = cache.cacheState.ToString();
				folderText = $"{cache.Int4 - cache.Int1} Folders";
				countText = $"{SongDirectoryWrapper.setlistSongEntries.Count} Songs Scanned";
				errorText = $"{cache.exceptions.Count} Errors";
				badSongsText = $"{cache.Int3} Bad Songs";
				yield return null;
			}
			SongDirectoryWrapper.sortCounter = -1;
			SongDirectoryWrapper.Sort(null, false);
			scanning = false;
		}
		#endregion

	}
}
