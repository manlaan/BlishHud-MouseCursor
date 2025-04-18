using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;
using System;


namespace Manlaan.MouseCursor.Views
{
    public class SettingsView : View
    {
        Panel colorPickerPanel;
        ColorPicker colorPicker;
        ColorBox colorBox;

        protected override void Build(Container buildPanel)
        {
            Panel parentPanel = new Panel()
            {
                CanScroll = false,
                Parent = buildPanel,
                Height = buildPanel.Height,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 700,  //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.,
            };
            parentPanel.LeftMouseButtonPressed += delegate
            {
                if (colorPickerPanel.Visible && !colorPickerPanel.MouseOver && !colorBox.MouseOver)
                    colorPickerPanel.Visible = false;
            };

            colorPickerPanel = new Panel()
            {
                Location = new Point(parentPanel.Width - 420 - 10, 10),
                Size = new Point(420, 255),
                Visible = false,
                ZIndex = 10,
                Parent = parentPanel,
                BackgroundTexture = Module.ModuleInstance.ContentsManager.GetTexture("155976.png"),
                ShowBorder = false,
            };
            Panel colorPickerBG = new Panel()
            {
                Location = new Point(15, 15),
                Size = new Point(colorPickerPanel.Size.X - 35, colorPickerPanel.Size.Y - 30),
                Parent = colorPickerPanel,
                BackgroundTexture = Module.ModuleInstance.ContentsManager.GetTexture("buttondark.png"),
                ShowBorder = true,
            };
            colorPicker = new ColorPicker()
            {
                Location = new Point(10, 10),
                CanScroll = false,
                Size = new Point(colorPickerBG.Size.X - 20, colorPickerBG.Size.Y - 20),
                Parent = colorPickerBG,
                ShowTint = false,
                Visible = true
            };
            colorPicker.SelectedColorChanged += delegate
            {
                colorBox.Color = colorPicker.SelectedColor;
                Module._settingMouseCursorColor.Value = colorPicker.SelectedColor.Name;
                colorPickerPanel.Visible = false;
            };
            colorPicker.LeftMouseButtonPressed += delegate { colorPickerPanel.Visible = false; };
            foreach (var color in Module._colors) colorPicker.Colors.Add(color);


            Label cursorLabel = new Label()
            {
                Location = new Point(10, 15),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Cursor: ",
            };
            Dropdown cursorSelect = new Dropdown()
            {
                Location = new Point(cursorLabel.Right + 8, cursorLabel.Top - 5),
                Width = 175,
                Parent = parentPanel,
            };
            foreach (var s in Module._mouseFiles) cursorSelect.Items.Add(s.Name);
            cursorSelect.SelectedItem = Module._settingMouseCursorImage.Value;
            cursorSelect.ValueChanged += delegate
            {
                Module._settingMouseCursorImage.Value = cursorSelect.SelectedItem;
            };
            Control prevContainer = cursorSelect;

            Label colorLabel = new Label()
            {
                Location = new Point(10, prevContainer.Bottom + 15),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Tint: ",
            };
            colorBox = new ColorBox()
            {
                Location = new Point(colorLabel.Right + 8, colorLabel.Top - 10),
                Parent = parentPanel,
                Color = Module._colors.Find(x => x.Name.Equals(Module._settingMouseCursorColor.Value)),
            };
            colorBox.Click += delegate { colorPickerPanel.Visible = !colorPickerPanel.Visible; };
            prevContainer = colorLabel;

            Label sizeLabel = new Label()
            {
                Location = new Point(10, prevContainer.Bottom + 5),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Size: ",
            };
            TrackBar sizeSlider = new TrackBar()
            {
                Location = new Point(sizeLabel.Right + 8, sizeLabel.Top),
                Width = 250,
                MaxValue = 250,
                MinValue = 0,
                Value = Module._settingMouseCursorSize.Value,
                Parent = parentPanel,
            };
            sizeSlider.ValueChanged += delegate { Module._settingMouseCursorSize.Value = (int)sizeSlider.Value; };
            prevContainer = sizeSlider;

            Label opacityLabel = new Label()
            {
                Location = new Point(10, prevContainer.Bottom + 7),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Opacity: ",
            };
            TrackBar opacitySlider = new TrackBar()
            {
                Location = new Point(opacityLabel.Right + 8, opacityLabel.Top),
                Width = 250,
                MaxValue = 100,
                MinValue = 0,
                Value = Module._settingMouseCursorOpacity.Value * 100,
                Parent = parentPanel,
            };
            opacitySlider.ValueChanged += delegate { Module._settingMouseCursorOpacity.Value = opacitySlider.Value / 100; };
            prevContainer = opacityLabel;

            IView settingAboveBlishView = SettingView.FromType(Module._settingMouseCursorAboveBlish, buildPanel.Width);
            ViewContainer _settingAboveBlishContainer = new ViewContainer()
            {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, prevContainer.Bottom + 5),
                Parent = parentPanel
            };
            _settingAboveBlishContainer.Show(settingAboveBlishView);
            prevContainer = _settingAboveBlishContainer;

            // IView settingCameraDragView = SettingView.FromType(Module._settingMouseCursorCameraDrag, buildPanel.Width);
            // ViewContainer _settingCameraDragContainer = new ViewContainer()
            // {
            //     WidthSizingMode = SizingMode.Fill,
            //     Location = new Point(10, prevContainer.Bottom + 5),
            //     Parent = parentPanel
            // };
            // _settingCameraDragContainer.Show(settingCameraDragView);
            // prevContainer = _settingCameraDragContainer;

            // IView settingOnlyCombatView = SettingView.FromType(Module._settingMouseCursorShow, buildPanel.Width);
            // ViewContainer _settingOnlyCombatContainer = new ViewContainer()
            // {
            //     WidthSizingMode = SizingMode.Fill,
            //     Location = new Point(10, prevContainer.Bottom + 5),
            //     Parent = parentPanel
            // };
            // _settingOnlyCombatContainer.Show(settingOnlyCombatView);
            // prevContainer = _settingOnlyCombatContainer;

            // IView settingCursorClipView = SettingView.FromType(Module._settingMouseCursorClip, buildPanel.Width);
            // ViewContainer _settingCursorClipContainer = new ViewContainer()
            // {
            //     WidthSizingMode = SizingMode.Fill,
            //     Location = new Point(10, prevContainer.Bottom + 5),
            //     Parent = parentPanel,

            // };
            // _settingCursorClipContainer.Show(settingCursorClipView);
            // prevContainer = _settingCursorClipContainer;


            // IView settingCursorClipOnlyCombatView = SettingView.FromType(Module._settingMouseCursorClipOnlyCombat, buildPanel.Width);
            // ViewContainer _settingCursorClipOnlyCombatContainer = new ViewContainer()
            // {
            //     WidthSizingMode = SizingMode.Fill,
            //     Location = new Point(10, prevContainer.Bottom + 5),
            //     Parent = parentPanel,
            // };
            // _settingCursorClipOnlyCombatContainer.Show(settingCursorClipOnlyCombatView);
            // prevContainer = _settingCursorClipOnlyCombatContainer;

            Label cursorShowLabel = new Label()
            {
                Location = new Point(10, prevContainer.Bottom + 10),
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Show Image: ",
                Width = 75,
            };
            Dropdown cursorShowSelect = new Dropdown()
            {
                Location = new Point(cursorShowLabel.Right + 5, prevContainer.Bottom + 5),
                Width = 160,
                Parent = parentPanel,
            };
            foreach (var s in Enum.GetNames(typeof(Module.ShowMode))) cursorShowSelect.Items.Add(s);
            cursorShowSelect.SelectedItem = Enum.GetName(typeof(Module.ShowMode), Module._settingMouseCursorShow.Value);
            cursorShowSelect.ValueChanged += delegate
            {
                Enum.TryParse(cursorShowSelect.SelectedItem, out Module.ShowMode showMode);
                Module._settingMouseCursorShow.Value = showMode;
            };
            prevContainer = cursorShowLabel;

            Label cursorClipLabel = new Label()
            {
                Location = new Point(10, prevContainer.Bottom + 10),
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Cursor Clip: ",
                Width = 75,
            };
            Dropdown cursorClipSelect = new Dropdown()
            {
                Location = new Point(cursorClipLabel.Right + 5, prevContainer.Bottom + 5),
                Width = 160,
                Parent = parentPanel,
            };
            foreach (var s in Enum.GetNames(typeof(Module.ClipMode))) cursorClipSelect.Items.Add(s);
            cursorClipSelect.SelectedItem = Enum.GetName(typeof(Module.ClipMode), Module._settingMouseCursorClip.Value);
            cursorClipSelect.ValueChanged += delegate
            {
                Enum.TryParse(cursorClipSelect.SelectedItem, out Module.ClipMode clipMode);
                Module._settingMouseCursorClip.Value = clipMode;
            };
            prevContainer = cursorClipLabel;

            IView settingFreezeAfterDragView = SettingView.FromType(Module._settingMouseCursorFreezeAfterDrag, buildPanel.Width);
            ViewContainer _settingFreezeAfterDragContainer = new ViewContainer()
            {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, prevContainer.Bottom + 5),
                Parent = parentPanel
            };
            _settingFreezeAfterDragContainer.Show(settingFreezeAfterDragView);
            prevContainer = _settingFreezeAfterDragContainer;

            IView settingFreezePeriodDragView = SettingView.FromType(Module._settingMouseCursorFreezeAfterDragPeriod, buildPanel.Width);
            ViewContainer _settingFreezePeriodContainer = new ViewContainer()
            {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, prevContainer.Bottom + 5),
                Parent = parentPanel,
            };
            _settingFreezePeriodContainer.Show(settingFreezePeriodDragView);
            prevContainer = _settingFreezePeriodContainer;
        }
    }
}
