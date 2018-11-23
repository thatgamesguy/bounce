using System.Collections.Generic;

namespace Bounce
{
    /// <summary>
    /// Implemented by any class that will be responsible for tracking whether all the current level shapes have been hit.
    /// </summary>
	public interface IConsumerListener  
	{
		List<ConsumableShapeControl> m_ConsumedShapes { get; }

		void AddShape( ConsumableShapeControl shape );

		bool AllConsumed();

		void ClearConsumedShapes();
	}
}