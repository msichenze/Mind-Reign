using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindRain
{
	public class GameLayer:CCLayer
	{
		static Box2D.Common.b2Vec2 gravity = new Box2D.Common.b2Vec2(0f,-10f);
		public Box2D.Dynamics.b2World world = new Box2D.Dynamics.b2World(gravity);

		public Dictionary<int, Character> characterDict = new Dictionary<int, Character>();
		public Dictionary<string, Box2D.Dynamics.b2Body> terrainObjectDict = new Dictionary<string, Box2D.Dynamics.b2Body>();
		Player playerOne = new Player();
		public CCSize screenSize = CCDirector.SharedDirector.WinSize;

		CollisionDetection collisionlistener = new CollisionDetection();
		GameInterfaceLayer interfaceLayer = null; 

		public GameLayer ()
		{
			//Note: This is all test stuff bellow here will stay the same
			Box2D.Common.b2Vec2 position = new Box2D.Common.b2Vec2(600f,150f);
			string objSpriteName = "sketchwalk"; 
			string objPropertiesFileName = "testProp";
			bool doSleep = true;
			this.world.SetAllowSleeping (doSleep);

			CCSize animationSize = new CCSize (400, 250);
			CharacterAnimations charAnim = new CharacterAnimations (objSpriteName);
			charAnim.ScaleAnimation (objSpriteName, animationSize);
			AddChild (charAnim.AddSpriteSheet (objSpriteName));
			//charAnim.SetAnimationPosition (objSpriteName,position.x, position.y);
			//charAnim.RunAnimation (objSpriteName);

			Object test = new Object (position, objSpriteName, objPropertiesFileName, this.world, charAnim);
			//Object test = new Object (position, objSpriteName, objPropertiesFileName, this.world);
			//var newSprite = test.objSprite;
			//AddChild(newSprite);

			string fightingHB = "testFHB";
			string testBS = "testBS";
			Character testC = new Character (test, fightingHB, testBS);

			int playerNum = 1;
			addCharacters (playerNum, testC);
			Player playerOne = new Player (testC, playerNum);
			this.playerOne = playerOne;

			Box2D.Common.b2Vec2 positionStatic = new Box2D.Common.b2Vec2(400f,600f);
			string staticObjSN = "Floor";
			string staticObjPFN = "testPropStatic";
			Object testStatic = new Object (positionStatic, staticObjSN, staticObjPFN, this.world);
			var newStaticSprite = testStatic.objSprite;
			AddChild(newStaticSprite);
			addTerrainObjects (staticObjSN, testStatic.body);

		}

		private void addCharacters (int playerNum, Character character)
		{
			//Note: adds characters to the character dictionary
			this.characterDict.Add (playerNum, character);
		}

		private void addTerrainObjects(string objectID, Box2D.Dynamics.b2Body terrain)
		{
			//Note: adds terrainobjects to the terrain object dictionary
			this.terrainObjectDict.Add (objectID, terrain);
		}

		public void Update ()
		{
			var runningScene = CCDirector.SharedDirector.RunningScene;
			interfaceLayer = (GameInterfaceLayer)runningScene.GetChildByTag (GameScene.gameInterfaceTag);

			for(Box2D.Dynamics.b2Body b = world.BodyList; b != null; b=b.Next)
			{
				if (b.UserData != null && terrainObjectDict.ContainsValue (b) != true) 
				{
					//Note: Collision stuff first then update movement stuff second.
					Character bCharacter = characterDict [1];
					if(b.ContactList != null)
					{
						collisionlistener.getCharacter (bCharacter);
						b.ContactList.Contact.Update (collisionlistener);
					}

					//bCharacter = //UpdateCharacter (b);
					playerOne.moveCharacter (b);
					b = bCharacter.characterObj.body;
					//CCSprite sprite = (CCSprite)b.UserData;
					bCharacter.characterObj.charAnimations.SetAnimationPosition (bCharacter.characterObj.objSprite.Name, b.Position.x, b.Position.y);
					//sprite.SetPosition (b.Position.x, b.Position.y);


					//Note: This will eventualy need to check for the current player 1 when there are multiple players
					interfaceLayer.updateVelLabel(b.LinearVelocity.x, b.LinearVelocity.y);

					//Note: may need to update rotation here as well
				} 
				else 
				{
					//b = gameLayer.UpdateCharacter (b);
					//CCSprite sprite = (CCSprite)b.UserData;
					//sprite.SetPosition (b.Position.x, b.Position.y);
					//Box2D.Common.b2Color red = new Box2D.Common.b2Color (250, 0, 0);
					//gameLayer.world.DrawShape (b.FixtureList, ref b.Transform,red);
					//Note: Could put some code here to work with specific terrain objects
				}
			}
		}


	}
}

