using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace Engine;

public class EntityManager
{
    public List<Entity> Entities;
    
    public EntityManager()
    {
        Entities = new List<Entity>();
    }


    public void Update(GameTime gameTime)
    {
        foreach (var entity in Entities.Where(e => !e.IsDestroyed))
        {
            entity.Update(gameTime.GetElapsedSeconds());
        }

        Entities.RemoveAll(e => e.IsDestroyed);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in Entities.Where(e => !e.IsDestroyed))
        {
            entity.Draw(spriteBatch);
        }
    }
}