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
	public class CharacterFightingHitBoxes
	{
		//Note: The dictionary holds a multitude of lists that make up all of a characters hitboxes for each animation.
		private Dictionary<string, List<HitBox>> HitBoxDict = new Dictionary<string,  List<HitBox>>();

		//Note: This node contains the rectangles that should be drawn for a character
		private CCNode hBDrawNode = new CCNode ();

		//Note: the tags are used to unequely identify each hitbox so it can be drawn when its frame is being displayed
		private int tagCount = 0;

		//Note: This list keeps track of the currently active hitboxes for this character
		//If this list is accessed do so after updates have occured to both the position and activity of the hitboxes
		//or else the information will be inaccurate
		public List<HitBox> ActiveHBL = new List<HitBox>();

		//Note: Creates an object to store the deserlized json in
		//It is also cleared after every use.
		private JsonInfoStorage jsonInfo = new JsonInfoStorage();

		//Note: This character is only for storing use. This is because it is pass by referance
		//In order to use it properly for things like animation position I would have to change
		//this those functions substantaly. Right now this is only for use by the fighting hb collision handler
		//public Character character = new Character(); 

		//Note: animationScale is the scale of the animation used this sizes the hitboxes in the long run

		public CharacterFightingHitBoxes ()
		{
		}
		public CharacterFightingHitBoxes(string fileName, Box2D.Common.b2Vec2 characterPos, CCSize animationScale)
		{
			//this.character = character;
			LoadHitBoxData (fileName, characterPos, animationScale);
			parseHBDict ();
		}
		public CharacterFightingHitBoxes(List<string> fileNameList, Box2D.Common.b2Vec2 characterPos, CCSize animationScale)
		{ 
			//this.character = character;
			foreach(string element in fileNameList)
			{
				LoadHitBoxData (element, characterPos, animationScale);
			}
			parseHBDict ();
		}
			
		//Note: Loads skylers hitbox files here per animation
		private void LoadHitBoxData(string fileName, Box2D.Common.b2Vec2 animationPos, CCSize animationScale)
		{
			//Note: Create list for this hitbox file. The list holds all the hitboxes for a single animation
			List<HitBox> hitBoxList = new List<HitBox> ();

			//Note: Load textfile
			//Note: Reads all the json into one string and ten deserialzes it into an object
			using(StreamReader r = new StreamReader("Content/hitBoxFiles/" + fileName + ".json"))
			{
				string json = r.ReadToEnd ();
				jsonInfo = (JsonConvert.DeserializeObject <JsonInfoStorage>(json));
			}
		
			//Note: Make create new hitbox using the information from the jsonInfo object
			for(int i = 0; i < jsonInfo.shapes.Count; i++)
			{
				var center = new CCPoint ((float) (jsonInfo.shapes [i].x), (float) (jsonInfo.shapes [i].y));

				var width = (float) (jsonInfo.shapes [i].w);
				var height = (float) (jsonInfo.shapes [i].h);
				var type = (jsonInfo.shapes[i].fill);
				var frame = (jsonInfo.shapes [i].frame);
				HitBox hitbox = new HitBox (center, width, height, type, frame, animationPos, tagCount, animationScale);

				//Note: increment tagCount
				tagCount = tagCount + 1;

				//Note: Add hitbox to list
				hitBoxList.Add (hitbox);
			}

			//Note: Add list to dictionary with animation name 
			HitBoxDict.Add (jsonInfo.image, hitBoxList);

			//Note: Clear reusable lists so that extra data isnt stored and preserves memory by not createing them over and over
			//hitBoxList.Clear ();
		}

		//Note: Updates the sprite position so that the hitboxes can have the correct ancor.
		//This will also call the update animation position method of a hitbox class and update its animation position.
		public void UpdateCharacterPosition(Box2D.Common.b2Vec2 characterPos, string hBName, bool flipped)
		{
			foreach(HitBox element in HitBoxDict[hBName])
			{
				element.UpdateAnimationPos (characterPos, flipped);
				updateNode (element);
			}
			//return hBDrawNode;
		}

		//Note: This function should be called before the draw active hitboxes function in order to preserve acuracy.
		//It also sets the visibility of a given animations hitboxes.
		public void UpdateHitBoxActivity(string hBName, float activeFrame)
		{
			foreach(HitBox element in HitBoxDict[hBName])
			{
				if(element.animationFrame == activeFrame)
				{
					element.active = true;
					this.hBDrawNode.GetChildByTag (element.tag).Visible = true;

					//Note: Adds a new element to the active hit box list if it matches the activeframe and 
					//is not already part of the list.
					if(ActiveHBL.Contains(element) == false)
					{
						ActiveHBL.Add (element);
					}
				}
				else
				{
					element.active = false;
					this.hBDrawNode.GetChildByTag (element.tag).Visible = false;

					//Note: Removes a hitbox element from the active HitBox list if it does not match the active frame
					//and the element is actualy in the list
					if(ActiveHBL.Contains(element) == true)
					{
						ActiveHBL.Remove (element);
					}
				}
			}
		}

		private void parseHBDict()
		{
			//Note: This may be an extreamly costly action, but it is only run once per-character at the starting of each match
			foreach (List<HitBox> element in HitBoxDict.Values.ToList())
			{
				foreach(HitBox elementHB in element)
				{
					AddToNode (elementHB);
				}
			}
		}

		private void AddToNode(HitBox HB)
		{
			CCDrawNode drawNode = new CCDrawNode ();
			drawNode.DrawRect (HB.rect, HB.color);
			drawNode.Tag = HB.tag;
			drawNode.Visible = false;
			hBDrawNode.AddChild (drawNode);
		}

		private void updateNode(HitBox HB)
		{
			CCDrawNode drawNode = new CCDrawNode ();
			drawNode.DrawRect (HB.rect, HB.color);
			drawNode.Tag = HB.tag;

			if(HB.active == true)
			{
				drawNode.Visible = true;
			}
			else
			{
				drawNode.Visible = false;
			}

			hBDrawNode.RemoveChildByTag (HB.tag);
			hBDrawNode.AddChild (drawNode);
		}

		public CCNode DrawActiveHitBoxes(bool drawHitBoxes)
		{
			if(drawHitBoxes == true)
			{
				//Note: If the bool is true dont do anything and draw the hitboxes
			}
			else
			{
				//Note: I hope this turns all the hitboxes in this node invisible
				hBDrawNode.Visible = false;
			}

			return hBDrawNode;
		}

		public class JsonInfoStorage
		{
			//Note this is the base class for json storage

			public string image { get; set;}
			public List<JsonInfo> shapes { get; set;}
		}
		public class JsonInfo
		{
			//Note this is the hit box info class, it is going to be used by the json info storage class in a list form
			//in order to create an object that can store incoming json information. The info is deserialized form the
			//json file and put into an object of the jsoninfoStorage class. The Hitbox data is stored in the classes
			//object specificly.

			//Note: Center cordanants 
			public float x { get; set;} 
			public float y { get; set;}

			//Note: Width
			public float w { get; set;}

			//Note: Height
			public float h { get; set;}

			//Note: Frame number
			public int frame { get; set;}

			//Note: theta is amount rotated, out of 360 degrees
			public float theta { get; set;}

			//Note: Fill is type of hitbox
			public int fill { get; set;}
		}

		public class HitBox
		{
			//Note: hitboxes work like checkpoints in an animation. Every animation will have a hitbox and 
			//it will check the distance between itself and all enemies on the field at somepoint the distance will be zero or negative
			//this is when a collision occures. Since there are hit boxes on each frame of the animation they increase the acuracy
			//of the hit detection to near perfect levels. But this may degrade performance. The hitbox class itself will not do the
			//checking to see if a collison has occured a diffrent class will handle that.

			//Note: A ccrectangle will be used as a hitbox, it can check for intersections of other ccrect's which is useful
			public CCRect rect = new CCRect ();

			//Note: This stores the location of the center point in relation to the original animation frame
			private CCPoint center = new CCPoint ();

			//Note: Key to what type of hitbox each set of data represents
			//private static int Offensive = 1;
			//private static int Damageable = 2;
			//private static int Invincible = 3;
			//private static int Intangible = 4;
			//private static int Reflective = 5;
			//private static int Shield = 6;
			//private static int Absorbing = 7;
			//private static int Grab = 8;
			//private static int CollisionDetection = 9;

			//Note: Current hitbox type
			public int hitboxType = 0;

			//Note: This bool tells if the hitbox should be taken into account when doing hitbox collision detection
			public bool active = false;

			//Note: Tells what animation frame the current hitbox data is from, this lets  the hitbox be synced with the correct
			//animation frame later
			public int animationFrame = 0;

			public Box2D.Common.b2Vec2 animationPos = new Box2D.Common.b2Vec2();

			public int tag = 0;

			public CCColor4B color = CCColor4B.Orange;

			private float xOffset = 0f;
			private float yOffset = 0f;
			private CCSize hitBoxScale = new CCSize();

			//public Character character = new Character();

			public HitBox()
			{

			}
			public HitBox(CCPoint center, float width, float height, int hitboxType, int animationFrame, Box2D.Common.b2Vec2 animationPos, int tag, CCSize hitBoxScale)
			{
				this.center = center;
				this.hitboxType = hitboxType;
				this.animationFrame = animationFrame;
				this.animationPos = animationPos;
				this.tag = tag;
				this.hitBoxScale = hitBoxScale;

				this.xOffset = CalculateXOffset();
				this.yOffset = CalculateYOffset();

				this.rect = new CCRect(animationPos.x- xOffset, animationPos.y - yOffset, width*hitBoxScale.Width, height*hitBoxScale.Width);

				AssignColor();
			}
				
			//Note: This function sets the hitboxes position on the screen using the current animation position and
			//the positional information stored in the class.
			private void HitBoxPosition(bool flipped)
			{
				if(flipped == true)
				{
					this.rect.Origin.X = (animationPos.x + xOffset);
				}
				else
				{
					this.rect.Origin.X = (animationPos.x - xOffset);
				}

				this.rect.Origin.Y = (animationPos.y - yOffset);
			}

			private float CalculateXOffset()
			{
				//Note: this offset formulat will change once I get a defined way of making animations
				var offset = center.X - (1280 * (animationFrame - 1)+(1280/2.3f));
				return offset;
			}

			private float CalculateYOffset()
			{
				var offset = 0;//center.Y-(720*(animationFrame-1));
				return offset;
			}

			//Note: The method uses the hitbox type to choose the correct color for the hitbox
			private void AssignColor()
			{
				switch(hitboxType)
				{
				case 0:
					//Note: This happends if a color is never taken from the inputfile or hitboxType is assigned to zero
					color = new CCColor4B (Color.FromNonPremultiplied(0,0,0,155));
					//color = CCColor4B.Black;
					break;
				case 1:
					color = new CCColor4B (Color.FromNonPremultiplied(255,0,0,155));
					//color = CCColor4B.Red;	
					break;
				case 2: 
					color = new CCColor4B (Color.FromNonPremultiplied(255,255,0,155));
					//color = CCColor4B.Yellow;
					break;
				case 3: 
					color = new CCColor4B (Color.FromNonPremultiplied(0,255,0,155));
					//color = CCColor4B.Green;
					break;
				case 4: 
					color = new CCColor4B (Color.FromNonPremultiplied(0,0,255,155));
					//color = CCColor4B.Blue;
					break;
				case 5:
					color = new CCColor4B (Color.FromNonPremultiplied(0,255,255,155));
					//color = new CCColor4B (Color.Aqua);
					break;
				case 6: 
					color = new CCColor4B (Color.FromNonPremultiplied(32,178,170,155));
					//color = new CCColor4B (Color.Cyan);
					break;
				case 7: 
					color = new CCColor4B (Color.FromNonPremultiplied(255,105,180,155));
					//color = new CCColor4B (Color.Pink);
					break;
				case 8: 
					color = new CCColor4B (Color.FromNonPremultiplied(255,0,255,155));
					//color = CCColor4B.Magenta;
					break;
				case 9: 
					color = new CCColor4B (Color.FromNonPremultiplied(128,128,128,155));
					//color = CCColor4B.Gray;
					break;
				default:
					color = new CCColor4B (Color.FromNonPremultiplied(255,165,0,155));
					//color = CCColor4B.Orange;
					break;
				}
			}

			//Note: This function updates the animations position in the class and then call the hitboxPosition function
			//This sets the current position of the hitbox when the animation moves.
			public void UpdateAnimationPos(Box2D.Common.b2Vec2 animationPos, bool flipped)
			{
				this.animationPos = animationPos;
				HitBoxPosition (flipped);
			}
				
		}

	}
}

