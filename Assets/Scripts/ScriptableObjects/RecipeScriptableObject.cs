using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
public class RecipeScriptableObject : ScriptableObject
{
    public RecipeItem[] inputs;
    public RecipeItem[] outputs;
    public float processingTime;

    [System.Serializable]
    public class RecipeItem
    {
        public Item.Type type;
        public byte amount;
    }
}
