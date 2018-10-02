namespace Led.Controller
{
    class EffectController
    {
        //private ModelController Model;

        //private Util.EffectLogic Logic;

        //public EffectController(ModelController Model)
        //{
        //    this.Model = Model;
        //    Logic = new Util.EffectLogic(Model);
        //}

        //public void RenderAll()
        //{
        //    //foreach (var Object in Model.GetObjectNames())
        //    //{
        //    //    RenderObject(Object);
        //    //}
        //}

        //public void RenderFrameRange(int ObjectID, short StartFrame, short EndFrame)
        //{
        //    FrameData Data = Model.FrameDataGet(ObjectID);
        //    ArrayList temp = new ArrayList();
        //    short frame = StartFrame;

        //    //Load previously started Functions
        //    foreach (var Effect in Data.Seconds[StartFrame / 40].Image.Functions)
        //    {
        //        if (Effect.EndFrame >= StartFrame)
        //            temp.Add(Effect);
        //    }

        //    for (int i = StartFrame / 40; i < EndFrame / 40; i++)
        //    {
        //        for (int j = 0; j < Data.Seconds[i].Frames.Count; j++)
        //        {
        //            if (j >= StartFrame - i * 40 && j <= EndFrame - i * 40)
        //            {
        //                if (Data.Seconds[i].Frames[j].Functions.Count > 0)
        //                {
        //                    for (int k = 0; k < Data.Seconds[i].Frames[j].Functions.Count; k++)
        //                    {
        //                        temp.Add(Data.Seconds[i].Frames[j].Functions[k]);
        //                    }
        //                }

        //                foreach (var Effect in temp)
        //                {
        //                    if (Effect as Util.IEffectLogic != null)
        //                        Data.Seconds[i].Frames[j].LedChanges.AddRange((Effect as Util.IEffectLogic).Calc(Logic, frame));
        //                    else if (Effect as List<Util.IEffectLogic> != null)
        //                        Data.Seconds[i].Frames[j].LedChanges.AddRange(Logic.OrderByPriority(Effect as List<Util.IEffectLogic>, frame));
        //                    //else
        //                    //LogDatShit  
        //                }
        //                frame++;
        //            }
        //        }
        //    }
        //}
    }
}
