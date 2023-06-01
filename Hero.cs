using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GME1011BattleRoyale
{
    internal class Hero
    {

        private int _health;  //hero health
        private Vector2 _position, _destination;  //vectors for where we are and where we're going
        private float _speed;  //how fast are we gonna get there?
        private int _damageTimer; //This helps to avoid multiple damage hits in a single collisions

        private Texture2D _heroSprite, _heroHealthSprite;  //textures for the hero and their health

        //argumented constructors
        public Hero(int health, Vector2 position, float speed, Texture2D heroSprite, Texture2D heroHealthSprite)
        {
            //set all the initial values
            _health = health;
            _position = position;
            _speed = speed;
            _heroSprite = heroSprite;
            _heroHealthSprite = heroHealthSprite;
            
            _damageTimer = 0;
            _destination = new Vector2(-1,-1);
        }

        //some accessors
        public Vector2 GetPosition(  ) { return _position;  }
        public Vector2 GetCenter() { return new Vector2(_position.X + _heroSprite.Width / 2, _position.Y + _heroSprite.Height / 2); }
        public Vector2 GetDestination() { return _destination; }
        public float GetSpeed() { return _speed; }
        public bool isDead() { return _health <= 0; }


        //this isn't being used, but this is how we get the collision box of an object
        public Rectangle GetBounds()
        {
            Rectangle bounds = new Rectangle((int)_position.X, (int)_position.Y, _heroSprite.Width, _heroSprite.Height);
            return bounds;
        }


        //some mutators
        public void SetPosition(Vector2 newPosition) { _position = newPosition; }
        public void SetDestination(Vector2 destination) { _destination = destination; }
        public void SetSpeed(float speed) { _speed = speed; }

        //take damage ONLY if the damage timer is > 30 (0.5 seconds), this creates a damage lag
        public void TakeDamage()
        {
            if (_damageTimer > 30)
            {
                _health--;
                _damageTimer = 0;
            }
        }

        //this is the main movement method - it bounces at the edges of the screen
        public void bounce(GameTime gameTime)
        {
            _destination.Normalize();                   // sets the vector to unit vector
            if (_position.X < 0 || _position.X > 800 - _heroSprite.Width)
            {
                _destination.X = -_destination.X;
            }
            if (_position.Y < 0 || _position.Y > 480 - _heroSprite.Height)
            {
                _destination.Y = -_destination.Y;
            }

            _position += _destination * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }



        //what to do every time step?
        public void Update(GameTime gameTime)
        {
            //call the bounce method, which also controls movement.
            bounce(gameTime);
            //damage timer is always increasing
            _damageTimer++;

            //uncomment this if you want to try the move method, instead of bounce( )
            /*
            if (Vector2.Distance(_position, _destination) > 10)
            {
                move(_destination, gameTime);
            }
            */
        }

        //here is the move method that we started with
        public void move(Vector2 target, GameTime gameTime)
        {
            //adpated from: https://stackoverflow.com/questions/12715096/xna-vector-math-movement

            if (_destination.X != -1 && _destination.Y != -1)
            {
                Vector2 temp = (target - _position); // gets the difference between the target and position
                temp.Normalize();                   // sets the vector to unit vector
                temp *= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                  // sets the vector to be the length of moveSpeed
                float x = temp.X;
                float y = temp.Y;

                _position += new Vector2(x, y);
            }
        }


        //here is our draw method that draws our hero and their health
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_heroSprite, _position, Color.White);
            for(int i = 0; i < _health; i++)
                spriteBatch.Draw(_heroHealthSprite, new Vector2(_position.X + i * 10, _position.Y - 15), Color.White);
            spriteBatch.End();
        }
    }
}
