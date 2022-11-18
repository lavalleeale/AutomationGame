using UnityEngine;
using System.Collections;

public abstract class ProcessingBuildingBehaviour : BuildingBehaviour
{
	public virtual bool CanInput(ItemStack itemStack)
	{
		return Active;
	}

	public abstract void Input(ItemStack itemStack);
}

