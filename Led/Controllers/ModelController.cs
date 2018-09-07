namespace Led.Controller
{
    public class ModelController
    {
        //    private Data _Data;

        //    public Dictionary<int, ObjectController> Objects;

        //    /// <summary>
        //    /// Default Constructor
        //    /// </summary>
        //    public ModelController()
        //    {
        //        _Data = new Data(Constants.Timebase);
        //        Objects = new Dictionary<int, ObjectController>();

        //        Log(this, "Created new model with timebase '" + Constants.Timebase + "'");
        //    }

        //    /// <summary>
        //    /// JsonConstructor
        //    /// <param name="DataPath">Absolute Path to the Data</param>
        //    /// </summary>
        //    public ModelController(string DataPath)
        //    {
        //        _Data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(DataPath));
        //        Objects = new Dictionary<int, ObjectController>();
        //        foreach (var x in _Data.Objects)
        //        {
        //            Objects.Add(x.Key, new ObjectController(x.Value));
        //        }
        //    }

        //    public string SaveJSON()
        //    {
        //        return JsonConvert.SerializeObject(_Data);
        //    }

        //    /// <summary>
        //    /// Adds Object to this Project
        //    /// </summary>
        //    /// <returns>ID of the Object</returns>
        //    public int AddObject()
        //    {
        //        int NextID = _Data.ObjectIDController.Next;
        //        _Data.Objects.Add(NextID, new Model.Object());
        //        Objects.Add(NextID, new ObjectController(_Data.Objects[NextID]));
        //        return NextID;
        //    }

        //    /// <summary>
        //    /// Permanently deletes an Object
        //    /// </summary>
        //    /// <param name="ObjectID">ID of the Object to delete</param>
        //    public void DeleteObject(int ObjectID)
        //    {
        //        //Removes from Model
        //        _Data.Objects.Remove(ObjectID);
        //        _Data.ObjectIDController.Remove((short)ObjectID);

        //        //Removes Controller
        //        Objects.Remove(ObjectID);
        //    }



        //    /// <summary>
        //    /// Adds Function into Frame and
        //    /// Adds ID to FunctionID Map in Object
        //    /// </summary>
        //    /// <param name="Name"></param>
        //    /// <param name="Data"></param>
        //    /// <returns></returns>
        //    public void AddFunction(int ObjectID, Model.Effect.Effect Data)
        //    {
        //        Data.ID = _Data.Objects[ObjectID].EffectIDController.Next;
        //        _Data.Objects[ObjectID].EffectIDMap.Add(Data.ID, Data.EffectType);
        //        _Data.Objects[ObjectID].FrameData.Seconds[Data.StartFrame / 40].Frames[Data.StartFrame % 40].Functions.Add(Data);

        //        int Duration = Data.EndFrame - Data.StartFrame;
        //        if (Duration > 40)
        //        {
        //            for (int i = Data.StartFrame / 40 + 1; i < Data.EndFrame / 40; i++)
        //            {
        //                _Data.Objects[ObjectID].FrameData.Seconds[i].Image.Functions.Add(Data);
        //            }
        //        }
        //    }

        //    public FrameData FrameDataGet(int ObjectID)
        //    {
        //        return _Data.Objects[ObjectID].FrameData;
        //    }
        //}
    }
}
