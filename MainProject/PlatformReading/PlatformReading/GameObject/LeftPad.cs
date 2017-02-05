using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace PlatformerReading
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class LeftPad
    {
      private Texture2D texture;
      public Vector2 position;
      public int width = 10;
      public int height = 50;

      public Level Level
      {
        get{return level;}
      }
      Level level;
      
      public LeftPad(Level level,Vector2 position)
      {
        // TODO: Construct any child components here
        this.level = level;
        this.position = position;
        LoadContent();
      }

      public void LoadContent()
      {
        texture = Level.Content.Load<Texture2D>("Sprite\\pad");
      }

      public void Draw(SpriteBatch spritebatch)
      {
        spritebatch.Draw(texture,position,Color.White);
      }

    }
}