using System.Reflection;
using System;
using UnityEngine;

namespace ChorusTweak
{
	public class Loader
	{
		public void LoadTweak()
		{
			if (this.gameObject != null)

			{
				return;
			}
			this.gameObject = new GameObject("Joost Tweak - Chorus", new Type[]
			{
				typeof(ChorusTweak)
			});
			UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
			this.gameObject.SetActive(true);
		}

		public void UnloadTweak()
		{
			if (this.gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(this.gameObject);
				this.gameObject = null;
			}
		}

		private GameObject gameObject;
	}
}