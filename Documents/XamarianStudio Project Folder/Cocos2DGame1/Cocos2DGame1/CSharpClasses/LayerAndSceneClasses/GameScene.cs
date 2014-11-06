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
		Camera camera = new MindRain.Camera ();

		public GameScene ()
		{
			var backgroundLayer = new BackgroundLayer ();
			AddChild(backgroundLayer,0);

			var spriteLayer = new GameLayer ();
			AddChild (spriteLayer,1, gameLayerTag);
		
			//Note: This camera only works because every sprite in a character
			//is positionaly updated every iteration of the game loop.
			//This fact could change and then the camera would need to change.
			var followAction = camera.camCreate (spriteLayer.characterDict[1]);

			var interfaceLayer = new GameInterfaceLayer ();
			AddChild (interfaceLayer, 2, gameInterfaceTag);

			this.RunAction (followAction);
		}
	}
}

