using UnityEngine;

public class PropVariationGenerator : MonoBehaviour
{
    [ContextMenu("Generate Variation")]
    internal void GenerateVariation()
    {
        var propSelections = GetComponents<PropSelectionXor>();
        foreach (var propSelection in propSelections)
        {
            propSelection.GenerateVariation();
        }
    }
}
