using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Led.Utility;

namespace Led.ViewModels
{
    class LedEntitySelectVM : LedEntityBaseVM
    {
        private bool _selectLeds;
        private LedEntityView _selectGroupView;

        private Utility.Rectangle _selection;
        public override List<Rectangle> FrontLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>(base.FrontLedGroups);
                if (_selectGroupView == LedEntityView.Front)
                    res.Add(_selection);
                return res;
            }
        }
        public override List<Rectangle> BackLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>(base.FrontLedGroups);
                if (_selectGroupView == LedEntityView.Back)
                    res.Add(_selection);
                return res;
            }
        }

        private List<int> _selectedLeds;
        public List<Utility.LedModelID> SelectedLeds
        {
            get
            {
                List<Utility.LedModelID> res = new List<Utility.LedModelID>();
                foreach (int index in _selectedLeds)
                {
                    res.Add(_IndexToLedID(index));
                }

                return res;
            }
        }

        public LedEntitySelectVM(Model.LedEntity LedEntity, List<Utility.LedModelID> SelectedLeds = null)
            : base(LedEntity)
        {
            _selectedLeds = new List<int>();
            if (SelectedLeds != null)
            {
                foreach (var ID in SelectedLeds)
                {
                    _selectedLeds.Add(ID.Led + LedOffsets[LedIDToGroupVM[new LedGroupIdentifier(ID.BusID, ID.PositionInBus)]].Offset);
                    Leds[ID.Led + LedOffsets[LedIDToGroupVM[new LedGroupIdentifier(ID.BusID, ID.PositionInBus)]].Offset].Brush = Defines.LedSelectedColor;
                }
            }

            FrontImageMouseDownCommand = new Command<MouseEventArgs>(OnFrontImageMouseDownCommand);
            FrontImageMouseMoveCommand = new Command<MouseEventArgs>(OnFrontImageMouseMoveCommand);

            BackImageMouseDownCommand = new Command<MouseEventArgs>(OnBackImageMouseDownCommand);
            BackImageMouseMoveCommand = new Command<MouseEventArgs>(OnBackImageMouseMoveCommand);

            ImageMouseUpCommand = new Command<MouseEventArgs>(OnImageMouseUpCommand);

            UpdateAllLedPositions(true);            
        }

        private void OnFrontImageMouseDownCommand(MouseEventArgs e)
        {
            _CreateSelectGroup(e, LedEntityView.Front);
            RaisePropertyChanged(nameof(FrontLedGroups));
        }
        private void OnBackImageMouseDownCommand(MouseEventArgs e)
        {
            _CreateSelectGroup(e, LedEntityView.Back);
            RaisePropertyChanged(nameof(BackLedGroups));
        }

        private void OnFrontImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_selectLeds)
                _ResizeGroup(e);
        }
        private void OnBackImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_selectLeds)
                _ResizeGroup(e);
        }

        private void OnImageMouseUpCommand(MouseEventArgs e)
        {
            _selectLeds = false;
            if (_selection != null)
                _selectedLeds.AddRange(_ScanForLeds());

            _selection = null;
            RaisePropertyChanged(nameof(FrontLedGroups));
            RaisePropertyChanged(nameof(BackLedGroups));
        }

        private void _CreateSelectGroup(MouseEventArgs e, LedEntityView View)
        {
            //Vielleicht scaling mit einberechnen
            Point MousePosition = e.GetPosition((IInputElement)e.Source);
            _selection = new Utility.Rectangle((int)MousePosition.X, (int)MousePosition.Y, Defines.LedSelectRectangleColor);

            _selectGroupView = View;
            _selectLeds = true;
        }

        private void _ResizeGroup(MouseEventArgs e)
        {
            if (!_selectLeds)
                return;

            Point MousePosition = e.GetPosition((IInputElement)e.Source);

            
            double deltaX = MousePosition.X - _selection.X;
            double deltaY = MousePosition.Y - _selection.Y;

            if (deltaX < 5)
                deltaX = 5;
            if (deltaY < 5)
                deltaY = 5;

            _selection.Width = (int)deltaX;
            _selection.Height = (int)deltaY;

            _ScanForLeds();

            //Debug.WriteLine("GridWidth: " + GridWidth + " ImageWidth: " + FrontImageWidth);
            //Debug.WriteLine("Start: X: " + _selectGroup.Start.X + " Y: " + _selectGroup.Start.Y);
            //Debug.WriteLine("End:   X: " + (_selectGroup.Start.X + _selectGroup.Size.Width) + " Y: " + (_selectGroup.Start.Y + _selectGroup.Size.Height));
            //Debug.WriteLine("Mouse: X: " + MousePosition.X + " Y: " + MousePosition.Y);
        }
        
        private List<int> _ScanForLeds()
        {
            List<int> LedIndices = new List<int>();

            Point Start = new Point(_selection.X, _selection.Y);
            Point End = new Point(Start.X + _selection.Width, Start.Y + _selection.Height);
            for (int i = 0; i < Leds.Count; i++)
            {
                LedVM Led = Leds[i];
                if (Led.View == _selectGroupView && Led.Position.X <= End.X && Led.Position.X >= Start.X && Led.Position.Y <= End.Y && Led.Position.Y >= Start.Y)
                {
                    LedIndices.Add(i);
                    Led.Brush = Defines.LedSelectedColor;
                }
                else if (!_selectedLeds.Contains(i))
                    Led.Brush = Defines.LedColor;
            }

            return LedIndices;
        }

        private Utility.LedModelID _IndexToLedID(int index)
        {
            int i, start = 0;

            for (i = 0; i < LedOffsets.Values.Count; i++)
            {
                if (start + LedOffsets.Values.ElementAt(i).Length > index)
                    break;
                else
                    start += LedOffsets.Values.ElementAt(i).Length;
            }

            return new Utility.LedModelID(LedOffsets.Keys.ElementAt(i).BusID, LedOffsets.Keys.ElementAt(i).PositionInBus, (ushort)(index - start));
        }

    }
}
