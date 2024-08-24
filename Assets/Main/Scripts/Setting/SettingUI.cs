using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public static SettingUI Instance { get; private set; }

    [SerializeField] private Button settingButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button leftTapButton;
    [SerializeField] private Button rightTapButton;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI LeftTapText;
    [SerializeField] private TextMeshProUGUI RightTapText;
    [SerializeField] private Transform rebindingPanel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Add event listeners
        moveUpButton.onClick.AddListener(() => RebindKey(PlayerInput.Binding.Move_Up));
        moveDownButton.onClick.AddListener(() => RebindKey(PlayerInput.Binding.Move_Down));
        moveLeftButton.onClick.AddListener(() => RebindKey(PlayerInput.Binding.Move_Left));
        moveRightButton.onClick.AddListener(() => RebindKey(PlayerInput.Binding.Move_Right));
        leftTapButton.onClick.AddListener(() => RebindKey(PlayerInput.Binding.Left_Tap));
        rightTapButton.onClick.AddListener(() => RebindKey(PlayerInput.Binding.Right_Tap));


    }

    private void MoveUpButton_onClick()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        UpdateVisual();
        CloseSetting();
        HideRebindKeyPanel();
    }

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        settingPanel.SetActive(false);
    }

    private void UpdateVisual()
    {
        moveUpText.text = PlayerInput.Instance.GetBidingText(PlayerInput.Binding.Move_Up);
        moveDownText.text = PlayerInput.Instance.GetBidingText(PlayerInput.Binding.Move_Down);
        moveLeftText.text = PlayerInput.Instance.GetBidingText(PlayerInput.Binding.Move_Left);
        moveRightText.text = PlayerInput.Instance.GetBidingText(PlayerInput.Binding.Move_Right);
        LeftTapText.text = PlayerInput.Instance.GetBidingText(PlayerInput.Binding.Left_Tap);
        RightTapText.text = PlayerInput.Instance.GetBidingText(PlayerInput.Binding.Right_Tap);
    }

    private void ShowRebindKeyPanel()
    {
        rebindingPanel.gameObject.SetActive(true);
    }

    private void HideRebindKeyPanel()
    {
        rebindingPanel.gameObject.SetActive(false);
    }

    public void RebindKey(PlayerInput.Binding binding)
    {
        ShowRebindKeyPanel();
        PlayerInput.Instance.RebindBinding(binding, () =>
        {
            HideRebindKeyPanel();
            UpdateVisual();
        });
    }
}
