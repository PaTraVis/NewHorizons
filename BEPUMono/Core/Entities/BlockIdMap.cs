using System;
using System.Collections.Generic;

namespace BEPUMono.Core.Entities
{
    public static class BlockIdMap
    {
	    private static Dictionary<BlockTypes, Type> _blocks;

	    static BlockIdMap()
	    {
		    _blocks = new Dictionary<BlockTypes, Type>
		    {
			    {BlockTypes.Dirt, typeof (DirtBlock)},
			    {BlockTypes.Stone, typeof (DirtBlock)}
		    };
	    }

	    public static Type GetBlockType(BlockTypes type)
	    {
		    return _blocks[type];
	    }
    }
}
