/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeSceneControl : MonoBehaviour
{
    public OVRTrackedKeyboard trackedKeyboard;
    public InputField StartingFocusField;
    public Text NameValue;
    public Text ConnectedValue;
    public Text StateValue;
    public Text TypeValue;
    public Color GoodStateColor = new Color(0.25f, 1, 0.25f, 1);
    public Color BadStateColor = new Color(1, 0.25f, 0.25f, 1);

    void Start()
    {
        StartingFocusField.Select();
        StartingFocusField.ActivateInputField();
    }

    void Update()
    {
        NameValue.text = trackedKeyboard.SystemKeyboardInfo.Name;
        ConnectedValue.text = ((bool)((trackedKeyboard.SystemKeyboardInfo.KeyboardFlags & OVRPlugin.TrackedKeyboardFlags.Connected) > 0)).ToString();
        StateValue.text = trackedKeyboard.TrackingState.ToString();
        TypeValue.text = trackedKeyboard.KeyboardQueryFlags.ToString();
        switch (trackedKeyboard.TrackingState)
        {
            case OVRTrackedKeyboard.TrackedKeyboardState.Uninitialized:
            case OVRTrackedKeyboard.TrackedKeyboardState.Error:
            case OVRTrackedKeyboard.TrackedKeyboardState.ErrorExtensionFailed:
            case OVRTrackedKeyboard.TrackedKeyboardState.StartedNotTracked:
            case OVRTrackedKeyboard.TrackedKeyboardState.Stale:
                StateValue.color = BadStateColor;
                break;
            default:
                StateValue.color = GoodStateColor;
                break;
        }

    }

    public void LoadBaselineScene()
    {
        SceneManager.LoadScene("Scenes/BaselineScene");
    }

    public void LoadVisualSupportScene()
    {
        SceneManager.LoadScene("Scenes/VisualSupportScene");
    }
}
