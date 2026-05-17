using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InsectUpgradePanel : MonoBehaviour
{
    private static InsectUpgradePanel _instance;
    public static InsectUpgradePanel Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public GameObject panelRoot;
    public Button atkButton;
    public Button hpButton;
    public Button closeButton;

    public TextMeshProUGUI nameLevelText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI hpText;

    [Header("虫虫图标（挂 SpriteRenderer 的物体）")]
    public SpriteRenderer insectIcon;

    private InsectWorldEntity _currentEntity;

    private void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        atkButton?.onClick.AddListener(OnAtkButtonClicked);
        hpButton?.onClick.AddListener(OnHpButtonClicked);
        closeButton?.onClick.AddListener(Hide);
    }

    public void Show(InsectWorldEntity entity)
    {
        _currentEntity = entity;

        // 先激活图标物体，再换图
        if (insectIcon != null)
        {
            insectIcon.gameObject.SetActive(true);
            
            InsectData data = entity?.GetComponent<InsectData>();
            if (data != null)
                insectIcon.sprite = data.insectImage;
        }

        UpdateDisplay();
        panelRoot?.SetActive(true);
        Debug.Log("激活");
    }

    public void Hide()
    {
        panelRoot?.SetActive(false);
        _currentEntity = null;
    }

    private void OnAtkButtonClicked()
    {
        _currentEntity?.UpgradeAtk();
        UpdateDisplay();
    }

    private void OnHpButtonClicked()
    {
        _currentEntity?.UpgradeHp();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_currentEntity == null) return;

        InsectData data = _currentEntity.GetComponent<InsectData>();
        if (data == null) return;

        nameLevelText.text = $"{data.Name}（Lv.{data.insectLevel}）";
        atkText.text = $" {data.insectAtk}";
        hpText.text = $" {data.insectHP}";
    }

    private void OnDestroy()
    {
        atkButton?.onClick.RemoveListener(OnAtkButtonClicked);
        hpButton?.onClick.RemoveListener(OnHpButtonClicked);
        closeButton?.onClick.RemoveListener(Hide);
    }
}