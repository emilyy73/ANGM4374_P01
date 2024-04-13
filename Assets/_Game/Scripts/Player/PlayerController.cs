using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Animator animatorSilhouette;
    [SerializeField]
    private float speed;
    [SerializeField] [Range(1f, 10f)]
    private float speedMax = 8f;
    [SerializeField] [Range(0f, 5f)]
    private float acceleration = 0.01f;
    [SerializeField]
    [Range(0.0001f, 0.1f)]
    private float pixelsPerUnit = 0.01f;

    private Transform playerTransform; 

    private Vector2 touchScreenPosition;

    private string[] buttonNames =
    { 
        "UI_Up", 
        "UI_Down",
        "UI_Left", 
        "UI_Right" 
    };

    private Vector3[] buttonDirections =
    {
        new Vector3(0,0,1),     // up
        new Vector3(0,0,-1),    // down
        new Vector3(-1,0,0),     // left
        new Vector3(1,0,0)     // right
    };

    private List<Button> buttons = new List<Button>();
    private List<RectTransform> buttonsRects = new List<RectTransform>();

    private GameObject gameController;
    private GameController gameControllerScript;

    private void Awake()
    {
        gameController = GameObject.Find("GameController");

        gameControllerScript = gameController.GetComponent<GameController>();

        GameObject playerCharacterArt = GameObject.Find("playerCharacter");
        GameObject playerCharacterSilhouette = GameObject.Find("playerCharacterSilhouette");
        animator = playerCharacterArt.GetComponent<Animator>();
        animatorSilhouette = playerCharacterSilhouette.GetComponent<Animator>();

        foreach (string name in buttonNames)
        {
            Button but = GameObject.Find(name).GetComponent<Button>();
            buttonsRects.Add(but.GetComponent<RectTransform>());
        }

        playerTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        touchScreenPosition = gameControllerScript.TouchInput.TouchScreenPosition;

        Move();
    }

    private void Move()
    {
        bool isHeld = gameControllerScript.TouchInput.TouchIsHeld;

        Vector3 direction = Vector3.zero;
        for (int i = 0; i < buttonDirections.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(buttonsRects[i], touchScreenPosition) && isHeld)
            {
                direction = buttonDirections[i];
                animator.SetInteger("Face", i);
                animatorSilhouette.SetInteger("Face", i);
                break;
            }

        }

        if (direction != Vector3.zero)
        {
            speed += acceleration;
            speed = Mathf.Clamp(speed, 0f, speedMax);
            playerTransform.position += direction * speed * pixelsPerUnit;
        } 
        else
        {
            animator.SetInteger("Face", 4);
            animatorSilhouette.SetInteger("Face", 4);
            speed = 0;
        }
    }
}
