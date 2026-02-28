using System;
using System.IO;
using WForms = System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Graphics.UI;
using static Blish_HUD.GameService;
using Manlaan.MouseCursor.Models;
using Manlaan.MouseCursor.Controls;
using System.Runtime.InteropServices;

#region Extern
internal enum CursorFlags
{
    CursorHiding,
    CursorShowing,
    CursorSuppressed
}
internal struct CursorInfo
{
    //
    // Summary:
    //     The caller must set this to Marshal.SizeOf(typeof(CURSORINFO))
    public int CbSize;

    public CursorFlags Flags;

    public IntPtr HCursor;

    public Point ScreenPosition;
}

internal struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

}

internal struct POINT
{
    public int x;
    public int y;

}

internal static class WinApi
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorInfo(ref CursorInfo pci);

    [DllImport("user32.dll")]
    private static extern bool GetCursorClip(ref RECT rec);

    [DllImport("user32.dll")]
    private static extern bool ClipCursor(ref RECT rec);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(IntPtr hWnd, ref POINT lpPoint);

    internal static CursorInfo GetCursorInfo()
    {
        CursorInfo cursorInfo = default;
        cursorInfo.CbSize = Marshal.SizeOf(typeof(CursorInfo));
        CursorInfo pci = cursorInfo;
        GetCursorInfo(ref pci);
        return pci;
    }

    internal static System.Drawing.Rectangle? GetCursorClip()
    {
        RECT rect = default;
        return GetCursorClip(ref rect) ?
            new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top) :
            null;
    }

    internal static System.Drawing.Rectangle? GetWindowRect(IntPtr hWnd)
    {
        RECT rect = default;
        return GetWindowRect(hWnd, ref rect) ?
            new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top) :
            null;
    }

    internal static System.Drawing.Rectangle? GetClientRect(IntPtr hWnd)
    {
        RECT rect = default;
        return GetClientRect(hWnd, ref rect) ?
            new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top) :
            null;
    }

    internal static System.Drawing.Point? ClientToScreen(IntPtr hWnd)
    {
        POINT point = default;
        return ClientToScreen(hWnd, ref point) ?
            new System.Drawing.Point(point.x, point.y) :
            null;
    }

    internal static System.Drawing.Point? GetCursorPos(IntPtr hWnd)
    {
        POINT point = default;
        return GetCursorPos(hWnd, ref point) ?
            new System.Drawing.Point(point.x, point.y) :
            null;
    }
}
#endregion

namespace Manlaan.MouseCursor
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {
        public enum ClipMode
        {
            Never,
            Always,
        }
        public enum ShowMode
        {
            Never,
            Always,
            Dragging,
            NotDragging,
        }


        internal static Module ModuleInstance;

        #region Service Managers
        private static readonly Logger Logger = Logger.GetLogger<Module>();
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;
        #endregion

        #region Settings
        public static SettingCollection _settingsHidden;
        public static SettingEntry<int> _settingMouseCursorSize;
        public static SettingEntry<float> _settingMouseCursorOpacity;
        public static SettingEntry<string> _settingMouseCursorImage;
        public static SettingEntry<string> _settingMouseCursorColor;
        public static SettingEntry<bool> _settingMouseCursorCameraDrag;
        public static SettingEntry<bool> _settingMouseCursorAboveBlish;
        public static SettingEntry<ShowMode> _settingMouseCursorShow;
        public static SettingEntry<ClipMode> _settingMouseCursorClip;
        public static SettingEntry<ShowMode> _settingMouseCursorShowCombat;
        public static SettingEntry<ClipMode> _settingMouseCursorClipCombat;
        public static SettingEntry<bool> _settingMouseCursorFreezeCursor;
        public static SettingEntry<float> _settingMouseCursorFreezeCursorPeriod;
        public static List<MouseFile> _mouseFiles = new List<MouseFile>();
        public static List<Gw2Sharp.WebApi.V2.Models.Color> _colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();
        #endregion

        private DrawMouseCursor _mouseImg;
        private TimeSpan _freezeStart;
        private Point _freezeStartPoint;
        private bool _freezeCursor = false;
        private bool _shouldClip = false;
        private bool _inActionCam = false;
        private bool _inActionCamChanged = false;
        private bool _camDragged = false;
        private bool _camDraggedChanged = false;
        private bool _cursorVis = true;
        private bool _cursorVisChanged = false;
        private double _cursorVel = 0.0f;
        private bool _cursorVelChanged = false;
        private MouseState _lastMouseState;


        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingMouseCursorImage = settings.DefineSetting("MouseCursorImage", "Circle Cyan.png", () => "");
            _settingMouseCursorColor = settings.DefineSetting("MouseCursorColor", "White0", () => "");
            _settingMouseCursorSize = settings.DefineSetting("MouseCursorSize", 70, () => "Size");
            _settingMouseCursorOpacity = settings.DefineSetting("MouseCursorOpacity", 1.0f, () => "Opacity");
            _settingMouseCursorCameraDrag = settings.DefineSetting("MouseCursorCameraDrag", false, () => "Show When Camera Dragging", () => "Shows the cursor when you move the camera.");
            _settingMouseCursorAboveBlish = settings.DefineSetting("MouseCursorAboveBlish", false, () => "Show Above Blish Windows");
            _settingMouseCursorShow = settings.DefineSetting("MouseCursorShow", ShowMode.Never, () => "");
            _settingMouseCursorShowCombat = settings.DefineSetting("MouseCursorShowCombat", ShowMode.Never, () => "");
            _settingMouseCursorClip = settings.DefineSetting("MouseCursorClip", ClipMode.Never, () => "");
            _settingMouseCursorClipCombat = settings.DefineSetting("MouseCursorClipCombat", ClipMode.Never, () => "");
            _settingMouseCursorFreezeCursor = settings.DefineSetting("MouseCursorCenterAfterDrag", false, () => "Freeze Cursor After Dragging");
            _settingMouseCursorFreezeCursorPeriod = settings.DefineSetting("MouseCursorFreezePeriod", 2f, () => "", () => $"{_settingMouseCursorFreezeCursorPeriod.Value:0} ms");

            _settingMouseCursorImage.SettingChanged += UpdateMouseCursorSettingsCursorImageNColor;
            _settingMouseCursorColor.SettingChanged += UpdateMouseCursorSettingsCursorImageNColor;
            _settingMouseCursorSize.SettingChanged += UpdateMouseCursorSettingsCursorSize;
            _settingMouseCursorOpacity.SettingChanged += UpdateMouseCursorSettingsOpacity;
            _settingMouseCursorAboveBlish.SettingChanged += UpdateMouseCursorSettingsAboveBlish;

            _settingMouseCursorSize.SetRange(0, 300);
            _settingMouseCursorOpacity.SetRange(0f, 1f);
            _settingMouseCursorFreezeCursorPeriod.SetRange(1f, 500f);
        }

        public override IView GetSettingsView()
        {
            return new Views.SettingsView();
        }

        protected override void Initialize()
        {
            _mouseFiles = new List<MouseFile>();
            _colors = new List<Gw2Sharp.WebApi.V2.Models.Color>();
            foreach (KeyValuePair<string, int[]> color in MouseColors.Colors)
            {
                _colors.Add(new Gw2Sharp.WebApi.V2.Models.Color() { Name = color.Key, Cloth = new Gw2Sharp.WebApi.V2.Models.ColorMaterial() { Rgb = color.Value } });
            }

            _mouseImg = new DrawMouseCursor();
            _mouseImg.Parent = Graphics.SpriteScreen;

        }

        protected override async Task LoadAsync()
        {
            await base.LoadAsync();
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
            foreach (string file in mousefiles)
            {
                ExtractFile(file);
            }
            string dailiesDirectory = DirectoriesManager.GetFullDirectoryPath("mousecursor");
            foreach (string file in Directory.GetFiles(dailiesDirectory, "."))
            {
                if (file.ToLower().Contains(".png"))
                {
                    _mouseFiles.Add(new MouseFile() { File = file, Name = file.Substring(dailiesDirectory.Length + 1) });
                }
            }
            _mouseFiles.Sort(delegate (MouseFile x, MouseFile y)
            {
                if (x.Name == null && y.Name == null) return 0;
                else if (x.Name == null) return -1;
                else if (y.Name == null) return 1;
                else return x.Name.CompareTo(y.Name);
            });
        }
        /// <inheritdoc />
        protected override void OnModuleLoaded(EventArgs e)
        {
            UpdateMouseCursorSettingsCursorSize();
            UpdateMouseCursorSettingsOpacity();
            UpdateMouseCursorSettingsAboveBlish();
            UpdateMouseCursorSettingsCursorImageNColor();

            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {
            // Logger.Debug($"==============================================================================");
            UpdateCursorState(gameTime);
            UpdateCursorClipping();
            UpdateCursorFreeze(gameTime);

            UpdateCursorImg();
            _lastMouseState = Mouse.GetState();
            // Logger.Debug($"======================================END=====================================");
        }

        private void UpdateCursorState(GameTime gt)
        {
            bool cursorVis = Input.Mouse.CursorIsVisible;
            _cursorVisChanged = _cursorVis != cursorVis;
            _cursorVis = cursorVis;

            bool camDragged = !cursorVis && !_inActionCam && (
                Mouse.GetState().RightButton == ButtonState.Pressed ||
                Mouse.GetState().LeftButton == ButtonState.Pressed
            );
            _camDraggedChanged = _camDragged != camDragged;
            _camDragged = camDragged;

            bool inActionCam = !cursorVis && !_camDragged &&
                WinApi.GetClientRect(GameIntegration.Gw2Instance.Gw2WindowHandle).GetValueOrDefault().Contains(
                    Mouse.GetState().Position.X, Mouse.GetState().Position.Y
                );
            _inActionCamChanged = _inActionCam != inActionCam;
            _inActionCam = inActionCam;


            var posdiff = Mouse.GetState().Position.ToVector2() - _lastMouseState.Position.ToVector2();
            double cursorVel = posdiff.Length() / gt.ElapsedGameTime.TotalSeconds;
            _cursorVelChanged = (cursorVel - _cursorVel) < 1e-9;
            _cursorVel = cursorVel;

            // Logger.Debug($"_cursorVisChanged         {_cursorVisChanged}");
            // Logger.Debug($"_cursorVis                {_cursorVis}");
            // Logger.Debug($"_camDraggedChanged        {_camDraggedChanged}");
            // Logger.Debug($"_camDragged               {_camDragged}");
            // Logger.Debug($"_inActionCamChanged       {_inActionCamChanged}");
            // Logger.Debug($"_inActionCam              {_inActionCam}");
            // Logger.Debug($"WForms.Cursor.Clip        {WForms.Cursor.Clip}");
            // Logger.Debug($"clientToScr               {WinApi.ClientToScreen(GameIntegration.Gw2Instance.Gw2WindowHandle)}");
            // Logger.Debug($"clientRect                {WinApi.GetClientRect(GameIntegration.Gw2Instance.Gw2WindowHandle)}");
        }

        private void UpdateCursorImg()
        {
            // Only display if not in cutscene, loading screen, character selection or actioncam
            _mouseImg.Visible = GameIntegration.Gw2Instance.Gw2HasFocus && GameIntegration.Gw2Instance.IsInGame && !_inActionCam;
            // Check if show only in combat or if we are in combat
            _mouseImg.Visible = _mouseImg.Visible &&
                (
                    (
                        !Gw2Mumble.PlayerCharacter.IsInCombat &&
                        (
                            (_settingMouseCursorShow.Value == ShowMode.Always) ||
                            (_settingMouseCursorShow.Value == ShowMode.Dragging && _camDragged) ||
                            (_settingMouseCursorShow.Value == ShowMode.NotDragging && !_camDragged)
                        )
                    ) ||
                    (
                        Gw2Mumble.PlayerCharacter.IsInCombat &&
                        (
                            (_settingMouseCursorShowCombat.Value == ShowMode.Always) ||
                            (_settingMouseCursorShowCombat.Value == ShowMode.Dragging && _camDragged) ||
                            (_settingMouseCursorShowCombat.Value == ShowMode.NotDragging && !_camDragged)
                        )
                    )
                );

            if (_cursorVis) _mouseImg.Location = new Point(
                Clamp(
                    Mouse.GetState().Position.X - _settingMouseCursorSize.Value / 2,
                    -_settingMouseCursorSize.Value / 2,
                    Graphics.WindowWidth - _settingMouseCursorSize.Value / 2
                ),
                Clamp(
                    Mouse.GetState().Position.Y - _settingMouseCursorSize.Value / 2,
                    -_settingMouseCursorSize.Value / 2,
                    Graphics.WindowHeight - _settingMouseCursorSize.Value / 2
                )
            );
        }

        private void UpdateCursorFreeze(GameTime gameTime)
        {
            // Check if the cursor should be frozen after dragging and if dragging is stopped
            _freezeCursor = ((_camDraggedChanged && !_camDragged && !_inActionCam) || (_inActionCamChanged && !_inActionCam)) ?
                _settingMouseCursorFreezeCursor.Value :
                _freezeCursor;
            _freezeStart = ((_camDraggedChanged && !_camDragged && !_inActionCam) || (_inActionCamChanged && !_inActionCam)) ?
                gameTime.TotalGameTime :
                _freezeStart;

            _freezeStartPoint = (_camDraggedChanged && _camDragged && !_inActionCamChanged) || (_inActionCamChanged && _inActionCam && !_camDraggedChanged) ?
                new Point(Mouse.GetState().Position.X, Mouse.GetState().Position.Y) :
                _freezeStartPoint;

            // Logger.Debug($"_freezeCursor              {_freezeCursor}");
            // Logger.Debug($"updateFreezeStartPoint     {(_camDraggedChanged && _camDragged && !_inActionCamChanged) || (_inActionCamChanged && _inActionCam && !_camDraggedChanged)}");
            // Logger.Debug($"_freezeStartPoint          {_freezeStartPoint}");
            // Logger.Debug($"Mouse.GetState().Position  {Mouse.GetState().Position}");
            // Logger.Debug($"_mouseImg.Location         {_mouseImg.Location}");
            // Logger.Debug($"_settingMouseCursorSize    {_settingMouseCursorSize.Value}");

            if (_freezeCursor)
            {
                double frozenFor = gameTime.TotalGameTime.Subtract(_freezeStart).TotalMilliseconds;
                if (frozenFor > _settingMouseCursorFreezeCursorPeriod.Value ||
                    !GameIntegration.Gw2Instance.IsInGame ||
                    !GameIntegration.Gw2Instance.Gw2HasFocus
                ) _freezeCursor = false;

                System.Drawing.Point? clientToScr = WinApi.ClientToScreen(GameIntegration.Gw2Instance.Gw2WindowHandle);
                System.Drawing.Rectangle? clientRect = WinApi.GetClientRect(GameIntegration.Gw2Instance.Gw2WindowHandle);
                WForms.Cursor.Position = _freezeCursor ? WForms.Cursor.Clip.Location : WForms.Cursor.Position;
                WForms.Cursor.Clip = new System.Drawing.Rectangle(
                    _freezeCursor ? clientToScr.GetValueOrDefault().X + _freezeStartPoint.X :
                        _shouldClip ? clientToScr.GetValueOrDefault().X :
                            0,
                    _freezeCursor ? clientToScr.GetValueOrDefault().Y + _freezeStartPoint.Y :
                        _shouldClip ? clientToScr.GetValueOrDefault().Y :
                            0,
                    _freezeCursor ? 1 :
                        _shouldClip ? clientRect.GetValueOrDefault().Width :
                            0,
                    _freezeCursor ? 1 :
                        _shouldClip ? clientRect.GetValueOrDefault().Height :
                            0
                );
                // Logger.Debug($"   CurrentFreezeTime       {frozenFor}");
                // Logger.Debug($"   _freezeCursor           {_freezeCursor}");
                // Logger.Debug($"   WForms.Cursor.Clip      {WForms.Cursor.Clip}");
            }
        }

        private void UpdateCursorClipping()
        {
            System.Drawing.Rectangle? clientRect = WinApi.GetClientRect(GameIntegration.Gw2Instance.Gw2WindowHandle);
            System.Drawing.Point? clientToScr = WinApi.ClientToScreen(GameIntegration.Gw2Instance.Gw2WindowHandle);
            bool shouldClip =
                // We already restrict the cursor with tighter bounds
                !_freezeCursor &&
                // Check if if there's a need to clip the cursor
                GameIntegration.Gw2Instance.IsInGame && GameIntegration.Gw2Instance.Gw2HasFocus &&
                // Check if we want to clip the cursor
                (
                    _inActionCam || _camDragged ||
                    (
                        !Gw2Mumble.PlayerCharacter.IsInCombat &&
                        (
                            _settingMouseCursorClip.Value == ClipMode.Always
                        )
                    ) ||
                    (
                        Gw2Mumble.PlayerCharacter.IsInCombat &&
                        (
                            _settingMouseCursorClipCombat.Value == ClipMode.Always
                        )
                    )
                );
            // Only clip cursor if it is in the clipping area
            clientRect.GetValueOrDefault().Contains(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

            bool shouldClipChanged = _shouldClip != shouldClip;
            _shouldClip = shouldClip;

            if (shouldClipChanged || (_cursorVisChanged && _cursorVis))
            {
                // Logger.Debug($"    Clip? {_shouldClip}");
                WForms.Cursor.Clip = new System.Drawing.Rectangle(
                    _shouldClip ? clientToScr.GetValueOrDefault().X : 0,
                    _shouldClip ? clientToScr.GetValueOrDefault().Y : 0,
                    _shouldClip ? clientRect.GetValueOrDefault().Width : 0,
                    _shouldClip ? clientRect.GetValueOrDefault().Height : 0
                );
                // Logger.Debug($"    WForms.Cursor.Clip {WForms.Cursor.Clip}");
            }

            // Logger.Debug($"End Setting cursor clip to {WForms.Cursor.Clip}");
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _settingMouseCursorImage.SettingChanged -= UpdateMouseCursorSettingsCursorImageNColor;
            _settingMouseCursorColor.SettingChanged -= UpdateMouseCursorSettingsCursorImageNColor;
            _settingMouseCursorSize.SettingChanged -= UpdateMouseCursorSettingsCursorSize;
            _settingMouseCursorOpacity.SettingChanged -= UpdateMouseCursorSettingsOpacity;
            _settingMouseCursorAboveBlish.SettingChanged -= UpdateMouseCursorSettingsAboveBlish;

            WForms.Cursor.Clip = new System.Drawing.Rectangle();
            _mouseImg?.Dispose();
            _mouseFiles = null;
            _colors = null;
            ModuleInstance = null;
        }

        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        private void UpdateMouseCursorSettingsCursorSize(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            _mouseImg.Size = new Point(_settingMouseCursorSize.Value, _settingMouseCursorSize.Value);
        }

        private void UpdateMouseCursorSettingsOpacity(object sender = null, ValueChangedEventArgs<float> e = null)
        {
            _mouseImg.Opacity = (float)_settingMouseCursorOpacity.Value;
        }

        private void UpdateMouseCursorSettingsCursorImageNColor(object sender = null, ValueChangedEventArgs<string> e = null)
        {
            MouseFile mouseFile = _mouseFiles.Find(x => x.Name.Equals(_settingMouseCursorImage.Value));
            if (mouseFile == null || string.IsNullOrEmpty(mouseFile.File) || !File.Exists(mouseFile.File))
            {
                _mouseImg.Texture = ContentsManager.GetTexture("Circle Cyan.png");
            }
            else
            {
                using (var gd = Graphics.LendGraphicsDeviceContext())
                {
                    _mouseImg.Texture = PremultiplyTexture(mouseFile.File, gd.GraphicsDevice);
                }
            }
            _mouseImg.Tint = ToRGB(_colors.Find(x => x.Name.Equals(_settingMouseCursorColor.Value)));
        }

        private void UpdateMouseCursorSettingsAboveBlish(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            _mouseImg.AboveBlish = _settingMouseCursorAboveBlish.Value;
        }

        private void ExtractFile(string filePath)
        {
            var fullPath = Path.Combine(DirectoriesManager.GetFullDirectoryPath("mousecursor"), filePath);
            //if (File.Exists(fullPath)) return;
            using (var fs = ContentsManager.GetFileStream(filePath))
            {
                fs.Position = 0;
                byte[] buffer = new byte[fs.Length];
                var content = fs.Read(buffer, 0, (int)fs.Length);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.WriteAllBytes(fullPath, buffer);
            }
        }

        private Texture2D PremultiplyTexture(string FilePath, GraphicsDevice device)
        {
            Texture2D texture;

            try
            {
                FileStream titleStream = File.OpenRead(FilePath);
                texture = Texture2D.FromStream(device, titleStream);
                titleStream.Close();
                Color[] buffer = new Color[texture.Width * texture.Height];
                texture.GetData(buffer);
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
                texture.SetData(buffer);
            }
            catch
            {
                texture = ContentsManager.GetTexture("Circle Cyan.png");
            }
            return texture;
        }

        private Color ToRGB(Gw2Sharp.WebApi.V2.Models.Color color)
        {
            if (color == null)
                return new Color(255, 255, 255);
            else
                return new Color(color.Cloth.Rgb[0], color.Cloth.Rgb[1], color.Cloth.Rgb[2]);
        }
    }

}
