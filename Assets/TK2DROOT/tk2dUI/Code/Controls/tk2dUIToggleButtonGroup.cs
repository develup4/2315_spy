using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/UI/tk2dUIToggleButtonGroup")]
public class tk2dUIToggleButtonGroup : MonoBehaviour
{
    /// <summary>
    /// Group of toggle buttons control but this manager. Only one can be selected at time
    /// </summary>
    [SerializeField]
    private tk2dUIToggleButton[] toggleBtns;

    public tk2dUIToggleButton[] ToggleBtns
    {
        get { return toggleBtns; }
    }

    /// <summary>
    /// Which toggle button is to be on first (index)
    /// </summary>
    [SerializeField]
    private int selectedIndex;

    public int SelectedIndex
    {
        get { return selectedIndex; }
        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value;
                SetToggleButtonUsingSelectedIndex();
            }
        }
    }

    private tk2dUIToggleButton selectedToggleButton;

    /// <summary>
    /// Currently selected toggle button
    /// </summary>
    public tk2dUIToggleButton SelectedToggleButton
    {
        get { return selectedToggleButton; }
        set
        {
            ButtonToggle(value);
        }
    }

    /// <summary>
    /// Event, on toggle button change
    /// </summary>
    public event System.Action<tk2dUIToggleButtonGroup> OnChange;

    protected virtual void Awake()
    {
        Setup();
    }

    protected void Setup()
    {
        foreach (tk2dUIToggleButton toggleBtn in toggleBtns)
        {
            if (toggleBtn != null)
            {
                toggleBtn.IsInToggleGroup = true;
                toggleBtn.IsOn = false;
                toggleBtn.OnToggle += ButtonToggle;
            }
        }
        SetToggleButtonUsingSelectedIndex();
    }

    /// <summary>
    /// Clears exists Group of toggle buttons, and adds new list of toggle buttons
    /// </summary>
    public void AddNewToggleButtons(tk2dUIToggleButton[] newToggleBtns)
    {
        ClearExistingToggleBtns();
        toggleBtns = newToggleBtns;
        Setup();
    }

    private void ClearExistingToggleBtns()
    {
        if (toggleBtns != null && toggleBtns.Length > 0)
        {
            foreach (tk2dUIToggleButton toggleBtn in toggleBtns)
            {
                toggleBtn.IsInToggleGroup = false;
                toggleBtn.OnToggle -= ButtonToggle;
                toggleBtn.IsOn = false;
            }
        }
    }

    private void SetToggleButtonUsingSelectedIndex()
    {
        tk2dUIToggleButton newToggleBtn = null;
        if (selectedIndex >= 0 && selectedIndex < toggleBtns.Length)
        {
            newToggleBtn = toggleBtns[selectedIndex];
            newToggleBtn.IsOn = true; //events will call ButtonToggle automatically
        }
        else
        {
            newToggleBtn = null;
            selectedIndex = -1;
            ButtonToggle(newToggleBtn);
        }
    }


    private void ButtonToggle(tk2dUIToggleButton toggleButton)
    {
        if (toggleButton == null || toggleButton.IsOn)
        {
            foreach (tk2dUIToggleButton tempToggleBtn in toggleBtns)
            {
                if (tempToggleBtn != toggleButton)
                {
                    tempToggleBtn.IsOn = false;
                }
            }

            if (toggleButton != selectedToggleButton)
            {
                selectedToggleButton = toggleButton;
                SetSelectedIndexFromSelectedToggleButton();
                if (OnChange != null) { OnChange(this); }
            }
        }
    }

    private void SetSelectedIndexFromSelectedToggleButton()
    {
        selectedIndex = -1;
        tk2dUIToggleButton tempToggleBtn;
        for (int n=0; n<toggleBtns.Length; n++)
        {
            tempToggleBtn = toggleBtns[n];
            if (tempToggleBtn == selectedToggleButton)
            {
                selectedIndex = n;
                break;
            }
        }
    }

}
