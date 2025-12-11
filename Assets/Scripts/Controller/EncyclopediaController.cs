using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using echo17.EndlessBook;

public class EncyclopediaController : MonoBehaviour, IController
{
    [Header("References")]
    public DynamicEndlessBook Book;
    public Camera PageRenderCamera;
    public EncyclopediaPageUI PageUIPrefab;
    public Transform PageRenderContainer; // Where we spawn UI to render

    [Header("Data")]
    public List<CropConfig> CropConfigs; // Assign in Inspector
    public Material BasePageMaterial; // Material with a shader that supports textures (e.g., Unlit/Texture)

    [Header("Settings")]
    public Vector2Int PageTextureSize = new Vector2Int(512, 512);

    private IKnowledgeModel _knowledgeModel;

    private bool _isOpen = false;

    private void Start()
    {
        _knowledgeModel = this.GetModel<IKnowledgeModel>();
        
        // Ensure the container is far away or on a specific layer to avoid being seen by main camera
        if (PageRenderContainer == null)
        {
            GameObject container = new GameObject("PageRenderContainer");
            container.transform.position = new Vector3(1000, 1000, 0);
            PageRenderContainer = container.transform;
        }

        if (PageRenderCamera == null)
        {
            GameObject camObj = new GameObject("PageRenderCamera");
            camObj.transform.parent = PageRenderContainer;
            camObj.transform.localPosition = new Vector3(0, 0, -10);
            PageRenderCamera = camObj.AddComponent<Camera>();
            PageRenderCamera.orthographic = true;
            PageRenderCamera.enabled = false; // We only render manually
            // Adjust camera size to fit the UI
            // Assuming UI is 100x100 world units or similar, need to calibrate
        }

        InitializeBook();
        gameObject.SetActive(false); // Start closed
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (_isOpen)
            {
                CloseEncyclopedia();
            }
            else
            {
                OpenEncyclopedia();
            }
        }
    }

    public void OpenEncyclopedia()
    {
        _isOpen = true;
        gameObject.SetActive(true);
        InitializeBook(); // Re-render to update unlocked status
        Book.SetState(EndlessBook.StateEnum.OpenFront);
    }

    public void CloseEncyclopedia()
    {
        _isOpen = false;
        Book.SetState(EndlessBook.StateEnum.ClosedFront, onCompleted: (from, to, page) => 
        {
            gameObject.SetActive(false);
        });
    }

    private void InitializeBook()
    {
        if (Book == null || PageUIPrefab == null || BasePageMaterial == null)
        {
            Debug.LogError("EncyclopediaController: Missing references!");
            return;
        }

        Book.ClearPages();

        foreach (var config in CropConfigs)
        {
            bool isUnlocked = _knowledgeModel.IsUnlocked(config.CropName);

            // Create Left Page
            AddPage(config, isUnlocked, true);

            // Create Right Page
            AddPage(config, isUnlocked, false);
        }

        // Reset book to start if needed, or just update
        // Book.SetPageNumber(1); 
    }

    private void AddPage(CropConfig config, bool isUnlocked, bool isLeftPage)
    {
        // 1. Instantiate UI
        EncyclopediaPageUI uiInstance = Instantiate(PageUIPrefab, PageRenderContainer);
        uiInstance.transform.localPosition = Vector3.zero;
        
        // Reset scale and rotation just in case
        uiInstance.transform.localRotation = Quaternion.identity;
        uiInstance.transform.localScale = Vector3.one;

        // 2. Setup Data
        uiInstance.Setup(config, isUnlocked, isLeftPage);

        // Force UI update
        Canvas.ForceUpdateCanvases();

        // 3. Render to Texture
        RenderTexture rt = new RenderTexture(PageTextureSize.x, PageTextureSize.y, 16);
        PageRenderCamera.targetTexture = rt;
        PageRenderCamera.Render();
        PageRenderCamera.targetTexture = null;

        // 4. Create Material
        Material pageMat = new Material(BasePageMaterial);
        pageMat.mainTexture = rt;

        // 5. Add to Book
        Book.AddPage(pageMat);

        // 6. Cleanup UI
        Destroy(uiInstance.gameObject);
    }

    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    // Navigation methods for buttons
    public void NextPage()
    {
        Book.TurnForward(1);
    }

    public void PreviousPage()
    {
        Book.TurnBackward(1);
    }
}
