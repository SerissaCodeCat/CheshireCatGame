using UnityEngine;

public class MenuControls : MonoBehaviour
{
    private PLayerControls controls;
    private PlayerMovement movementScriptRefferance;

    // Start is called before the first frame update
    void Start()
    {
        movementScriptRefferance = GameObject.Find("Chesenay").GetComponent<PlayerMovement>();
        controls = movementScriptRefferance.Controls;

        controls.Gameplay.AccessMenu.performed += ctx => OpenMenu();

    }

    public void OpenMenu()
    {
        Application.Quit();
    }


}
