using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace GME1011BattleRoyale
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _gamefont; //we only used this for debugging

        private Random _rng;  //Mmmmm RNG
        
        private List<Hero> _heroList;  //heroes in a list, great idea

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            //initialize hero list and rng
            _heroList = new List<Hero>();
            _rng = new Random();
            
            
            base.Initialize();
        }



        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _gamefont = Content.Load<SpriteFont>("GameFont");  //just for debugging
            
            //create 15 heroes and put them in a list
            for (int i = 0; i < 15; i++)
            {
                int tempHealth = _rng.Next(2, 7); //health will be between 2 and 6
                Vector2 tempStartPosition = new Vector2(_rng.Next(1, 725), _rng.Next(1, 375)); //start position
                int tempSpeed = _rng.Next(75, 151);  //speed
                Vector2 tempDestination = new Vector2(_rng.Next(-725, 725), _rng.Next(-375, 375));

                //create the hero
                Hero tempHero = new Hero(tempHealth, tempStartPosition, tempSpeed, Content.Load<Texture2D>("tinyHero"), Content.Load<Texture2D>("health"));
                //set the destination
                tempHero.SetDestination(tempDestination);
                //add the temporary hero to the list
                _heroList.Add(tempHero);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //This is the magic.
            //For every hero in the list, we ask every other hero if they are too close to us.
            //If they are, we bounce in a random direction.
            for(int i = 0; i < _heroList.Count; i++)
            {
                for(int j = 0; j < _heroList.Count; j++)
                {
                    if (i != j) //don't compare us to ourselves, that causes issues
                    {
                        //if we are less than 50 pixels away from another hero...
                        if (Vector2.Distance(_heroList[i].GetCenter(), _heroList[j].GetCenter()) < 50)
                        {
                            //create a random destination and it's inverse
                            Vector2 tempDirection1 = new Vector2(_rng.Next(-725, 725), _rng.Next(-375, 375));
                            Vector2 tempDirection2 = new Vector2(-tempDirection1.X, -tempDirection1.Y);
                            
                            //normalize the new directions
                            tempDirection1.Normalize();
                            tempDirection2.Normalize();

                            //set the two heros to go in different directions.
                            _heroList[i].SetDestination(tempDirection1);
                            _heroList[j].SetDestination(tempDirection2);

                            //we need this in case the heros are overlapping - this moves them apart
                            _heroList[i].SetPosition(_heroList[i].GetPosition() + tempDirection1 * 5f);
                            _heroList[j].SetPosition(_heroList[j].GetPosition() + tempDirection2 * 5f);
                            
                            //we collided (too close) so let's take some damage!!
                            _heroList[i].TakeDamage();
                            _heroList[j].TakeDamage();
                        }
                    }
                   
                }
            }

            //for each hero in the list, call update, and if they are dead ( < 0 health ), remove them from the hero list
            for(int i = 0; i < _heroList.Count; i++)
            {
                _heroList[i].Update(gameTime);
                if (_heroList[i].isDead())
                {
                    _heroList.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //some debug to solve things in class
            /*
            _spriteBatch.Begin();
            foreach (Hero hero in _heroList)
            {
                //_spriteBatch.DrawString(_gamefont, hero.GetPosition() + "", new Vector2(10,10), Color.White);
                //_spriteBatch.DrawString(_gamefont, _collisionCount + "", new Vector2(10,10), Color.White);
                //_spriteBatch.DrawString(_gamefont, _heroList[0].GetBounds() + "", new Vector2(10, 30), Color.White);
            }
            _spriteBatch.End();
            */
            
            //draw the heroes!!!
            foreach (Hero hero in _heroList)
            {
                hero.Draw(_spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}