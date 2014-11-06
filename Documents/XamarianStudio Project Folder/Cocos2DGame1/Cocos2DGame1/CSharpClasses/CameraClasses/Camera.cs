using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindRain
{
	public class Camera
	{
		public Camera ()
		{
		}
		public CCFollow camCreate(Character character)
		{
			CCPoint origin = CCDirector.SharedDirector.VisibleOrigin;
			CCSize size = CCDirector.SharedDirector.VisibleSize;
			CCPoint center = new CCPoint (size.Width / 2 + origin.X, size.Height / 2 + origin.Y);

			var spriteNode = character.characterObj.charAnimations.GetAnimSprite (character.characterObj.charAnimations.spriteNameList [0]);
			spriteNode.SetPosition (center.X, center.Y);

			float playField_width = size.Width * 2.0f;
			float playField_height = size.Height * 2.0f;
			CCRect cameraBounds = new CCRect (center.X - playField_width / 2, center.Y - playField_height / 2, playField_width, playField_height);
			CCFollow cameraFollow = new CCFollow (spriteNode,cameraBounds);

			return cameraFollow;
		}
	}
}

