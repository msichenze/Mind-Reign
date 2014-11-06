using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindRain
{
	//Note: The damaged class is the class that calculates the damage done to a hit target and 
	//calculates the knockback of the hit
	public class Damaged
	{
		//Note: This classes constructor is empty because it is just a container for methods, none
		//need to be run when this class is created.
		public Damaged ()
		{
		}

		public void HitDamagable(ref Character characterHit, ref Character attackingCharacter)
		{
			//Note: Add damage to hit character
			characterHit.state.healthPercentage += CalculateDamage (characterHit, attackingCharacter);

			//Note: All of this stuff is just test stuff, it may or maynot change
			float knockBack = CalculateKnockBack (characterHit, attackingCharacter);

			//Note: to create a force that goes to a certain speed at by a certain time use the f=ma formula where 
			//m is the mass and a is the amount of frames it should take out of 60
			float hitForce = characterHit.characterObj.body.Mass * (knockBack) / (1f);
			Box2D.Common.b2Vec2 hitForceVec = new Box2D.Common.b2Vec2 (hitForce, 0);

			//Note: apply knockback to hit character
			//Note: Could change this to loading a script that runs a specific kind of force, for more complicated attacks
			characterHit.characterObj.body.ApplyForce(hitForceVec, characterHit.characterObj.body.WorldCenter);

		}

		public void HitShield(ref Character characterHit, ref Character attackingCharacter)
		{
			characterHit.state.currentShieldHealth -= attackingCharacter.state.attackNameDict [attackingCharacter.currentAttackName];
			float knockBack = CalculateShieldedKnockBack (characterHit, attackingCharacter);

			//Note: to create a force that goes to a certain speed at by a certain time use the f=ma formula where 
			//m is the mass and a is the amount of frames it should take out of 60
			float hitForce = characterHit.characterObj.body.Mass * (knockBack) / (1f);
			Box2D.Common.b2Vec2 hitForceVec = new Box2D.Common.b2Vec2 (hitForce, 0);

			//Note: apply knockback to hit character
			//Note: Could change this to loading a script that runs a specific kind of force, for more complicated attacks
			characterHit.characterObj.body.ApplyForce(hitForceVec, characterHit.characterObj.body.WorldCenter);
		}

		private float CalculateDamage(Character characterHit, Character attackingCharacter)
		{
			float attackDamage = attackingCharacter.state.attackNameDict [attackingCharacter.currentAttackName];
			float damage =  attackDamage - (attackDamage * (characterHit.state.armor/100));
			return damage;
		}
			
		private float CalculateKnockBack(Character characterHit, Character attackingCharacter)
		{
			float attackingKB = attackingCharacter.state.knockBackNameDict [attackingCharacter.currentKnockBackName];
			attackingKB += attackingKB * (characterHit.state.healthPercentage / 100);
			float knockB = attackingKB - (attackingKB*(characterHit.state.launchResistance/100));
			return knockB;
		}

		private float CalculateShieldedKnockBack(Character characterHit, Character attackingCharacter)
		{
			float shieldKBReduction = 0.95f;
			float attackingKB = attackingCharacter.state.knockBackNameDict [attackingCharacter.currentKnockBackName];
			attackingKB += attackingKB * (characterHit.state.healthPercentage / 100);
			attackingKB -= attackingKB * (characterHit.state.launchResistance / 100);
			float knockB = attackingKB - (attackingKB * shieldKBReduction);
			return knockB;
		}
	}
}

