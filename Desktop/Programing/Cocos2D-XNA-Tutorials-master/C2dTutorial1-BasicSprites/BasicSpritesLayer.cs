using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework.Input;

namespace C2dTutorial1_BasicSprites
{
    /// <summary>
    /// A Cocos2D-XNA layer that manages all the sprites in the game.  A few caveats about what I did here:
    /// * I wanted to bounce my sprites around the game screen after spawning at random locations.  This didn't lend itself
    ///   well to the action style of movement that Cocos2D-XNA provides (i.e. stringing together one or more actions to complete
    ///   movement).  So I updated the sprite positions manually through a scheduled call to the Update method.  It's not wrong (at least
    ///   I don't think so), it's just different.
    /// * Many of the sprite operations can be performed by corresponding actions as well.  Again, I chose to manually modify the sprite
    ///   properties instead of using the corresponding actions, mainly because I'm still learning.  I'll be addressing actions in a
    ///   different tutorial at some point in the future.
    /// </summary>
    internal class BasicSpritesLayer : CCLayer
    {
        private Dictionary<string, BasicSprite> _sprites;        // Contains the list of all the sprites in the sprite layer
        private Random _rand;                                    // Your friendly neighborhood random number generator
        private bool _doUpdate = true;                           // Determines if the Update event for the sprite layer is called

        public BasicSpritesLayer()
        {
            // Initialize the sprite list
            _sprites = new Dictionary<string, BasicSprite>();

            // Initialize the random number generator
            _rand = new Random();

            // Start with three sprites on the sprite layer
            for (int i = 0; i < 3; i++)
                CreateSprite();

            // Calling this method tells Cocos2D-XNA to call the Update method of the layer during screen updates
            ScheduleUpdate();
        }

        /// <summary>
        /// Creates a new sprite and adds it to the sprite list & screen.
        /// </summary>
        internal void CreateSprite()
        {
            // Get the dimensions of the game window
            var winSize = CCDirector.SharedDirector.WinSize;

            // Get the new number of sprites on the screen and setup an id & image for the sprite (cycles between the 3 available images)
            var numSprites = _sprites.Count + 1;
            var spriteId = "sprite" + numSprites;
            var spriteImage = "Images/Sprite" + ((numSprites % 3) + 1);

            // Create a new instance of the sprite, then setup the position, velocity, & zorder
            var newSprite = new BasicSprite(spriteImage);
            newSprite.SetPosition(_rand.Next(150, (int)(winSize.Width - 150f)), _rand.Next(150, (int)(winSize.Height - 150f)));
            newSprite.SetVelocity(new CCPoint(_rand.Next(1, 7), _rand.Next(1, 7)));
            newSprite.ZOrder = _sprites.Count;

            // Add the new sprite to the sprite list
            _sprites.Add(spriteId, newSprite);

            // Add the sprite to the layer
            AddChild(newSprite);
        }

        /// <summary>
        /// Deletes a sprite from the sprite list & screen.
        /// </summary>
        internal void DeleteSprite()
        {
            // Make sure we have some sprites
            if (_sprites.Any())
            {
                // Get the id of the last sprite in the sprite list
                var spriteId = "sprite" + _sprites.Count;

                // Get the sprite to delete
                var deleteSprite = _sprites[spriteId];

                // Remove the sprite from the sprite list
                _sprites.Remove(spriteId);

                // Remove the sprite from the layer
                RemoveChild(deleteSprite, true);
            }
        }

        /// <summary>
        /// Toggles the state of the call to the Update method for the sprite layer.  Basically, it stops/starts the sprite movement.
        /// </summary>
        internal void ToggleUpdateSchedule()
        {
            // Toggle the update flag
            _doUpdate = !_doUpdate;

            // Tell Cocos2D-XNA to schedule or unschedule the call to the Update method
            if (_doUpdate)
                ScheduleUpdate();
            else
                UnscheduleUpdate();
        }

        /// <summary>
        /// Moves all the sprites on the screen based on the arrow key that is pressed.
        /// </summary>
        /// <param name="arrowKey">The key code of the arrow key that is pressed.</param>
        internal void SetPosition(Keys arrowKey)
        {
            // Stop the sprites on the screen if necessary
            if (_doUpdate)
                ToggleUpdateSchedule();

            // Get the dimensions of the game window
            var winSize = CCDirector.SharedDirector.WinSize;

            // Calculate a position offset to move the sprites, negative or positive based on the direction to move
            var positionOffset = (arrowKey == Keys.Left || arrowKey == Keys.Down) ? -4 : 4;

            // Set the position of each sprite in the sprite list
            foreach (var sprite in _sprites)
            {
                // By default, Cocos2D-XNA has the sprite origin in the center of the sprite.  So we determine the midpoint to help keep 
                // the sprite in the bounds of the game window.
                var halfWidth = sprite.Value.ContentSizeInPixels.Width / 2;
                var halfHeight = sprite.Value.ContentSizeInPixels.Height / 2;

                // Move the sprite horizontally by the position offset, keeping the sprite in the window
                if (arrowKey == Keys.Left || arrowKey == Keys.Right)
                    sprite.Value.PositionX = Math.Min(winSize.Width - halfWidth, Math.Max(halfWidth, sprite.Value.PositionX + positionOffset));

                // Move the sprite vertically by the position offset, keeping the sprite in the window
                if (arrowKey == Keys.Up || arrowKey == Keys.Down)
                    sprite.Value.PositionY = Math.Min(winSize.Height - halfHeight, Math.Max(halfHeight, sprite.Value.PositionY + positionOffset));
            }
        }

        /// <summary>
        /// Toggles the visible state of all the sprites.
        /// </summary>
        internal void ToggleVisible()
        {
            foreach (var sprite in _sprites)
                sprite.Value.Visible = !sprite.Value.Visible;
        }

        /// <summary>
        /// Changes the z-order of the sprites on the screen.
        /// </summary>
        internal void ChangeZOrder()
        {
            // Align the z-orders between 1-3... keeps all colors in sync on the same z-order
            foreach (var sprite in _sprites)
                sprite.Value.ZOrder = (sprite.Value.ZOrder % 3) + 1;
        }

        /// <summary>
        /// Changes the opacity of the sprites on the screen.
        /// </summary>
        /// <param name="moreVisible">A boolean value that determines if we're making the sprites more visible.</param>
        internal void Opacity(bool moreVisible)
        {
            // Determine opacity offset
            var opacityOffset = moreVisible ? 1 : -1;
            foreach (var sprite in _sprites)
                sprite.Value.Opacity = (byte)(sprite.Value.Opacity + opacityOffset);
        }

        /// <summary>
        /// Rotates all the sprites clockwise or counter-clockwise.
        /// </summary>
        /// <param name="clockwise">A boolean value that determines if we're rotating in a clockwise direction.</param>
        internal void Rotate(bool clockwise)
        {
            // Determine rotation offset
            var rotateOffset = clockwise ? 5 : -5;
            foreach (var sprite in _sprites)
                sprite.Value.Rotation = (sprite.Value.Rotation + rotateOffset) % 360;
        }

        /// <summary>
        /// Scales all the sprites bigger or smaller.
        /// </summary>
        /// <param name="bigger">A boolean value that determines if the sprites should be scaled bigger.</param>
        internal void Scale(bool bigger)
        {
            // Determine scaling offset
            var scaleOffset = bigger ? 0.02f : -0.02f;

            // Keep the scaling value from going less than zero... doesn't cause errors, but sprites start violating the window bounds if the scale goes negative
            foreach (var sprite in _sprites)
                sprite.Value.Scale = Math.Max(0, sprite.Value.Scale + scaleOffset);
        }

        /// <summary>
        /// Skews the sprites forward or backward.
        /// </summary>
        /// <param name="skewForward"></param>
        internal void Skew(bool skewForward)
        {
            // Determine the skew offset based on the direction
            var skewOffset = skewForward ? 0.2f : -0.2f;

            // Adjust the x & y skew values by the offset
            foreach (var sprite in _sprites)
            {
                sprite.Value.SkewX += skewOffset;
                sprite.Value.SkewY += skewOffset;
            }
        }

        /// <summary>
        /// Flips all the sprites horizontally or vertically.
        /// </summary>
        /// <param name="flipX">A boolean value that determines if the sprites should be flipped horizontally.</param>
        internal void Flip(bool flipX)
        {
            foreach (var sprite in _sprites)
            {
                if (flipX)
                    sprite.Value.FlipX = !sprite.Value.FlipX;
                else
                    sprite.Value.FlipY = !sprite.Value.FlipY;
            }
        }

        /// <summary>
        /// Update method for the sprites layer.  Only called if Cocos2D-XNA has been instructed to schedule the update.
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);

            // Move all the sprites based on their velocity
            foreach (var sprite in _sprites)
                sprite.Value.MoveByVelocity();
        }

    }
}
