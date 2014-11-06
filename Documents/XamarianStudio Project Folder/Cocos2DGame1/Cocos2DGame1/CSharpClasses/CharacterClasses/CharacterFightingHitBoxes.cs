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

		private CCDrawNode drawNode = new CCDrawNode ();

		//Note: the tags are used to unequely identify each hitbox so it can be drawn when its frame is being displayed
		private int tagCount = 0;

		//Note: This list keeps track of the currently active hitboxes for this character
		//If this list is accessed do so after updates have occured to both the position and activity of the hitboxes
		//or else the information will be inaccurate
		public List<HitBox> ActiveHBL = new List<HitBox>();

		//Note: Creates an object to store the deserlized json in
		//It is also cleared after every use.
		private List<JsonInfoStorage> jsonInfo = new List<JsonInfoStorage>();

		//Note: Create list for this hitbox file. The list holds all the hitboxes for a single animation
		//It is also cleared after every use.
		private List<HitBox> hitBoxList = new List<HitBox> ();

		//Note: This character is only for storing use. This is because it is pass by referance
		//In order to use it properly for things like animation position I would have to change
		//this those functions substantaly. Right now this is only for use by the fighting hb collision handler
		public Character character = new Character(); 


		public CharacterFightingHitBoxes ()
		{
		}
		public CharacterFightingHitBoxes(string fileName, CCPoint characterPos, ref Character character)
		{
			this.character = character;
			LoadHitBoxData (fileName, characterPos);
			parseHBDict ();
		}
		public CharacterFightingHitBoxes(List<string> fileNameList, CCPoint characterPos, ref Character character)
		{ 
			this.character = character;
			foreach(string element in fileNameList)
			{
				LoadHitBoxData (element, characterPos);
			}
			parseHBDict ();
		}
			
		//Note: Loads skylers hitbox files here per animation
		private void LoadHitBoxData(string fileName, CCPoint animationPos)
		{
			//Note: Load textfile
			//Note: Reads all the json into one string and ten deserialzes it into an object
			using(StreamReader r = new StreamReader(fileName + ".json"))
			{
				string json = r.ReadToEnd ();
				jsonInfo = JsonConvert.DeserializeObject <List<JsonInfoStorage>>(json);
			}
		
			//Note: Make create new hitbox using the information from the jsonInfo object
			for(int i = 0; i <= jsonInfo[0].hitboxes.Count; i++)
			{
				var center = new CCPoint ((float)jsonInfo [0].hitboxes [i].x, (float)jsonInfo [0].hitboxes [i].y);

				var width = jsonInfo [0].hitboxes [i].width;
				var height = jsonInfo [0].hitboxes [i].height;
				var type = jsonInfo [0].hitboxes [i].type;
				var frame = jsonInfo [0].hitboxes [i].frame;
				HitBox hitbox = new HitBox (center, width, height, type, frame, animationPos, tagCount, this.character);

				//Note: increment tagCount
				tagCount = tagCount + 1;

				//Note: Add hitbox to list
				hitBoxList.Add (hitbox);
			}

			//Note: Add list to dictionary with animation name
			HitBoxDict.Add (jsonInfo [0].image, hitBoxList);

			//Note: Clear reusable lists so that extra data isnt stored and preserves memory by not createing them over and over
			jsonInfo.Clear ();
			hitBoxList.Clear ();
		}

		//Note: Updates the sprite position so that the hitboxes can have the correct ancor.
		//This will also call the update animation position method of a hitbox class and update its animation position.
		public void UpdateAnimPosition(CCPoint animationPos, string key)
		{
			foreach(HitBox element in HitBoxDict[key])
			{
				element.UpdateAnimationPos (animationPos);
			}
		}

		//Note: This function should be called before the draw active hitboxes function in order to preserve acuracy.
		//It also sets the visibility of a given animations hitboxes.
		public void UpdateHitBoxActivity(string key, float activeFrame)
		{
			foreach(HitBox element in HitBoxDict[key])
			{
				if(element.animationFrame == activeFrame)
				{
					element.active = true;
					hBDrawNode.GetChildByTag (element.tag).Visible = true;

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
					hBDrawNode.GetChildByTag (element.tag).Visible = false;

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
			drawNode.DrawRect (HB.rect, HB.color);
			drawNode.Tag = HB.animationFrame;
			drawNode.Visible = false;
			hBDrawNode.AddChild (drawNode);

			//Note: This should clear the draw node and not affect the the hitbox draw node. This way i can reuse the draw node without memory
			//a memory leak occuring of it creating a new drawnode over and over.
			drawNode.Clear ();
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

			public string image;
			public List<JsonHBInfo> hitboxes;
		}
		public class JsonHBInfo
		{
			//Note this is the hit box info class, it is going to be used by the json info storage class in a list form
			//in order to create an object that can store incoming json information. The info is deserialized form the
			//json file and put into an object of the jsoninfoStorage class. The Hitbox data is stored in the classes
			//object specificly.

			//Note: Center cordanants 
			public float x; 
			public float y;

			public float width;
			public float height;

			public int type;
			public int frame;

			//Note Tilt is out of 360 degrees
			public float tilt;
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

			public CCPoint animationPos = new CCPoint ();

			public int tag = 0;

			public CCColor4B color = CCColor4B.Orange;

			public Character character = new Character();

			public HitBox()
			{

			}
			public HitBox(CCPoint center, float width, float height, int hitboxType, int animationFrame, CCPoint animationPos, int tag, Character character)
			{
				this.center = center;
				this.rect = new CCRect(animationPos.X+center.X, animationPos.Y+center.Y, width, height);
				this.hitboxType = hitboxType;
				this.animationFrame = animationFrame;
				this.animationPos = animationPos;
				this.tag = tag;
				this.character = character;
				AssignColor();
			}

			//Note: This function sets the hitboxes position on the screen using the current animation position and
			//the positional information stored in the class.
			private void HitBoxPosition()
			{
				rect.Origin.X = animationPos.X + center.X;
				rect.Origin.Y = animationPos.Y + center.Y;
			}

			//Note: The method uses the hitbox type to choose the correct color for the hitbox
			private void AssignColor()
			{
				switch(hitboxType)
				{
				case 0:
					//Note: This happends if a color is never taken from the inputfile or hitboxType is assigned to zero
					color = CCColor4B.Black;
					break;
				case 1:
					color = CCColor4B.Red;	
					break;
				case 2: 
					color = CCColor4B.Yellow;
					break;
				case 3: 
					color = CCColor4B.Green;
					break;
				case 4: 
					color = CCColor4B.Blue;
					break;
				case 5: 
					color = new CCColor4B (Color.Aqua);
					break;
				case 6: 
					color = new CCColor4B (Color.Cyan);
					break;
				case 7: 
					color = new CCColor4B (Color.Pink);
					break;
				case 8: 
					color = CCColor4B.Magenta;
					break;
				case 9: 
					color = CCColor4B.Gray;
					break;
				default:
					color = CCColor4B.Orange;
					break;
				}
			}

			//Note: This function updates the animations position in the class and then call the hitboxPosition function
			//This sets the current position of the hitbox when the animation moves.
			public void UpdateAnimationPos(CCPoint animationPos)
			{
				this.animationPos = animationPos;
				HitBoxPosition ();
			}
				
		}

	}
}

