using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;


namespace Manlaan.MouseCursor.Views
{
    public class SettingsView : View
    {
        ColorPicker colorPicker;
        ColorBox colorBox;

        protected override void Build(Container buildPanel) {
            Panel _parentPanel = new Panel() {
                CanScroll = false,
                Parent = buildPanel,
                Height = buildPanel.Height,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 700,  //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.,
            };
            _parentPanel.LeftMouseButtonPressed += delegate {
                if (colorPicker.Visible && !colorPicker.MouseOver && !colorBox.MouseOver)
                    colorPicker.Visible = false;
            };

            Label cursorLabel = new Label() {
                Location = new Point(10, 15),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Cursor: ",
            };
            Dropdown cursorSelect = new Dropdown() {
                Location = new Point(cursorLabel.Right + 8, cursorLabel.Top - 5),
                Width = 175,
                Parent = _parentPanel,
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
                Parent = _parentPanel,
                Text = "Tint: ",
            };
            colorBox = new ColorBox() {
                Location = new Point(colorLabel.Right + 8, colorLabel.Top - 10),
                Parent = _parentPanel,
                Color = Module._colors.Find(x => x.Name.Equals(Module._settingMouseCursorColor.Value)),
            };
            colorBox.Click += delegate { colorPicker.Visible = !colorPicker.Visible; };
            colorPicker = new ColorPicker() {
                Location = new Point(colorBox.Right + 10, colorBox.Top),
                CanScroll = true,
                Size = new Point(465, 255),
                Visible = false,
                AssociatedColorBox = colorBox,
                ZIndex = 10,
                Parent = _parentPanel,
            };
            colorPicker.SelectedColorChanged += delegate {
                colorBox.Color = colorPicker.SelectedColor;
                Module._settingMouseCursorColor.Value = colorPicker.SelectedColor.Name;
                colorPicker.Visible = false;
            };
            colorPicker.LeftMouseButtonPressed += delegate {
                colorPicker.Visible = false;
            };
            foreach (var color in Module._colors) {
                colorPicker.Colors.Add(color);
            }

            Label sizeLabel = new Label() {
                Location = new Point(10, colorBox.Bottom + 5),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Size: ",
            };
            TrackBar sizeSlider = new TrackBar() {
                Location = new Point(sizeLabel.Right + 8, sizeLabel.Top),
                Width = 250,
                MaxValue = 250,
                MinValue = 0,
                Value = Module._settingMouseCursorSize.Value,
                Parent = _parentPanel,
            };
            sizeSlider.ValueChanged += delegate { Module._settingMouseCursorSize.Value = (int)sizeSlider.Value; };

            Label opacityLabel = new Label() {
                Location = new Point(10, sizeSlider.Bottom + 7),
                Width = 60,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = _parentPanel,
                Text = "Opacity: ",
            };
            TrackBar opacitySlider = new TrackBar() {
                Location = new Point(opacityLabel.Right + 8, opacityLabel.Top),
                Width = 250,
                MaxValue = 1,
                MinValue = 0,
                Value = Module._settingMouseCursorOpacity.Value,
                Parent = _parentPanel,
            };
            sizeSlider.ValueChanged += delegate { Module._settingMouseCursorOpacity.Value = opacitySlider.Value; };

            IView settingCameraDragView = SettingView.FromType(Module._settingMouseCursorCameraDrag, buildPanel.Width);
            ViewContainer _settingCameraDragContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, opacityLabel.Bottom+5),
                Parent = _parentPanel
            };
            _settingCameraDragContainer.Show(settingCameraDragView);
            IView settingAboveBlishView = SettingView.FromType(Module._settingMouseCursorAboveBlish, buildPanel.Width);
            ViewContainer _settingAboveBlishContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, _settingCameraDragContainer.Bottom+5),
                Parent = _parentPanel
            };
            _settingAboveBlishContainer.Show(settingAboveBlishView);
        }
    }
}
