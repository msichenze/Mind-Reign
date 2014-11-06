using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.IO;

namespace MindRain
{
	public class Player
	{
		public int playerNumber = 0;
		public Character character = new Character();
		private bool rDoubleTap = false;
		private bool lDoubleTap = false;
		float lastPos = 0;
		private bool applyImp = true;
		public Dictionary<string,Keys> keyBindingsDict = new Dictionary<string, Keys> ();

		public Player ()
		{

		}

		public Player (Character character, int playerNumber, string keyBindingsFileName)
		{
			this.character = character;
			this.playerNumber = playerNumber;
			CreateKeys (loadJson (keyBindingsFileName));
			lastPos =character.characterObj.body.Position.y;
		}

		public Character moveCharacter(Box2D.Dynamics.b2Body characterBody)
		{
			//Note: speed and dashspeeds are now max speeds so the inpulses need to be fixed and more velocity checking needs to be done bellow in the if statments
			float jumpForce = characterBody.Mass * (character.state.jumpLaunchSpeed) / (1/60f);
			Box2D.Common.b2Vec2 jumpImpulse = new Box2D.Common.b2Vec2 (0f, jumpForce);

			float jumpForceAfter = characterBody.Mass * (character.state.jumpLaunchSpeed) / (1/60f);
			Box2D.Common.b2Vec2 jumpImpulseAfter = new Box2D.Common.b2Vec2 (0f, jumpForceAfter);

			//Note: to create a force that goes to a certain speed at by a certain time use the f=ma formula where m is the mass and a is the amount of frames it should take out of 60
			float forceR = characterBody.Mass * (character.state.speed) / (1f);
			Box2D.Common.b2Vec2 forceVecR = new Box2D.Common.b2Vec2 (forceR, 0);

			float instantForceR = character.state.dashSpeed / (1f);
			Box2D.Common.b2Vec2 dashForceVecR = new Box2D.Common.b2Vec2 (instantForceR, characterBody.LinearVelocity.y);


			float forceL = characterBody.Mass * (-character.state.speed) / (1f);
			Box2D.Common.b2Vec2 forceVecL = new Box2D.Common.b2Vec2 (forceL, 0);

			float instantForceL = (-character.state.dashSpeed) / (1f);
			Box2D.Common.b2Vec2 dashForceVecL = new Box2D.Common.b2Vec2 (instantForceL, characterBody.LinearVelocity.y);


			//Note: Reference for the input helper
			var input = Game1.Input;

			if (input.IsOldPress(this.keyBindingsDict["moveRight"]) || input.IsOldPress(Keys.D))
			{
				rDoubleTap = true;
			}
			else if (input.IsOldPress(this.keyBindingsDict["moveLeft"]) || input.IsOldPress(Keys.A))
			{
				lDoubleTap = true;
			}
			if (input.IsCurPress(this.keyBindingsDict["moveRight"]) || input.IsCurPress(Keys.D))
			{
				character.characterObj.charAnimations.FlipAnimation (character.characterObj.charAnimations.spriteNameList, false);
				character.characterObj.charAnimations.flipped = false;
				if(rDoubleTap == true)
				{
					character.state.walking = false;
					character.state.sprinting = true;

					//Note:Right DoubleTap
					if(characterBody.LinearVelocity.x < character.state.dashSpeed)
					{
						characterBody.LinearVelocity = dashForceVecR;
						lDoubleTap = false;
					}
				}
				else
				{
					//Note: Right movement hold
					character.state.walking = true;
					character.state.sprinting = false;

					if(characterBody.LinearVelocity.x < character.state.speed)
					{
						characterBody.ApplyForce(forceVecR,characterBody.WorldCenter);
					}
				}
				lDoubleTap = false;
			}
			else if (input.IsCurPress(this.keyBindingsDict["moveLeft"]) || input.IsCurPress(Keys.A))
			{

				character.characterObj.charAnimations.FlipAnimation (character.characterObj.charAnimations.spriteNameList, true);
				character.characterObj.charAnimations.flipped = true;
				if(lDoubleTap == true)
				{
					character.state.walking = false;
					character.state.sprinting = true;

					//Note: double tap movement
					if(characterBody.LinearVelocity.x > -character.state.dashSpeed)
					{
						characterBody.LinearVelocity = dashForceVecL;
						rDoubleTap = false;
					}
				}
				else
				{
					//Note: Left movement hold
					character.state.walking = true;
					character.state.sprinting = false;

					if(characterBody.LinearVelocity.x > -character.state.speed)
					{
						characterBody.ApplyForce(forceVecL,characterBody.WorldCenter);
					}
				}
				rDoubleTap = false;
			}
			else
			{
				character.state.walking = false;
				character.state.sprinting = false;
			}

			if(input.IsNewPress(this.keyBindingsDict["jump"]) || input.IsNewPress(Keys.W))
			{
				//Note: Jumping will be done with forces later and have a counter to check the number of jumps allowed until the ground is touched or character is hit
				if(character.state.jumpsDone < character.state.numberOfJumps)
				{
					//characterBody.ApplyLinearImpulse (jumpImpulse, characterBody.WorldCenter);
					//characterBody.ApplyForce (jumpImpulse, characterBody.WorldCenter);

					//Note: Put burst impuse of jump here small but short
					character.state.jumpsDone += 1;
					lastPos = characterBody.Position.y;
					applyImp = true;
				}

			}
			else if((input.IsCurPress(this.keyBindingsDict["jump"]) || input.IsCurPress(Keys.W)) && applyImp == true)
			{

				if(character.state.jumpsDone < character.state.numberOfJumps)
				{
					if(characterBody.Position.y < character.state.maxJumpHeight+lastPos)
					{
						//Note: this is the continuous impulse that drives the jump to its peak
						characterBody.ApplyLinearImpulse (jumpImpulse, characterBody.WorldCenter);
					}
					else
					{
						applyImp = false;
					}
					//characterBody.ApplyLinearImpulse (jumpImpulse, characterBody.WorldCenter);
					//characterBody.ApplyLinearImpulse (jumpImpulse, characterBody.WorldCenter);
					//characterBody.ApplyForce (jumpImpulse, characterBody.WorldCenter);
					//characterBody.ApplyLinearImpulse (jumpImpulseAfter, characterBody.WorldCenter);
				}

			}

			return character;
		}

		private void CreateKeys(keyBindingsJson KBJObject)
		{
			Keys key;

			Enum.TryParse(KBJObject.crouch, out key);
			this.keyBindingsDict.Add ("crouch", key);

			Enum.TryParse(KBJObject.dodge, out key);
			this.keyBindingsDict.Add ("dodge", key);

			Enum.TryParse(KBJObject.grab, out key);
			this.keyBindingsDict.Add ("grab", key);

			Enum.TryParse(KBJObject.jump, out key);
			this.keyBindingsDict.Add ("jump", key);

			Enum.TryParse(KBJObject.moveLeft, out key);
			this.keyBindingsDict.Add ("moveLeft", key);

			Enum.TryParse(KBJObject.moveRight, out key);
			this.keyBindingsDict.Add ("moveRight", key);

			Enum.TryParse(KBJObject.primaryAttack, out key);
			this.keyBindingsDict.Add ("primaryAttack", key);

			Enum.TryParse(KBJObject.secondaryAttack, out key);
			this.keyBindingsDict.Add ("secondaryAttack", key);

			Enum.TryParse(KBJObject.shield, out key);
			this.keyBindingsDict.Add ("shield", key);

			Enum.TryParse(KBJObject.ultraAttack, out key);
			this.keyBindingsDict.Add ("ultraAttack", key);
		}

		public void UpdateKeys()
		{
			//Note: not implemented yet :D
		}

		private keyBindingsJson loadJson(string jsonFN)
		{
			keyBindingsJson KBJObject = new keyBindingsJson();

			//Note: Load textfile
			//Note: Reads all the json into one string and ten deserialzes it into an object
			using(StreamReader r = new StreamReader("Content/propertyFiles/keyBindings/" +jsonFN + ".json"))
			{
				string json = r.ReadToEnd ();
				KBJObject = JsonConvert.DeserializeObject <keyBindingsJson>(json);
			}
			return KBJObject;
		}

		//Note: Class for json to be read into and keybindings taken out off
		private class keyBindingsJson
		{
			public string moveLeft { get; set; }
			public string moveRight { get; set; }
			public string jump { get; set; }
			public string crouch { get; set; }
			public string primaryAttack { get; set; }
			public string secondaryAttack { get; set; }
			public string grab { get; set; }
			public string shield { get; set; }
			public string dodge { get; set; }
			public string ultraAttack { get; set; }
		}

	}
}

