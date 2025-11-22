using UnityEngine;
using UnityEngine.SceneManagement;      

public class ButtonController : MonoBehaviour
{
    public void PlayButton(){
        SceneData.sceneToLoad = 2;  
        SceneManager.LoadScene("LoadingScene");
    }
    
    public void QuitButton(){
        Application.Quit();
    }

    public void OpenUIButton(GameObject ui){
        ui.SetActive(true);
    }

    public void CloseUIButton(GameObject ui){
        ui.SetActive(false);
    }
}
