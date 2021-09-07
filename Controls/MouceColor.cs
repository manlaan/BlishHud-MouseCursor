using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.MouseCursor.Controls
{
    static class MouseColors {

        public static List<MouseColor> Colors = new List<MouseColor>();
        static MouseColors() {
            Colors.Add(new MouseColor() { Name = "White0", RGB = new int[] { 255, 255, 255 } });
            Colors.Add(new MouseColor() { Name = "White-1", RGB = new int[] { 212, 212, 212 } });
            Colors.Add(new MouseColor() { Name = "White-2", RGB = new int[] { 169, 169, 169 } });
            Colors.Add(new MouseColor() { Name = "White-3", RGB = new int[] { 127, 127, 127 } });
            Colors.Add(new MouseColor() { Name = "White-4", RGB = new int[] { 85, 85, 85 } });
            Colors.Add(new MouseColor() { Name = "White-5", RGB = new int[] { 42, 42, 42 } });
            Colors.Add(new MouseColor() { Name = "White-6", RGB = new int[] { 0, 0, 0 } });
            Colors.Add(new MouseColor() { Name = "Red3", RGB = new int[] { 255, 191, 191 } });
            Colors.Add(new MouseColor() { Name = "Red2", RGB = new int[] { 255, 127, 127 } });
            Colors.Add(new MouseColor() { Name = "Red1", RGB = new int[] { 255, 63, 63 } });
            Colors.Add(new MouseColor() { Name = "Red0", RGB = new int[] { 255, 0, 0 } });
            Colors.Add(new MouseColor() { Name = "Red-1", RGB = new int[] { 191, 0, 0 } });
            Colors.Add(new MouseColor() { Name = "Red-2", RGB = new int[] { 127, 0, 0 } });
            Colors.Add(new MouseColor() { Name = "Red-3", RGB = new int[] { 63, 0, 0 } });
            Colors.Add(new MouseColor() { Name = "Orange3", RGB = new int[] { 255, 223, 191 } });
            Colors.Add(new MouseColor() { Name = "Orange2", RGB = new int[] { 255, 191, 127 } });
            Colors.Add(new MouseColor() { Name = "Orange1", RGB = new int[] { 255, 159, 63 } });
            Colors.Add(new MouseColor() { Name = "Orange0", RGB = new int[] { 255, 127, 0 } });
            Colors.Add(new MouseColor() { Name = "Orange-1", RGB = new int[] { 191, 95, 0 } });
            Colors.Add(new MouseColor() { Name = "Orange-2", RGB = new int[] { 127, 63, 0 } });
            Colors.Add(new MouseColor() { Name = "Orange-3", RGB = new int[] { 63, 31, 0 } });
            Colors.Add(new MouseColor() { Name = "Yellow3", RGB = new int[] { 255, 255, 191 } });
            Colors.Add(new MouseColor() { Name = "Yellow2", RGB = new int[] { 255, 255, 127 } });
            Colors.Add(new MouseColor() { Name = "Yellow1", RGB = new int[] { 255, 255, 63 } });
            Colors.Add(new MouseColor() { Name = "Yellow0", RGB = new int[] { 255, 255, 0 } });
            Colors.Add(new MouseColor() { Name = "Yellow-1", RGB = new int[] { 191, 191, 0 } });
            Colors.Add(new MouseColor() { Name = "Yellow-2", RGB = new int[] { 127, 127, 0 } });
            Colors.Add(new MouseColor() { Name = "Yellow-3", RGB = new int[] { 63, 63, 0 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen3", RGB = new int[] { 223, 255, 191 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen2", RGB = new int[] { 191, 255, 127 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen1", RGB = new int[] { 159, 255, 63 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen0", RGB = new int[] { 127, 255, 0 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen-1", RGB = new int[] { 95, 191, 0 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen-2", RGB = new int[] { 63, 127, 0 } });
            Colors.Add(new MouseColor() { Name = "YellowGreen-3", RGB = new int[] { 31, 63, 0 } });
            Colors.Add(new MouseColor() { Name = "Green3", RGB = new int[] { 191, 255, 191 } });
            Colors.Add(new MouseColor() { Name = "Green2", RGB = new int[] { 127, 255, 127 } });
            Colors.Add(new MouseColor() { Name = "Green1", RGB = new int[] { 63, 255, 63 } });
            Colors.Add(new MouseColor() { Name = "Green0", RGB = new int[] { 0, 255, 0 } });
            Colors.Add(new MouseColor() { Name = "Green-1", RGB = new int[] { 0, 191, 0 } });
            Colors.Add(new MouseColor() { Name = "Green-2", RGB = new int[] { 0, 127, 0 } });
            Colors.Add(new MouseColor() { Name = "Green-3", RGB = new int[] { 0, 63, 0 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen3", RGB = new int[] { 191, 255, 223 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen2", RGB = new int[] { 127, 255, 191 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen1", RGB = new int[] { 63, 255, 159 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen0", RGB = new int[] { 0, 255, 127 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen-1", RGB = new int[] { 0, 191, 95 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen-2", RGB = new int[] { 0, 127, 63 } });
            Colors.Add(new MouseColor() { Name = "CyanGreen-3", RGB = new int[] { 0, 63, 31 } });
            Colors.Add(new MouseColor() { Name = "Cyan3", RGB = new int[] { 191, 255, 255 } });
            Colors.Add(new MouseColor() { Name = "Cyan2", RGB = new int[] { 127, 255, 255 } });
            Colors.Add(new MouseColor() { Name = "Cyan1", RGB = new int[] { 63, 255, 255 } });
            Colors.Add(new MouseColor() { Name = "Cyan0", RGB = new int[] { 0, 255, 255 } });
            Colors.Add(new MouseColor() { Name = "Cyan-1", RGB = new int[] { 0, 191, 191 } });
            Colors.Add(new MouseColor() { Name = "Cyan-2", RGB = new int[] { 0, 127, 127 } });
            Colors.Add(new MouseColor() { Name = "Cyan-3", RGB = new int[] { 0, 63, 63 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue3", RGB = new int[] { 191, 223, 255 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue2", RGB = new int[] { 127, 191, 255 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue1", RGB = new int[] { 63, 159, 255 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue0", RGB = new int[] { 0, 127, 255 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue-1", RGB = new int[] { 0, 95, 191 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue-2", RGB = new int[] { 0, 63, 127 } });
            Colors.Add(new MouseColor() { Name = "CyanBlue-3", RGB = new int[] { 0, 31, 63 } });
            Colors.Add(new MouseColor() { Name = "Blue3", RGB = new int[] { 191, 191, 255 } });
            Colors.Add(new MouseColor() { Name = "Blue2", RGB = new int[] { 127, 127, 255 } });
            Colors.Add(new MouseColor() { Name = "Blue1", RGB = new int[] { 63, 63, 255 } });
            Colors.Add(new MouseColor() { Name = "Blue0", RGB = new int[] { 0, 0, 255 } });
            Colors.Add(new MouseColor() { Name = "Blue-1", RGB = new int[] { 0, 0, 191 } });
            Colors.Add(new MouseColor() { Name = "Blue-2", RGB = new int[] { 0, 0, 127 } });
            Colors.Add(new MouseColor() { Name = "Blue-3", RGB = new int[] { 0, 0, 63 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue3", RGB = new int[] { 223, 191, 255 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue2", RGB = new int[] { 191, 127, 255 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue1", RGB = new int[] { 159, 63, 255 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue0", RGB = new int[] { 127, 0, 255 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue-1", RGB = new int[] { 95, 0, 191 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue-2", RGB = new int[] { 63, 0, 127 } });
            Colors.Add(new MouseColor() { Name = "MagentaBlue-3", RGB = new int[] { 31, 0, 63 } });
            Colors.Add(new MouseColor() { Name = "Magenta3", RGB = new int[] { 255, 191, 255 } });
            Colors.Add(new MouseColor() { Name = "Magenta2", RGB = new int[] { 255, 127, 255 } });
            Colors.Add(new MouseColor() { Name = "Magenta1", RGB = new int[] { 255, 63, 255 } });
            Colors.Add(new MouseColor() { Name = "Magenta0", RGB = new int[] { 255, 0, 255 } });
            Colors.Add(new MouseColor() { Name = "Magenta-1", RGB = new int[] { 191, 0, 191 } });
            Colors.Add(new MouseColor() { Name = "Magenta-2", RGB = new int[] { 127, 0, 127 } });
            Colors.Add(new MouseColor() { Name = "Magenta-3", RGB = new int[] { 63, 0, 63 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed3", RGB = new int[] { 255, 191, 223 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed2", RGB = new int[] { 255, 127, 191 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed1", RGB = new int[] { 255, 63, 159 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed0", RGB = new int[] { 255, 0, 127 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed-1", RGB = new int[] { 191, 0, 95 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed-2", RGB = new int[] { 127, 0, 63 } });
            Colors.Add(new MouseColor() { Name = "MagentaRed-3", RGB = new int[] { 63, 0, 31 } });
        }
    }

    class MouseColor
    {
        public string Name { get; set; }
        public int[] RGB { get; set; }

        public MouseColor() {
            Name = "";
            RGB = new int[] { 0, 0, 0 };
        }
    }
}

