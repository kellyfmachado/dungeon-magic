using EventArgs;
using UnityEngine;
using Item;
using System;
using UnityEngine.Events;

namespace Chest {
    public class ChestScript : MonoBehaviour {
        
        public Interaction interaction;
        public GameObject itemHolder;
        public Item.Item item;

        public ChestOpenEvent onOpen = new();

        private Animator thisAnimator;

        void Awake(){
            thisAnimator = GetComponent<Animator>();
        }

        void Start(){
            interaction.OnInteraction += OnInteraction;
            interaction.SetActionText("Open Chest");
        }

        void Update(){

        }
 
        private void OnInteraction(object sender, InteractionEventArgs args)
        {

            interaction.SetAvailable(false);
            thisAnimator.SetTrigger("tOpen");

            var itemObjectPrefab = item.ObjectPrefab;
            var position = itemHolder.transform.position;
            var rotation = itemObjectPrefab.transform.rotation;
            var itemObject = Instantiate(itemObjectPrefab, position, rotation);
            itemObject.transform.localScale = new Vector3(1,1,1);
            itemObject.transform.SetParent(itemHolder.transform);

            var itemType = item.itemType;
            if(itemType == ItemType.Key){
                GameManager.Instance.keys++;
            }
            else if (itemType == ItemType.BossKey) {
                GameManager.Instance.hasBossKey = true;
            }
            else if(itemType == ItemType.Potion){
                var player = GameManager.Instance.player;
                var playerLife = player.GetComponent<LifeScript>();
                playerLife.Heal();
            } else if (itemType == ItemType.Sword){
                GameManager.Instance.simpleSword.SetActive(false);
                GameManager.Instance.superSword.SetActive(true);
                
                var player = GameManager.Instance.player.GetComponent<PlayerController>();
                var damages = player.damageByStage;
                for (int i = 0; i < damages.Count; i++){
                    player.damageByStage[i] *= 2;
                }

            }

            onOpen?.Invoke(gameObject);

            // Update UI
            var gameplayUI = GameManager.Instance.gameplayUI;
            gameplayUI.AddObject(itemType); 

        }

    }

    [Serializable] public class ChestOpenEvent : UnityEvent<GameObject> {}

}