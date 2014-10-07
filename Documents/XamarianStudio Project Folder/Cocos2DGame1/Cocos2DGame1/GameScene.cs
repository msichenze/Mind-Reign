using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace MindRain
{
	public class GameScene: CCScene
	{
		//Note: Identifies the game layer scene for quick access later
		public const int gameLayerTag = 1;
		public const int gameInterfaceTag = 2;
		public GameScene ()
		{
			var backgroundLayer = new BackgroundLayer ();
			AddChild(backgroundLayer,0);

			var spriteLayer = new GameLayer ();
			AddChild (spriteLayer,1, gameLayerTag);

			var interfaceLayer = new GameInterfaceLayer ();
			AddChild (interfaceLayer, 2, gameInterfaceTag);
		}
	}
}

