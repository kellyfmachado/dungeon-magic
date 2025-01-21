using System;
using EventArgs;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameObject widgetPrefab;
    [SerializeField] private Vector3 widgetOffset;
    public float radius = 10f;

    private GameObject widget;
    private bool isAvailable = true;
    private bool isActive;

    public event EventHandler<InteractionEventArgs> OnInteraction;

    private void OnEnable (){
        GameManager.Instance.interactionList.Add(this);
    }

    private void OnDisable (){
        GameManager.Instance.interactionList.Remove(this);
    }

    void Awake (){

        widget = Instantiate(widgetPrefab, transform.position + widgetOffset, widgetPrefab.transform.rotation);
        widget.transform.SetParent(gameObject.transform, true);

    }

    void Start()
    {

        var worldUiCamera = GameManager.Instance.worldUiCamera;
        var canvas = widget.GetComponent<Canvas>();
        if(canvas != null){
            canvas.worldCamera = worldUiCamera;
        }

        var interactionWidgetComponent = widget.GetComponent<InteractionWidget>();
        if(interactionWidgetComponent != null){
            interactionWidgetComponent.worldUiCamera = worldUiCamera;
        }
    }

    void Update()
    {
        
    }

    public bool IsActive(){
        return isActive;
    }

    public void SetActive(bool isActive){
        var wasActive = this.isActive;
        this.isActive = isActive;   

        var interactionWidget = widget.GetComponent<InteractionWidget>();  
        if(isActive){
            interactionWidget.Show();
        }
        else {
            interactionWidget.Hide();
        }

    }

    public bool IsAvailable (){
        return isAvailable;
    }

    public void SetAvailable (bool isAvailable){
        this.isAvailable = isAvailable;
    }

    public void Interact(){

        OnInteraction?.Invoke(this, new InteractionEventArgs());

    }

    public void SetActionText(string text){

        var interactionWidget = widget.GetComponent<InteractionWidget>();  
        interactionWidget.SetActionText(text);

    }

}
