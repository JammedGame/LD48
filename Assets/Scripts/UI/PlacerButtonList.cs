using UnityEngine;

public class PlacerButtonList : GameUIComponent
{
	public PlacerButton PlacerButtonPrefab;
	PlacerButton[] placerButtons;

	public readonly FacilityType[] FacilityTypesToInit = new FacilityType[]
	{
		FacilityType.Tunnel,
		FacilityType.MineralExtractor,
		FacilityType.PowerPlant,
		FacilityType.Battery,
		FacilityType.GeothermalPowerPlant,
		FacilityType.Booster,
		FacilityType.NuclearPowerPlant,
		FacilityType.BuildBotFacility,
		FacilityType.BFCoreExtractor,
	};

	protected override void Awake()
	{
		placerButtons = new PlacerButton[FacilityTypesToInit.Length];

		for (int i = 0; i < placerButtons.Length; i++)
		{
			var newButton = GameObject.Instantiate(PlacerButtonPrefab, transform);
			newButton.Init(FacilityTypesToInit[i]);
			placerButtons[i] = newButton;
		}
	}
}