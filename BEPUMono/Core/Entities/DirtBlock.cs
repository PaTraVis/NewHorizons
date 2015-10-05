using BEPUutilities;

namespace BEPUMono.Core.Entities
{
    public class DirtBlock : Block
    {
	    public DirtBlock()
	    {
		    Initialize();
	    }

	    public DirtBlock(Vector3 position) : base(position)
	    {
            Initialize();
	    }

	    private void Initialize()
	    {
            Id = 1;

            IsDynamic = true;
            Mass = 50;
        }
    }
}
