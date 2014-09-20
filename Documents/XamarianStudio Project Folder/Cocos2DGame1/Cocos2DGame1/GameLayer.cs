using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;


namespace MindRain
{
	public class GameLayer: CCLayerColor
	{
		static Box2D.Common.b2Vec2 gravity = new Box2D.Common.b2Vec2(0f,0f);
		Box2D.Dynamics.b2World world = new Box2D.Dynamics.b2World(gravity);

		public GameLayer ()
		{
			// create and initialize a Label
			//var label = new CCLabelTTF("testing Layers", "MarkerFelt", 22);

			// position the label on the center of the screen
			//label.Position = CCDirector.SharedDirector.WinSize.Center;

			// add the label as a child to this Layer
			//AddChild(label);

			//Note: This is all test stuff bellow here say for the base.update(gametime) that will stay the same
			Box2D.Common.b2Vec2 position = new Box2D.Common.b2Vec2(0f,0f);
			Box2D.Common.b2Vec2 gravity = new Box2D.Common.b2Vec2(0f,0f);
			string objSpriteName = "Hex"; 
			string objPropertiesFileName = "testProp";
			//this.world.Gravity = gravity;

			Object test = new Object (position, objSpriteName, objPropertiesFileName, this.world);
			var newSprite = test.objSprite;
			AddChild(newSprite);
			string fightingHB = "testFHB";
			string testBS = "testBS";
			Character testC = new Character (test, fightingHB, testBS);

		}

	}
}

