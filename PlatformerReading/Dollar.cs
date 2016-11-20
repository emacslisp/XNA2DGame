using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace PlatformerReading
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Dollar
    {
      private Texture2D texture;
      public Vector2 position;
      public int width = 20;
      public int height = 20;
      public Level level;

      public Level Level
      {
        get{return level;}
      }
      public Dollar(Level level,Vector2 position)
      {
        this.level = level;
        this.position = position;
        LoadContent();
      }

      public void LoadContent()
      {
        texture = Level.Content.Load<Texture2D>("Sprite\\dollar");
      }

      public void Draw(SpriteBatch spritebatch)
      {
          spritebatch.Draw(texture, position, Color.White);
      }
    }
}