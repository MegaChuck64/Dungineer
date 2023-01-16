using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace GameCode.Entities;

public class TileSelector : Sprite
{
    public TileMap Map { get; set; }
    public OrthographicCamera Camera { get; set; }
   
    public TileSelector(BaseGame game, TileMap map, OrthographicCamera camera, Texture2D texture, Vector2 pos) : 
        base(game, texture, pos)
    {
        Map = map;
        Camera = camera;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        HandleSelectorMovement();
    }

    private void HandleSelectorMovement()
    {
        var (x, y) = Map.WorldToMapPosition(Game.MouseState.Position.ToVector2() + Camera.Position);
        if (Map.TryGetTile(x, y) != null)
        {
            Transform.Position = Map.MapToWorldPosition((x, y));
        }
    }



    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }
}