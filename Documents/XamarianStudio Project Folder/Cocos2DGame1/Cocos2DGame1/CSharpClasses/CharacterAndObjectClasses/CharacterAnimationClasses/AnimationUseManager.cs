using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MindRain
{
	/// <summary>
	/// Note: This class manages the use and priority of animations.
	/// The constructor for this class will be empty because this class is just a container for many methods.
	/// Note: Methods will be created to handle animation playing depending on character state, 
	/// Locking in an animation till its compleation, unlocking an animation when animation cancling is enabled,
	/// and the transitioning between animations.
	/// <summary>

	public class AnimationUseManager
	{
		public AnimationUseManager ()
		{
		}

		//Note: Default Character states below
		//bool hit = false;
		//bool helpless = false;
		//bool invincible = false;
		//bool dizzy = false;
		//bool airborne = false;
		//bool attacking = false;
		//bool dead = false;
		//Note: untargetable means no interaction what so ever, think of this like a dodge where other characters move though yours
		//bool untargetable = false;
		//bool blocking = false;
		//bool teching = false;
		//bool knockedDown = false;
		//bool walking = false;
		//bool sprinting = false;
		//bool crouching = false;

		public void CharacterStateAnimations(Character character)
		{
			if(character.state.hit)
			{

			}
			if(character.state.helpless)
			{

			}
			if(character.state.invincible)
			{

			}
			if(character.state.dizzy)
			{

			}
			if(character.state.airborne)
			{

			}
			if(character.state.attacking)
			{

			}
			if(character.state.dead)
			{

			}
			if(character.state.untargetable)
			{

			}
			if(character.state.blocking)
			{

			}
			if(character.state.teching)
			{

			}
			if(character.state.knockedDown)
			{

			}
			if(character.state.walking)
			{
				character.characterObj.charAnimations.RunAnimation (character.characterObj.charAnimations.spriteNameList[0]);
				int frame = character.characterObj.charAnimations.GetFrameNumber (character.characterObj.charAnimations.spriteNameList [0]);
				character.characterFHB.UpdateHitBoxActivity ("test", frame);
			}
			else
			{
				character.characterObj.charAnimations.StopAnimation (character.characterObj.charAnimations.spriteNameList[0]);
			}
			if(character.state.sprinting)
			{
				character.characterObj.charAnimations.RunAnimation (character.characterObj.charAnimations.spriteNameList[1]);
			}
			else
			{
				character.characterObj.charAnimations.StopAnimation (character.characterObj.charAnimations.spriteNameList[1]);
			}
			if(character.state.crouching)
			{

			}
		}
	}
}

