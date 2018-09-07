using System;
using System.Collections.Generic;

namespace Led.Controller
{
    public class ObjectController
    {
        //private Dictionary<int, PartController> Parts;
        //private Model.Object _Object;

        //public string Name { get { return _Object.Name; } set { _Object.Name = value; } }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="Object">Object this Controller holds</param>
        //public ObjectController(Model.Object Object)
        //{
        //    _Object = Object;
        //}

        /// <summary>
        /// Adds Part to this Object
        /// </summary>
        /// <returns>ID of the Part</returns>
        //public int AddPart()
        //{
        //    int NextID = _Object.PartIDController.Next;
        //    _Object.Parts.Add(NextID, new Model.ObjectPart());
        //    return NextID;
        //}

        /// <summary>
        /// Updates all Part fields
        /// </summary>
        /// <param name="PartID">ID of Part to update</param>
        /// <param name="LEDs">Positions of LEDs</param>
        /// <param name="BusID"></param>
        /// <param name="BusIndex">Index on the bus</param>
        /// <param name="PositionX">Position of Part in Object</param>
        /// <param name="PositionY">Position of Part in Object</param>
        //public List<int> SetPartInformation(int PartID, List<int []> LEDs, int BusID, int BusIndex, int PositionX, int PositionY)
        //{
        //    Model.ObjectPart _Part = _Object.Parts[PartID];
        //    List<int> LEDIDs = new List<int>();

        //    _Part.BusID = BusID;
        //    _Part.BusIndex = BusIndex;
        //    _Part.Position = new int[] { PositionX, PositionY };

        //    //Removing all old LED-IDs to free some space
        //    if (_Part.Leds != null)
        //    {
        //        foreach (var Led in _Part.Leds)
        //        {
        //            _Object.LedIDController.Remove((short)Led.ID);
        //            _Object.LedMap.Remove((short)Led.ID);
        //        }
        //    }

        //    int[] Max = new int[] { 0, 0 };
        //    int[] Min = new int[] { 0, 0 };

        //    //Add all new LEDs to the Part List
        //    //and find Max Ranges of the LEDs
        //    _Part.Leds = new List<Model.Led>();
        //    int numLED = 0;
        //    foreach (int[] Position in LEDs)
        //    {
        //        int NewID = _Object.LedIDController.Next;
        //        _Part.Leds.Add(new Model.Led(NewID, Position));
        //        _Object.LedMap.Add(NewID, new Model.LedMap(BusID, BusIndex, numLED));
        //        LEDIDs.Add(NewID);
        //        numLED++;

        //        if (Position[0] > Max[0])
        //            Max[0] = Position[0];

        //        if (Position[1] > Max[1])
        //            Max[1] = Position[1];

        //        if (Position[0] < Min[0])
        //            Min[0] = Position[0];

        //        if (Position[1] < Min[1])
        //            Min[1] = Position[1];
        //    }

        //    _Part.MatrixRanges = new int[] { Max[0] - Min[0] + 1, Max[1] - Min[1] + 1 };


        //    return LEDIDs;
        //}

        /// <summary>
        /// Permanently deletes Part
        /// </summary>
        /// <param name="PartID">ID of the part to delete</param>
        //public void DeletePart(int PartID)
        //{
        //    //Removes from Model
        //    _Object.Parts.Remove(PartID);
        //    _Object.PartIDController.Remove((short)PartID);
        //}

        //public int? GetBusID(int PartID)
        //{
        //    return _Object.Parts[PartID]?.BusID;
        //}

        //public int? GetBusIndex(int PartID)
        //{
        //    return _Object.Parts[PartID]?.BusIndex;
        //}

        //public int? GetPositionX(int PartID)
        //{
        //    if (_Object.Parts[PartID]?.Position != null)
        //        return _Object.Parts[PartID]?.Position[0];
        //    else
        //        return null;
        //}

        //public int? GetPositionY(int PartID)
        //{
        //    if (_Object.Parts[PartID]?.Position != null)
        //        return _Object.Parts[PartID]?.Position[1];
        //    else
        //        return null;
        //}

        //public int? GetRangeX(int PartID)
        //{
        //    if (_Object.Parts[PartID]?.MatrixRanges != null)
        //        return _Object.Parts[PartID]?.MatrixRanges[0];
        //    else
        //        return null;
        //}

        //public int? GetRangeY(int PartID)
        //{
        //    if (_Object.Parts[PartID]?.MatrixRanges != null)
        //        return _Object.Parts[PartID]?.MatrixRanges[1];
        //    else
        //        return null;
        //}

        //public Model.Led GetLed(short ObjectID, short ID)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
