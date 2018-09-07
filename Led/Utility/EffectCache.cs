using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Utility
{
    class EffectCache
    {
        public class LedData
        {
            public short LedID { get; private set; }

            public short FunctionID { get; private set; }

            public EffectType EffectType { get; private set; }

            public Color Color { get; private set; }

            public short PosPriority { get; private set; }

            public short ColPriority { get; private set; }

            public LedData(short LedID, short FunctionID, EffectType FunctionType, Color Color, short PosPriority, short ColPriority)
            {
                this.LedID = LedID;
                this.FunctionID = FunctionID;
                this.EffectType = FunctionType;
                this.Color = Color;
                this.PosPriority = PosPriority;
                this.ColPriority = ColPriority;
            }
        }

        public List<LedData> LedChanges { get; private set; }

        public short FunctionsAdded { get; private set; }

        public List<EffectType> FunctionTypes { get; private set; }

        public short LowestPosPriority { get; private set; }

        public short LowestColPriority { get; private set; }

        private bool HasBlinkFunction { get; set; }

        public bool ChangeBlinkBehaviour { get; private set; }

        public EffectCache()
        {
            LedChanges = new List<LedData>();
            FunctionsAdded = 0;
            FunctionTypes = new List<EffectType>();
            LowestPosPriority = 0;
            LowestColPriority = 0;
        }

        public void AddRange(List<Model.LedChangeData> data, EffectType FunctionType, short PosPriority, short ColPriority)
        {
            foreach(var LED in data)
            {
                //LedChanges.Add(new LedChangeData(LED.LedID, LED.FunctionID, FunctionType, LED.Color, PosPriority, ColPriority));
            }
            if (PosPriority > LowestPosPriority)
                LowestPosPriority = PosPriority;
            if (ColPriority > LowestColPriority)
                LowestColPriority = ColPriority;

            if (FunctionType == EffectType.Blink)
                HasBlinkFunction = true;

            //Check for Functions which change the Color
            //Go to Model.Constants.FunctionID to look it up
            if (HasBlinkFunction && (int)FunctionType > 0 && (int)FunctionType < 3)
                ChangeBlinkBehaviour = true;

            FunctionsAdded++;
        }

        public List<Model.LedChangeData> GetData()
        {
            List<Model.LedChangeData> temp = new List<Model.LedChangeData>();
            foreach(var x in LedChanges)
            {
                //temp.Add(new Model.LedChangeData(x.LedID, x.FunctionID, x.Color));
            }
            return temp;
        }
    }
}
