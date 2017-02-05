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
  public class AnimationPlayer
  {
    private Texture2D _heli;
    private Vector2 _animTextureOrigin = Vector2.Zero;
    private double _animFrameElapsed = 0.0;
    private int _animFrameNum = 0;
    private Level level;
    private string animationName;
    private Vector2 position;

    private const int ANIM_FRAME_COUNT = 34;
    public AnimationPlayer(Level level,string animationName,Vector2 position)
    {
      // TODO: Construct any child components here
      this.level = level;
      this.animationName = animationName;
      this.position = position;
      LoadContent();
    }

    protected void LoadContent()
    {
      _heli = level.Content.Load<Texture2D>(animationName);
      _animTextureOrigin.X = (_heli.Width/2)/ANIM_FRAME_COUNT;
      _animTextureOrigin.Y = _heli.Height/2;
    }

    public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
    {
      _animFrameElapsed += gameTime.ElapsedGameTime.TotalSeconds;

      if(_animFrameElapsed >= 0.05)
        {
          _animFrameNum = (_animFrameNum + 1)%ANIM_FRAME_COUNT;
          _animFrameElapsed = 0;
        }

      Rectangle sourceRectangle = new Rectangle(_animFrameNum*(_heli.Width/ANIM_FRAME_COUNT),0,_heli.Width/ANIM_FRAME_COUNT,_heli.Height);

      spriteBatch.Draw(_heli,position,sourceRectangle,Color.White,0.0f,_animTextureOrigin,1.0f,SpriteEffects.None,0.0f);
    }

  }
}