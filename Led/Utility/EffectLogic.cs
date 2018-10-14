using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;
using Led.Model.Effect;

namespace Led.Utility
{
    public class EffectLogic
    {
        private Color ColorBlack;

        public EffectLogic()
        {
            ColorBlack = Colors.Black;
        }

        public List<Model.LedChangeData> OrderByPriority(List<EffectBase> data, short Frame)
        {
            //Filling the cache with all Information
            EffectCache cache = new EffectCache();

            foreach (var Effect in data)
            {
                //cache.AddRange(Effect.Calc(this, Frame), Effect.EffectType, Effect.PosPriority, Effect.ColPriority);
            }

            //Sorting for PosPriority, Lowest wins
            List<short> LedIDsToKeep = new List<short>();
            foreach (var Led in cache.LedChanges.FindAll(x => x.PosPriority == cache.LowestPosPriority))
            {
                LedIDsToKeep.Add(Led.LedID);
            }

            foreach (var Led in cache.LedChanges)
            {
                if (!LedIDsToKeep.Contains(Led.LedID))
                    cache.LedChanges.Remove(Led);
            }

            //Sorting for ColPriority, Lowest wins
            //If we have a blink function and another color setting function,
            //blink will act like on/off and the function "under" it will set the color
            foreach (var ID in LedIDsToKeep)
            {
                bool delete = false;
                foreach (var Led in cache.LedChanges.FindAll(x => x.LedID == ID).OrderBy(x => x.ColPriority))
                {
                    if (delete)
                        cache.LedChanges.Remove(Led);
                    else if (cache.ChangeBlinkBehaviour && Led.EffectType == EffectType.Blink)
                    {
                        if (Led.Color == ColorBlack)
                            delete = true;
                        else
                            cache.LedChanges.Remove(Led);
                    }
                    else
                        delete = true;
                }
            }

            return cache.GetData();
        }

        //public List<Model.LedChangeData> SetColorLogic(EffectSetColor Data, int Frame)
        //{
        //    List<Model.LedChangeData> temp = new List<Model.LedChangeData>(Data.Leds.Count);
        //    foreach (var x in Data.Leds)
        //    {
        //        //temp.Add(new Model.LedChangeData(x.LedID, Data.ID, Data.Color));
        //    }
        //    return temp;
        //}

        //public List<Model.LedChangeData> BlinkColorLogic(EffectBlinkColor Data, int ActFrame)
        //{
        //    List<Model.LedChangeData> temp = new List<Model.LedChangeData>(Data.Leds.Count);
        //    int Frame = ActFrame - Data.StartFrame;

        //    if (Frame % Data.BlinkFrames == 0)
        //    {
        //        foreach (var x in Data.Leds)
        //        {
        //            //temp.Add(new Model.LedChangeData(x.LedID, Data.ID, Data.Color[Data.CurrColor]));
        //        }

        //        if (Data.CurrentColor == Data.Colors.Count - 1)
        //            Data.CurrentColor = 0;
        //        else
        //            Data.CurrentColor++;
        //    }
        //    return temp;
        //}

        public List<Model.LedChangeData> FadeColorLogic(EffectFadeColor Data, int Frame)
        {
            return null;
        }

        private static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
    }
}
