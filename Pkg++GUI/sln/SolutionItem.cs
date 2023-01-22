using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pkg__
{
    public class SolutionItem : Control

    {
        public SolutionItem()
        {
            Size = new Size(240, 48);
            // Normal
            // Hover
            // Selected
            // SelectedHover
            colors = new Color[4]
            {
                Color.White, // Normal
                Color.Gray, // Hover
                Color.RoyalBlue, // Selected
                Color.Blue, // SelectedHover
            };
            secondColors = new Color[4]
                {
                    Color.Gainsboro,
                    Color.DarkGray,
                    Color.Cyan,
                    Color.Cyan
                };

            State = ItemState.Normal;
            //BackColor = Color.FromArgb(52, 52, 52);
            _iconRect = new Rectangle(8, 8,
                                      36 - 8, Height - 16);
        }
        public Color SecondBackColor { get; set; }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);

            var rect = ClientRectangle;
            LinearGradientBrush brush = new LinearGradientBrush(rect, colors[(int)State], secondColors[(int)State], LinearGradientMode.ForwardDiagonal);

            pevent.Graphics.FillRectangle(brush, rect);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            DetermineState();
        }
        public event EventHandler ItemSelected;
        public event EventHandler ItemClick;
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            DetermineState();
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                Selected = !Selected;
            }
            ItemClick?.Invoke(this, e);
        }
        public virtual void OnStateChanged(EventArgs e)
        {
            BackColor = colors[(int)State];
        }
        void DetermineState()
        {
            State =
                   _hover && _selected ? ItemState.SelectedHover :
                   _hover && !_selected ? ItemState.Hover :
                   !_hover && _selected ? ItemState.Selected :
                   ItemState.Normal;
        }
        public virtual void OnSelectedChanged(EventArgs e)
        {
            DetermineState();
        }
        private ItemState _state;

        private bool _hover;
        private bool _selected;
        public override string ToString()
        {
            return Text;
        }
        public bool Selected { get => _selected; set { _selected = value; OnSelectedChanged(EventArgs.Empty); if (value) ItemSelected?.Invoke(this, EventArgs.Empty); } }
        public ItemState State
        {
            get { return _state; }
            set { _state = value; OnStateChanged(EventArgs.Empty); Invalidate(); }
        }

        public enum ItemState
        {
            Normal,
            Hover,
            Selected,
            SelectedHover
        }
        public Color[] colors;
        public Color[] secondColors;
        public Image Icon { get; set; }
        private Rectangle _iconRect;

        protected override void OnSizeChanged(EventArgs e)
        {
            _iconRect = new Rectangle(4, 4,
                                      28, Height - 8);

            base.OnSizeChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Icon != null)
            {
                e.Graphics.DrawImage(Icon, _iconRect);
            }


            var m = TextRenderer.MeasureText(Text, Font);

            TextRenderer.DrawText(e.Graphics, Text, Font, new Point(
                _iconRect.X + _iconRect.Width + 8,
                _iconRect.Height / 2 - _iconRect.Y
                ), ForeColor);
        }

        public void SetState(ItemState state)
        {
            switch (state)
            {
                case ItemState.Normal:
                    _hover = false;
                    _selected = false;
                    break;
                case ItemState.Hover:
                    _hover = true;
                    _selected = false;
                    break;
                case ItemState.Selected:
                    _selected = true;
                    _hover = false;
                    break;
                case ItemState.SelectedHover:
                    _selected = true;
                    _hover = true;
                    break;
                default:
                    break;
            }
            State = state;
        }
    }
}
