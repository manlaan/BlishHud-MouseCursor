using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.MouseCursor.Controls
{
    public class DrawMouseCursor : Container
    {

        public Texture2D Texture;
        public Color Tint = Color.White;
        public bool AboveBlish = false;
        public DrawMouseCursor()
        {
            this.Location = new Point(0, 0);
            this.Visible = true;
            this.Padding = Thickness.Zero;
        }

        protected override CaptureType CapturesInput()
        {
            return CaptureType.Filter;
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            if (AboveBlish)
                ZIndex = int.MaxValue;
            else
                ZIndex = 0;
            if (this.Texture != null)
                spriteBatch.DrawOnCtrl(this,
                    this.Texture,
                    new Rectangle(0, 0, Size.X, Size.Y),
                    null,
                    Tint
                    );
        }

    }
}
