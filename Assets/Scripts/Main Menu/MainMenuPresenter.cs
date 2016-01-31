using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MainMenuPresenter : MonoBehaviour {
    // UI Panels
    public CanvasGroup IntroPanel;
    public CanvasGroup MainMenuPanel;
    public CanvasGroup RulesPanel;
    public CanvasGroup HistoryPanel;
    public CanvasGroup CreditsPanel;

    // Animations
    public float FadeInDuration = 0.2f;
    public Ease FadeEase = Ease.Linear;
    
    // Use this for initialization
    void Start () {
        // Force intro panel
        this.HideAllPanels();
        this.ForcePanelStatus(this.IntroPanel, true);
    }

    // Generic UI
    public void ForcePanelStatus(CanvasGroup panel,bool show)
    {
        panel.DOKill();
        panel.alpha = show ? 1 : 0;

        if(show)
            panel.transform.SetAsLastSibling();

    }

    public void BringToForefront(CanvasGroup panel)
    {
        panel.transform.SetAsLastSibling();

        panel.DOKill();
        panel.DOFade(1.0f, this.FadeInDuration).SetEase(this.FadeEase);
    }

    public void HideAllPanels()
    {
        this.ForcePanelStatus(this.IntroPanel,false);
        this.ForcePanelStatus(this.MainMenuPanel, false);
        this.ForcePanelStatus(this.RulesPanel, false);
        this.ForcePanelStatus(this.HistoryPanel, false);
        this.ForcePanelStatus(this.CreditsPanel, false);
    }
	
    // Go Tos
    public void GoToIntroPanel()
    {
        // Hide current panels
        this.HideAllPanels();

        // Go to panel
        this.BringToForefront(this.IntroPanel);
    }

    public void GoToMainMenuPanel()
    {
        // Hide current panels
        this.HideAllPanels();

        // Go to panel
        this.BringToForefront(this.MainMenuPanel);
    }

    public void GoToRulesPanel()
    {
        // Hide current panels
        this.HideAllPanels();

        // Go to panel
        this.BringToForefront(this.RulesPanel);
    }

    public void GoToHistoryPanel()
    {
        // Hide current panels
        this.HideAllPanels();

        // Go to panel
        this.BringToForefront(this.HistoryPanel);
    }

    public void GoToCredits()
    {
        // Hide current panels
        this.HideAllPanels();

        // Go to panel
        this.BringToForefront(this.CreditsPanel);
    }

}
