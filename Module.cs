using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Manlaan.MouseCursor.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using static Blish_HUD.GameService;
using System.IO;
using System.Collections.Generic;
using Blish_HUD.Settings.UI.Views;

namespace Manlaan.MouseCursor
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        #region Service Managers
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;
        #endregion


        private SettingCollection _settingsHidden;
        private SettingEntry<int> _settingMouseCursorSize;
        private SettingEntry<int> _settingMouseCursorOpacity;
        private SettingEntry<string> _settingMouseCursorImage;
        private SettingEntry<string> _settingMouseCursorColor;
        private SettingEntry<bool> _settingMouseCursorCameraDrag;
        private SettingEntry<bool> _settingMouseCursorAboveBlish;
        private DrawMouseCursor _mouseImg;
        private Point _mousePos = new Point(0, 0);
        private WindowTab _moduleTab;
        private List<MouseFile> _mouseFiles = new List<MouseFile>();
        private List<Gw2Sharp.WebApi.V2.Models.Color> _colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingsHidden = settings.AddSubCollection("Hidden");

            _settingMouseCursorImage = _settingsHidden.DefineSetting("MouseCursorImage", "Circle Cyan.png");
            _settingMouseCursorColor = _settingsHidden.DefineSetting("MouseCursorColor", "White0");
            _settingMouseCursorSize = _settingsHidden.DefineSetting("MouseCursorSize", 70, "Size", "");
            _settingMouseCursorOpacity = _settingsHidden.DefineSetting("MouseCursorOpacity", 100, "Opacity", "");
            _settingMouseCursorCameraDrag = settings.DefineSetting("MouseCursorCameraDrag", false, "Show When Camera Dragging", "Shows the cursor when you move the camera.");
            _settingMouseCursorAboveBlish = settings.DefineSetting("MouseCursorAboveBlish", false, "Show Above Blish Windows", "");

            _settingMouseCursorSize.SettingChanged += UpdateMouseSettings_int;
            _settingMouseCursorOpacity.SettingChanged += UpdateMouseSettings_int;
            _settingMouseCursorCameraDrag.SettingChanged += UpdateMouseSettings_bool;
            _settingMouseCursorAboveBlish.SettingChanged += UpdateMouseSettings_bool;

            //_settingMouseCursorSize.SetRange(0, 200);
            //_settingMouseCursorOpacity.SetRange(0f, 1f);
        }
        protected override void Initialize()
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
                    _mouseFiles.Add(new MouseFile() { File = file, Name = file.Substring(dailiesDirectory.Length + 1) } );
                }
            }
            _mouseFiles.Sort(delegate (MouseFile x, MouseFile y) {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });

            foreach (MouseColor color in MouseColors.Colors) {
                _colors.Add( new Gw2Sharp.WebApi.V2.Models.Color() { Name = color.Name, Cloth = new Gw2Sharp.WebApi.V2.Models.ColorMaterial() { Rgb = color.RGB } } );
            }

            _mouseImg = new DrawMouseCursor();
            _mouseImg.Parent = Graphics.SpriteScreen;

            UpdateMouseSettings_int();
            UpdateMouseSettings_bool();
            UpdateMouseSettings_string();

            Input.Mouse.RightMouseButtonPressed += UpdateMousePos;
        }
        protected override async Task LoadAsync()
        {
        }
        protected override void OnModuleLoaded(EventArgs e)
        {
            Texture2D _pageIcon = ContentsManager.GetTexture("mousebw.png"); 
            _moduleTab = Overlay.BlishHudWindow.AddTab("Mouse Cursor Settings", _pageIcon, BuildWindow());

            base.OnModuleLoaded(e);
        }

        private Panel BuildWindow() {
            Panel _parentPanel = new Panel() {
                CanScroll = false,
                Size = Overlay.BlishHudWindow.ContentRegion.Size,
            };

            string imgFile;
            if (_mouseFiles.Find(x => x.Name.Equals(_settingMouseCursorImage.Value)) == null) {
                imgFile = "";
            } else {
                imgFile = _mouseFiles.Find(x => x.Name.Equals(_settingMouseCursorImage.Value)).File;
            }
            Gw2Sharp.WebApi.V2.Models.Color tintcolor = _colors.Find(x => x.Name.Equals(_settingMouseCursorColor.Value));
            if (_colors.Find(x => x.Name.Equals(_settingMouseCursorColor.Value)) == null) {
                tintcolor = _colors.Find(x => x.Name.Equals("White0"));
            }

            Image cursorImg = new Image() {
                Size = new Point(_settingMouseCursorSize.Value, _settingMouseCursorSize.Value),
                Texture = PremultiplyTexture(imgFile, GameService.Graphics.GraphicsDevice),
                Tint = ToRGB(tintcolor),
                Opacity = (float)_settingMouseCursorOpacity.Value / 100,
                Parent = _parentPanel,
            };
            cursorImg.Location = new Point(_parentPanel.Right - 20 - cursorImg.Size.X, 20);

            Label cursorLabel = new Label() {
                Location = new Point(10, 15),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Cursor: ",
            };
            Dropdown cursorSelect = new Dropdown() {
                Location = new Point(cursorLabel.Right + 8, cursorLabel.Top-5),
                Width = 175,
                Parent = _parentPanel,
            };
            foreach (var s in _mouseFiles) {
                cursorSelect.Items.Add(s.Name);
            }
            cursorSelect.SelectedItem = _settingMouseCursorImage.Value;
            cursorSelect.ValueChanged += delegate {
                cursorImg.Texture = PremultiplyTexture(_mouseFiles.Find(x => x.Name.Equals(cursorSelect.SelectedItem)).File, GameService.Graphics.GraphicsDevice);
            };

            Label colorLabel = new Label() {
                Location = new Point(10, cursorSelect.Bottom + 10),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Tint: ",
            };
            ColorBox colorBox = new ColorBox() {
                Location = new Point(colorLabel.Right + 8, colorLabel.Top - 10),
                Parent = _parentPanel,
                Color = _colors.Find(x => x.Name.Equals(_settingMouseCursorColor.Value)),
            };

            Label sizeLabel = new Label() {
                Location = new Point(10, colorBox.Bottom + 10),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Size: ",
            };
            TextBox sizeText = new TextBox() {
                Location = new Point(sizeLabel.Right + 10, sizeLabel.Top - 5),
                Width = 50,
                Parent = _parentPanel,
                Text = _settingMouseCursorSize.Value.ToString(),
            };
            sizeText.TextChanged += delegate {
                int size = 0;
                try {
                    size = (string.IsNullOrEmpty(sizeText.Text)) ? 0 : int.Parse(sizeText.Text);
                } catch { }
                cursorImg.Size = new Point(size, size);
                cursorImg.Location = new Point(_parentPanel.Right - 20 - cursorImg.Size.X, 20);
            };
            Label opacityLabel = new Label() {
                Location = new Point(10, sizeText.Bottom + 10),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Opacity: ",
            };
            TextBox opacityText = new TextBox() {
                Location = new Point(opacityLabel.Right + 10, opacityLabel.Top - 5),
                Width = 50,
                Parent = _parentPanel,
                Text = _settingMouseCursorOpacity.Value.ToString(),
            };
            opacityText.TextChanged += delegate {
                int opactiy = 100;
                try {
                    opactiy = (string.IsNullOrEmpty(opacityText.Text) || int.Parse(opacityText.Text) == null) ? 0 : int.Parse(opacityText.Text);
                }
                catch { }
                cursorImg.Opacity = (float)(opactiy) / 100;
            };
            Label opacityLabelDesc = new Label() {
                Location = new Point(opacityText.Right + 10, opacityLabel.Top),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "(0 - 100)",
            };

            ColorPicker colorPicker = new ColorPicker() {
                Location = new Point(colorBox.Right + 30, colorBox.Top),
                CanScroll = true,
                Size = new Point(260, 470),
                Visible = false,
                AssociatedColorBox = colorBox,
                Parent = _parentPanel,
            };
            colorPicker.SelectedColorChanged += delegate {
                colorBox.Color = colorPicker.SelectedColor;
                cursorImg.Tint = ToRGB(colorPicker.SelectedColor);
            };
            foreach (var color in _colors) {
                colorPicker.Colors.Add(color);
            }
            colorBox.Click += delegate { colorPicker.Visible = !colorPicker.Visible; };


            StandardButton saveButton = new StandardButton() {
                Text = "Save & Apply",
                Size = new Point(110, 25),
                Location = new Point(10, opacityText.Bottom+5),
                Parent = _parentPanel,
            };
            saveButton.Click += delegate {
                int size = 0;
                try {
                    size = (string.IsNullOrEmpty(sizeText.Text)) ? 0 : int.Parse(sizeText.Text);
                }
                catch { }
                int opactiy = 100;
                try {
                    opactiy = (string.IsNullOrEmpty(opacityText.Text) || int.Parse(opacityText.Text) == null) ? 0 : int.Parse(opacityText.Text);
                }
                catch { }
                _settingMouseCursorImage.Value = cursorSelect.SelectedItem;
                _settingMouseCursorColor.Value = colorBox.Color.Name;
                _settingMouseCursorOpacity.Value = opactiy;
                _settingMouseCursorSize.Value = size;
                UpdateMouseSettings_string();
                UpdateMouseSettings_int();
            };

            return _parentPanel;
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
            Overlay.BlishHudWindow.RemoveTab(_moduleTab);
            _settingMouseCursorSize.SettingChanged -= UpdateMouseSettings_int;
            _settingMouseCursorOpacity.SettingChanged -= UpdateMouseSettings_int;
            _settingMouseCursorCameraDrag.SettingChanged -= UpdateMouseSettings_bool;
            _settingMouseCursorAboveBlish.SettingChanged -= UpdateMouseSettings_bool;
            Input.Mouse.RightMouseButtonPressed -= UpdateMousePos;
            _mouseImg?.Dispose();
        }

        private void UpdateMouseSettings_int(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            _mouseImg.Size = new Point(_settingMouseCursorSize.Value, _settingMouseCursorSize.Value);
            _mouseImg.Opacity = (float)(_settingMouseCursorOpacity.Value) / 100;
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
