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
    public class SpriteToRender
    {
      public Texture2D texture;
      public Rectangle rect;
      public Rectangle sourceRect;
      public Color color;
      public SpriteToRender(Texture2D setTexture,Rectangle setRect,
                            Rectangle setSourceRect,Color setColor)
        {
            // TODO: Construct any child components here
          texture = setTexture;
          rect = setRect;
          sourceRect = setSourceRect;
          color = setColor;
        }
    }
}