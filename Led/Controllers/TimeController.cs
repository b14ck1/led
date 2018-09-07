using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Controller
{
    class TimeController
    {
        /// <summary>
        /// Ticks/Frames since the Time Measurement started
        /// </summary>
        public long Ticks { get; private set; }

        /// <summary>
        /// Holds time for one tick in ms
        /// </summary>
        private int Ticktime;

        /// <summary>
        /// Indicates if Time Measurement has started or not
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Stopwatch for TimeMeasurement
        /// </summary>
        private System.Diagnostics.Stopwatch Time;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="Timebase">Atm Timebase in FPS to initialize Tick Measurment, Can't be changed once initialized</param>
        public TimeController(int Timebase)
        {
            Ticktime = 1000 / Timebase;
            Time = new System.Diagnostics.Stopwatch();
            IsRunning = false;
        }

        /// <summary>
        /// Updates the current Ticks, if the Time is running
        /// </summary>
        public void Update()
        {
            if (IsRunning)
                Ticks = Time.ElapsedMilliseconds / Ticktime;
        }

        /// <summary>
        /// Starts the Time
        /// </summary>
        public void Start()
        {
            Time.Start();
            IsRunning = true;
        }
        
        /// <summary>
        /// Stops the Time
        /// </summary>
        public void Stop()
        {
            Time.Stop();
            IsRunning = false;
        }

        /// <summary>
        /// Resets Time to 0 without starting it again
        /// </summary>
        public void Reset()
        {
            Time.Reset();
            IsRunning = false;
        }
    }
}
