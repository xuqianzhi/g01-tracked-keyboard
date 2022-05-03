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
        Double wpm = Math.Round(correctWordCount / interval * 60, 2);
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
            "naples", "stord", "gnaw", "lapmy", "oath", "saw", "fixed", "notein", "trust", "buys", "outking", "founts",
            "splits", "woo", "maker", "namebut", "scoured", "sayt", "offend", "choose", "ship", "dark", "passd",
            "cavil", "denayd", "others", "wheeli", "long", "wondred", "erst", "lordyou", "shown", "made", "whelps",
            "wooden", "ruld", "rascal", "putting", "dismayd", "graced", "injurd", "soulin", "bigger", "severed", "sat",
            "base", "highand", "thenand", "mould", "himis", "straw", "bondage", "woes", "crammd", "famous", "foolish",
            "surgeon", "replete", "sinyea", "blazing", "action", "powrful", "loppd", "scard", "foemans", "turret",
            "hearing", "sonwhom", "cull", "gone", "maine", "cousins", "surly", "wail", "life", "tears", "feature",
            "southam", "tookem", "glass", "eyethe", "twain", "rolling", "fearmy", "hapif", "bandy", "gulf", "chafed",
            "asaph", "hideous", "themthe", "item", "rogue", "toasted", "control", "mansay", "yearsof", "shadowi",
            "goodi", "borne", "smildst", "saycade", "babesi", "bend", "glud", "orphan", "singi", "easly", "risefor",
            "weeps", "wishes", "manhis", "relent", "hearfor", "standst", "mindfie", "ifields", "butby", "bodes",
            "however", "kinghis", "wolfso", "ledst", "spend", "hideand", "thissay", "manfew", "fables", "bells",
            "sinyet", "car", "whelp", "obscure", "hadeven", "endwas", "blows", "cradle", "landsis", "trainif",
            "thuscry", "simony", "meexit", "chokd", "heretic", "bedking", "comeour", "rooms", "flinty", "tidea",
            "grinfor", "roger", "heels", "author", "fails", "stephen", "drive", "yorks", "perfume", "mena", "slaveto",
            "itself", "boldto", "silent", "tobe", "muffled", "soeer", "bereave", "biting", "freeze", "bleeds", "handle",
            "bristol", "deedso", "keptas", "latei", "romes", "flatter", "doubt", "deign", "purge", "dick", "gallop",
            "smiling", "fever", "months", "breech", "thin", "flawand", "ensuing", "townby", "facethe", "former",
            "stain", "bird", "himi", "employd", "fightbe", "many", "riches", "spur", "brute", "lordand", "deedthe",
            "ascend", "revenge", "reapd", "spoild", "claimto", "propose", "sonsthy", "thinkst", "hanging", "sole",
            "northe", "heno", "slip", "kindle", "image", "thought", "malady", "liquid", "fortha", "glue", "stones",
            "loveo", "toyour", "takeby", "cross", "seasand", "bestdo", "tennis", "issueor", "kingyet", "methere",
            "emfor", "foxby", "poise", "act", "warmy", "sameput", "divided", "girdled", "theeuse", "itcome", "hale",
            "using", "waxen", "gatesto", "youold", "herefor", "needful", "romeor", "thema", "dainty", "coasti",
            "sirrahi", "signof", "gravity", "cryo", "deathso", "endas", "statei", "poisons", "dost", "mend", "stayif",
            "auntfor", "lessshe", "charged", "behind", "gallias", "cornif", "atand", "intends", "methis", "eacha",
            "held", "agony", "wretcho", "nightto", "browof", "lay", "saunder", "bluntly", "dad", "tied", "slainif",
            "bloodhe", "casei", "love", "heardif", "waitsas", "spit", "low", "contain", "cimber", "youtake", "laysand",
            "laws", "usherd", "mutiny", "tos", "bloodof", "sect", "beshut", "warwhat", "estate", "method", "smell",
            "eminto", "pate", "footand", "mought", "planet", "sonsee", "quoth", "bya", "killing", "soto", "dress",
            "helmets", "citizen", "gowith", "along", "onenor", "half", "isnot", "forlorn", "rueif", "notto", "fromt",
            "raind", "cover", "felt", "pawnd", "allfor", "watch", "denny", "faithto", "minesee", "doneyou", "vaunts",
            "civilst", "mehes", "lewdly", "doorson", "hop", "manly", "flew", "expulsd", "margery", "meever", "lead",
            "kissd", "cloud", "eveto", "proofof", "canvass", "foot", "subornd", "byas", "albeit", "lifeand", "ragetis",
            "saxton", "waves", "hermust", "upright", "flames", "byif", "foes", "none", "coward", "wordsor", "marchd",
            "manto", "nofrom", "denied", "nips", "foewith", "cannoti", "himking", "isthat", "valour", "cruel",
            "trowest", "unsayt", "notsay", "lifethe", "maidand", "attempt", "danger", "daggers", "can", "prelate",
            "perish", "race", "peacebe", "outi", "matewe", "peering", "knew", "fled", "country", "ranks", "pence",
            "madefor", "ofi", "uponuse", "earsfor", "wordsif", "plant", "coil", "goldthe", "theseif", "bias", "tread",
            "compact", "pooror", "enjoys", "lovenot", "downto", "follows", "lovdand", "dard", "sitsa", "waking",
            "cravd", "book", "stormsi", "chariot", "hail", "crystal", "andwhen", "slept", "infor", "daintry", "toqueen",
            "sith", "hairmy", "equity", "thou", "thyself", "all", "youhe", "kingly", "burns", "lies", "headbut", "sick",
            "nothow", "submit", "gripd", "scholar", "albans", "sight", "effect", "very", "teeth", "eyei", "air",
            "mineis", "notbe", "dogs", "outat", "wivesas", "pinewas", "dayin", "fagots", "gaols", "stateof", "fresh",
            "unfit", "iyet", "annoy", "counsel", "summers", "offwhen", "waft", "legate", "sparkle", "doi", "map", "ama",
            "amityo", "hence", "thrifty", "govern", "dials", "littlei", "thesame", "red", "expend", "gait", "owner",
            "allall", "seei", "money", "eldest", "aimto", "damnd", "whereof", "buckled", "sterile", "offor", "receipt",
            "testify", "pardons", "steado", "lewis", "conquer", "resign", "mender", "outface", "unite", "befalln",
            "glues", "townwe", "crave", "snow", "accent", "execute", "custom", "muchas", "kingsto", "cursbid", "madeit",
            "twelve", "upreard", "adopt", "onwhose", "christ", "should", "fieldmy", "headthe", "britain", "ofmy",
            "battry", "bucks", "swept", "advisd", "livery", "killd", "emby", "lordi", "swine", "mickle", "bush",
            "plies", "needit", "confine", "nameas", "bones", "restord", "sunders", "doom", "gomy", "henceas", "stood",
            "takst", "mass", "woman", "deadher", "fig", "wordsi", "drove", "pies", "ini", "doit", "appear", "saba",
            "dull", "herself", "demand", "wifes", "yethave", "powras", "prone", "page", "ladyi", "summa", "than",
            "will", "stagger", "cheeks", "walkd", "manmuch", "longas", "affect", "andsend", "yorkfor", "springs",
            "callsi", "fount", "dutyas", "dinner", "coast", "yetthe", "whichor", "muzzle", "portion", "mew", "rewards",
            "samson", "deemd", "thiso", "ghostas", "blushto", "bothfor", "wordson", "thunder", "heavnbe", "mingled",
            "park", "fawns", "torment", "enforce", "hewn", "inwe", "proper", "herone", "placemy", "bornand", "germans",
            "espials", "opeand", "earth", "ambut", "daring", "longto", "tilt", "childto", "titles", "win", "lordnot",
            "backi", "mended", "reasons", "bury", "noon", "lastly", "corrupt", "loseand", "courses", "cain", "ancient",
            "narrow", "flesh", "destroy", "unfolda", "seest", "madking", "groom", "lifei", "breathe", "rags", "wrinkle",
            "thatand", "tempera", "chain", "brazen", "feet", "hurries", "notshe", "thustis", "untoi", "lust", "regina",
            "towton", "virgina", "oaths", "ushave", "seawhat", "althaea", "amongst", "oddsa", "carei", "pard",
            "herholy", "usethou", "iron", "sinking", "eret", "wonboy", "oracle", "earthor", "youmost", "rackd",
            "pityno", "hopst", "tardy", "irish", "mountme", "honest", "cannot", "speaks", "burn", "rate", "rouena",
            "prolong", "justhe", "deserts", "regions", "againso", "headgo", "india", "about", "heartby", "bloodmy",
            "access", "intcade", "servd", "joinda", "abbots", "closd", "nell", "guard", "longand", "censure", "witand",
            "ruder", "cheerd", "jury", "deafbe", "withali", "beheld", "wast", "amazdat", "livdmy", "dastard", "withme",
            "ragged", "eating", "tackles", "rashly", "writs", "hours", "adage", "most", "landon", "kentish", "sideand",
            "atye", "flooda", "sweet", "william", "printed", "viceroy", "hurry", "hating", "orator", "battery",
            "langton", "thread", "frosty", "thorns", "sprung", "heryet", "grin", "flybut", "ramping", "buzzing",
            "require", "stayand", "handthy", "hoods", "breadi", "tothat", "feelto", "wasin", "canbut", "hiss", "ironan",
            "wedlock", "exitthe", "flyyork", "feast", "dally", "foe", "happen", "augment", "escapes", "enemies",
            "trueas", "cthe", "betters", "dangeri", "hates", "hellas", "toll", "theeas", "wayis", "decline", "eyessee",
            "himthou", "sweatof", "agues", "spyst", "sonsir", "moles", "couldmy", "ripened", "slay", "bishops",
            "horner", "justand", "room", "heartis", "niece", "blush", "mea", "bloodin", "excuses", "dieyork", "handof",
            "metal", "ayall", "beggars", "pry", "vows", "army", "aweless", "carders", "drawto", "kneein", "falls",
            "tigers", "sly", "liberty", "profit", "facewho", "cuphis", "error", "morei", "beguild", "usbut", "crew",
            "water", "fed", "domy", "shed", "donei", "reachof", "which", "garret", "fears", "betwixt", "vowthat",
            "worldly", "motley", "stabs", "cobbler", "seas", "herd", "prayer", "brooks", "memake", "tongue", "lurking",
            "thine", "megive", "madcap", "coronet", "bodyi", "nobly", "birth", "cloaks", "end", "awayour", "shotand",
            "crowds", "uselse", "bodyand", "yorkthe", "anearls", "furnace", "faiths", "kings", "clapthe", "wartis",
            "nocade", "bowold", "glance", "lawthe", "grow", "lordill", "homeby", "amisshe", "henryhe", "crazy",
            "belong", "toonot", "usage", "ground", "womband", "seato", "urged", "flung", "allnow", "careand", "mineit",
            "truths", "backor", "whereer", "usand", "heedyes", "field", "flameas", "wisdoms", "vainwho", "rabble",
            "wayand", "tideand", "sheathe", "heart", "drop", "arti", "havenow", "sing", "huberts", "forage", "rites",
            "someand", "theego", "everno", "animis", "bothmy", "tribute", "trow", "byall", "saythe", "proudly",
            "honoura", "mentis", "removd", "absolvd", "wanti", "changd", "endvaux", "wordis", "assault", "withawl",
            "goffea", "brown", "detaind", "having", "speakye", "pardon", "amain", "vowdto", "getlady", "rear",
            "smooths", "lease", "heara", "trees", "softest", "putrefy", "lionel", "shameto", "morn", "surrey",
            "michael", "seemly", "emyet", "troopi", "unruly", "himno", "woods", "music", "absence", "wifein", "tried",
            "thick", "mepray", "arise", "sircome", "pie", "faints", "arisemy", "quitei", "edge", "affects", "lack",
            "mockery", "bow", "homeand", "withi", "yorkbut", "eyesyet", "twolady", "madeand", "parish", "current",
            "pope", "overher", "adonis", "fertile", "yhave", "inand", "cut", "claims", "townsby", "brooded", "copy",
            "unloose", "black", "liefest", "night", "pauvres", "matters", "bankd", "nowto", "witsto", "minute", "wars",
            "rankd", "facei", "hag", "himmy", "fells", "staying", "ride", "aughtmy", "wound", "manage", "solemn",
            "youthat", "bushes", "captain", "dreamer", "marksi", "allhere", "poisond", "sleep", "sirhear", "called",
            "thisto", "handled", "clouted", "menacd", "mace", "court", "tumbled", "fallst", "rageand", "swear",
            "blushit", "health", "claimd", "overthe", "sightbe", "breed", "papal", "manis", "sung", "foxor", "feethis",
            "undoing", "defamd", "iden", "itthird", "shadow", "sent", "eyeshe", "iah", "les", "medcine", "meand",
            "sixteen", "deeper", "calm", "menpray", "baysell", "ireful", "blind", "maim", "supper", "roads", "flag",
            "laugher", "fall", "canst", "preface", "hardy", "values", "brandon", "lackd", "bid", "iithe", "fiddle",
            "priam", "nowthey", "onking", "shakes", "vital", "rung", "faceas", "parched", "xkent", "letthy", "craved",
            "shows", "smokes", "mademay", "taught", "size", "hopehe", "hasty", "harvest", "mostit", "savdah", "denial",
            "winters", "beamso", "umpire", "aloft", "inter", "oceanor", "labour", "raise", "whipt", "sack", "opendhe",
            "touch", "theesh", "outthat", "streets", "navytoo", "briefly", "certain", "keepers", "allayd", "noisome",
            "weepmy", "gods", "itanne", "study", "faciant", "ajax", "passion", "foeand", "neck", "swan", "iand",
            "honouro", "newcome", "deadthe", "namea", "eyesi", "public", "winged", "lend", "wordnor", "approvd",
            "joyto", "apt", "dread", "empire", "escaped", "pinchd", "meno", "rotten", "uswith", "tally", "writhis",
            "praisd", "finish", "yelping", "points", "flynow", "acts", "traitor", "greece", "sircade", "eyemen",
            "certes", "howso", "wildly", "peaceif", "uncivil", "holily", "yorkmy", "hawking", "advise", "goin",
            "dowries", "madamhe", "jewel", "promisd", "mayt", "leaves", "trouble", "impugns", "leaguei", "maybe",
            "newsand", "winddid", "course", "anger", "homefor", "claylet", "sawyer", "emi", "necks", "theei", "absurd",
            "better", "thishim", "bornto", "skill", "cuckold", "burst", "shrubs", "mancame", "barons", "womani", "mind",
            "setter", "bosoms", "supply", "crownd", "pleases", "crowni", "sawthe", "putst", "adders", "nob", "agree",
            "setbut", "thinks", "tug", "memphis", "errand", "losti", "minos", "emperor", "london", "deathor", "scribes",
            "nearand", "pityto", "keeps", "moreto", "houseas", "thank", "grieve", "termsas", "crowna", "stampd",
            "childs", "holding", "palaces", "pledges", "bewitch", "snard", "nowyour", "handto", "ask", "ons", "elders",
            "daythe", "ravish", "weavers", "mourn", "unsurd", "fin", "support", "toldthe", "iswhat", "tiber", "hinder",
            "cowards", "wreckas", "shout", "settled", "firefor", "wishing", "exitold", "dowry", "faroff", "leanand",
            "like", "bonahis", "survive", "falsei", "waveand", "worm", "pitchy", "seasthe", "gonei", "liberal",
            "clutch", "hounds", "have", "streams", "restyet", "sooner", "hell", "chasd", "soila", "ravens", "minutes",
            "begreat", "thirds", "mount", "theeah", "goodshe", "lives", "pinkd", "caution", "morebut", "melun",
            "lordsye", "falland", "madand", "painand", "armsif", "daythat", "proppd", "tide", "truthor", "leaveon",
            "bagsof", "satthe", "tush", "books", "rightby", "ashes", "triple", "thief", "toocome", "ere", "window",
            "wombif", "uphis", "diefor", "dothand", "ageand", "closer", "witches", "whiles", "earthi", "twit",
            "profits", "conveyd", "yeking", "heiri", "lucre", "rattle", "laid", "death", "rebels", "marks", "uponnow",
            "whip", "famish", "feigned", "broad", "charge", "choler", "goif", "twere", "frames", "slumber", "closet",
            "throws", "tires", "theefor", "diedmy", "sakein", "spy", "away", "tideto", "disdain", "ambush", "jot",
            "treat", "fury", "upmy", "shining", "awake", "drinks", "bad", "waythat", "asleep", "flatly", "upbraid",
            "iyork", "thisin", "severe", "willd", "mewhat", "memuch", "crack", "adieu", "widows", "drain", "battles",
            "sixth", "beating", "duchess", "sleek", "hubert", "dayand", "shriver", "sizeto", "usurper", "yielded",
            "angers", "itby", "townhis", "wholl", "move", "ear", "aboutby", "kissing", "rents", "dustand", "verb",
            "etext", "hallowd", "infants", "sethis", "north", "blew", "scale", "uponfor", "gladded", "obtaind",
            "hartin", "observe", "smack", "sardis", "disable", "manners", "purpose", "subdued", "meto", "isfirst",
            "convey", "crooked", "upis", "scarce", "unknown", "seewhen", "fearas", "sobut", "baiting", "age", "private",
            "him", "ending", "bushand", "hales", "seated", "preyand", "wishand", "lovei", "methen", "cloak", "smile",
            "oracles", "parthot", "cars", "wretchs", "hats", "peril", "later", "faults", "lordsif", "applied", "youof",
            "beaten", "hair", "tyrants", "throe", "ranst", "case", "cousin", "madammy", "robs", "collars", "toss",
            "middest", "glad", "dust", "iti", "envy", "intenti", "notsee", "sufferd", "roar", "baseand", "outcade",
            "masters", "scape", "resti", "thrill", "power", "modesty", "infixed", "whom", "sonhath", "gan", "aidand",
            "revengd", "talbots", "run", "whilemy", "tossd", "benefit", "gnaws", "wasand", "lodge", "basely", "fromthe",
            "workman", "pacein", "yclad", "bestand", "conduct", "withsir", "mindand", "theethy", "curseon", "rain",
            "dam", "sink", "modern", "armsof", "defile", "truein", "deny", "crest", "hard", "target", "cup", "vigour",
            "handsah", "mounted", "comfort", "perhaps", "housei", "onedick", "sidethe", "prayrs", "hearyou", "assurdi",
            "billows", "heads", "nowthis", "pitchd", "yorkwe", "illare", "leaps", "impose", "there", "hovers",
            "deathno", "want", "leaving", "ivernon", "barge", "date", "courts", "think", "avenge", "seemsas", "strings",
            "birds", "downmy", "topthe", "bona", "thembut", "humphry", "wellto", "shoreor", "five", "madam", "madami",
            "herthat", "brewd", "bitten", "cleft", "yei", "coatto", "clap", "pyramis", "queenan", "adding", "oerbear",
            "fellow", "train", "dismal", "draw", "returnd", "provoke", "witchd", "disturb", "inly", "sport", "pitythe",
            "treadhe", "anenemy", "tomyris", "thus", "met", "drew", "himbe", "furious", "manking", "onewhen", "usweret",
            "unmanly", "lieu", "kingdom", "unequal", "failone", "wasbut", "father", "dopray", "clangor", "ruler",
            "worldif", "makes", "dolphin", "lambthe", "twenty", "tentbut", "nations", "actthe", "upour", "knows",
            "amen", "woundso", "exceed", "slaves", "amainto", "younger", "hap", "casca", "redeem", "rust", "sloth",
            "ass", "shave", "breeder", "blast", "lame", "new", "outwith", "vilest", "himand", "armyork", "treadah",
            "lustful", "nourish", "meyea", "alencon", "complot", "bidst", "riddle", "earthis", "suburbs", "leftthe",
            "ghosts", "halting", "nothis", "growand", "cheers", "joves", "bides", "hadthe", "smoke", "hope", "lucky",
            "setting", "sandbag", "lizards", "isill", "tricks", "says", "pauls", "entredi", "comst", "beardi",
            "vizards", "yon", "fateto", "bearer", "imeddle", "hopeall", "midst", "device", "noble", "lists", "diecade",
            "fathera", "cannon", "entring", "arm", "mates", "youto", "hellfor", "marry", "hopesin", "hereshe", "hairs",
            "airthy", "alterd", "tongues", "dowhat", "pattern", "notboy", "risein", "wifemay", "repose", "unowed",
            "cityand", "joys", "itbutts", "memy", "amends", "worser", "misdeed", "needo", "honesty", "prevail",
            "romanos", "struck", "bemoand", "letteri", "smoketo", "ivbona", "thisfor", "shines", "yetbut", "agreed",
            "maiden", "heedwas", "oer", "whichis", "cracker", "person", "came", "askthan", "hug", "weali", "bullens",
            "goodwin", "falcons", "loath", "oft", "fitter", "sustain", "tomba", "coverd", "shutbut", "midday", "ageby",
            "welli", "latin", "suck", "trains", "lent", "guardwe", "landor", "yegive", "well", "wellfor", "whateer",
            "forceto", "losttis", "popes", "awful", "bide", "writing", "render", "sucking", "misgive", "walls",
            "spread", "hedgd", "trifle", "wealthy", "alarum", "wield", "erecta", "passbut", "revelld", "plants", "tall",
            "rightno", "degree", "fori", "headif", "vulture", "charm", "refusd", "itsir", "wratha", "gallery", "lesser",
            "knowhow", "youth", "spurns", "awayand", "other", "sunsfor", "thatno", "shipt", "booko", "anywe", "doorat",
            "gonerun", "proveth", "trade", "bloomd", "child", "losebut", "ways", "fool", "less", "upward", "pawns",
            "piece", "mehe", "roseas", "gaolers", "such", "vexd", "papers", "deface", "romemy", "pick", "cursd",
            "forked", "solewis", "pleased", "honours", "ominous", "venture", "peoples", "chooses", "riotous", "vauxwho",
            "thence", "carries", "kneei", "mirror", "lifebut", "mento", "ito", "pityif", "wrenby", "coldly", "furythe",
            "weakas", "fell", "food", "dim", "whirl", "toohave", "orlaid", "clime", "owen", "swainto", "sixthof", "rue",
            "writis", "careful", "wing", "safe", "boyhere", "inspird", "scales", "fares", "sorrowi", "wooer", "handwe",
            "pretend", "harry", "cassius", "plains", "timeof", "toohath", "grapple", "bothher", "bethe", "enoughi",
            "france", "boys", "willa", "dare", "example", "notthe", "apish", "yeevn", "axebut", "pour", "tearthe",
            "alongof", "slough", "silly", "kennel", "opinion", "barren", "large", "stamps", "etcthis", "bawd", "feard",
            "congee", "youand", "taper", "kingthe", "bended", "rumours", "seldom", "roses", "from", "alton", "unvexd",
            "its", "general", "midwife", "popego", "joan", "ownto", "fard", "grass", "warking", "carrion", "tothen",
            "odiousi", "cursei", "viands", "orphans", "defied", "doori", "viewand", "servile", "thighs", "endsfor",
            "draws", "sweeps", "circled", "shrewd", "betwo", "sothese", "homethe", "fearful", "wantst", "died",
            "whores", "loveto", "loose", "yearhow", "spies", "cityid", "strew", "via", "farst", "upthat", "lineal",
            "plain", "chaseth", "partbut", "earis", "fingers", "incaged", "lousy", "bores", "ipswich", "frenzy",
            "veins", "sofor", "realm", "uncle", "confer", "judgest", "howland", "seducd", "isa", "soout", "kneel",
            "tokeep", "hempen", "leaden", "title", "teach", "youths", "cheap", "haps", "suits", "secrecy", "snares",
            "beads", "heatto"
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
