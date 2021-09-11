using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Graphics.UI;
using Manlaan.MouseCursor.Models;
using Manlaan.MouseCursor.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using static Blish_HUD.GameService;
using System.IO;
using System.Collections.Generic;

namespace Manlaan.MouseCursor
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        internal static Module ModuleInstance;

        #region Service Managers
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;
        #endregion


        public static SettingCollection _settingsHidden;
        public static SettingEntry<int> _settingMouseCursorSize;
        public static SettingEntry<float> _settingMouseCursorOpacity;
        public static SettingEntry<string> _settingMouseCursorImage;
        public static SettingEntry<string> _settingMouseCursorColor;
        public static SettingEntry<bool> _settingMouseCursorCameraDrag;
        public static SettingEntry<bool> _settingMouseCursorAboveBlish;
        private DrawMouseCursor _mouseImg;
        private Point _mousePos = new Point(0, 0);
        //private WindowTab _moduleTab;
        public static List<MouseFile> _mouseFiles = new List<MouseFile>();
        public static List<Gw2Sharp.WebApi.V2.Models.Color> _colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();
        

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this;  }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingMouseCursorImage = settings.DefineSetting("MouseCursorImage", "Circle Cyan.png");
            _settingMouseCursorColor = settings.DefineSetting("MouseCursorColor", "White0");
            _settingMouseCursorSize = settings.DefineSetting("MouseCursorSize", 70, "Size", "");
            _settingMouseCursorOpacity = settings.DefineSetting("MouseCursorOpacity", 1.0f, "Opacity", "");
            _settingMouseCursorCameraDrag = settings.DefineSetting("MouseCursorCameraDrag", false, "Show When Camera Dragging", "Shows the cursor when you move the camera.");
            _settingMouseCursorAboveBlish = settings.DefineSetting("MouseCursorAboveBlish", false, "Show Above Blish Windows", "");

            _settingMouseCursorImage.SettingChanged += UpdateMouseSettings_string;
            _settingMouseCursorColor.SettingChanged += UpdateMouseSettings_string;
            _settingMouseCursorSize.SettingChanged += UpdateMouseSettings_int;
            _settingMouseCursorOpacity.SettingChanged += UpdateMouseSettings_float;
            _settingMouseCursorCameraDrag.SettingChanged += UpdateMouseSettings_bool;
            _settingMouseCursorAboveBlish.SettingChanged += UpdateMouseSettings_bool;


            _settingMouseCursorSize.SetRange(0, 300);
            _settingMouseCursorOpacity.SetRange(0f, 1f);
        }
        public override IView GetSettingsView() {
            return new MouseCursor.Views.SettingsView();
            //return new SettingsView( (this.ModuleParameters.SettingsManager.ModuleSettings);
        }

        protected override void Initialize()
        {
            _mouseFiles = new List<MouseFile>();
            _colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();
            foreach (KeyValuePair<string, int[]> color in MouseColors.Colors) {
                _colors.Add( new Gw2Sharp.WebApi.V2.Models.Color() { Name = color.Key, Cloth = new Gw2Sharp.WebApi.V2.Models.ColorMaterial() { Rgb = color.Value } } );
            }

            _mouseImg = new DrawMouseCursor();
            _mouseImg.Parent = Graphics.SpriteScreen;

            Input.Mouse.RightMouseButtonPressed += UpdateMousePos;
        }
        protected override async Task LoadAsync()
        {
            string[] mousefiles = {
                "Circle Blue.png",
                "Circle Cyan.png",
                "Circle Green.png",
                "Circle Magenta.png",
                "Circle Red.png",
                "Circle Yellow.png",
                "Arrow 1.png",
                "Arrow 2.png",
                "Arrow 3.png",
                "Circle 1.png",
                "Circle 2.png",
                "Circle 3.png",
                "Circle 4.png",
                "Circle 5.png",
                "Circle 6.png",
                "Circle 7.png",
                "Circle 8.png",
                "Cross 1.png",
                "Cross 2.png",
                "Cross 3.png",
                "Cross 4.png",
                "Cross 5.png",
                "mouse.psd",
            };
            foreach (string file in mousefiles) {
                ExtractFile(file);
            }
            string dailiesDirectory = DirectoriesManager.GetFullDirectoryPath("mousecursor");
            foreach (string file in Directory.GetFiles(dailiesDirectory, ".")) {
                if (file.ToLower().Contains(".png")) {
                    _mouseFiles.Add(new MouseFile() { File = file, Name = file.Substring(dailiesDirectory.Length + 1) });
                }
            }
            _mouseFiles.Sort(delegate (MouseFile x, MouseFile y) {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });
        }
        protected override void OnModuleLoaded(EventArgs e)
        {
            UpdateMouseSettings_int();
            UpdateMouseSettings_float();
            UpdateMouseSettings_bool();
            UpdateMouseSettings_string();

            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {
            _mouseImg.Visible = _settingMouseCursorCameraDrag.Value || !Input.Mouse.CameraDragging;
            if (Input.Mouse.State.RightButton != ButtonState.Pressed && _mouseImg.Visible)
            {
                int x = Input.Mouse.Position.X - _settingMouseCursorSize.Value / 2;
                int y = Input.Mouse.Position.Y - _settingMouseCursorSize.Value / 2;
                _mouseImg.Location = new Point(x, y);
            }
            else if (Input.Mouse.State.RightButton == ButtonState.Pressed && _mouseImg.Visible)
            {
                int x = _mousePos.X - _settingMouseCursorSize.Value / 2;
                int y = _mousePos.Y - _settingMouseCursorSize.Value / 2;
                _mouseImg.Location = new Point(x, y);
            }
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingMouseCursorImage.SettingChanged -= UpdateMouseSettings_string;
            _settingMouseCursorColor.SettingChanged -= UpdateMouseSettings_string;
            _settingMouseCursorSize.SettingChanged -= UpdateMouseSettings_int;
            _settingMouseCursorOpacity.SettingChanged -= UpdateMouseSettings_float;
            _settingMouseCursorCameraDrag.SettingChanged -= UpdateMouseSettings_bool;
            _settingMouseCursorAboveBlish.SettingChanged -= UpdateMouseSettings_bool;
            Input.Mouse.RightMouseButtonPressed -= UpdateMousePos;
            _mouseImg?.Dispose();
            _mouseFiles = null;
            _colors = null;
            ModuleInstance = null;
        }

        private void UpdateMouseSettings_int(object sender = null, ValueChangedEventArgs<int> e = null) {
            _mouseImg.Size = new Point(_settingMouseCursorSize.Value, _settingMouseCursorSize.Value);
        }
        private void UpdateMouseSettings_float(object sender = null, ValueChangedEventArgs<float> e = null) {
            _mouseImg.Opacity = (float)(_settingMouseCursorOpacity.Value);
        }
        private void UpdateMouseSettings_string(object sender = null, ValueChangedEventArgs<string> e = null) {
            MouseFile mouseFile = _mouseFiles.Find(x => x.Name.Equals(_settingMouseCursorImage.Value));
            if (mouseFile == null || string.IsNullOrEmpty(mouseFile.File) || !File.Exists(mouseFile.File)) {
                _mouseImg.Texture = ContentsManager.GetTexture("Circle Cyan.png");
            }
            else {
                _mouseImg.Texture = PremultiplyTexture(mouseFile.File, GameService.Graphics.GraphicsDevice);
            }
            _mouseImg.Tint = ToRGB(_colors.Find(x => x.Name.Equals(_settingMouseCursorColor.Value)));
        }
        private void UpdateMouseSettings_bool(object sender = null, ValueChangedEventArgs<bool> e = null) {
            _mouseImg.AboveBlish = _settingMouseCursorAboveBlish.Value;
        }
        private void UpdateMousePos(object sender, MouseEventArgs e)
        {
            _mousePos = Input.Mouse.Position;
        }
        private void ExtractFile(string filePath) {
            var fullPath = Path.Combine(DirectoriesManager.GetFullDirectoryPath("mousecursor"), filePath);
            //if (File.Exists(fullPath)) return;
            using (var fs = ContentsManager.GetFileStream(filePath)) {
                fs.Position = 0;
                byte[] buffer = new byte[fs.Length];
                var content = fs.Read(buffer, 0, (int)fs.Length);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.WriteAllBytes(fullPath, buffer);
            }
        }
        private Texture2D PremultiplyTexture(String FilePath, GraphicsDevice device) {
            Texture2D texture;

            try {
                FileStream titleStream = File.OpenRead(FilePath);
                texture = Texture2D.FromStream(device, titleStream);
                titleStream.Close();
                Color[] buffer = new Color[texture.Width * texture.Height];
                texture.GetData(buffer);
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
                texture.SetData(buffer);
            }
            catch {
                texture = ContentsManager.GetTexture("Circle Cyan.png");
            }
            return texture;
        }
        private Color ToRGB(Gw2Sharp.WebApi.V2.Models.Color color) {
            if (color == null)
                return new Color(255,255,255);
            else
                return new Color(color.Cloth.Rgb[0], color.Cloth.Rgb[1], color.Cloth.Rgb[2]);
        }
    }

}
