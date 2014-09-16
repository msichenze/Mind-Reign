using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Box2D;


namespace MindRain
{
    class Object
    {
        public string objPropertiesFileName;
        public Box2D.Dynamics.b2Body body;
        public Box2D.Common.b2Vec2 postion;
        public CCSprite objSprite;
        //Note: Files loaded into the propertyDict should have the "CanMove" string followed by a 1 or a zero for yes or no
        //The File should also have "Density" and "Friction" if Can Move ==1
        public Dictionary<string,float> propertyDict;
        //Note: world defines things like gravity and if the object is sleeping one world will be defined
        //for this project and that world will be passed to each object.
        public Box2D.Dynamics.b2World world;
        
        public Object(Box2D.Common.b2Vec2 position, string objSpriteName, string objPropertiesFileName, Box2D.Dynamics.b2World world)
        {
            this.postion = position;
            this.world = world;
            this.objPropertiesFileName = objPropertiesFileName;
			LoadObjectFiles(objSpriteName, propertyDict);
            CreateBoundingBox();

        }
        
		private void LoadObjectFiles(string spriteName, Dictionary<string,float> propertyDict)
        {
            //Load sprite
            //Note: The .png can change I dont know what the image type i will use
            this.objSprite = new CCSprite("Content/Sprites/" + spriteName + ".png");
            //Read the properties of the object into an array
            //Note: Read AllLines reads in by line all the content on the line
            string[] properties = System.IO.File.ReadAllLines("Content/PropertyFiles/" + this.objPropertiesFileName + ".txt");
            foreach (string property in properties)
            {
                float propertyVal = 0;
                string numberonly = "";
                string onlyLetters = "";
                //Note: This should find only the letters from each line of the file
                onlyLetters = new string(property.Where(Char.IsLetter).ToArray());
                //Note: This should find only the numbers in each line of the file
                numberonly = System.Text.RegularExpressions.Regex.Match(property, @"\d+").Value;
                propertyVal = float.Parse(numberonly);
                //Note: This should add both the letters and the numbers from a single line of
                //the file to a dictionary for ease of access
                propertyDict.Add(onlyLetters,propertyVal);
            }
			this.propertyDict = propertyDict;
        }

        private void CreateBoundingBox()
        {
            Box2D.Dynamics.b2BodyDef bodyDef = new Box2D.Dynamics.b2BodyDef();
            Box2D.Collision.Shapes.b2PolygonShape bodyBox = new Box2D.Collision.Shapes.b2PolygonShape();
            Box2D.Dynamics.b2FixtureDef fixtureDef = new Box2D.Dynamics.b2FixtureDef();

            //Note: If the properyDict contains the CanMove prop and it is 1 or yes then create a dynamic body and not a static one
            //Note: Dynamic bodies are for things like characters and moving objects, static bodies cannot move ever.
            if(this.propertyDict.ContainsKey("CanMove") && this.propertyDict["CanMove"] == 1)
            {
                //Note: Set body type
                bodyDef.type = Box2D.Dynamics.b2BodyType.b2_dynamicBody;
                bodyDef.position.Set(this.postion.x, this.postion.y);
                //Note The b2Body body must be createdd after the bodyDef is compleated
                this.body = this.world.CreateBody(bodyDef);
                //Note: Uses the sprite's bounding box to make the actual dynamic bounding box
                bodyBox.SetAsBox(this.objSprite.BoundingBox.Size.Width / 2.0f, this.objSprite.BoundingBox.Size.Height / 2.0f);
                //Note: The line below should read ...=dynamicBox;
                fixtureDef.shape = bodyBox;
                fixtureDef.density = this.propertyDict["Density"];
                fixtureDef.friction = this.propertyDict["Friction"];
                this.body.CreateFixture(fixtureDef);
            }
            //Note: If the dict says CanMove is 0 or no then create a static body
            else
            {
                //Note: Normaly before creating the body box one would set a bodyDef type but in this case it is static and that is the default
                bodyDef.position.Set(this.postion.x, this.postion.y);
                //Note The b2Body body must be createdd after the bodyDef is compleated
                this.body = this.world.CreateBody(bodyDef);
                //Note: Uses the sprite's bounding box to make the actual staic bounding box
                bodyBox.SetAsBox(this.objSprite.BoundingBox.Size.Width/2.0f, this.objSprite.BoundingBox.Size.Height/2.0f);
                this.body.CreateFixture(bodyBox, 0.0f);
            }
        }
    }
}
