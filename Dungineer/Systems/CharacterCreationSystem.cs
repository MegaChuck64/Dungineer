using Dungineer.Components.GameWorld;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungineer.Systems;

public class CharacterCreationSystem : BaseSystem
{
    private readonly SpriteBatch sb;
    public CharacterCreationSystem(BaseGame game) : base(game)
    {
        sb = new SpriteBatch(game.GraphicsDevice);
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "CharacterCreation") return;

        //DRAWING
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo


        foreach (var ent in entities)
        {
            if (ent.HasTag("Player"))
            {
                DrawPlayer(ent);
            }
        }

        sb.End();
    }

    private void DrawPlayer(Entity ent)
    {
        var obj = ent.GetComponent<MapObject>();
        var stats = ent.GetComponent<CreatureStats>();
        var wardrobe = ent.GetComponent<Wardrobe>();

        var info = Settings.MapObjectAtlas[obj.Type];

        var x = Game.Width / 3;
        var y = Game.Height / 2;
        var w = Settings.TileSize * 2;
        var h = Settings.TileSize * 2;
        var bnds = new Rectangle(x, y, w, h);

        var txt = Settings.TextureAtlas[info.TextureName];

        sb.Draw(txt, bnds, info.Source, obj.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);

        if (wardrobe.BodySlot.HasValue)
        {
            var winfo = Settings.WardrobeAtlas[wardrobe.BodySlot.Value];
            var wtxt = Settings.TextureAtlas[winfo.TextureName];

            sb.Draw(wtxt, bnds, winfo.Source, obj.Tint, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
        }
    }

}