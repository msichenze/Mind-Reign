using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

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

		public Player ()
		{

		}

		public Player (Character character, int playerNumber)
		{
			this.character = character;
			this.playerNumber = playerNumber;
			lastPos =character.characterObj.body.Position.y;
		}

		public void moveCharacter(Box2D.Dynamics.b2Body characterBody)
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

			if (input.IsOldPress(Keys.Right) || input.IsOldPress(Keys.D))
			{
				rDoubleTap = true;
				//character.characterObj.charAnimations.SetAnimationSpeed (character.characterObj.objSprite.Name, 0.001f);
			}
			else if (input.IsOldPress(Keys.Left) || input.IsOldPress(Keys.A))
			{
				lDoubleTap = true;
				//character.characterObj.charAnimations.SetAnimationSpeed (character.characterObj.objSprite.Name, 0.001f);
			}
			if (input.IsCurPress(Keys.Right) || input.IsCurPress(Keys.D))
			{
				character.characterObj.charAnimations.FlipAnimation (character.characterObj.objSprite.Name, false);
				character.characterObj.charAnimations.RunAnimation (character.characterObj.objSprite.Name);
				if(rDoubleTap == true)
				{
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
					if(characterBody.LinearVelocity.x < character.state.speed)
					{
						characterBody.ApplyForce(forceVecR,characterBody.WorldCenter);
					}
				}
				lDoubleTap = false;
			}
			else if (input.IsCurPress(Keys.Left) || input.IsCurPress(Keys.A))
			{

				character.characterObj.charAnimations.FlipAnimation (character.characterObj.objSprite.Name, true);
				character.characterObj.charAnimations.RunAnimation (character.characterObj.objSprite.Name);
				if(lDoubleTap == true)
				{
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
					if(characterBody.LinearVelocity.x > -character.state.speed)
					{
						characterBody.ApplyForce(forceVecL,characterBody.WorldCenter);
					}
				}
				rDoubleTap = false;
			}
			else
			{
				character.characterObj.charAnimations.StopAnimation(character.characterObj.objSprite.Name);
			}

			if(input.IsNewPress(Keys.Up) || input.IsNewPress(Keys.W))
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
			else if((input.IsCurPress(Keys.Up) || input.IsCurPress(Keys.W)) && applyImp == true)
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

		}

	}
}

