using BiendeoCHLib.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace MoveLyrics.Settings
{
    public class BarePositionableLabel
    {
		public int X;
		public int Y;

		[XmlIgnore]
		public bool DraggableWindowsEnabled;

		public void DrawLabelWindow(int id)
		{
			var r = GUILayout.Window(id, new Rect(X, Y, 20.0f, 30.0f), DraggableWindow, string.Empty);
			X = (int)r.x;
			Y = (int)r.y;
		}

		public virtual void ConfigureGUI(GUIConfigurationStyles styles)
		{
			if (!DraggableWindowsEnabled)
			{
				GUILayout.Label("X", styles.SmallLabel);
				X = (int)GUILayout.HorizontalSlider(X, -3840.0f, 3840.0f, styles.HorizontalSlider, styles.HorizontalSliderThumb);
				if (int.TryParse(GUILayout.TextField(X.ToString(), styles.TextField), out int x)) X = x;
			}
			else
			{
				GUILayout.Label($"X - {X.ToString()}", styles.SmallLabel);
			}
			if (!DraggableWindowsEnabled)
			{
				GUILayout.Label("Y", styles.SmallLabel);
				Y = (int)GUILayout.HorizontalSlider(Y, -2160, 2160.0f, styles.HorizontalSlider, styles.HorizontalSliderThumb);
				if (int.TryParse(GUILayout.TextField(Y.ToString(), styles.TextField), out int y)) Y = y;
			}
			else
			{
				GUILayout.Label($"Y - {Y.ToString()}", styles.SmallLabel);
			}
		}

		private void DraggableWindow(int id)
		{
			GUI.DragWindow();
		}

		public Rect Rect => new Rect(X, Y, 0.1f, 0.1f);

		public virtual GUIStyle Style => new GUIStyle
		{
		};
	}
}
