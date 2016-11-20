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
    public class Target
    {
      private Texture2D texture;
      public Vector2 position = Vector2.Zero;
      private Level level;
      
      public Target(Level level,Vector2 position)
      {
        // TODO: Construct any child components here
        this.level = level;
        this.position = position;
        LoadContent();
      }

      public void LoadContent()
      {
        texture = level.Content.Load<Texture2D>("Sprite\\hole");
      }

      public void Draw(SpriteBatch spriteBatch)
      {
        spriteBatch.Draw(texture,position,Color.White);
      }
    }
}