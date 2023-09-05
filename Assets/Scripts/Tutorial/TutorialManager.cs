using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public TutorialSteps TutorialSteps = TutorialSteps.AsteroidCome;

    public Transform PlayerShipPosition;
    public Transform CenterShipPosition;
    public Transform EnemyShipPosition;
    public RectTransform SpawnPosition;
    public RectTransform DecorationPosition;

    public GameObject PlayerShip;

    [HideInInspector] public GameObject currentObject;

    public ParticleSystem StarsParticle;

    public TMP_Text HeaderText;

    public TMP_Text CollectedOre;
    public TMP_Text CollectedCrystalls;

    public GameObject Asteroid;
    public GameObject Asteroid2;
    public GameObject FirstEnemy;
    public GameObject Dealer;
    public GameObject Target;
    public GameObject Pirate;

    public GameObject DisabledButtons;
    public GameObject FirstSet;
    public GameObject SecondSet;
    public GameObject ThirdSet;
    public GameObject FourthSet;
    public GameObject TestTargetSet;
    public GameObject Pirates1;
    public GameObject Pirates2;
    public GameObject Pirates3;
    public GameObject Pirates4;
    public GameObject Pirates5;

    public GameObject FirstClue;
    public GameObject FirstReward;
    public GameObject SecondClue;
    public GameObject SecondReward;
    public GameObject ThirdChoose;
    public GameObject ThirdClue;
    public GameObject ThirdReward;
    public GameObject DealerDialog1;
    public GameObject DealerDialog2;
    public GameObject DealerClue;
    public GameObject DealerEnd;
    public GameObject PirateCome;
    public GameObject YouLose;
    public GameObject YouWin;

    public Image firstPictureImage;
    public Image secondPictureImage;

    public GameObject EnergyBar;
    public GameObject EnergyText;
    public GameObject EnergyIcon;

    private GameObject CurrentDecoration;
    public GameObject Decoration1;
    public GameObject Decoration2;

    public GameObject Skybox;
    [HideInInspector] public Vector3 SkyboxStep = new Vector3(-22f, 0f, 0f);

    private Action<TurnState> EndTurnHandler;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);
    }


    void Start()
    {
        TutorialStep();
        EndTurnHandler = (e) => { if (e == TurnState.PlayerTurn) HeaderText.text = "Enemy turn!"; else HeaderText.text = "Your turn!"; };
    }

    public void TutorialStep(bool fromButton = false)
    {
        if (fromButton) GetComponent<AudioSource>().Play();

        switch (TutorialSteps)
        {
            case TutorialSteps.FlyToAsteroid:
                StartCoroutine(PlayMoveAnimation());
                break;
            case TutorialSteps.AsteroidCome:
                HeaderText.text = "explore the asteroid collect resources";
                FirstClue.SetActive(true);
                break;
            case TutorialSteps.AsteroidFirstPicture:
                HeaderText.text = "Click the tile";
                FirstClue.SetActive(false);
                DisabledButtons.SetActive(true);
                FirstSet.SetActive(true);

                foreach (Transform picture in FirstSet.transform)
                {
                    var image = picture.Find("Canvas/BackImage").GetComponent<Image>();
                    var button = picture.Find("Canvas/Button").GetComponent<Button>();

                    if (image != firstPictureImage)
                    {
                        image.color = new Color(0.5f, 0.5f, 0.5f);
                        button.interactable = false;
                    }
                }

                break;
            case TutorialSteps.AsteroidSecondPicture:
                HeaderText.text = "now this tile";

                foreach (Transform picture in FirstSet.transform)
                {
                    var image = picture.Find("Canvas/BackImage").GetComponent<Image>();
                    var button = picture.Find("Canvas/Button").GetComponent<Button>();

                    if (image == secondPictureImage)
                    {
                        image.color = new Color(1f, 1f, 1f);
                        button.interactable = true;
                        break;
                    }
                }
                break;
            case TutorialSteps.AsteroidCollect:
                HeaderText.text = "Match the same tiles!";

                foreach (Transform picture in FirstSet.transform)
                {
                    var image = picture.Find("Canvas/BackImage").GetComponent<Image>();
                    var button = picture.Find("Canvas/Button").GetComponent<Button>();
                    image.color = new Color(1f, 1f, 1f);
                    button.interactable = true;
                }
                break;
            case TutorialSteps.AsteroidReward:
                HeaderText.text = "Received:";
                DisabledButtons.SetActive(false);
                FirstSet.SetActive(false);
                FirstReward.SetActive(true);
                break;
            case TutorialSteps.AsteroidLeave:
                HeaderText.text = "";
                FirstReward.SetActive(false);

                StartCoroutine(RemoveCurrentObject());
                break;
            case TutorialSteps.SecondAsteroidCome:
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
                HeaderText.text = "Accumulate ship energy by matching pairs";
                SecondClue.SetActive(true);
                break;
            case TutorialSteps.SecondAsteroidCollect:
                SecondClue.SetActive(false);
                HeaderText.text = "Match the same tiles!";
                DisabledButtons.SetActive(true);
                SecondSet.SetActive(true);
                break;
            case TutorialSteps.SecondAsteroidReward:
                DisabledButtons.SetActive(false);
                SecondSet.SetActive(false);
                HeaderText.text = "Received:";
                SecondReward.SetActive(true);
                break;
            case TutorialSteps.SecondAsteroidLeave:
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
                HeaderText.text = "";
                SecondReward.SetActive(false);
                StartCoroutine(RemoveCurrentObject());
                break;
            case TutorialSteps.PlanetCome:
                HeaderText.text = "You come to planet 2b2t";
                ThirdChoose.SetActive(true);
                break;
            case TutorialSteps.PlanetClue:
                TutorialGameManager.instance.CollectedCrystall = 0;
                TutorialGameManager.instance.CollectedOre = 0;
                HeaderText.text = "There are other scavengers nearby. Try to collect as many as possible!";
                StartCoroutine(SpawnOtherObject(FirstEnemy));
                ThirdChoose.SetActive(false);
                ThirdClue.SetActive(true);
                break;
            case TutorialSteps.PlanetCollect:
                ThirdClue.SetActive(false);
                HeaderText.text = "Your turn!";
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
                currentObject.GetComponent<TutorialShipEffects>().SwitchEnergy();
                DisabledButtons.SetActive(true);
                ThirdSet.SetActive(true);

                TutorialGameManager.instance.TurnEnd += EndTurnHandler;

                TutorialGameManager.instance.EnemyShip = currentObject.GetComponent<TutorialShip>();
                TutorialAI.instance.IsAIDisabled = false;
                foreach (Transform picture in ThirdSet.transform)
                {
                    TutorialAI.instance.Pictures.Add(picture.gameObject.GetComponent<TutorialPicture>());
                }
                break;
            case TutorialSteps.PlanetCollect2:
                ThirdSet.SetActive(false);
                FourthSet.SetActive(true);

                foreach (Transform picture in FourthSet.transform)
                {
                    var tutPic = picture.gameObject.GetComponent<TutorialPicture>();

                    StartCoroutine(tutPic.PictureAppearance());
                    TutorialAI.instance.Pictures.Add(tutPic);
                }
                break;
            case TutorialSteps.PlanetReward:
                TutorialGameManager.instance.EnemyShip = null;
                TutorialAI.instance.IsAIDisabled = true;
                TutorialGameManager.instance.TurnEnd -= EndTurnHandler;
                FourthSet.SetActive(false);
                DisabledButtons.SetActive(false);
                ThirdReward.SetActive(true);
                CollectedOre.text = TutorialGameManager.instance.CollectedOre.ToString();
                CollectedCrystalls.text = TutorialGameManager.instance.CollectedCrystall.ToString();
                HeaderText.text = "Received:";
                break;
            case TutorialSteps.PlanetLeave:
                ThirdReward.SetActive(false);
                HeaderText.text = "";
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
                StartCoroutine(RemoveCurrentObject());
                StartCoroutine(RemoveCurrentDecoration());
                break;
            case TutorialSteps.DealerCome:
                HeaderText.text = "Hello, scavenger! Do you have something to protect yourself from pirates?";
                DealerDialog1.SetActive(true);
                break;
            case TutorialSteps.DealerDeal:
                HeaderText.text = "Then I have something to offer you";
                DealerDialog1.SetActive(false);
                DealerDialog2.SetActive(true);
                break;
            case TutorialSteps.DealerClue:
                HeaderText.text = "Let's test it! Destroy the drone by matching pairs";
                DealerDialog2.SetActive(false);
                DealerClue.SetActive(true);
                break;
            case TutorialSteps.DealerTraining:
                HeaderText.text = "Destroy the drone";
                StartCoroutine(RemoveCurrentObject(false));
                StartCoroutine(SpawnOtherObject(Target));
                DealerClue.SetActive(false);
                DisabledButtons.SetActive(true);
                TestTargetSet.SetActive(true);
                break;
            case TutorialSteps.DealerEnd:
                DisabledButtons.SetActive(false);
                TestTargetSet.SetActive(false);
                DealerEnd.SetActive(true);
                StartCoroutine(SpawnOtherObject(Dealer));
                HeaderText.text = "Great! By the way, there is a reward for capturing pirates! See you soon!";
                break;
            case TutorialSteps.DealerLeave:
                DealerEnd.SetActive(false);
                HeaderText.text = "";
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchHealth();
                StartCoroutine(RemoveCurrentObject());
                break;
            case TutorialSteps.PiratesCome:
                StartCoroutine(SpawnOtherObject(Pirate));
                HeaderText.text = "Pirates are attacking you!";
                PirateCome.SetActive(true);
                currentObject.GetComponent<TutorialShip>().CalculateMaxEnergy();
                TutorialGameManager.instance.globalPictureCount = 8;
                break;
            case TutorialSteps.PiratesFight1:
                PirateCome.SetActive(false);
                HeaderText.text = "Your turn!";
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchHealth();
                PlayerShip.GetComponent<TutorialShipEffects>().SwitchDamage();
                currentObject.GetComponent<TutorialShipEffects>().SwitchEnergy();
                currentObject.GetComponent<TutorialShipEffects>().SwitchHealth();
                currentObject.GetComponent<TutorialShipEffects>().SwitchDamage();
                DisabledButtons.SetActive(true);
                Pirates1.SetActive(true);

                TutorialGameManager.instance.TurnEnd += EndTurnHandler;

                TutorialGameManager.instance.EnemyShip = currentObject.GetComponent<TutorialShip>();

                TutorialAI.instance.IsAIDisabled = false;

                foreach (Transform picture in Pirates1.transform)
                {
                    TutorialAI.instance.Pictures.Add(picture.gameObject.GetComponent<TutorialPicture>());
                }
                break;
            case TutorialSteps.PiratesFight2:
                Pirates1.SetActive(false);
                Pirates2.SetActive(true);

                foreach (Transform picture in Pirates2.transform)
                {
                    var tutPic = picture.gameObject.GetComponent<TutorialPicture>();

                    StartCoroutine(tutPic.PictureAppearance());
                    TutorialAI.instance.Pictures.Add(tutPic);
                }
                break;
            case TutorialSteps.PiratesFight3:
                Pirates2.SetActive(false);
                Pirates3.SetActive(true);

                foreach (Transform picture in Pirates3.transform)
                {
                    var tutPic = picture.gameObject.GetComponent<TutorialPicture>();

                    StartCoroutine(tutPic.PictureAppearance());
                    TutorialAI.instance.Pictures.Add(tutPic);
                }
                break;
            case TutorialSteps.PiratesFight4:
                Pirates3.SetActive(false);
                Pirates4.SetActive(true);

                foreach (Transform picture in Pirates4.transform)
                {
                    var tutPic = picture.gameObject.GetComponent<TutorialPicture>();

                    StartCoroutine(tutPic.PictureAppearance());
                    TutorialAI.instance.Pictures.Add(tutPic);
                }
                break;
            case TutorialSteps.PiratesFight5:
                Pirates4.SetActive(false);
                Pirates5.SetActive(true);

                foreach (Transform picture in Pirates5.transform)
                {
                    var tutPic = picture.gameObject.GetComponent<TutorialPicture>();

                    StartCoroutine(tutPic.PictureAppearance());
                    TutorialAI.instance.Pictures.Add(tutPic);
                }
                break;
            case TutorialSteps.PiratesEnd:
                TutorialGameManager.instance.TurnEnd -= EndTurnHandler;
                DisabledButtons.SetActive(false);
                Pirates1.SetActive(false);
                Pirates2.SetActive(false);
                Pirates3.SetActive(false);
                Pirates4.SetActive(false);
                Pirates5.SetActive(false);
                if (TutorialGameManager.instance.PlayerShip.Destroyed)
                {
                    HeaderText.text = "Abandon ship! So close...";
                    YouLose.SetActive(true);
                }
                else
                {
                    HeaderText.text = "Congratulations! You complete the tutorial! The galaxy is waiting for you!";
                    YouWin.SetActive(true);
                }
                break;
            case TutorialSteps.TutorialLeave:
                if (TutorialGameManager.instance.PlayerShip.Destroyed)
                {
                    SceneManager.LoadScene("TutorialScene");
                }
                else
                {
                    SceneManager.LoadScene("SetGame");
                }
                break;
        }

        TutorialSteps++;
    }

    public IEnumerator PlayMoveAnimation()
    {
        StartCoroutine(MoveSkybox());

        StarsParticle.gameObject.SetActive(true);
        StarsParticle.Play();

        while (PlayerShip.transform.position != CenterShipPosition.position)
        {
            PlayerShip.transform.position = Vector3.MoveTowards(PlayerShip.transform.position, CenterShipPosition.position, 3f * Time.deltaTime);
            yield return 0;
        }

        yield return new WaitForSeconds(2.5f);

        if (TutorialSteps == TutorialSteps.AsteroidCome)
        {
            StartCoroutine(SpawnOtherObject(Asteroid));
        }
        else if (TutorialSteps == TutorialSteps.SecondAsteroidCome)
        {
            StartCoroutine(SpawnOtherObject(Asteroid2));
        }
        else if (TutorialSteps == TutorialSteps.PlanetCome)
        {
            StartCoroutine(SpawnDecoration(Decoration1));
        }
        else if (TutorialSteps == TutorialSteps.DealerCome)
        {
            StartCoroutine(SpawnOtherObject(Dealer));
        }
        else if (TutorialSteps == TutorialSteps.PiratesCome)
        {
            StartCoroutine(SpawnDecoration(Decoration2));
        }

        yield return new WaitForSeconds(0.5f);

        while (PlayerShip.transform.position != PlayerShipPosition.position)
        {
            PlayerShip.transform.position = Vector3.MoveTowards(PlayerShip.transform.position, PlayerShipPosition.position, 3f * Time.deltaTime);
            yield return 0;
        }

        TutorialStep();
    }

    public IEnumerator SpawnOtherObject(GameObject objectToSpawn)
    {
        currentObject = Instantiate(objectToSpawn, SpawnPosition);

        if (objectToSpawn == Target)
        {
            TutorialGameManager.instance.EnemyShip = currentObject.GetComponent<TutorialShip>();
            PlayerShip.GetComponent<TutorialShipEffects>().SwitchEnergy();
            PlayerShip.GetComponent<TutorialShipEffects>().SwitchHealth();
            currentObject.GetComponent<TutorialShipEffects>().SwitchHealth();
        }

        while (currentObject.transform.position != EnemyShipPosition.position)
        {
            currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, EnemyShipPosition.position, 3f * Time.deltaTime);
            yield return 0;
        }
    }

    public IEnumerator RemoveCurrentObject(bool leave = true)
    {
        var curObj = currentObject;
        currentObject = null;

        var target = SpawnPosition.position;

        while (curObj.transform.position != target)
        {
            curObj.transform.position = Vector3.MoveTowards(curObj.transform.position, target, 4f * Time.deltaTime);
            yield return 0;
        }

        Destroy(curObj);

        if (leave)
        {
            StartCoroutine(PlayMoveAnimation());
        }
    }

    public IEnumerator SpawnDecoration(GameObject decoration)
    {
        CurrentDecoration = Instantiate(decoration, SpawnPosition);

        while (CurrentDecoration.transform.position != DecorationPosition.position)
        {
            CurrentDecoration.transform.position = Vector3.MoveTowards(CurrentDecoration.transform.position, DecorationPosition.position, 3f * Time.deltaTime);
            yield return 0;
        }
    }

    public IEnumerator RemoveCurrentDecoration()
    {
        var target = SpawnPosition.position;

        while (CurrentDecoration.transform.position != target)
        {
            CurrentDecoration.transform.position = Vector3.MoveTowards(CurrentDecoration.transform.position, target, 4f * Time.deltaTime);
            yield return 0;
        }

        Destroy(CurrentDecoration);
        CurrentDecoration = null;
    }

    public IEnumerator MoveSkybox()
    {
        var target = Skybox.transform.position + SkyboxStep;

        while (Skybox.transform.position != target)
        {
            Skybox.transform.position = Vector3.MoveTowards(Skybox.transform.position, target, 4.5f * Time.deltaTime);
            yield return 0;
        }
    }
}

public enum TutorialSteps
{
    FlyToAsteroid = 0,
    AsteroidCome = 1,
    AsteroidFirstPicture = 2,
    AsteroidSecondPicture = 3,
    AsteroidCollect = 4,
    AsteroidReward = 5,
    AsteroidLeave = 6,
    SecondAsteroidCome = 7,
    SecondAsteroidCollect = 8,
    SecondAsteroidReward = 9,
    SecondAsteroidLeave = 10,
    PlanetCome = 11,
    PlanetClue = 12,
    PlanetCollect = 13,
    PlanetCollect2 = 14,
    PlanetReward = 15,
    PlanetLeave = 16,
    DealerCome = 17,
    DealerDeal = 18,
    DealerClue = 19,
    DealerTraining = 20,
    DealerEnd = 21,
    DealerLeave = 22,
    PiratesCome = 23,
    PiratesFight1 = 24,
    PiratesFight2 = 25,
    PiratesFight3 = 26,
    PiratesFight4 = 27,
    PiratesFight5 = 28,
    PiratesEnd = 29,
    TutorialLeave = 30
}