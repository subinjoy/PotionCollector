using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionData", menuName = "GameData/PotionData")]
public class PotionData : ScriptableObject
{
    public string potionName;
    public int potency = 10;
    public Sprite icon;
    public string description;
    public string addressableLabel;
}
