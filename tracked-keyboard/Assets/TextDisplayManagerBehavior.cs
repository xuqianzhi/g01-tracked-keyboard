using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = System.Random;

public class TextDisplayManagerBehavior : MonoBehaviour
{
    public InputField textInputField;
    public Text primaryLabel;
    public Text secondaryLabel;
    
    private string[] sentence;
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
        Event e = Event.current;
        if (e.Equals(Event.KeyboardEvent("return")))
        {
            sentence = getSentence(70);
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
        } else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            SceneManager.LoadScene("Scenes/HomeScene");
        }
    }
    
    void UserCompleteSession()
    {
        didSessionBegin = false;
        EventSystem.current.SetSelectedGameObject(null);
        textInputField.DeactivateInputField();
        
        timer.Stop();
        Double interval = timer.Elapsed.TotalSeconds;
        int letterCount = 0;
        foreach (string word in sentence)
        {
            letterCount += word.Length;
        }

        double wordCount = letterCount / 5;
        Double wpm = Math.Round(wordCount / (interval / 60 / 1000), 2);
        Double accuracy = Math.Round((Double)correctWordCount / sentence.Length, 4) * 100 ;
        primaryLabel.text = "Press ENTER for new session, or ESC for home!";
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

    String[] getSentence(int count)
    {
        string[] words = new string[]
        {
   "apples", "stored", "gnaw", "lap", "oath", "saw", "fixed", "note", "trust", "buys", "outing", "fountain",
   "splits", "woo", "maker", "name", "scoured", "said", "offend", "choose", "ship", "dark", "passed",
   "civil", "denied", "others", "wheel", "long", "wondered", "terse", "lord", "shown", "made", "overwhelm",
   "wooden", "rusty", "rascal", "putting", "dismayed", "graced", "injured", "soulful", "bigger", "severed", "sat",
   "base", "hiring", "themed", "moldy", "whimsy", "straw", "bondage", "woes", "crammed", "famous", "foolish",
   "surgeon", "replete", "synthesis", "blazing", "action", "powerful", "lopsided", "scared", "torment", "turret",
   "hearing", "woman", "cull", "gone", "maine", "cousins", "surly", "wail", "life", "tears", "feature",
   "southern", "taken", "glass", "eerie", "twisting", "rolling", "steamed", "happy", "brandy", "gulf", "chafed",
   "asphalt", "hideous", "thematic", "item", "rogue", "toasted", "control", "mainstay", "years", "shadowy",
   "goodbye", "birth", "smiled", "cascade", "babies", "bend", "glued", "orphan", "singing", "easily", "arise",
   "weeps", "wishes", "manhunt", "relent", "heard", "stand", "midwife", "fields", "bounty", "bodes",
   "however", "kingship", "wolves", "leader", "spend", "hide", "speak", "male", "fables", "bells",
   "human", "car", "accept", "obscure", "cadaver", "waste", "blows", "cradle", "landfill", "training",
   "thus", "symphony", "hoarse", "choked", "heretic", "exhaust", "feast", "rooms", "flint", "create",
   "house", "stabilize", "heels", "author", "fails", "stepped", "drive", "yolks", "perfume", "woman", "enslave",
   "itself", "brazen", "silent", "exist", "muffled", "spear", "bereave", "biting", "freeze", "bleeds", "handle",
   "pistol", "deeds", "capital", "lateness", "rummage", "flatter", "doubt", "decide", "purge", "deify", "gallop",
   "smiling", "fever", "months", "breech", "thin", "flavored", "ensuing", "townsfolk", "faces", "former",
   "stain", "bird", "animal", "employed", "fighter", "many", "riches", "spur", "brute", "leader", "dying",
   "ascend", "revenge", "reaped", "spoiled", "cilantro", "propose", "smithy", "thinks", "hanging", "sole",
   "northern", "hemisphere", "slip", "kindle", "image", "thought", "malady", "liquid", "fortify", "glue", "stones",
   "loved", "belong", "thieves", "cross", "season", "greatest", "tennis", "issue", "monarch", "present",
   "dessert", "entice", "poise", "act", "warmly", "suspect", "divided", "guided", "thesaurus", "entrance", "hail",
   "using", "candle", "gelato", "aging", "listen", "purpose", "scheme", "thirst", "dainty", "coasting",
   "spurred", "signal", "gravity", "crypto", "deaths", "ending", "state", "poisons", "mammal", "mend", "staying",
   "aunt", "lessen", "charged", "behind", "gallivant", "corny", "attend", "intends", "melted", "leach",
   "held", "agony", "wretched", "circle", "eyebrow", "lay", "saunter", "bluntly", "dad", "tied", "slain",
   "blood", "castle", "love", "heard", "waitress", "spit", "low", "contain", "cimber", "taken", "sand",
   "laws", "ushered", "mutiny", "beach", "tossed", "sect", "closing", "warfare", "estate", "method", "smell",
   "minty", "fresh", "foot", "drought", "planet", "invisible", "quote", "barely", "killing", "storied", "dress",
   "helmets", "citizen", "growth", "along", "tenor", "half", "snot", "forlorn", "ruler", "torn", "front",
   "rained", "cover", "felt", "pawned", "meadow", "watch", "desire", "faith", "owner", "justice", "haunts",
   "civilized", "mushy", "lewdly", "doorway", "hop", "manly", "flew", "expose", "imagery", "beaver", "lead",
   "kissed", "cloud", "velvet", "proof", "canvas", "foot", "subordinate", "beverage", "albeit", "life", "raged",
   "saxophone", "waves", "hermit", "upright", "flames", "frost", "foes", "none", "coward", "sword", "marched",
   "mantle", "conform", "denied", "pins", "foes", "cannot", "hiking", "identify", "valor", "cruel",
   "western", "remove", "refrain", "life", "maiden", "attempt", "danger", "daggers", "can", "proctor",
   "perish", "race", "peaceful", "outing", "maternal", "peering", "knew", "fled", "country", "ranks", "sixpence",
   "angry", "assess", "pause", "heard", "spoken", "plant", "coil", "golden", "sifted", "bias", "tread",
   "compact", "poorer", "enjoys", "despise", "ground", "follows", "lovely", "dowdy", "sitter", "waking",
   "craved", "book", "stormy", "chariot", "hail", "crystal", "when", "slept", "linked", "dainty", "queen",
   "siting", "enchant", "equity", "thousand", "thinker", "tall", "fall", "kingly", "burns", "lies", "headbutt", "sick",
   "instruct", "submit", "griped", "scholar", "albeit", "sight", "effect", "very", "teeth", "eye", "air",
   "mined", "oppose", "dogs", "treat", "wives", "pine", "dating", "fodder", "goals", "stately", "fresh",
   "unfit", "still", "annoy", "counsel", "summers", "often", "waft", "legal", "sparkle", "doing", "map", "amaze",
   "amiable", "dance", "thrifty", "govern", "dials", "littlest", "sesame", "red", "expend", "gait", "owner",
   "gather", "seeing", "money", "eldest", "strive", "demand", "where", "buckled", "sterile", "finish", "receipt",
   "testify", "pardons", "instead", "person", "conquer", "resign", "mended", "office", "unite", "befallen",
   "glues", "town", "crave", "snow", "accent", "execute", "custom", "plenty", "kings", "cursing", "complete",
   "twelve", "uproar", "adopt", "whose", "felon", "should", "field", "headed", "britain", "poser",
   "battery", "bucks", "swept", "advised", "livery", "killed", "embed", "exclaim", "swine", "fickle", "bush",
   "plier", "needle", "confine", "booked", "bones", "restored", "under", "doom", "gloomy", "hence", "stood",
   "tasked", "mass", "woman", "dying", "fig", "fling", "drove", "pies", "innate", "press", "appear", "satisfy",
   "dull", "herself", "demand", "wives", "undone", "powers", "prone", "page", "lady", "summer", "than",
   "will", "stagger", "cheeks", "walked", "mammoth", "long", "affect", "send", "fortify", "springs",
   "calls", "fountain", "meal", "dinner", "coast", "boat", "witch", "muzzle", "portion", "kitten", "rewards",
   "heroic", "deemed", "doomed", "ghost", "blush", "bother", "legacy", "thunder", "heaven", "mingled",
   "park", "fawns", "torment", "enforce", "beaten", "upright", "proper", "heroine", "placement", "born", "germans",
   "vials", "opened", "earth", "ambush", "daring", "longing", "tilt", "child", "titles", "win", "manor",
   "back", "mended", "reasons", "burly", "noon", "lastly", "corrupt", "loser", "courses", "cane", "ancient",
   "narrow", "flesh", "destroy", "unfold", "seen", "crazed", "groom", "lifeline", "breathe", "rags", "wrinkle",
   "hatch", "tempted", "chain", "brazen", "feet", "hurries", "shell", "thrust", "toils", "lust", "queen",
   "towed", "virginia", "oaths", "require", "sweat", "finger", "amongst", "odds", "caring", "pardon",
   "holy", "used", "iron", "sinking", "treat", "victor", "oracle", "earthly", "most", "racks",
   "pity", "hopper", "tardy", "irish", "mountain", "honest", "cannot", "speaks", "burn", "rate", "rouse",
   "prolong", "just", "deserts", "regions", "against", "head", "india", "about", "hearth", "bloody",
   "access", "indicate", "served", "joint", "rabbits", "closed", "kneel", "guard", "launder", "censure", "witty",
   "ruder", "cheer", "jury", "deaf", "withhold", "behold", "vast", "amazed", "lived", "dastardly", "meat",
   "ragged", "eating", "tackles", "rashly", "writs", "hours", "adage", "most", "london", "traffic", "side",
   "hate", "florida", "sweet", "william", "printed", "vices", "hurry", "hating", "orator", "battery",
   "slain", "thread", "frosty", "thorns", "sprung", "heard", "grin", "insect", "ramp", "buzzing",
   "require", "stay", "hand", "hoods", "breaded", "that", "feel", "felon", "ability", "hiss", "irony",
   "wedlock", "exit", "flying", "feast", "daily", "foe", "happen", "augment", "escapes", "enemies",
   "truce", "abduct", "betters", "danger", "hates", "hello", "toll", "trust", "waves", "decline", "glasses",
   "worked", "sweaty", "water", "trust", "heir", "moles", "could", "ripened", "slay", "bishops",
   "holler", "just", "room", "heart", "niece", "blush", "broom", "bloodied", "excuses", "fired", "handle",
   "metal", "deer", "beggars", "pry", "vows", "army", "lawless", "gardeners", "drawn", "kneeling", "falls",
   "tigers", "sly", "liberty", "profit", "facial", "couple", "error", "rebel", "beguiled", "stopped", "crew",
   "water", "fed", "dummy", "shed", "gilded", "reach", "which", "carrot", "fears", "between", "vowed",
   "worldly", "motley", "stabs", "cobbler", "seas", "herd", "prayer", "brooks", "make", "tongue", "lurking",
   "thine", "given", "hatter", "coronate", "body", "knobbly", "birth", "cloaks", "end", "away", "shot",
   "crowds", "useless", "body", "lawful", "pearls", "furnace", "faiths", "kings", "clap", "perils",
   "brocade", "bold", "glance", "latent", "grow", "roadkill", "homely", "missed", "henry", "crazy",
   "belong", "toonot", "usage", "ground", "womb", "seat", "urged", "flung", "current", "caring", "mine",
   "truths", "back", "wherever", "fled", "heed", "field", "flames", "wisdoms", "vain", "rabble",
   "way", "tide", "sheath", "heart", "drop", "artist", "haven", "sing", "trumpet", "forage", "rites",
   "someand", "learner", "ever", "animal", "botch", "tribute", "towel", "ball", "align", "proudly",
   "honors", "mentor", "removed", "absolved", "wanting", "changed", "endeavor", "author", "assault", "withdrawal",
   "coffee", "brown", "detained", "having", "speak", "pardon", "ascend", "vowed", "marry", "rear",
   "smoothed", "lease", "hear", "trees", "softest", "putrefy", "lion", "shame", "morning", "surreal",
   "michael", "seemly", "truth", "troops", "unruly", "male", "woods", "music", "absence", "wife", "tried",
   "thick", "spray", "arise", "respect", "pie", "faints", "arise", "quiet", "edge", "affects", "lack",
   "mockery", "bow", "homeland", "within", "yorkshire", "eyes", "lady", "made", "parish", "current",
   "pope", "overhear", "handsome", "fertile", "have", "inland", "cut", "claims", "towns", "brooded", "copy",
   "loose", "black", "liars", "night", "prefer", "matters", "banked", "tense", "wits", "minute", "wars",
   "ranked", "facing", "hag", "jimmy", "felled", "staying", "ride", "naughty", "wound", "manage", "solemn",
   "whence", "bushes", "captain", "dreamer", "marks", "present", "poisoned", "sleep", "heard", "called",
   "thistle", "handled", "clout", "menaced", "mace", "court", "tumbled", "fell", "rage", "swear",
   "blushed", "health", "claimed", "overt", "sight", "breed", "pupil", "mantis", "sung", "foxy", "feet",
   "undoing", "defamed", "widen", "third", "shadow", "sent", "eyes", "pariah", "lest", "medicine", "meant",
   "sixteen", "deeper", "calm", "pray", "sell", "irate", "blind", "maim", "supper", "roads", "flag",
   "laughter", "fall", "canned", "preface", "hardy", "values", "brandish", "lacked", "bid", "lithe", "fiddle",
   "prim", "they", "honking", "shakes", "vital", "rung", "faces", "parched", "expense", "wherefore", "craved",
   "shows", "smokes", "dismay", "taught", "size", "hoped", "hasty", "harvest", "most", "frilly", "denial",
   "winters", "beams", "umpire", "aloft", "splinter", "ocean", "labor", "raise", "whipped", "sack", "opened",
   "touch", "geese", "outback", "streets", "navy", "briefly", "certain", "keepers", "allayed", "noise",
   "weeping", "gods", "witty", "study", "facial", "sizes", "passion", "foes", "neck", "swan", "land",
   "honor", "newborn", "dead", "name", "eyes", "public", "winged", "lend", "word", "approved",
   "joy", "apt", "dread", "empire", "escaped", "pinched", "memo", "rotten", "with", "tally", "writhed",
   "praised", "finish", "yelping", "points", "flight", "acts", "traitor", "grease", "cascade", "blast",
   "ocean", "pack", "wildly", "peace", "uncivil", "holiday", "note", "hawking", "advise", "going",
   "dowry", "boxes", "jewel", "promised", "maybe", "leaves", "trouble", "impugn", "league", "maybe",
   "news", "winded", "course", "anger", "home", "clay", "saws", "lemming", "necks", "their", "absurd",
   "better", "this", "born", "skill", "cuckold", "burst", "shrubs", "macaw", "barons", "woman", "mind",
   "setter", "pitch", "supply", "crowned", "pleases", "crown", "dusty", "input", "adder", "knob", "agree",
   "setback", "thinks", "tug", "memo", "errand", "lost", "swindle", "emperor", "london", "death", "scribes",
   "near", "pity", "keeps", "addition", "house", "thank", "grieve", "terms", "crow", "stamped",
   "child", "holding", "palaces", "pledges", "bewitch", "snare", "wealth", "hand", "ask", "onus", "elders",
   "daily", "ravish", "weavers", "mourn", "insured", "film", "support", "told", "what", "timber", "hinder",
   "cowards", "wreck", "shout", "settled", "fire", "wishing", "exit", "wish", "faraway", "lean",
   "like", "bone", "survive", "false", "wave", "worm", "pitchy", "seas", "gone", "liberal",
   "clutch", "hounds", "have", "streams", "rested", "sooner", "hell", "chased", "done", "ravens", "minutes",
   "great", "thirds", "mount", "heal", "goods", "lives", "pink", "caution", "more", "melon",
   "lords", "fall", "madman", "pained", "elbow", "daily", "propped", "tide", "truth", "leave",
   "bags", "sat", "bottom", "books", "rightly", "ashes", "triple", "thief", "toothy", "beware", "window",
   "womb", "upholster", "die", "dotted", "aged", "closer", "witches", "while", "earth", "twitter",
   "profits", "conveyed", "early", "hired", "lucrative", "rattle", "laid", "death", "rebels", "marks", "upon",
   "whip", "famish", "feigned", "broad", "charge", "holler", "golf", "twinkle", "frames", "slumber", "closet",
   "throws", "tires", "therefore", "died", "sake", "spy", "away", "tide", "disdain", "ambush", "jolt",
   "treat", "fury", "lumpy", "shining", "awake", "drinks", "bad", "wait", "asleep", "flatly", "braid",
   "spoon", "thesis", "severe", "wild", "matches", "fertile", "crack", "dirty", "widows", "drain", "battles",
   "sixth", "beating", "duchess", "sleek", "humor", "day", "shiver", "sized", "usurper", "yielded",
   "angers", "bitten", "township", "whole", "move", "ear", "about", "kissing", "rents", "dusty", "verb",
   "text", "hallowed", "infants", "seethe", "north", "blew", "scale", "upon", "gladden", "obtained",
   "hearth", "observe", "smack", "sardine", "disable", "manners", "purpose", "subdued", "meteor", "first",
   "convey", "crooked", "upside", "scarce", "unknown", "when", "fearful", "scout", "baiting", "age", "private",
   "him", "ending", "husband", "hails", "seated", "prey", "wished", "lover", "appoint", "cloak", "smile",
   "oracles", "party", "cars", "wretches", "hats", "peril", "later", "faults", "lords", "applied", "youth",
   "beaten", "hair", "tyrants", "thrown", "rants", "case", "cousin", "madame", "robs", "collars", "toss",
   "middle", "glad", "dust", "iterate", "envy", "intention", "blind", "suffered", "roar", "based", "concave",
   "masters", "escape", "resting", "thrill", "power", "modesty", "infixed", "whom", "hath", "garden", "aided",
   "revenge", "robots", "run", "while", "tossed", "benefit", "gnaws", "works", "lodge", "basely", "from",
   "worker", "paces", "clad", "best", "conduct", "within", "mind", "teeth", "cursor", "rain",
   "dam", "sink", "modern", "arms", "defile", "true", "deny", "crest", "hard", "target", "cup", "vigor",
   "handy", "mounted", "comfort", "perhaps", "house", "one", "side", "prayers", "hear", "assured",
   "billows", "heads", "now", "pitched", "fork", "ill", "leaps", "impose", "there", "hovers",
   "wreath", "want", "leaving", "liver", "barge", "date", "courts", "think", "avenge", "seems", "strings",
   "birds", "downmy", "top", "liver", "them", "dry", "well", "shore", "five", "six", "seven",
   "heart", "brewed", "bitten", "cleft", "soul", "coat", "clap", "pyramid", "queen", "adding", "overbear",
   "fellow", "train", "dismal", "draw", "returned", "provoke", "bewitched", "disturb", "only", "sport", "pity",
   "tread", "enemy", "wrist", "thus", "met", "drew", "thimble", "furious", "making", "when", "sweat",
   "unmanly", "lieu", "kingdom", "unequal", "fail", "but", "father", "pray", "clangor", "ruler",
   "world", "makes", "dolphin", "lamb", "twenty", "tent", "nations", "act", "uproar", "knows",
   "amen", "wounds", "exceed", "slaves", "meant", "younger", "chap", "axe", "redeem", "rust", "sloth",
   "brass", "shave", "bread", "blast", "lame", "new", "with", "vilest", "cloth", "army", "tread",
   "lustful", "nourish", "meal", "alcove", "plot", "compile", "riddle", "earth", "suburbs", "the",
   "ghosts", "halting", "this", "grow", "cheers", "cloth", "bide", "time", "smoke", "hope", "lucky",
   "setting", "sandbag", "lizards", "swill", "tricks", "says", "paltry", "entreat", "commit", "beard",
   "wizards", "yonder", "fate", "bearer", "meddle", "hope", "midst", "device", "noble", "lists", "decade",
   "father", "cannon", "entering", "arm", "mates", "your", "hell", "marry", "hopes", "here", "hairs",
   "earthy", "altered", "tongues", "what", "pattern", "bobble", "risen", "wife", "repose", "unowed",
   "city", "joys", "putter", "many", "amends", "worsen", "misdeed", "needy", "honesty", "prevail",
   "romantic", "struck", "bemoaned", "letter", "smoke", "words", "thisfor", "shines", "yet", "agreed",
   "maiden", "heed", "over", "which", "cracker", "person", "came", "than", "hug", "welt", "bullish",
   "goodwill", "falcons", "loath", "oft", "fitter", "sustain", "tomb", "covered", "shut", "midday", "agency",
   "welcome", "latin", "suck", "trains", "lent", "guard", "landlord", "give", "spelt", "barley", "whatever",
   "force", "lost", "oats", "awful", "bide", "writing", "render", "sucking", "misgive", "walls",
   "spread", "hedged", "trifle", "wheat", "alarm", "wield", "rye", "pass", "reveled", "plants", "tall",
   "right", "degree", "fortify", "head", "vulture", "charm", "refused", "itself", "wrath", "gallery", "lesser",
   "know", "youth", "spurns", "away", "other", "suns", "that", "ship", "book", "anyway", "doormat",
   "gone", "proven", "trade", "bloomed", "child", "loss", "ways", "fool", "less", "upward", "pawns",
   "piece", "monkey", "redness", "galore", "such", "vexed", "papers", "deface", "romance", "pick", "cursed",
   "forked", "solid", "pleased", "honors", "ominous", "venture", "peoples", "chooses", "riotous", "vexed",
   "fence", "carries", "kneel", "mirror", "life", "memento", "bitten", "pity", "worn", "coldly", "fury",
   "weakness", "fell", "food", "dim", "whirl", "behave", "laid", "climb", "owed", "swore", "sixth", "ruthless",
   "writer", "careful", "wing", "safe", "boy", "inspired", "scales", "fares", "sorrow", "worker", "hand",
   "pretend", "harry", "castle", "plains", "timed", "chocolate", "grapple", "bother", "bent", "enough",
   "france", "boys", "will", "dare", "example", "not", "ape", "shear", "but", "pour", "tear",
   "along", "slough", "silly", "kennel", "opinion", "barren", "large", "stamps", "this", "bawled", "feared",
   "convoke", "and", "taper", "you", "bend", "rumors", "seldom", "roses", "from", "stilted", "vexed",
   "wits", "general", "midwife", "ego", "loan", "own", "ford", "grass", "barking", "carrion", "then",
   "odious", "cursing", "villian", "orphans", "defied", "door", "view", "servile", "thighs", "ends",
   "draws", "sweeps", "circled", "shrewd", "betting", "these", "home", "fearful", "wants", "died",
   "lady", "love", "loose", "year", "spies", "city", "strewn", "via", "farce", "that", "linear",
   "plain", "chased", "but", "earring", "fingers", "caged", "lousy", "boredom", "places", "frenzy",
   "veins", "for", "realm", "uncle", "confer", "judging", "landing", "grains", "vista", "soot", "kneel",
   "tokeep", "temporary", "leaden", "title", "teach", "youths", "cheap", "happens", "suits", "secrecy", "snares",
   "beads", "heat"
};


        Random rnd = new Random();
        string[] result = new string[count];
        for (int i = 0; i < count; i++)
        {
            int index = rnd.Next(words.Length);
            result[i] = words[index];
        }

        return result;
    }
}
