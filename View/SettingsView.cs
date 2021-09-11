using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;

namespace Manlaan.MouseCursor.Controls
{
    public class SettingsView : View
    {
        protected override void Build(Container buildPanel) {
            Panel _parentPanel = new Panel() {
                            CanScroll = false,
                            Parent = buildPanel,
                            Height = buildPanel.Height,
                            HeightSizingMode = SizingMode.AutoSize,
                            Width = 700,  //bug? with buildPanel.Width changing to 40 after loading a different module settings and coming back.  buildPanel.Width,
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
                Color = Module._colors.Find(x => x.Name.Equals(Module._settingMouseCursorColor.Value)),
            };
            ColorPicker colorPicker = new ColorPicker() {
                Location = new Point(colorBox.Right + 30, colorBox.Top),
                CanScroll = true,
                Size = new Point(470, 260),
                Visible = false,
                AssociatedColorBox = colorBox,
                ZIndex = 10,
                Parent = _parentPanel,
            };
            colorPicker.SelectedColorChanged += delegate {
                colorBox.Color = colorPicker.SelectedColor;
                colorBox.Color = colorPicker.SelectedColor;
                Module._settingMouseCursorColor.Value = colorPicker.SelectedColor.Name;
                colorPicker.Visible = false;
            };
            foreach (var color in Module._colors) {
                colorPicker.Colors.Add(color);
            }
            colorBox.Click += delegate { colorPicker.Visible = !colorPicker.Visible; };


            IView settingSizeView = SettingView.FromType(Module._settingMouseCursorSize, buildPanel.Width);
            ViewContainer _settingSizeContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, colorBox.Bottom),
                Parent = _parentPanel
            };
            _settingSizeContainer.Show(settingSizeView);
            IView settingOpacityView = SettingView.FromType(Module._settingMouseCursorOpacity, buildPanel.Width);
            ViewContainer _settingOpacityContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, _settingSizeContainer.Bottom),
                Parent = _parentPanel
            };
            _settingOpacityContainer.Show(settingOpacityView);
            IView settingCameraDragView = SettingView.FromType(Module._settingMouseCursorCameraDrag, buildPanel.Width);
            ViewContainer _settingCameraDragContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, _settingOpacityContainer.Bottom),
                Parent = _parentPanel
            };
            _settingCameraDragContainer.Show(settingCameraDragView);
            IView settingAboveBlishView = SettingView.FromType(Module._settingMouseCursorAboveBlish, buildPanel.Width);
            ViewContainer _settingAboveBlishContainer = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(10, _settingCameraDragContainer.Bottom),
                Parent = _parentPanel
            };
            _settingAboveBlishContainer.Show(settingAboveBlishView);
        }
    }
}
