using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextDisplayManagerBehavior : MonoBehaviour
{
    public GameObject cube;
    public InputField textInputField;
    public Text indicatorTextField;
    private String[] sentence;

    private int sentencePointer;

    // Start is called before the first frame update
    void Start()
    { 
        // activate input field
        textInputField.Select();
        textInputField.ActivateInputField();
        
        // initialize variables
        sentence = new[] { "i", "hope", "meta", "hire", "better", "software", "developer", "for", "the", "oculus", "team", "because", "the", "documentation", "literally", "sucks" };
        sentencePointer = 0;
        indicatorTextField.text = sentence[0];
    }

    // Update is called once per frame
    void Update()
    {
        String userInput = textInputField.text;
        WordInputState state = GetWordInputState(userInput);
        HandleProceedToNextWord(state);
        UpdateInputFieldColor(state);
    }

    void UpdateInputFieldColor(WordInputState state)
    {
        if (state == WordInputState.InCorrect)
        {
            // update text color to red
            textInputField.textComponent.color = Color.red;
        }
        else
        {
            // update text color to normal white
            textInputField.textComponent.color = Color.black;
        }
    }

    void HandleProceedToNextWord(WordInputState state)
    {
        if (state == WordInputState.SessionCompleted)
            return;
        Event currEvent = Event.current;
        if (state != WordInputState.Empty &&
            (currEvent.Equals(Event.KeyboardEvent("return")) || currEvent.Equals(Event.KeyboardEvent("space"))))
        {
            // update input field and indicator text field
            textInputField.text = "";
            sentencePointer += 1;
            if (sentencePointer >= sentence.Length)
            {
                indicatorTextField.text = "Session Completed!";
                indicatorTextField.color = Color.green;
            }
            else
            {
                indicatorTextField.text = sentence[sentencePointer];
            }
            
            // update UI, accuracy, and error rate according to input state
            if (state == WordInputState.Correct)
            {
                // TODO: update accuracy
            } else if (state == WordInputState.InCorrect || state == WordInputState.InProgress) {
                // TODO: update error rate
            }
        }
    }

    WordInputState GetWordInputState(String userInput)
    {
        if (sentencePointer >= sentence.Length)
            return WordInputState.SessionCompleted;
        if (userInput.Length == 0)
            return WordInputState.Empty;
        String target = sentence[sentencePointer];
        if (IsPrefix(target, userInput))
        {
            if (userInput.Length == target.Length)
            {
                return WordInputState.Correct;
            }
            else
            {
                return WordInputState.InProgress;
            }
        }
        else
        {
            return WordInputState.InCorrect;
        }
    }
    
    private enum WordInputState
    {
        Empty, // len(current user input) == 0 
        Correct, // current user input == target word
        InCorrect, // current user input != prefix of target word
        InProgress, // current user input == prefix of target word
        SessionCompleted // user typed all words in the sentence
    }
    

    Boolean IsPrefix(String longWord, String shortWord)
    {
        // check if short word is prefix of long word
        if (longWord.Length < shortWord.Length)
            return false;
        for (int i = 0; i < shortWord.Length; i++)
        {
            char c1 = longWord[i];
            char c2 = shortWord[i];
            if (!c1.Equals(c2))
                return false;
        }

        return true;
    }
}
