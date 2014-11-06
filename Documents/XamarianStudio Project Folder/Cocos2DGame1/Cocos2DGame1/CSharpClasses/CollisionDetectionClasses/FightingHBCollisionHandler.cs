using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindRain
{
	public class FightingHBCollisionHandler
	{
		private List<Tuple<CharacterFightingHitBoxes.HitBox, Character>> attacker = new List<Tuple<CharacterFightingHitBoxes.HitBox, Character>>();
		private List<Tuple<CharacterFightingHitBoxes.HitBox, Character>> defender = new List<Tuple<CharacterFightingHitBoxes.HitBox, Character>>();
		private List<Tuple<CharacterFightingHitBoxes.HitBox, Character>> grabber = new List<Tuple<CharacterFightingHitBoxes.HitBox, Character>>();

		private Damaged damaged = new Damaged();

		private static int Offensive = 1;
		private static int Damageable = 2;
		private static int Invincible = 3;
		private static int Intangible = 4;
		private static int Reflective = 5;
		private static int Shield = 6;
		private static int Absorbing = 7;
		private static int Grab = 8;
		//private static int CollisionDetection = 9;

		public FightingHBCollisionHandler ()
		{
		}

		public void DetectCollision(List<Character> currentAlliedCharacters, List<Character> currentEnemyCharacters)
		{
			//Note: Another way of doing this may be to try and create my own listener for the rect class
			//just a note for later.

			for(int i=0; i<currentAlliedCharacters.Count; i++)
			{
				for(int j=0; j<currentAlliedCharacters [i].characterFHB.ActiveHBL.Count; j++)
				{
					for(int k=0; k<currentEnemyCharacters.Count; k++)
					{
						for(int l=0; l<currentEnemyCharacters[k].characterFHB.ActiveHBL.Count; l++)
						{
							if(currentAlliedCharacters [i].characterFHB.ActiveHBL [j].rect.IntersectsRect (currentEnemyCharacters [k].characterFHB.ActiveHBL [l].rect))
							{
								//Note: Creates 2 tuples whic contain the hitbox in question and the character and passes them to the collision handling function
								Tuple<CharacterFightingHitBoxes.HitBox, Character> alliedHBT = new Tuple<CharacterFightingHitBoxes.HitBox, Character> (currentAlliedCharacters [i].characterFHB.ActiveHBL [j], currentAlliedCharacters [i]);
								Tuple<CharacterFightingHitBoxes.HitBox, Character> enemyHBT = new Tuple<CharacterFightingHitBoxes.HitBox, Character> (currentEnemyCharacters [k].characterFHB.ActiveHBL [l], currentEnemyCharacters [k]);
								HandleCollision (alliedHBT,enemyHBT);
							}
						}
					}
				}
			}

		}

		private void HandleCollision(Tuple<CharacterFightingHitBoxes.HitBox, Character> alliedHB, Tuple<CharacterFightingHitBoxes.HitBox, Character> enemyHB)
		{
			var attacker = FindAttacker (alliedHB, enemyHB);
			var defender = FindDefender(alliedHB, enemyHB);
			var grabber =  FindGrabber(alliedHB, enemyHB);

			if(attacker == null && grabber == null)
			{
				//Note: NO ATTACK COLLISION SECITION
				//Note: This happends if two characters from oposing teams literally run into eachoter
				//in every case like this the characters are moved apart so they are not ontop of eachother.
				//But in there is one situation where this is not true, if 2 intangible targets touch nothing at all must happen.

				if(defender[0].Item1.hitboxType == defender[1].Item1.hitboxType && defender[0].Item1.hitboxType == Intangible)
				{
					//Note: if 2 intangible hitboxes touch dont do anything
				}
				else
				{
					//Note: if 2 of any other kind of defenseive hitboxes touch move them apart from eachother
				}
			}
			else if(grabber != null)
			{
				//Note: GRAB SECTION
				//Note: All grab cases are represented bellow.
				//Grabbing any non-offensive hitbox besides intangable and invincible should result in a grab landing.
				//Grabbing an offensive hitbox will result in nothing happening as well as grabbing a intangable target or invincible.
				//Grabbing another grab hitbox will cancel cancel a grab.

				if(grabber.Count > 1)
				{
					//Note: Grabs cancel eachother out
				}
				else if(attacker != null || defender[0].Item1.hitboxType == Intangible || defender[0].Item1.hitboxType == Invincible)
				{
					//Note: nothing happends
				}
				else if(defender[0].Item1.hitboxType == Damageable ||
					defender[0].Item1.hitboxType == Reflective || 
					defender[0].Item1.hitboxType == Shield || 
					defender[0].Item1.hitboxType == Absorbing)
				{
					//Note: Grab lands
				}
			}
			else
			{
				//Note: ATTACKER SECTION

				//Note: 0. simultanious attack case, This case occures when two players from opposite teams attack eachother and their attack hitboxes 
				//hit. In this case both attacks should be cancled. If the priority is the same. 
				//Note: Priority needs to be implemented, its not even in the property files yet.

				//Note: The last case is if one player is attack an enemy, there are several guidlines for this case. 
				//1. If the attacker hits a damageable hb it does damage and applies nockback.
				//2. If the attacker hits an invincible hb the attack does no damage or knockback but the attack hits. 
				//3. If the attacker tries to hit an intangable hb the hit does nothing, and does not hit the target. 
				//4. If the attacker hits a reflective target hitbox then damage and knockback are reflected to the attacker.  
				//5. If the attacker hits a shield hitbox only the shield takes damage but there is no knockback applied.
				//6. If the attacker hits an absorbing hitbox the enemy heals off of the attackers hit and takes no knockback.
				//7. If an attacker hits a collisionDetection hitbox it has not been decided what happends.

				 
				if(attacker.Count > 1)
				{
					//Note:0. simultanious attack case, This case occures when two players from opposite teams attack eachother and their attack hitboxes 
					//hit. In this case both attacks should be cancled. If the priority is the same.

				}
				else if(defender[0].Item1.hitboxType == Damageable)
				{
					//Note: 1. If the attacker hits a damageable hb it does damage and applies knockback.
					Character defenderCharacter = defender [0].Item2;
					Character attackerCharacter = attacker [0].Item2;
					damaged.HitDamagable (ref defenderCharacter, ref attackerCharacter);

				}
				else if(defender[0].Item1.hitboxType == Invincible)
				{
					//Note: 2. If the attacker hits an invincible hb the attack does no damage or knockback but the attack hits.

				}
				else if(defender[0].Item1.hitboxType == Intangible)
				{
					//Note: 3. If the attacker tries to hit an intangable hb the hit does nothing, and does not hit the target.
				}
				else if(defender[0].Item1.hitboxType == Reflective)
				{
					//Note: 4. If the attacker hits a reflective target hitbox then damage and knockback are reflected to the attacker.

				}
				else if(defender[0].Item1.hitboxType == Shield)
				{
					//Note: 5. If the attacker hits a shield hitbox only the shield takes damage but there is minimal knockback applied.
					Character defenderCharacter = defender [0].Item2;
					Character attackerCharacter = attacker [0].Item2;
					damaged.HitShield (ref defenderCharacter, ref attackerCharacter);
				}
				else if(defender[0].Item1.hitboxType == Absorbing)
				{
					//Note: 6. If the attacker hits an absorbing hitbox the enemy heals off of the attackers hit and takes no knockback.
					//only usable on ranged attacks

				}
			}
		}
			
		private List<Tuple<CharacterFightingHitBoxes.HitBox, Character>> FindDefender(Tuple<CharacterFightingHitBoxes.HitBox, Character> alliedHB, Tuple<CharacterFightingHitBoxes.HitBox, Character> enemyHB)
		{
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

			//Note: Makes sure the attacker list is clear so that the check is valid
			defender.Clear ();

			if((alliedHB.Item1.hitboxType == Damageable && enemyHB.Item1.hitboxType == Damageable) || 
				(alliedHB.Item1.hitboxType == Invincible && enemyHB.Item1.hitboxType == Invincible) || 
				(alliedHB.Item1.hitboxType == Reflective && enemyHB.Item1.hitboxType == Reflective) ||
				(alliedHB.Item1.hitboxType == Shield && enemyHB.Item1.hitboxType == Shield) ||
				(alliedHB.Item1.hitboxType == Absorbing && enemyHB.Item1.hitboxType == Absorbing))
			{
				defender.Add (alliedHB);
				defender.Add (enemyHB);
			}
			else if(alliedHB.Item1.hitboxType == Damageable || 
				alliedHB.Item1.hitboxType == Invincible || 
				alliedHB.Item1.hitboxType == Reflective || 
				alliedHB.Item1.hitboxType == Shield || 
				alliedHB.Item1.hitboxType == Absorbing)
			{
				defender.Add(alliedHB);
			}
			else if(enemyHB.Item1.hitboxType == Damageable ||
				enemyHB.Item1.hitboxType == Invincible ||
				enemyHB.Item1.hitboxType == Reflective ||
				enemyHB.Item1.hitboxType == Shield ||
				enemyHB.Item1.hitboxType == Absorbing)
			{
				defender.Add(enemyHB);
			}
			else
			{
				defender = null;
			}

			return defender;
		}

		private List<Tuple<CharacterFightingHitBoxes.HitBox, Character>> FindAttacker(Tuple<CharacterFightingHitBoxes.HitBox, Character> alliedHB, Tuple<CharacterFightingHitBoxes.HitBox, Character> enemyHB)
		{
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

			//Note: Makes sure the attacker list is clear so that the check is valid
			attacker.Clear ();

			if(alliedHB.Item1.hitboxType == Offensive && enemyHB.Item1.hitboxType == Offensive)
			{
				attacker.Add (alliedHB);
				attacker.Add (enemyHB);
			}
			else if(alliedHB.Item1.hitboxType == Offensive)
			{
				attacker.Add(alliedHB);
			}
			else if(enemyHB.Item1.hitboxType == Offensive)
			{
				attacker.Add(enemyHB);
			}
			else
			{
				attacker = null;
			}

			return attacker;
		}

		private List<Tuple<CharacterFightingHitBoxes.HitBox, Character>> FindGrabber(Tuple<CharacterFightingHitBoxes.HitBox, Character> alliedHB, Tuple<CharacterFightingHitBoxes.HitBox, Character> enemyHB)
		{
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

			//Note: Makes sure the attacker list is clear so that the check is valid
			grabber.Clear ();

			if(alliedHB.Item1.hitboxType == Grab && enemyHB.Item1.hitboxType == Grab)
			{
				grabber.Add (alliedHB);
				grabber.Add (enemyHB);
			}
			else if(alliedHB.Item1.hitboxType == Grab)
			{
				grabber.Add(alliedHB);
			}
			else if(enemyHB.Item1.hitboxType == Grab)
			{
				grabber.Add(enemyHB);
			}
			else
			{
				grabber = null;
			}

			return grabber;
		}
	}
}

