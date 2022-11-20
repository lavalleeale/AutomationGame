using UnityEngine;
using System.Collections;

public abstract class ProcessingBuildingBehaviour : BuildingBehaviour
{
	public abstract bool Input(ItemStack itemStack);
}

