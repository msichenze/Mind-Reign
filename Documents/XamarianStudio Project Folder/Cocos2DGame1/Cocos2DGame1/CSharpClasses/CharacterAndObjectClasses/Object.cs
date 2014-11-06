using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Box2D;
using Microsoft.Xna.Framework;

namespace MindRain
{
    public class Object
    {
        //public string objPropertiesFileName;
        public Box2D.Dynamics.b2Body body;
        public Box2D.Common.b2Vec2 position;
        public CCSprite objSprite;
		//Note: PTM_Ratio sands for pixles to meters. Box2d is not optimized to deal with large amounts of pixesl
		// but if we divide it by a ratio then box2d can handle it for simulations
		public float PTM_RATIO = 32.0f;

        //Note: Files loaded into the propertyDict should have the "CanMove" string followed by a 1 or a zero for yes or no
        //The File should also have "Density" and "Friction" if Can Move ==1
		public Dictionary<string,float> propertyDict = new Dictionary<string, float>();
        //Note: world defines things like gravity and if the object is sleeping one world will be defined
        //for this project and that world will be passed to each object.
        public Box2D.Dynamics.b2World world;

		public CCSize winSize = CCDirector.SharedDirector.WinSize;
		public CharacterAnimations charAnimations = new CharacterAnimations();
		private string currentSpriteName;

		public Object()
		{
			//Note: empty default constructor. This is for creating empty Object vas so they can be set equal to a created object.
		}

        public Object(Box2D.Common.b2Vec2 position, string objSpriteName, string objPropertiesFileName, Box2D.Dynamics.b2World world)
        {
            this.position = position;
            this.world = world;
            //this.objPropertiesFileName = objPropertiesFileName;
			LoadObjectFiles(objSpriteName, propertyDict, objPropertiesFileName);
            CreateBoundingBox();
        }

		public Object(Box2D.Common.b2Vec2 position, string objSpriteName, string objPropertiesFileName, Box2D.Dynamics.b2World world, CharacterAnimations charAnimations)
		{
			this.position = position;
			this.world = world;
			this.charAnimations = charAnimations;
			currentSpriteName = objSpriteName;
			LoadObjectFiles(objSpriteName, propertyDict, objPropertiesFileName);
			CreateBoundingBox();
			//StartAnimation (charAnimations, objSpriteName);

		}

		private void LoadObjectFiles(string spriteName, Dictionary<string,float> propertyDict, string objPropertiesFileName)
        {
            //Load sprite
            //Note: The .png can change I dont know what the image type i will use
			// Get the dimensions of the game window
			this.objSprite = new CCSprite("sprites/" + spriteName + ".png");
			this.objSprite.Name = spriteName;
			this.objSprite.SetPosition ((int)(this.winSize.Width - this.position.x), (int)(this.winSize.Height - this.position.y));
            //Read the properties of the object into an array
            //Note: Read AllLines reads in by line all the content on the line
			string[] properties = System.IO.File.ReadAllLines ("Content/propertyFiles/normalFiles/" + objPropertiesFileName + ".txt");
            foreach (string property in properties)
            {
                float propertyVal = 0;
                string numberonly = "";
                string onlyLetters = "";
                //Note: This should find only the letters from each line of the file
                onlyLetters = new string(property.Where(Char.IsLetter).ToArray());
                //Note: This should find only the numbers in each line of the file
				numberonly = System.Text.RegularExpressions.Regex.Match(property, @"\d+(\.\d+)").Value;
                propertyVal = float.Parse(numberonly);
                //Note: This should add both the letters and the numbers from a single line of
                //the file to a dictionary for ease of access
                propertyDict.Add(onlyLetters,propertyVal);
            }
			this.propertyDict = propertyDict;
        }

        private void CreateBoundingBox()
        {
            Box2D.Collision.Shapes.b2PolygonShape bodyBox = new Box2D.Collision.Shapes.b2PolygonShape();
			//Note: this bodyBox.SetAsBox (10f, 10f); is temporary till i can figure out hot to do the sprite box thing

            //Note: If the properyDict contains the CanMove prop and it is 1 or yes then create a dynamic body and not a static one
            //Note: Dynamic bodies are for things like characters and moving objects, static bodies cannot move ever.
            if(this.propertyDict.ContainsKey("CanMove") && this.propertyDict["CanMove"] == 1)
            {
                //Note: Set body type
				Box2D.Dynamics.b2BodyDef bodyDef = new Box2D.Dynamics.b2BodyDef();
				bodyBox.SetAsBox (10f, 10f);

				Box2D.Dynamics.b2FixtureDef fixtureDef = new Box2D.Dynamics.b2FixtureDef();

                bodyDef.type = Box2D.Dynamics.b2BodyType.b2_dynamicBody;
				bodyDef.position.Set((int)(this.winSize.Width - this.position.x), (int)(this.winSize.Height - this.position.y));
				bodyDef.userData = this.objSprite;
				bodyDef.fixedRotation = true;
				bodyDef.gravityScale = this.propertyDict ["gravityScale"];
                //Note The b2Body body must be createdd after the bodyDef is compleated
				this.body = this.world.CreateBody(bodyDef);

                //Note: Uses the sprite's bounding box to make the actual dynamic bounding box
                //bodyBox.SetAsBox(this.objSprite.BoundingBox.Size.Width / 2.0f, this.objSprite.BoundingBox.Size.Height / 2.0f);
                //Note: The line below should read ...=dynamicBox;

                fixtureDef.shape = bodyBox;
                fixtureDef.density = this.propertyDict["density"];
                fixtureDef.friction = this.propertyDict["friction"];
			
				//Note: Restutution is bouncyness or elacsticity setting a value of 1 will make the object bounce back with all the force of the original collsion.
				fixtureDef.restitution = this.propertyDict ["restitution"]; 
                this.body.CreateFixture(fixtureDef);
            }
            //Note: If the dict says CanMove is 0 or no then create a static body
            else
            {
				//Note: Static I am using static bodys for terrain and objects that dont move
				Box2D.Dynamics.b2BodyDef staticBodyDef = new Box2D.Dynamics.b2BodyDef();
				Box2D.Dynamics.b2FixtureDef staticFixtureDef = new Box2D.Dynamics.b2FixtureDef();

				staticBodyDef.type = Box2D.Dynamics.b2BodyType.b2_staticBody;
                //Note: Normaly before creating the body box one would set a bodyDef type but in this case it is static and that is the default
				staticBodyDef.position.Set((int)(this.winSize.Width - this.position.x), (int)(this.winSize.Height - this.position.y));
				staticBodyDef.userData = this.objSprite;
                //Note The b2Body body must be createdd after the bodyDef is compleated
				this.body = this.world.CreateBody(staticBodyDef);

				//Note: give properties to the static body.
				staticFixtureDef.density = this.propertyDict["density"];
				staticFixtureDef.friction = this.propertyDict["friction"];
				staticFixtureDef.restitution = this.propertyDict ["restitution"];
                //Note: Uses the sprite's bounding box to make the actual staic bounding box
                //bodyBox.SetAsBox(this.objSprite.BoundingBox.Size.Width/2.0f, this.objSprite.BoundingBox.Size.Height/2.0f);

				//Note: this bodyBox.SetAsBox (10f, 10f); is temporary till i can figure out hot to do the sprite box thing
				Box2D.Common.b2Vec2 center = new Box2D.Common.b2Vec2(this.position.x-325,this.position.y-100);
				bodyBox.SetAsBox (35000f, 150f);

                this.body.CreateFixture(bodyBox, 1.0f);
            }
        }

		private void StartAnimation(CharacterAnimations charAnimations, string objSpriteName)
		{
			charAnimations.SetAnimationPosition (objSpriteName, this.position.x, this.position.y);
			charAnimations.RunAnimation (objSpriteName);
		}

		public void UpdateAnimation(string newAnimSpriteName)
		{
			this.charAnimations.StopAnimation (currentSpriteName);
			currentSpriteName = newAnimSpriteName;
			this.charAnimations.RunAnimation (newAnimSpriteName);
		}
    }
}
