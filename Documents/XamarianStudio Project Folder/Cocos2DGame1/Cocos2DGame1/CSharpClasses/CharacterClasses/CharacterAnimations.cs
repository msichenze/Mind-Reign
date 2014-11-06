using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Box2D;
using Microsoft.Xna.Framework;

namespace MindRain
{
	public class CharacterAnimations
	{
		//Note: Character will take an instance of this class, that instance will include a list with all the animations for the specified character
		//and the methods to add and remove the animation from the screen will be in this characteranimation class instance

		//Note: the animationDictionary uses a key of the animation name and the value of  the animated sprite
		private Dictionary<string,CCSprite> animationDict = new Dictionary<string, CCSprite>();
		private Dictionary<string, CCAction> actionDict = new Dictionary<string, CCAction>();
		private Dictionary<string, CCSpriteBatchNode> spriteSheetDict = new Dictionary<string, CCSpriteBatchNode> ();
		public List<string> spriteNameList = new List<string> ();

		private CCSize winSize = CCDirector.SharedDirector.WinSize;

		public CharacterAnimations ()
		{
		}
		public CharacterAnimations (string spriteName)
		{
			float delay = 0.08f;
			LoadObjectAnimation (spriteName, delay);
		}

		public CharacterAnimations (List<Tuple<string,float>> spriteList)
		{
			foreach(Tuple<string,float> element in spriteList)
			{
				spriteNameList.Add(element.Item1);
				LoadObjectAnimation (element.Item1,element.Item2);
			}
		}

		public void LoadObjectAnimation(string spriteName, float delay)
		{
			CCSprite animSprite = new CCSprite ();

			CCSpriteFrameCache sharedSpriteFrameCache = CCSpriteFrameCache.SharedSpriteFrameCache;

			//Note: need to add paths for the folders that will contain the .plist and .png stuff
			string plistName = "Content/sprites/spriteSheets/data_files/" + spriteName + ".plist";
			string textureName = "sprites/spriteSheets/texture_files/" + spriteName + ".png";

			CCTexture2D texture = CCTextureCache.SharedTextureCache.AddImage(textureName);

			System.IO.Stream s = System.IO.File.OpenRead (plistName);

			sharedSpriteFrameCache.AddSpriteFramesWithFile (s, texture);

			CCSpriteBatchNode spriteSheet = new CCSpriteBatchNode (texture);

			List<CCSpriteFrame> animationFrames = new List<CCSpriteFrame> ();

			//Note: this adds each frame of the animation to a list of frames one at a time
			// is how many frames there are in the animation
			for(int i=1; i<=9; i++)
			{
				string spriteFrameName = spriteName + "." + String.Format("{0:000}", i) + ".png";
				animationFrames.Add (sharedSpriteFrameCache.SpriteFrameByName (spriteFrameName));
			}

			//Note the second number is the delay from the animation
			CCAnimation animation = new CCAnimation (animationFrames, delay);

			//Load sprite
			//Note: need to add paths for the folders that will contain the .plist and .png stuff
			animSprite = new CCSprite(animationFrames[0]);
			animSprite.Name = spriteName;

			CCAnimate actionWithAnimation = new CCAnimate (animation);
			CCRepeatForever actionWithAction = new CCRepeatForever (actionWithAnimation);
			CCAction animationAction = actionWithAction;

			//Note: stores the original animation in the animSprite.userdata so that the animation can be modified
			animSprite.UserData = animation;
			animSprite.Visible = false;
			spriteSheet.AddChild (animSprite);

			actionDict.Add(spriteName, animationAction);
			animationDict.Add (spriteName, animSprite);

			//Note: add sprite sheet to a dicitionary, all addChild calls must be done in the gamelayer 
			//this dictionary helps getting the correct spritesheet to add as a child in the gamelayer
			spriteSheetDict.Add (spriteName, spriteSheet);

		}

		public void RunAnimation(string spriteName)
		{
			if(animationDict [spriteName].NumberOfRunningActions() == 0)
			{
				//Note: The player will call this to start an animation
				animationDict [spriteName].Visible = true;
				animationDict [spriteName].RunAction (actionDict [spriteName]);
			}
		}

		public void StopAnimation(string spriteName)
		{
			//Note: The player will call this to stop an animation
			if(animationDict [spriteName].NumberOfRunningActions() == 1)
			{
				animationDict [spriteName].Visible = false;
				animationDict [spriteName].StopAction (actionDict [spriteName]);
			}
		}

		public void SetAnimationPosition(string spriteName, float x, float y)
		{
			animationDict [spriteName].SetPosition(x,y);
		}
		public void SetAnimationPosition(List<string> spriteNameList, float x, float y)
		{
			foreach(string spriteName in spriteNameList)
			{
				animationDict [spriteName].SetPosition(x,y);
			}
		}

		public CCSpriteBatchNode AddSpriteSheet(string spriteName)
		{
			return spriteSheetDict [spriteName];
		}

		public void SetAnimationSpeed(string spriteName, float delay)
		{
			var animation = (CCAnimation)animationDict [spriteName].UserData;
			animation.DelayPerUnit = delay;
			CCAnimate actionWithAnimation = new CCAnimate (animation);
			CCRepeatForever actionWithAction = new CCRepeatForever (actionWithAnimation);
			CCAction animationAction = actionWithAction;
			actionDict [spriteName] = animationAction;
		}

		public void FlipAnimation(string spriteName, bool flipBool)
		{
			//Note: flipBool tells if the animation should be fliped or not, if true the animation will be flipped,
			//if false it wont be
			animationDict [spriteName].FlipX = flipBool;
		}
		public void FlipAnimation( List<string> spriteNameList, bool flipBool)
		{
			//Note: flipBool tells if the animation should be fliped or not, if true the animation will be flipped,
			//if false it wont be
			foreach(string spriteName in spriteNameList)
			{
				animationDict [spriteName].FlipX = flipBool;
			}
		}

		public void ScaleAnimation(string spriteName, CCSize size)
		{
			animationDict [spriteName].ScaleTo (size);
		}

		public CCSprite GetAnimSprite(string spriteName)
		{
			return animationDict [spriteName];
		}
	}
}

