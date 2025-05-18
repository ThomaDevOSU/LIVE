using UnityEngine;
using UnityEngine.EventSystems;

/*
UISoundTrigger.cs
Author: Doug Hillyer, Spring 2025

This component plays UI sound effects on hover and click events. 
If hoverSound or clickSound are not assigned in the Inspector, 
they will default to the AudioLibrary singleton values at runtime.

Usage:
- Attach this script to any UI component with a selectable (Button, Toggle, etc.)
- Assign clips in the Inspector or let them be pulled from AudioLibrary.
*/

public class UISoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private void Start()
    {
        if (hoverSound == null && AudioLibrary.Instance != null)
            hoverSound = AudioLibrary.Instance.uiHover;

        if (clickSound == null && AudioLibrary.Instance != null)
            clickSound = AudioLibrary.Instance.uiClick;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
            AudioManager.Instance.PlaySFX(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            AudioManager.Instance.PlaySFX(clickSound);
    }
}
