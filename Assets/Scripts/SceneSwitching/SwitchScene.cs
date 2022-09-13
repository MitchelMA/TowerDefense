using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSwitching
{
    public class SwitchScene : MonoBehaviour
    {
        public void SwitchTo(int sceneIdx)
        {
            SceneManager.LoadScene(sceneIdx);
        }

        public void SwitchTo(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
