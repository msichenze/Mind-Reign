using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Box2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindRain
{
	public class CollisionDetection: Box2D.Dynamics.b2ContactListener
	{
		Character character = new Character();

		public CollisionDetection ()
		{

		}
		public override void BeginContact (Box2D.Dynamics.Contacts.b2Contact contact)
		{
			base.BeginContact (contact);
		}
		public override void EndContact (Box2D.Dynamics.Contacts.b2Contact contact)
		{
			base.EndContact (contact);
		}
		public override void PostSolve (Box2D.Dynamics.Contacts.b2Contact contact, ref Box2D.Dynamics.b2ContactImpulse impulse)
		{
			//throw new NotImplementedException ();
		}
		public override void PreSolve (Box2D.Dynamics.Contacts.b2Contact contact, Box2D.Collision.b2Manifold oldManifold)
		{
			//throw new NotImplementedException ();
			character.state.jumpsDone = 0f;
		}
		public void getCharacter(Character character)
		{
			this.character = character;
		}
	}
}

