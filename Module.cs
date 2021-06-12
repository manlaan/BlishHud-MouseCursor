﻿using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using manla.MouseCursor.Controls;
using Microsoft.Xna.Framework;
using static Blish_HUD.GameService;

namespace manla.MouseCursor
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


        public enum MouseFiles
        {
            CircleCyan,
            CircleBlue,
            CircleRed,
            CircleGreen,
            CircleMagenta,
            CircleYellow,
        }

        private SettingEntry<int> _settingMouseCursorSize;
        private SettingEntry<float> _settingMouseCursorOpacity;
        private SettingEntry<MouseFiles> _settingMouseCursorImage;
        private SettingEntry<bool> _settingMouseCursorCameraDrag;
        private DrawMouseCursor _mouseImg;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingMouseCursorImage = settings.DefineSetting("MouseCursorImage", MouseFiles.CircleCyan, "Image", "");
            _settingMouseCursorSize = settings.DefineSetting("MouseCursorSize", 50, "Size", "");
            _settingMouseCursorOpacity = settings.DefineSetting("MouseCursorOpacity", 100f, "Opacity", "");
            _settingMouseCursorCameraDrag = settings.DefineSetting("MouseCursorCameraDrag", false, "Show When Camera Dragging", "Shows the cursor when you move the camera.");
            _settingMouseCursorSize.SettingChanged += UpdateMouseSettings_Size;
            _settingMouseCursorOpacity.SettingChanged += UpdateMouseSettings_Opacity;
            _settingMouseCursorImage.SettingChanged += UpdateMouseSettings_Img;

            //not sure why .SetRange doesn't exist for me, so values adjusted in UpdateMouseSettings()
            //_settingMouseCursorRadius.SetRange(20, 200);
            //_settingMouseCursorOpacity.SetRange(0f, 1f);
        }

        protected override void Initialize()
        {
            //To Do:
            //change to reading files in a directory so user can add own graphic files easily
            //setup directory and copy default mouse files 

            _mouseImg = new DrawMouseCursor();
            _mouseImg.Parent = Graphics.SpriteScreen;
            UpdateMouseSettings_Size();
            UpdateMouseSettings_Opacity();
            UpdateMouseSettings_Img();
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
            _mouseImg.Visible = _settingMouseCursorCameraDrag.Value || !Input.Mouse.CameraDragging;
            int x = Input.Mouse.Position.X - (_settingMouseCursorSize.Value + 20) / 2;
            int y = Input.Mouse.Position.Y - (_settingMouseCursorSize.Value + 20) / 2;
            _mouseImg.Location = new Point(x, y);
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingMouseCursorSize.SettingChanged -= UpdateMouseSettings_Size;
            _settingMouseCursorOpacity.SettingChanged -= UpdateMouseSettings_Opacity;
            _settingMouseCursorImage.SettingChanged -= UpdateMouseSettings_Img;
            _mouseImg?.Dispose();
        }


        private void UpdateMouseSettings_Size(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            _mouseImg.Size = new Point(_settingMouseCursorSize.Value + 20, _settingMouseCursorSize.Value + 20);
        }
        private void UpdateMouseSettings_Opacity(object sender = null, ValueChangedEventArgs<float> e = null)
        {
            _mouseImg.Opacity = _settingMouseCursorOpacity.Value / 100;
        }
        private void UpdateMouseSettings_Img(object sender = null, ValueChangedEventArgs<MouseFiles> e = null)
        {
            _mouseImg.Texture = ContentsManager.GetTexture(getMouseFileName(_settingMouseCursorImage.Value));
        }

        private string getMouseFileName(MouseFiles mouseFile)
        {
            string filename;
            switch (mouseFile)
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
