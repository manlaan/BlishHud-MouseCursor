using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MouseCursor
{
    public class DrawMouseCursor : Container
    {

        public Texture2D Texture;
        public DrawMouseCursor()
        {
            this.Location = new Point(0, 0);
            this.Visible = true;
            this.ZIndex = 0;   //Would like to set this above Blish, but CaptureType.ForceNone requires drawing this below Blish or all clicks go through blish, including menus/forms.  Conditional CapturesInput?
            this.Padding = Thickness.Zero;
        }

        protected override CaptureType CapturesInput()
        {
            return CaptureType.ForceNone;
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.DrawOnCtrl(this,
                this.Texture,
                new Rectangle(0, 0, Size.X, Size.Y),
                null,
                Color.White
                );
        }

    }
}
