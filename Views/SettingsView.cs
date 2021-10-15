using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;


namespace Manlaan.MouseCursor.Views
{
    public class SettingsView : View
    {
        Panel colorPickerPanel;
        ColorPicker colorPicker;
        ColorBox colorBox;

        protected override void Build(Container buildPanel) {
            Panel parentPanel = new Panel() {
                CanScroll = false,
                Parent = buildPanel,
                Height = buildPanel.Height,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 700,  //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.,
            };
            parentPanel.LeftMouseButtonPressed += delegate {
                if (colorPickerPanel.Visible && !colorPickerPanel.MouseOver && !colorBox.MouseOver)
                    colorPickerPanel.Visible = false;
            };

            colorPickerPanel = new Panel() {
                Location = new Point(parentPanel.Width - 420 - 10, 10),
                Size = new Point(420, 255),
                Visible = false,
                ZIndex = 10,
                Parent = parentPanel,
                BackgroundTexture = Module.ModuleInstance.ContentsManager.GetTexture("155976.png"),
                ShowBorder = false,
            };
            Panel colorPickerBG = new Panel() {
                Location = new Point(15, 15),
                Size = new Point(colorPickerPanel.Size.X - 35, colorPickerPanel.Size.Y - 30),
                Parent = colorPickerPanel,
                ShowTint = true,
                ShowBorder = true,
            };
            colorPicker = new ColorPicker() {
                Location = new Point(10, 10),
                CanScroll = false,
                Size = new Point(colorPickerBG.Size.X - 20, colorPickerBG.Size.Y - 20),
                Parent = colorPickerBG,
                ShowTint = false,
                Visible = true
            };
            colorPicker.SelectedColorChanged += delegate {
                colorBox.Color = colorPicker.SelectedColor;
                Module._settingMouseCursorColor.Value = colorPicker.SelectedColor.Name;
                colorPickerPanel.Visible = false;
            };
            colorPicker.LeftMouseButtonPressed += delegate {
                colorPickerPanel.Visible = false;
            };
            foreach (var color in Module._colors) {
                colorPicker.Colors.Add(color);
            }


            Label cursorLabel = new Label() {
                Location = new Point(10, 15),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Cursor: ",
            };
            Dropdown cursorSelect = new Dropdown() {
                Location = new Point(cursorLabel.Right + 8, cursorLabel.Top - 5),
                Width = 175,
                Parent = parentPanel,
            };
            foreach (var s in Module._mouseFiles) {
                cursorSelect.Items.Add(s.Name);
            }
            cursorSelect.SelectedItem = Module._settingMouseCursorImage.Value;
            cursorSelect.ValueChanged += delegate {
                Module._settingMouseCursorImage.Value = cursorSelect.SelectedItem;
            };

            Label colorLabel = new Label() {
                Location = new Point(10, cursorSelect.Bottom + 15),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Tint: ",
            };
            colorBox = new ColorBox() {
                Location = new Point(colorLabel.Right + 8, colorLabel.Top - 10),
                Parent = parentPanel,
                Color = Module._colors.Find(x => x.Name.Equals(Module._settingMouseCursorColor.Value)),
            };
            colorBox.Click += delegate { colorPickerPanel.Visible = !colorPickerPanel.Visible; };

            Label sizeLabel = new Label() {
                Location = new Point(10, colorBox.Bottom + 5),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Size: ",
            };
            TrackBar sizeSlider = new TrackBar() {
                Location = new Point(sizeLabel.Right + 8, sizeLabel.Top),
                Width = 250,
                MaxValue = 250,
                MinValue = 0,
                Value = Module._settingMouseCursorSize.Value,
                Parent = parentPanel,
            };
            sizeSlider.ValueChanged += delegate { Module._settingMouseCursorSize.Value = (int)sizeSlider.Value; };

            Label opacityLabel = new Label() {
                Location = new Point(10, sizeSlider.Bottom + 7),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = parentPanel,
                Text = "Opacity: ",
            };
            TrackBar opacitySlider = new TrackBar() {
                Location = new Point(opacityLabel.Right + 8, opacityLabel.Top),
                Width = 250,
                MaxValue = 100,
                MinValue = 0,
                Value = Module._settingMouseCursorOpacity.Value * 100,
                Parent = parentPanel,
            };
            opacitySlider.ValueChanged += delegate { Module._settingMouseCursorOpacity.Value = opacitySlider.Value / 100; };

            IView settingCameraDragView = SettingView.FromType(Module._settingMouseCursorCameraDrag, buildPanel.Width);
            ViewContainer _settingCameraDragContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, opacityLabel.Bottom+5),
                Parent = parentPanel
            };
            _settingCameraDragContainer.Show(settingCameraDragView);
            IView settingAboveBlishView = SettingView.FromType(Module._settingMouseCursorAboveBlish, buildPanel.Width);
            ViewContainer _settingAboveBlishContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, _settingCameraDragContainer.Bottom+5),
                Parent = parentPanel
            };
            _settingAboveBlishContainer.Show(settingAboveBlishView);
        }
    }
}
