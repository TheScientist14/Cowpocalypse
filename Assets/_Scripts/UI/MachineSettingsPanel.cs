using UnityEngine;

public class MachineSettingsPanel : Panel
{
    [SerializeField]
    private RessourceUI _recipeRessourceUI;

    public RessourceUI RecipeRessourceUI { get => _recipeRessourceUI; }
}
