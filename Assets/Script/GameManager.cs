using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyUI.Toast;

public class GameManager : MonoBehaviour
{
    int FieldNumber, levelunlocked = 1, CurrentLevel, Star = 0, NewStar = 0,LevelText;

    [SerializeField]
    TextMeshProUGUI TimerText, LevelCompletedText;

    [SerializeField]
    GameObject[] AllPanels,Stars;

    [SerializeField]
    List<GameObject> Levels;

    [SerializeField]
    Categories[] TotalCategory;

    [SerializeField]
    TextMeshProUGUI QuestionText;

    [SerializeField]
    TextMeshProUGUI[] OptionText;

    [SerializeField]
    Button MusicButton, SoundButton;

    [SerializeField]
    Sprite MusicOnSprite, SoundOnSprite, MusicOffSprite, SoundOffSprite;

    [SerializeField]
    AudioSource MusicSource, SoundSource;

    [SerializeField]
    AudioClip[] AllSoundClips,AllMusicClips;

    [SerializeField]
    bool MusicAction, SoundAction;

    bool RepeatQuestion,CountDown,RetryOfSuccess,RetryOfGameOver,CurrentLevelPlus,NextLevelPlus;


    float Timer = 30.0f;

    // fillname,question,star

    public void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            int SavedFieldData = PlayerPrefs.GetInt("CompletedQuestions" + i, 0);

            if (TotalCategory[i].AllData.Length <= SavedFieldData)
            {
                AllPanels[1].transform.GetChild(4).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (AllPanels[4].activeInHierarchy)
            {
                SettingsCloseButtonClickAction();
            }
            else if (AllPanels[6].activeInHierarchy)
            {
                MainmenuCloseButtonClickAction();
            }   
            else if (AllPanels[3].activeInHierarchy)
            {
                if (AllPanels[7].activeInHierarchy)
                {
                    EasyUI.Toast.Toast.Show("First Close Success Screen", 2f, ToastColor.Black);
                    //Debug.Log("First Close Success Panel.");

                }
                else if (AllPanels[8].activeInHierarchy)
                {
                    EasyUI.Toast.Toast.Show("First Close Game Over Screen", 2f, ToastColor.Black);
                   // Debug.Log("First Close Game Over Panel.");
                }
                else if (AllPanels[9].activeInHierarchy)
                {
                    EasyUI.Toast.Toast.Show("First Close Time Over Screen", 2f, ToastColor.Black);
                   // Debug.Log("First Close Time Over Panel.");
                }
                else
                {
                    BackButtonClickAction();
                }
            }
            else if (AllPanels[2].activeInHierarchy)
            {
                AllPanels[2].SetActive(false);
                AllPanels[1].SetActive(true);
            }
            else if (AllPanels[1].activeInHierarchy)
            {
                AllPanels[1].SetActive(false);
                AllPanels[0].SetActive(true);
            }
            else if (AllPanels[5].activeInHierarchy)
            {
                ExitPanelNoClickAction();
            }
            else
            {
                AllPanels[5].SetActive(true);
            }

        }
        if (CountDown)
        {
            Timer -= Time.deltaTime;
            if (Timer >= 0f)
            {
                Timer = Timer % 100f;
                //Debug.Log("111111:=" + Timer);
                CreateTimer();
            }
            else
            {
                TimeOver();
            }
        }
    }

    public void CreateTimer()
    {
        if (Mathf.RoundToInt(Timer) >= 60)
        {
            TimerText.GetComponent<TextMeshProUGUI>().text = "01:" + Mathf.RoundToInt(Timer);
        }
        else if (Mathf.RoundToInt(Timer) < 10)
        {
            TimerText.GetComponent<TextMeshProUGUI>().text = "00:0" + Mathf.RoundToInt(Timer);
        }
        else
        {
            TimerText.GetComponent<TextMeshProUGUI>().text = "00:" + Mathf.RoundToInt(Timer);
        }
    }

    public void GetCompletedLevelData()
    {
        int CompletedQuestionData = PlayerPrefs.GetInt("CompletedQuestions" + FieldNumber, 0);

        for (int i = 0; i < Levels.Count; i++)
        {
            if (i <= CompletedQuestionData)
            {
                Levels[i].transform.GetChild(1).gameObject.SetActive(false);
                Levels[i].GetComponent<Button>().interactable = true;
                Levels[i].GetComponent<Button>().transition = Selectable.Transition.ColorTint;
            }
            else
            {
                Levels[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void PlayButtonClickAction()
    {
        SoundPlayOnButtonClick();
        AllPanels[0].SetActive(false);
        AllPanels[1].SetActive(true);
    }

    public void SelectionPanelClickAction(int Value)
    {
        SoundPlayOnButtonClick();
        AllPanels[1].SetActive(false);
        AllPanels[2].SetActive(true);
        FieldNumber = Value;
        GetCompletedLevelData();
        LevelNumbersOnLevelsPanel();
        StarFill();
    }

    public void LevelNumbersOnLevelsPanel()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            int j = i + 1;
            AllPanels[2].transform.GetChild(4).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = j.ToString();
        }
    }

    public void LevelButtonClickAction(int ThisLevel)
    {
        SoundPlayOnButtonClick();
        AllPanels[2].SetActive(false);
        AllPanels[3].SetActive(true);
        MusicPlayOnGameScreen();
        CurrentLevel = ThisLevel;
        RepeatQuestion = true;
        //CountDown = true;
        GetQuestionsAndOptions();
    }

    public void GetQuestionsAndOptions()
    {
        Timer = 30.0f;
        CountDown = true;

        int CompletedQuestionData = PlayerPrefs.GetInt("CompletedQuestions" + FieldNumber, 0);

        if (CurrentLevel < CompletedQuestionData && RepeatQuestion)
        {
            QuestionText.text = TotalCategory[FieldNumber].AllData[CurrentLevel].Question;

            for (int i = 0; i < TotalCategory[FieldNumber].AllData[CurrentLevel].AllOptions.Length; i++)
            {
                OptionText[i].text = TotalCategory[FieldNumber].AllData[CurrentLevel].AllOptions[i].Option;
            }
            Debug.Log(TotalCategory[FieldNumber].AllData[CurrentLevel].Answer);
        }
        else
        {
            RepeatQuestion = false;

            if (TotalCategory[FieldNumber].AllData.Length > CompletedQuestionData)
            {
                QuestionText.text = TotalCategory[FieldNumber].AllData[CompletedQuestionData].Question;

                for (int i = 0; i < TotalCategory[FieldNumber].AllData[CompletedQuestionData].AllOptions.Length; i++)
                {
                    OptionText[i].text = TotalCategory[FieldNumber].AllData[CompletedQuestionData].AllOptions[i].Option;
                }
                Debug.Log(TotalCategory[FieldNumber].AllData[CompletedQuestionData].Answer);
            }
            else
            {
                AllPanels[10].SetActive(true);
                AllPanels[1].transform.GetChild(4).transform.GetChild(0).transform.GetChild(0).transform.GetChild(FieldNumber).transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void CheckAnswer(TextMeshProUGUI AnswerText)
    {
        int CompletedQuestionData = PlayerPrefs.GetInt("CompletedQuestions" + FieldNumber, 0);

        if (RetryOfSuccess)
        {
            Debug.Log("RetryOfSuccess");
            RetryOfSuccess = false;
            CompletedQuestionData--;
            RepeatQuestion = true;
            //DoubleRetry = true;
        }

        if (RetryOfGameOver)
        {
            Debug.Log("RetryOfGameOver");
            RetryOfGameOver = false;
           // CompletedQuestionData--;
            RepeatQuestion = false;
        }


        if (CurrentLevel < CompletedQuestionData && RepeatQuestion)
        {
            Debug.Log("If Original :- " + TotalCategory[FieldNumber].AllData[CurrentLevel].Answer);
            Debug.Log("If Clicked :- " + AnswerText.text);
            if (AnswerText.text == TotalCategory[FieldNumber].AllData[CurrentLevel].Answer)
            {
                Success(CurrentLevel);
                CurrentLevelPlus = true;
                // GetQuestionsAndOptions();
            }
            else
            {
                GameOver();
                Debug.Log("Game Over");
            }
        }
        else
        {
            Debug.Log("Else Original :- " + TotalCategory[FieldNumber].AllData[CompletedQuestionData].Answer);
            Debug.Log("Else Clicked :- " + AnswerText.text);
            RepeatQuestion = false;
            if (AnswerText.text == TotalCategory[FieldNumber].AllData[CompletedQuestionData].Answer)
            {
                Success(CompletedQuestionData);
                NextLevelPlus = true;
                //GetQuestionsAndOptions();
            }
            else
            {
                GameOver();
                Debug.Log("Game Over");
            }
        }
    }

    public void Success(int Level)
    {
        SoundPlayOnSuccessPanelOpen();
        MusicSource.mute = true;
        CountDown = false;
        AllPanels[7].SetActive(true);

        int CompletedQuestionData = PlayerPrefs.GetInt("CompletedQuestions" + FieldNumber, 0);

        if (CurrentLevel < CompletedQuestionData && RepeatQuestion)
        {
            Debug.Log("CurrentLevel");
            //if (SuccessedLevel)
            //{
            //    SuccessedLevel = false;
            //    LevelText = CurrentLevel;
            //    LevelCompletedText.text = "Level " + LevelText.ToString() + " Completed";
            //}
            //else
            //{
                LevelText = CurrentLevel;
                LevelText = LevelText + 1;
                LevelCompletedText.text = "Level " + LevelText.ToString() + " Completed";
            //}

            if (Timer >= 20.0f)
            {
                Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                Stars[1].transform.GetChild(0).gameObject.SetActive(true);
                Stars[2].transform.GetChild(0).gameObject.SetActive(true);
                NewStar = 3;
            }
            else if (Timer < 20 && Timer >= 10)
            {
                Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                Stars[1].transform.GetChild(0).gameObject.SetActive(true);
                Stars[2].transform.GetChild(0).gameObject.SetActive(false);
                NewStar = 2;
            }
            else
            {
                Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                Stars[1].transform.GetChild(0).gameObject.SetActive(false);
                Stars[2].transform.GetChild(0).gameObject.SetActive(false);
                NewStar = 1;
            }

            if (NewStar > Star)
            {
                if (Timer >= 20.0f)
                {
                    Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                    Stars[1].transform.GetChild(0).gameObject.SetActive(true);
                    Stars[2].transform.GetChild(0).gameObject.SetActive(true);
                    Star = 3;
                    PlayerPrefs.SetInt("GetedStars" + FieldNumber + Level, Star);
                }
                else if (Timer < 20 && Timer >= 10)
                {
                    Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                    Stars[1].transform.GetChild(0).gameObject.SetActive(true);
                    Stars[2].transform.GetChild(0).gameObject.SetActive(false);
                    Star = 2;
                    PlayerPrefs.SetInt("GetedStars" + FieldNumber + Level, Star);
                }
                else
                {
                    Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                    Stars[1].transform.GetChild(0).gameObject.SetActive(false);
                    Stars[2].transform.GetChild(0).gameObject.SetActive(false);
                    Star = 1;
                    PlayerPrefs.SetInt("GetedStars" + FieldNumber + Level, Star);
                }
            }
        }
        else
        {
            //if (SuccessedLevel)
            //{
            //    SuccessedLevel = false;
            //    LevelText = CompletedQuestionData;
            //    LevelCompletedText.text = "Level " + LevelText.ToString() + " Completed";
            //}
            //else
            //{
                LevelText = CompletedQuestionData;
                LevelText = LevelText + 1;
                LevelCompletedText.text = "Level " + LevelText.ToString() + " Completed";
          //  }

            if (Timer >= 20.0f)
            {
                Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                Stars[1].transform.GetChild(0).gameObject.SetActive(true);
                Stars[2].transform.GetChild(0).gameObject.SetActive(true);
                Star = 3;
                PlayerPrefs.SetInt("GetedStars" + FieldNumber + Level, Star);
            }
            else if (Timer < 20 && Timer >= 10)
            {
                Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                Stars[1].transform.GetChild(0).gameObject.SetActive(true);
                Stars[2].transform.GetChild(0).gameObject.SetActive(false);
                Star = 2;
                PlayerPrefs.SetInt("GetedStars" + FieldNumber + Level, Star);
            }
            else
            {
                Stars[0].transform.GetChild(0).gameObject.SetActive(true);
                Stars[1].transform.GetChild(0).gameObject.SetActive(false);
                Stars[2].transform.GetChild(0).gameObject.SetActive(false);
                Star = 1;
                PlayerPrefs.SetInt("GetedStars" + FieldNumber + Level, Star);
            }
        }
    }

    public void SuccessPanelExitAnimation()
    {
        AllPanels[7].transform.GetChild(1).GetComponent<Animator>().Play("SuccessExit");
        AllPanels[7].transform.GetChild(2).GetComponent<Animator>().Play("LevelCompletedExit");
        StartCoroutine(SuccessPanelBgWaiting());
        MusicOnAndOff();
    }

    IEnumerator SuccessPanelBgWaiting()
    {
        yield return new WaitForSeconds(0.40f);
        AllPanels[7].SetActive(false);
        //CountDown = true;
    }

    public void StarFill()
    {

        for (int i = 0; i < Levels.Count; i++)
        {
            int StarData = PlayerPrefs.GetInt("GetedStars" + FieldNumber + i, 0);
            GetStarData(i, StarData);
        }
       
       
    }

    public void GetStarData(int LevelNumber,int Stars)
    {

        switch (Stars)
        {
            case 0:
                Levels[LevelNumber].transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
                Levels[LevelNumber].transform.GetChild(2).GetChild(1).GetChild(0).gameObject.SetActive(false);
                Levels[LevelNumber].transform.GetChild(2).GetChild(2).GetChild(0).gameObject.SetActive(false);
                break;  
            case 1:
                Levels[LevelNumber].transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
                Levels[LevelNumber].transform.GetChild(2).GetChild(1).GetChild(0).gameObject.SetActive(false);
                Levels[LevelNumber].transform.GetChild(2).GetChild(2).GetChild(0).gameObject.SetActive(false);
                break;
            case 2:
                Levels[LevelNumber].transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
                Levels[LevelNumber].transform.GetChild(2).GetChild(1).GetChild(0).gameObject.SetActive(true);
                Levels[LevelNumber].transform.GetChild(2).GetChild(2).GetChild(0).gameObject.SetActive(false);
                break;
            case 3:
                Levels[LevelNumber].transform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
                Levels[LevelNumber].transform.GetChild(2).GetChild(1).GetChild(0).gameObject.SetActive(true);
                Levels[LevelNumber].transform.GetChild(2).GetChild(2).GetChild(0).gameObject.SetActive(true);
                break;
        }
    }

    public void GameOver()
    {
        SoundPlayOnGameOverPanelOpen();
        CountDown = false;
        AllPanels[8].SetActive(true);
        MusicSource.mute = true;
    }

    public void GameOverPanelExitAnimation()
    {
        AllPanels[8].transform.GetChild(1).GetComponent<Animator>().Play("GameOverExit");
        AllPanels[8].transform.GetChild(2).GetComponent<Animator>().Play("GameOverTextExit");
        StartCoroutine(GameOverPanelBgWaiting());
        MusicOnAndOff();
    }

    IEnumerator GameOverPanelBgWaiting()
    {
        yield return new WaitForSeconds(0.40f);
        AllPanels[8].SetActive(false);
        //CountDown = true;
    }

    public void TimeOver()
    {
        SoundPlayOnGameOverPanelOpen();
        CountDown = false;
        AllPanels[9].SetActive(true);
        MusicSource.mute = true;
    }

    public void TimeOverPanelExitAnimation()
    {
        AllPanels[9].transform.GetChild(1).GetComponent<Animator>().Play("TimeOverExit");
        AllPanels[9].transform.GetChild(2).GetComponent<Animator>().Play("TimeOverTextExit");
        StartCoroutine(TimeOverPanelBgWaiting());
        MusicOnAndOff();
    }

    IEnumerator TimeOverPanelBgWaiting()
    {
        yield return new WaitForSeconds(0.40f);
        AllPanels[9].SetActive(false);
        //CountDown = true;
    }
 
    public void RetryButtonClickAction()
    {
        SoundPlayOnButtonClick();
        Timer = 30.0f;
        CountDown = true;

        if (AllPanels[7].activeInHierarchy)
        {
            SuccessPanelExitAnimation() ;
            AllPanels[3].SetActive(true);
            RetryOfSuccess = true;
        }
        MainMenuPanelExitAnimation();
        if (AllPanels[8].activeInHierarchy)
        {
            GameOverPanelExitAnimation();
            AllPanels[3].SetActive(true);
            RetryOfGameOver = true;
        }
        TimeOverPanelExitAnimation();
    }

    public void BackButtonClickAction()
    {
        SoundPlayOnButtonClick();
        MusicPlayOnMainScreen();
        CountDown = false;
        GetCompletedLevelData();
        StarFill();

        if (AllPanels[3].activeInHierarchy)
        {
            AllPanels[3].SetActive(false);
            AllPanels[2].SetActive(true);
        }
        else if (AllPanels[2].activeInHierarchy)
        {
            AllPanels[2].SetActive(false);
            AllPanels[1].SetActive(true);
        }
        else
        {
            AllPanels[1].SetActive(false);
            AllPanels[0].SetActive(true);
        }
    }

    public void SettingsButtonClickAction()
    {
        SoundPlayOnButtonClick();
        MusicOnAndOff();
        AllPanels[4].SetActive(true);
    }

    public void SettingsCloseButtonClickAction()
    {
        SoundPlayOnButtonClick();
        SettingsPanelExitAnimation();
        //AllPanels[4].SetActive(false);
    }

    public void SettingsPanelExitAnimation()
    {
        AllPanels[4].transform.GetChild(1).GetComponent<Animator>().Play("SettingsExit");
        AllPanels[4].transform.GetChild(2).GetComponent<Animator>().Play("SettingsCloseExit");
        AllPanels[4].transform.GetChild(3).GetComponent<Animator>().Play("SettingsTextExit");
        StartCoroutine(SettingsPanelBgWaiting());

        if (AllPanels[0].activeInHierarchy)
        {
            MusicOnAndOff();
        }
        else if (AllPanels[1].activeInHierarchy)
        {
            MusicOnAndOff();
        }
        else
        {
            MusicOnAndOff();
        }
    }

    IEnumerator SettingsPanelBgWaiting()
    {
        yield return new WaitForSeconds(0.40f);
        AllPanels[4].SetActive(false);
    }

    public void ExitButtonClickAction()
    {
        SoundPlayOnButtonClick();
        AllPanels[5].SetActive(true);
        MusicSource.mute = true;
    }

    public void ExitPanelYesClickAction()
    {
        SoundPlayOnButtonClick();
        UnityEngine.Application.Quit();
    }

    public void ExitPanelNoClickAction()
    {
        SoundPlayOnButtonClick();
        //AllPanels[5].SetActive(false);
        ExitPanelExitAnimation();
    }

    public void ExitPanelExitAnimation()
    {
        AllPanels[5].transform.GetChild(1).GetComponent<Animator>().Play("ExitExit");
        AllPanels[5].transform.GetChild(2).GetComponent<Animator>().Play("ExitTextExit");
        StartCoroutine(ExitPanelBgWaiting());
        MusicOnAndOff();
    }

    IEnumerator ExitPanelBgWaiting()
    {
        yield return new WaitForSeconds(0.40f);
        AllPanels[5].SetActive(false);
    }

    public void HomeButtonClickAction()
    {
        SoundPlayOnButtonClick();
        MusicPlayOnMainScreen();
        if (AllPanels[2].activeInHierarchy)
        {
            AllPanels[2].SetActive(false);
            AllPanels[0].SetActive(true);
        }
        else if (AllPanels[6].activeInHierarchy)
        {
            MainMenuPanelExitAnimation();
            AllPanels[3].SetActive(false);
            AllPanels[0].SetActive(true);
        }
        else if (AllPanels[7].activeInHierarchy)
        {
            SuccessPanelExitAnimation();
            AllPanels[3].SetActive(false);
            AllPanels[0].SetActive(true);
        }
        else if (AllPanels[8].activeInHierarchy)
        {
            GameOverPanelExitAnimation();
            AllPanels[3].SetActive(false);
            AllPanels[0].SetActive(true);
        }
        else if (AllPanels[9].activeInHierarchy)
        {
            TimeOverPanelExitAnimation();
            AllPanels[3].SetActive(false);
            AllPanels[0].SetActive(true);
        }
    }

    public void MainmenuButtonClickAction()
    {
        SoundPlayOnButtonClick();
        CountDown = false;
        AllPanels[6].SetActive(true) ;
        MusicSource.mute = true;
    }

    public void MainmenuCloseButtonClickAction()
    {
        SoundPlayOnButtonClick();
        MainMenuPanelExitAnimation();
        //AllPanels[6].SetActive(false) ;
    }

    public void MainMenuPanelExitAnimation()
    {
        AllPanels[6].transform.GetChild(1).GetComponent<Animator>().Play("MainMenuExit");
        AllPanels[6].transform.GetChild(2).GetComponent<Animator>().Play("MainMenuCloseExit");
        AllPanels[6].transform.GetChild(3).GetComponent<Animator>().Play("MainMenuTextExit");
        StartCoroutine(MainMenuPanelBgWaiting());
        MusicOnAndOff();
    }

    IEnumerator MainMenuPanelBgWaiting()
    {
        yield return new WaitForSeconds(0.40f);
        AllPanels[6].SetActive(false);
        CountDown = true;
    }

    public void AllLevelsCompletedExitAnimation()
    {
        AllPanels[10].transform.GetChild(1).GetComponent<Animator>().Play("AllLevelCompletedTextExit");
        StartCoroutine(AllLevelsCompletedBgWaiting());
        MusicOnAndOff();
    }

    IEnumerator AllLevelsCompletedBgWaiting()
    {
        yield return new WaitForSeconds(1);
        AllPanels[10].SetActive(false);
        BackButtonClickAction();
    }

    public void NextButtonClickAction()
    {
        SoundPlayOnButtonClick();
        SuccessPanelExitAnimation();
        Timer = 30.0f;
        if (CurrentLevelPlus)
        {
            CurrentLevelPlus = false;
            CurrentLevel++;
        }
        if (NextLevelPlus)
        {
            NextLevelPlus = false;
            int CompletedQuestionData = PlayerPrefs.GetInt("CompletedQuestions" + FieldNumber, 0);
            CompletedQuestionData++;
            PlayerPrefs.SetInt("CompletedQuestions" + FieldNumber, CompletedQuestionData);

            if (levelunlocked <= CompletedQuestionData)
            {
                Levels[levelunlocked].transform.GetChild(1).gameObject.SetActive(false);
                Levels[levelunlocked].GetComponent<Button>().interactable = true;
                Levels[levelunlocked].GetComponent<Button>().transition = Selectable.Transition.ColorTint;
                levelunlocked++;
            }
        }
        AllPanels[3].SetActive(true) ;
        CountDown = true;
        GetQuestionsAndOptions();
    }

    public void MusicManagement()
    {
        SoundPlayOnButtonClick();
        if (!MusicAction)
        {
            MusicButton.GetComponent<Image>().sprite = MusicOffSprite;
            MusicSource.mute = true;
            MusicAction = true;
        }
        else
        {
            MusicButton.GetComponent<Image>().sprite = MusicOnSprite;
            MusicSource.mute = false;
            MusicAction = false;
        }
    }

    public void SoundManagement()
    {
        SoundPlayOnButtonClick();
        if (!SoundAction)
        {
            SoundButton.GetComponent<Image>().sprite = SoundOffSprite;
            SoundSource.mute = true;
            SoundAction = true;
        }
        else
        {
            SoundButton.GetComponent<Image>().sprite = SoundOnSprite;
            SoundSource.mute = false;
            SoundAction = false;
        }
    }

    public void SoundPlayOnButtonClick()
    {
        SoundSource.clip = AllSoundClips[0];
        SoundSource.Play();
    }

    public void SoundPlayOnSuccessPanelOpen()
    {
        SoundSource.clip = AllSoundClips[1];
        SoundSource.Play();
    }

    public void SoundPlayOnGameOverPanelOpen()
    {
        SoundSource.clip = AllSoundClips[2];
        SoundSource.Play();
    }

    public void MusicPlayOnMainScreen()
    {
        MusicSource.clip = AllMusicClips[0];
        MusicSource.volume = 0.5f;
        MusicSource.Play();
    }

    public void MusicPlayOnGameScreen()
    {
        MusicSource.clip = AllMusicClips[1];
        MusicSource.volume = 1;
        MusicSource.Play();
    }

    public void MusicOnAndOff()
    {
        bool Music = MusicAction;
        MusicSource.mute = Music;
    }
}

[System.Serializable]
class Categories
{
    public string CategoryName;
    [SerializeField]
    public CategoryData[] AllData;
}

[System.Serializable]
class CategoryData
{
    public string Question;
    public string Answer;
    [SerializeField]
    public Options[] AllOptions;
}

[System.Serializable]
class Options
{
   public string Option;
}
