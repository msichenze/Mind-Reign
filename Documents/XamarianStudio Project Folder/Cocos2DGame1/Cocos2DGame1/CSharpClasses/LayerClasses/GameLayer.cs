using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

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
		AnimationUseManager animMan = new AnimationUseManager();


		public GameLayer ()
		{
			//Note: Create a Static object like wall or platform
			Box2D.Common.b2Vec2 positionStatic = new Box2D.Common.b2Vec2(400f,600f);
			string staticObjSN = "Floor";
			string staticObjPFN = "testPropStatic";
			CreateStaticObject(positionStatic, staticObjSN, staticObjPFN);


			//Note: Create an entire Character
			Box2D.Common.b2Vec2 startPosition = new Box2D.Common.b2Vec2(600f,150f);

			//Note: objectSpriteName is used to load a characters property files and creates an objectSprite for the bodyDefinitin of the character
			//This sprite is only used for naming and definiation purposes within the body definition.
			string objSpriteName = "sketchwalk";
			string objPropertiesFileName = "testProp";

			//Note: load a single file that has the file name of every animation and their animation run speed for each character
			var nSObject = loadJson ("testCharAnimationNameSpeed");
			List<Tuple<string,float>> animationNameList = new List<Tuple<string,float>> ();
			animationNameList = nSObject.ConvertToList (animationNameList);

			//Note: Sets the size of all animations of a character
			CCSize animationSize = new CCSize (400, 250);

			//Note: Sets the player number
			int playerNum = 1;

			//Note: Sets the name of the bonus stats file name
			string bonusStatsFN = "testBS";
			string keyBindingsFN = "testKeyBindings";
			CreateCharacter (startPosition, objSpriteName, objPropertiesFileName, animationNameList, animationSize, bonusStatsFN, playerNum, keyBindingsFN);
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

		private void CreateCharacter(Box2D.Common.b2Vec2 startPosition, string objSpriteName, string objPropertiesFileName, 
			List<Tuple<string,float>> animationNameList, CCSize animationSize, string bonusStatsFN, int playerNum, string keyBindingsFN)
		{
			//Note: This is all test stuff bellow here will stay the same
			bool doSleep = true;
			this.world.SetAllowSleeping (doSleep);

			CharacterAnimations charAnim = new CharacterAnimations (animationNameList);
			foreach(Tuple<string,float> element in animationNameList)
			{
				charAnim.ScaleAnimation (element.Item1, animationSize);
				AddChild (charAnim.AddSpriteSheet (element.Item1));
			}

			Object test = new Object (startPosition, objSpriteName, objPropertiesFileName, this.world, charAnim);

			Character testC = new Character (test, bonusStatsFN);

			addCharacters (playerNum, testC);
			Player playerOne = new Player (testC, playerNum, keyBindingsFN);
			this.playerOne = playerOne;
		}

		public void CreateStaticObject(Box2D.Common.b2Vec2 positionStatic, string staticObjSN, string staticObjPFN)
		{
			Object testStatic = new Object (positionStatic, staticObjSN, staticObjPFN, this.world);
			var newStaticSprite = testStatic.objSprite;
			AddChild(newStaticSprite);
			addTerrainObjects (staticObjSN, testStatic.body);
		}

		private AnimationNameSpeedJson loadJson(string jsonFN)
		{
			AnimationNameSpeedJson ANSJObject = new AnimationNameSpeedJson();

			//Note: Load textfile
			//Note: Reads all the json into one string and ten deserialzes it into an object
			using(StreamReader r = new StreamReader("Content/propertyFiles/animationNameSpeed/" +jsonFN + ".json"))
			{
				string json = r.ReadToEnd ();
				ANSJObject = JsonConvert.DeserializeObject <AnimationNameSpeedJson>(json);
			}
			return ANSJObject;
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
						
					bCharacter = playerOne.moveCharacter (b);
					b = bCharacter.characterObj.body;

					bCharacter.characterObj.charAnimations.SetAnimationPosition (bCharacter.characterObj.charAnimations.spriteNameList, b.Position.x, b.Position.y);
					animMan.CharacterStateAnimations (bCharacter);

					//Note: This will eventualy need to check for the current player 1 when there are multiple players
					interfaceLayer.UpdateInterfacePosition ();
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

		//Note: these classes are for json importing of the animation name speed information
		private class AnimationNameSpeedJson
		{
			public string character { get; set; }
			public List<NameSpeedInfo> NameSpeed { get; set; }

			public List<Tuple<string,float>> ConvertToList(List<Tuple<string,float>> animationNameList)
			{
				foreach(NameSpeedInfo element in NameSpeed)
				{
					Tuple<string,float> nameSpeedTuple = new Tuple<string, float> (element.animationName, element.animationSpeed);
					animationNameList.Add (nameSpeedTuple);
				}
				return animationNameList;
			}
		}
		private class NameSpeedInfo
		{
			public string animationName { get; set; }
			public float animationSpeed { get; set; }
		}
	}
}

