using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Music;

namespace UI {
    public class TitleScreenUI : MonoBehaviour {
        public Animator thisAnimator;
        public float fadeOutDuration;

        private bool isFadingOut;

        public FadeMusic fadeMusic;

        public void FadeOut() {
            // Debounce
            if (isFadingOut) return;
            isFadingOut = true;

            // Fade out
            // thisAnimator.SetTrigger("tFadeOut");
            thisAnimator.enabled =true;
            // Schedule scene
            StartCoroutine(TransitionToNextScene());
        }

        private IEnumerator TransitionToNextScene() {
            yield return new WaitForSeconds(fadeOutDuration);
            SceneManager.LoadScene("Scenes/Dungeon");
        }

        public void FadeOutMusic() {
            fadeMusic.FadeOut();
        }

    }
}