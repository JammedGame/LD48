using UnityEngine;

[CreateAssetMenu]
public class FacilitySettings : ScriptableObject
{
	public FacilityType FacilityType;
	public string Name;
	[TextArea] public string Description;

	[Header("Game Mechanics")]
	public int BuildPointsCost = 1;
	public MineralPrice MineralPrice;
	public EnergyContribution EnergyContribution;
	public Production Production;
	public Requirements Requirements;
}