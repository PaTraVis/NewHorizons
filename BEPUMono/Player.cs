using System;
using BEPUutilities;

namespace BEPUMono
{
    public delegate void PlayerToolbarIndexEvent(object sender, ToolbarSelectionEventArgs eventArgs);

	public class ToolbarSelectionEventArgs
	{
        public int SelectedIndex { get; set; }
	}

	public class Player
	{
        public Camera Camera { get; }
		public event PlayerToolbarIndexEvent PlayerToolbarIndexChanged;
        public int SelectedToolbarIndex { get; set; }

		public Player(NewHorizonGame game)
		{
			Camera = new Camera(game, new Vector3(0, 10, 30), 5);
		}

		public void SetSelectedToolbarIndex(int index)
		{
			SelectedToolbarIndex = index;
			OnPlayerToolbarIndexChanged(new ToolbarSelectionEventArgs {SelectedIndex = index});
		}

		public void DecreasePlayerToolbarSelection()
		{
			if (SelectedToolbarIndex == 1)
			{
				SelectedToolbarIndex = 9;
			}
			else
			{
				SelectedToolbarIndex--;
			}
            OnPlayerToolbarIndexChanged(new ToolbarSelectionEventArgs { SelectedIndex =  SelectedToolbarIndex });
		}

        public void IncreasePlayerToolbarSelection()
        {
	        if (SelectedToolbarIndex == 9)
	        {
		        SelectedToolbarIndex = 1;
	        }
	        else
	        {
		        SelectedToolbarIndex++;
	        }
            OnPlayerToolbarIndexChanged(new ToolbarSelectionEventArgs {SelectedIndex =  SelectedToolbarIndex});
        }

        protected virtual void OnPlayerToolbarIndexChanged(ToolbarSelectionEventArgs eventargs)
        {
            PlayerToolbarIndexChanged?.Invoke(this, eventargs);
        }

	}
}