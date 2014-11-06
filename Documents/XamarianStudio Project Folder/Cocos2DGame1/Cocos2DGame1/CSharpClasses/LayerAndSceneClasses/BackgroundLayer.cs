using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace MindRain
{
	/// <summary>
	/// A Cocos2D-XNA layer that manages the background in the game.
	/// </summary>
	internal class BackgroundLayer : CCLayer 
	{
		public BackgroundLayer()
		{
			// Load the background image into a sprite
			var backgroundImage = new CCSprite ("images/Background.xnb");

			// Get the dimensions of the game window
			var screenSize = CCDirector.SharedDirector.WinSize;

			// Set the position of the background sprite to the center of the screen
			backgroundImage.SetPosition (screenSize.Width / 2, screenSize.Height / 2);
			backgroundImage.Scale = 1f;
			// Add the background sprite to the layer
			AddChild(backgroundImage, 1);

			CCPoint origin = CCDirector.SharedDirector.VisibleOrigin;
			CCSize size = CCDirector.SharedDirector.VisibleSize;

			// setup our color for the background
			//Note: This will eventualy need to change to the size of the level not just the screen
			CCLayerColor LayerColor = new CCLayerColor (CCColor4B.Blue, screenSize.Width*2, screenSize.Height*2);
			LayerColor.SetPosition (origin.X - size.Width/2, origin.Y - size.Height/2);

			AddChild (LayerColor,0);
		}
	}
}
