using Led.Model.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Led.Utility.Timeline
{
	public interface ITimeLineManager
	{
		Boolean CanAddToTimeLine(EffectBase item);
	}
}
