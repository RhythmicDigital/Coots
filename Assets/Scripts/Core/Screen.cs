using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class Screen : MonoBehaviour
{
    [SerializeField] List<TMP_Text> menuItems;

    int previousSelection = 0;
    int selection = 0;

    public event Action<int> OnSelected;

    public void HandleUpdate()
    {   
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
            AudioManager.i.PlaySfx(SfxId.UISelect);

        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
            ++selection;

        else if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
            --selection;

        else if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0)
        {
            previousSelection = selection;
            selection += 3;
        }
        else if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)
        {
            selection -= 3 - previousSelection;
        }
        selection = Mathf.Clamp(selection, 0, menuItems.Count - 1);
        
        UpdateItemSelection();
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.GetButtonDown("Submit"))
        {
            OnSelected?.Invoke(selection);
            AudioManager.i.PlaySfx(SfxId.UIConfirm);
        }
    }

    public void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selection)
                menuItems[i].color = GlobalSettings.i.HighlightedColor;
            else
                menuItems[i].color = GlobalSettings.i.UnhighlightedColor;
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

    public void Open()
    {
        SetActive(true);
    }

    public void Close()
    {
        SetActive(false);
    }
}