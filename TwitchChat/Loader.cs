using Common.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TwitchChat
{
	public class Loader
	{
		public void LoadTweak()
		{
			WrapperBase.InitializeLoaders();
			if (this.gameObject != null)

			{
				return;
			}
			this.gameObject = new GameObject("Joost Tweak - Twitch Chat", new Type[]
			{
				typeof(TwitchChat)
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
