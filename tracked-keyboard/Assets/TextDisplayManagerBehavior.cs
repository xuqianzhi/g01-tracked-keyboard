using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextDisplayManagerBehavior : MonoBehaviour
{
    public InputField textInputField;
    public Text primaryLabel;
    public Text secondaryLabel;
    
    private String[] sentence;
    private int wordPointer;
    private Boolean didSessionBegin;
    
    private Stopwatch timer;
    private int correctWordCount;

    // Start is called before the first frame update
    void Start()
    {
        // deactivate input field until user begin session
        textInputField.DeactivateInputField();
        
        // initialize variables
        didSessionBegin = false;
        sentence = new[] { "hello", "world", "hello", "world", };
        wordPointer = 0;
        timer = new Stopwatch();
        correctWordCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!didSessionBegin)
        {
            UserBeginSession();
            return;
        }
        String userInput = textInputField.text;
        userInput = userInput.Replace(" ", String.Empty);
        WordInputState state = GetWordInputState(userInput);
        HandleProceedToNextWord(state);
        UpdateInputFieldColor(state);
    }

    void UserBeginSession()
    {
        if (Event.current.Equals(Event.KeyboardEvent("return")))
        {
            // TODO: reset sentence
            textInputField.Select();
            textInputField.ActivateInputField();
            wordPointer = 0;
            correctWordCount = 0;
            didSessionBegin = true;
            primaryLabel.color = Color.black;
            primaryLabel.text = sentence[0];
            secondaryLabel.text = String.Empty;
            
            timer.Reset();
            timer.Start();
        }
    }
    
    void UserCompleteSession()
    {
        didSessionBegin = false;
        EventSystem.current.SetSelectedGameObject(null);
        textInputField.DeactivateInputField();
        
        timer.Stop();
        Double interval = timer.Elapsed.TotalSeconds;
        Double wpm = Math.Round(correctWordCount / interval * 60, 2);
        Double accuracy = Math.Round((Double)correctWordCount / sentence.Length, 4) * 100 ;
        primaryLabel.text = "Press enter for a new session!";
        primaryLabel.color = Color.green;
        secondaryLabel.text = $"wpm: {wpm}, accuracy: {accuracy}%";
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
            // update text color to normal black
            textInputField.textComponent.color = Color.black;
        }
    }

    void HandleProceedToNextWord(WordInputState state)
    {
        if (state == WordInputState.SessionCompleted) return;
        
        Event currEvent = Event.current;
        if (state != WordInputState.Empty && currEvent.Equals(Event.KeyboardEvent("space")))
        {
            // user finished typing the current word
            
            // update accurate word count
            if (state == WordInputState.Correct)
            {
                // update correct word count
                correctWordCount += 1;
            }
            
            // clear input field and increment word pointer
            textInputField.text = "";
            wordPointer += 1;
            if (wordPointer >= sentence.Length)
            {
                // session completed
                UserCompleteSession();
            }
            else
            {
                // proceed to next word
                primaryLabel.text = sentence[wordPointer];
            }
        }
    }

    WordInputState GetWordInputState(String userInput)
    {
        if (wordPointer >= sentence.Length)
            return WordInputState.SessionCompleted;
        if (userInput.Length == 0)
            return WordInputState.Empty;
        String target = sentence[wordPointer];
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
