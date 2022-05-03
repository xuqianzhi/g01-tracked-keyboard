import { Component, createRef } from "react";
import {
  TextField,
  Typography,
  Stack,
  Button,
  ThemeProvider,
} from "@mui/material";
import { alpha, styled } from "@mui/material/styles";
import { createTheme } from "@mui/material/styles";
import words from './words.js';

const theme = createTheme({
  palette: {
    primary: {
      main: "#414DBB",
    },
    secondary: {
      main: "#21C166",
    },
    background: {
      main: "#F2EFE7",
    },
  },
});

const ButtonStyled = styled(Button)(({ theme }) => ({
  color: "white",
  margin: 8,
  width: "180px",
  height: "60px",
  borderRadius: "45px",
  background: theme.palette.primary.main,
  "&:hover": {
    background: theme.palette.secondary.main,
  },
  textTransform: "none",
  fontSize: "17px",
}));

class App extends Component {
  constructor() {
    super();
    this.state = {
      wordPointer: 0,
      didSessionBegin: false,
      startTime: null,
      correctCount: 0,
      primaryLabel: "Press START to begin session",
      secondaryLabel: "",
    };
    this.inputField = createRef();
    this.userBeginSession = this.userBeginSession.bind(this);
    this.userCompleteSession = this.userCompleteSession.bind(this);
    this.handleKeyDown = this.handleKeyDown.bind(this);
    this.onTextFieldChange = this.onTextFieldChange.bind(this);
  }

  userBeginSession() {
    const generateSentence = () => {
      const result = [];
      for (let i = 0; i < 70; i++) {
        result.push(words[Math.floor(Math.random() * words.length)]);
      }
      return result;
    };
    const currSentence = generateSentence();
    this.currSentence = currSentence;
    this.setState({
      wordPointer: 0,
      correctCount: 0,
      didSessionBegin: true,
      startTime: new Date(),
      primaryLabel: currSentence[0],
      secondaryLabel: "",
      inputStatus: "success",
    }, () => {
      this.inputField.current.focus();
    });
  }

  userCompleteSession() {
    this.setState({
      didSessionBegin: false,
    });
  }

  handleKeyDown({ keyCode, key }) {
    if (keyCode === 32) {
      // space bar
      let userInput = this.inputField.current.value;
      userInput = userInput.replace(/\s/g, '');
      this.inputField.current.value = "";
      let { wordPointer, correctCount } = this.state;
      const currSentence = this.currSentence;
      const target = currSentence[wordPointer];
      if (userInput === target) {
        console.log("correct");
        correctCount += 1;
      }
      if (wordPointer + 1 < currSentence.length) {
        // session continue
        this.setState({
          primaryLabel: currSentence[wordPointer + 1],
          wordPointer: wordPointer + 1,
          correctCount: correctCount,
          inputStatus: "success"
        });
      } else {
        // session ended
        const startTime = this.state.startTime;
        const endTime = new Date();
        const interval = endTime - startTime; //in ms
        let numChar = 0;
        for (const word of currSentence) {
          numChar += word.length;
        }
        const wpm = ((numChar / 5) / (interval / 1000 / 60)).toFixed(2);
        const accuracy = (correctCount / currSentence.length).toFixed(2) * 100;
        const secondaryLabel = `wpm: ${wpm}, accuracy: ${accuracy}%`
        this.setState({ didSessionBegin: false, primaryLabel: "Press START to restart", secondaryLabel: secondaryLabel });
      }
    }
  }

  onTextFieldChange(input) {
    const isPrefix = (shortWord, longWord) => {
      // check if short word is prefix of long word
      if (longWord.length < shortWord.length) {
        return false;
      }
      for (let i = 0; i < shortWord.length; i++) {
        const c1 = longWord.charAt(i);
        const c2 = shortWord.charAt(i);
        if (c1 != c2) {
          return false;
        }
      }
      return true;
    };
    input = input.replace(/\s/g, '');

    const currSentence = this.currSentence;
    const wordPointer = this.state.wordPointer;
    const target = currSentence[wordPointer];
    console.log(input, target);

    if (isPrefix(input, target)) {
      this.setState({ inputStatus: "success" });
    } else {
      this.setState({ inputStatus: "error" });
    }
  }

  render() {
    const {
      wordPointer,
      didSessionBegin,
      startTime,
      correctCount,
      primaryLabel,
      secondaryLabel,
      inputStatus
    } = this.state;
    return (
      <ThemeProvider theme={theme}>
        <div
          style={{
            display: "flex",
            flexDirection: "row",
            justifyContent: "center",
          }}
        >
          <Stack
            sx={{
              width: "50vw",
              height: "80vh",
              direction: "column",
              alignItems: "center",
              marginTop: "300px",
            }}
          >
            <Typography variant="h4" margin={"30px"} marginBottom="0px">
              {primaryLabel}
            </Typography>
            <Typography variant="h4" margin={"30px"}>
              {secondaryLabel}
            </Typography>
            <TextField
              color={inputStatus}
              fullWidth
              variant="filled"
              rows={1}
              onKeyUp={this.handleKeyDown}
              disabled={!didSessionBegin}
              inputRef={this.inputField}
              onChange={(e) => this.onTextFieldChange(e.target.value)}
            />
            <ButtonStyled
              disabled={didSessionBegin}
              onClick={this.userBeginSession}
              sx={{ marginTop: '50px' }}
            >
              Start
            </ButtonStyled>
          </Stack>
        </div>
      </ThemeProvider>
    );
  }
}

export default App;
