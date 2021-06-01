using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace MouseCursor
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion


        public enum MouseFiles
        {
            CircleCyan = 0,
            CircleBlue = 1,
            CircleRed = 2,
            CircleGreen = 3,
            CircleMagenta = 4,
            CircleYellow = 5,
        }

        private SettingEntry<float> _settingMouseCursorRadius;
        private SettingEntry<float> _settingMouseCursorOpacity;
        private SettingEntry<MouseFiles> _settingMouseCursorType;
        private string _mouseCursorFile;
        private DrawMouseCursor _mouseImg;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingMouseCursorType = settings.DefineSetting("MouseCursorType", MouseFiles.CircleCyan, "Mouse Type", "");
            _settingMouseCursorRadius = settings.DefineSetting("MouseCursorRadius", 50f, "Mouse Size", "");
            _settingMouseCursorOpacity = settings.DefineSetting("MouseCursorOpacity", 100f, "Mouse Opacity", "");
        }

        protected override void Initialize()
        {
            //change to reading files in a directory so user can add own graphic files easily
            //setup directory and copy default mouse files 

            _settingMouseCursorRadius.SettingChanged += UpdateMouseSettings;
            _settingMouseCursorOpacity.SettingChanged += UpdateMouseSettings;

            _mouseImg = new DrawMouseCursor();
            _mouseImg.Parent = GameService.Graphics.SpriteScreen;
        }

        protected override async Task LoadAsync()
        {

        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {
            //better way to handle when settings select box is modified?
            if (_mouseCursorFile != getMouseTexture())
                UpdateMouseSettings();

            int x = (int)(GameService.Input.Mouse.Position.X - ((_settingMouseCursorRadius.Value + 20) / 2));
            int y = (int)(GameService.Input.Mouse.Position.Y - ((_settingMouseCursorRadius.Value + 20) / 2));
            _mouseImg.Location = new Point(x, y);
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingMouseCursorRadius.SettingChanged -= UpdateMouseSettings;
            _settingMouseCursorOpacity.SettingChanged -= UpdateMouseSettings;
            _mouseImg?.Dispose();
        }


        private void UpdateMouseSettings(object sender = null, ValueChangedEventArgs<float> e = null)
        {
            _mouseCursorFile = getMouseTexture();
            _mouseImg.Size = new Point((int)(_settingMouseCursorRadius.Value + 20), (int)(_settingMouseCursorRadius.Value + 20));
            _mouseImg.Opacity = _settingMouseCursorOpacity.Value / 100;
            _mouseImg.Texture = ContentsManager.GetTexture(_mouseCursorFile);
        }
        private string getMouseTexture()
        {
            string filename;
            switch (_settingMouseCursorType.Value)
            {
                default:
                case MouseFiles.CircleCyan:
                    filename = "CircleCyan.png";
                    break;
                case MouseFiles.CircleBlue:
                    filename = "CircleBlue.png";
                    break;
                case MouseFiles.CircleRed:
                    filename = "CircleRed.png";
                    break;
                case MouseFiles.CircleGreen:
                    filename = "CircleGreen.png";
                    break;
                case MouseFiles.CircleMagenta:
                    filename = "CircleMagenta.png";
                    break;
                case MouseFiles.CircleYellow:
                    filename = "CircleYellow.png";
                    break;
            }
            return filename;
        }


    }

}
