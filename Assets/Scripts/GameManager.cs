using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Order")]
    [SerializeField] private Bolt[] boltsInOrder;

    [Header("Timings")]
    [SerializeField] private float tutorialWaitSeconds = 1f;
    [SerializeField] private float badFeedbackSeconds = 0.5f;
    [SerializeField] private float goodFeedbackSeconds = 0.5f;

    private Bolt selectedBolt;

    private int boltIndex;
    private int currentStep;

    private bool canSelectBolts;

    [Header("UI")]
    [SerializeField]
    private GameObject WinCanvas;

    private void Start()
    {
        WinCanvas.SetActive(false);
        StartCoroutine(TutorialRoutine());
    }

    private void Update()
    {
        if (!canSelectBolts) return;

        HandleHover();
    }

    // -------------------- HOVER --------------------

    private void HandleHover()
    {
        ClearPreviousSelection();

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TrySelectBolt(hit);
        }
    }

    private void ClearPreviousSelection()
    {
        if (!selectedBolt) return;

        selectedBolt.ResetHighlight();
        selectedBolt = null;
    }

    private void TrySelectBolt(RaycastHit hit)
    {
        selectedBolt = hit.collider.GetComponent<Bolt>();

        if (!selectedBolt) return;

        selectedBolt.Highlight(Color.orange);
    }

    // -------------------- INPUT --------------------

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed || !selectedBolt || !canSelectBolts)
            return;

        canSelectBolts = false;

        bool isCorrect = IsCorrectBolt(selectedBolt);

        if (isCorrect)
        {
            HandleCorrectSelection();
        }
        else
        {
            HandleIncorrectSelection();
            StartCoroutine(BadFeedbackRoutine(selectedBolt));
        }

        selectedBolt = null;
    }

    private bool IsCorrectBolt(Bolt bolt)
    {
        return bolt == boltsInOrder[boltIndex];
    }

    private void HandleCorrectSelection()
    {
        Debug.Log("correct");
        StartCoroutine(GoodFeedbackRoutine(selectedBolt));

        ApplyStepAction(selectedBolt);

        boltIndex++;

        if (boltIndex >= boltsInOrder.Length)
        {
            boltIndex = 0;
            currentStep++;
        }

        Debug.Log($"Step {currentStep} completed");
    }
    private void ApplyStepAction(Bolt bolt)
    {
        if (currentStep == 0)
        {
            bolt.PlaceNut();
        }
        else if (currentStep == 1)
        {
            bolt.Screw();
        }
    }
    private void HandleIncorrectSelection()
    {
        StartCoroutine(BadFeedbackRoutine(selectedBolt));
    }

    // -------------------- GAME FLOW --------------------
    // is called from restart button
    public void Restart()
    {
        currentStep = 0;
        boltIndex = 0;

        foreach (Bolt bolt in boltsInOrder)
        {
            bolt.Reset();
        }

        WinCanvas.SetActive(false);
        
        StartCoroutine(TutorialRoutine());
    }
    private void Win()
    {
        canSelectBolts = false;
        WinCanvas.SetActive(true);
    }

    // -------------------- COROUTINES --------------------

    private IEnumerator TutorialRoutine()
    {
        canSelectBolts = false;

        foreach (Bolt bolt in boltsInOrder)
        {
            bolt.Highlight(Color.green);
            yield return new WaitForSeconds(tutorialWaitSeconds);
            bolt.ResetHighlight();
        }

        canSelectBolts = true;
    }

    private IEnumerator BadFeedbackRoutine(Bolt bolt)
    {
        bolt.Highlight(Color.red);

        yield return new WaitForSeconds(badFeedbackSeconds);

        bolt.ResetHighlight();

        Restart();
    }

    private IEnumerator GoodFeedbackRoutine(Bolt bolt)
    {
        bolt.Highlight(Color.green);

        yield return new WaitForSeconds(goodFeedbackSeconds);

        bolt.ResetHighlight();

        canSelectBolts = true;

        if(currentStep == 2)
        {
            Win();
        }
    }
}