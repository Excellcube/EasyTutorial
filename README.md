# Easy Tutorial
Unity로 개발하는 게임의 튜토리얼을 쉽게 만들어줄 수 있는 Unity 패키지입니다. 대화창을 이용해 유저에게 메시지를 전달하는 `Dialog` 방식과 특정 액션의 수행을 유도하는 `Action` 방식으로 튜토리얼을 구현하는 기능을 제공합니다.

<img width="511" src="https://user-images.githubusercontent.com/104705295/198575165-c4feab4f-b1ca-43a2-b2e2-5a45fd93484e.gif"/>

## 주요 기능
* 대화창 형식의 Dialog
* 특정 버튼 혹은 행동을 유도하는 Action
* 튜토리얼 스킵
* 튜토리얼 클리어 결과 저장
* 문자열 지역화 모듈 지원

## Dependencies
Easy Tutorial은 다음과 같은 의존성을 가집니다.
* com.unity.textmeshpro
* com.coffee.unmask (https://github.com/mob-sakai/UnmaskForUGUI.git)

### com.coffee.unmask
특정 영역만 터치 가능하고 나머지 영역은 터치가 불가능한 unmasking을 가능하게 만들어주는 패키지입니다. 다음 링크를 통해 설치하실 수 있습니다.
* https://github.com/mob-sakai/UnmaskForUGUI#installation

## Installation
* To do

## Prerequisite
### UI 디자인
세부 디자인의 경우 사용자의 커스텀이 필요합니다. `EasyTutorial` prefab 하위의 `DialogTutorialPageView`와 `ActionTutorialPageView`를 직접 수정하여 원하는 디자인으로 변경합니다.

### TextMesh Pro 폰트
> 첨부된 TMPro 폰트 파일이 깨지거나 원하는 폰트가 별도로 존재하는 경우 아래와 같은 방법을 통해 TMPro를 할당해줍니다.

1. `Excellcube/EasyTutorial/Prefabs` 경로의 `EasyTutorial.prefab`을 Scene의 Canvas 하위에 추가합니다.

    <img width="663" src="https://user-images.githubusercontent.com/104705295/198577832-204828b9-204e-424a-bfb6-e83c2259bf1c.png"/>

1. TextMeshPro를 이용하여 원하는 폰트를 TMPro용으로 생성합니다.

1. Scene에 추가된 EasyTutorial내의 `TextMeshProGUI`를 찾아 원하는 TMPro 폰트로 교체합니다
<img width="1147" alt="스크린샷 2022-10-29 오전 12 39 35" src="https://user-images.githubusercontent.com/104705295/198677793-afbcd851-e0ee-4397-a60f-20b6e5b8f162.png">

## How to use


1. 구현하고자 하는 만큼 `TutorialPageData`를 추가합니다.

    <img width="511" src="https://user-images.githubusercontent.com/104705295/198594805-79ef5103-54da-4567-a66f-44e27c39c2ff.png"/>

1. 아래와 같이 원하는대로 `TutorialPageData`를 설정합니다.

    <img width="501" src="https://user-images.githubusercontent.com/104705295/198586982-ccbc0555-fcf0-4e07-9700-ae803bcb3e59.png"/>

* `Action`의 `Complete Key`는 다음과 같이 설정해줍니다.
    ```c#
    using Excellcube.EasyTutorial.Utils;

    // 버튼을 눌렀을 경우 아래 메서드 실행.
    public void PressTutorialButton()
    {
        TutorialEvent.Instance.Broadcast("TUTORIAL_BUTTON_02");
    }
    ```

1. 다음과 같이 튜토리얼이 완성 됐습니다!

    <img src="https://user-images.githubusercontent.com/104705295/198583565-3fb5e7c1-49ed-42d2-854b-5dd1daecb0d9.gif" width="50%"/>

## 세부 요소 설명
<img width="513" src="https://user-images.githubusercontent.com/104705295/198577827-b08adf93-9d1f-4bd3-82ec-3cbe8cb662a1.png">

* **Play On Awake** - 이 값이 `true`일 경우 현재 Scene이 시작됨과 동시에 튜토리얼을 시작하는지 여부를 설정합니다. `false`인 경우 스크립트에서 다음과 같이 호출을 함으로서 튜토리얼을 시작할 수 있습니다.
    ```c#
    public ECEasyTutorial m_EasyTutorial;

    /* ... */

    void MyMethod()
    {
        m_EasyTutorial.StartTutorial();
    }
    ```
* **Use Localization** - 이 값이 `true`일 경우 아래의 `Text Localizer`를 사용하여 문자열 지역화를 적용합니다. `false`일 경우에는 지역화를 사용하지 않습니다.
* **Text Localizer** - `Use Localization`가 `true`일 경우에 본 필드에 할당 된 Text Localizer를 이용하여 문자열 지역화를 수행합니다. `TextLocalizer` 클래스를 상속 받은 클래스의 인스턴스가 할당될 수 있습니다. Unity의 Localization 모듈을 사용하는 경우 아래와 같이 구현할 수 있습니다.
    ```c#
    using UnityEngine.Localization.Settings;

    namespace Excellcube.EasyTutorial.Utils {
        public class UnityTextLocalizer : TextLocalizer
        {
            public override string GetLocalizedText(string table, string key)
            {
                return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
            }
        }
    }
    ```
* **Localization Table** - (Optional) Unity Localization 모듈과 같이 별도의 테이블 이름이 필요한 경우 사용하는 필드입니다. 사용 중인 모듈이 테이블 정보를 사용하지 않는다면 공란으로 비워도 문제 없습니다.
* **Enable Skip Flag** - 디버깅 시 사용하는 skip flag를 사용할지 여부를 설정합니다. 이 값이 `true`일 경우, 튜토리얼 데이터의 skip 필드 값이 `true`인 튜토리얼 페이지를 건너뜁니다. 이 값이 `false`인 경우 튜토리얼 데이터의 skip 필드와 상관 없이 모든 튜토리얼 페이지를 사용합니다.
* **On Tutorial Skipped** - 튜토리얼 스킵 버튼을 눌렀을 때 이벤트를 지정합니다. 튜토리얼을 종료시키는 이벤트를 `UnityAction`의 형태로 매개변수로 넘깁니다. 이 값이 할당되지 않을 경우, 튜토리얼 스킵 버튼을 누를 시 즉시 튜토리얼이 종료 됩니다.
* **Clear tutorial completion flag** - 튜토리얼 종료 시 PlayerPref에 튜토리얼 종료 플래그가 설정됩니다. 이로 인해 다시 게임을 실행해도 튜토리얼이 출력되지 않습니다. 튜토리얼 종료 이후에 다시 튜토리얼을 보고 싶은 경우 본 버튼을 클릭합니다.

## 세부 요소 설명 (Tutorial Page Data)
### Tutorial Page Data (공통)
* **Skip** - 튜토리얼 진행 시 본 페이지를 건너뛸지 여부를 설정합니다.
* **Hide Skip Button** - 본 페이지에서 튜토리얼 스킵 버튼을 숨길지 여부를 설정합니다.
* **Start Delay** - 본 페이지를 시작할 때 설정된 시간(초)만큼 대기 후 페이지가 시작됩니다.
* **Block Touch During Delay** - 위에 설정된 대기 시간동안 사용자가 터치를 못하도록 막습니다
* **On Tutorial Begin** - 본 페이지가 시작될 때 실행되는 이벤트를 할당합니다.
* **On Tutorial Ended** - 본 페이지가 끝날 때 실행되는 이벤트를 할당합니다.

### Dialog Tutorial Page Data
<img width="501" src="https://user-images.githubusercontent.com/104705295/198597362-dcffd9e3-4d4c-487f-a24d-dc710d7ecd30.png">

* **Left Sprite**, **Right Sprite** - 대사창이 출력될 때 캐릭터 이미지가 어디에 출력될지 설정합니다. 둘 중 하나에 값을 할당하는 것은 물론이고, 두 필드 모두 비울 경우 대사창만 출력되고 두 필드 모두 채울 경우 Left Sprite의 캐릭터만 출력 됩니다.
* **Character Name** - 대사창이 출력될 때 캐릭터의 이름을 설정합니다. Use Localization이 true일 경우 이 필드는 사용되지 않습니다. 
* **Chracter Name Key** - 대사창이 출력될 때 사용될 캐릭터 이름의 지역화 코드를 설정합니다.
* **Dialog** - 대사창의 대사를 설정합니다. 
* **Dialog Key** - 대사창의 대사에 대한 지역화 코드를 설정합니다.


### Action Tutorial Page Data
<img width="501" src="https://user-images.githubusercontent.com/104705295/198597371-8ca14832-1ec6-4533-8cc4-75717ae8ed3c.png">

* **Action Log** - 사용자의 행동을 유도할 때 도움말 문구를 설정합니다. Use Localization이 true일 경우 이 필드는 사용되지 않습니다.
* **Action Log Key** - 사용자의 행동을 유도하는 도움말 문구에 대한 지역화 코드를 설정합니다.
* **Highlight Target** - 강조할 대상을 설정합니다. 주로 버튼을 할당합니다. 할당된 RectTransform의 size 영역 내부만 터치가 가능하고 그 외부 영역은 터치를 할 수 없게 됩니다.
* **Dynamic Target Root** - ListView의 cell과 같이 동적으로 할당되어 `Hightlight Taget`에 할당할 수 없을 경우 사용되는 필드입니다. 할당된 `Dynamic Target Root`의 children들을 모두 순회하며 `TutorialSelectionTarget` 컴포넌트를 모두 찾습니다. 찾아낸 `TutorialSelectionTarget` 중 아래의 `Dynamic Target Key`에 설정된 키값을 가진 오브젝트를 찾아 Highlight Target으로 사용합니다.
* **Dynamic Target Key** - 위의 설명과 같이 동적으로 생성되는 대상을 Highlight Target으로 사용할 때 본 필드에 값을 할당합니다. 
* **Indicator Position** - Highlight Target을 가리키는 화살표의 위치를 설정합니다.
* **Complete Key** - 본 튜토리얼 페이지의 클리어 조건을 알리는 이벤트의 키값을 설정합니다. 사용자가 버튼을 클릭하거나 특정 조건을 만족한 경우 다음과 같이 키 값을 전송하는 코드를 실행하면 튜토리얼 페이지의 클리어를 알릴 수 있습니다.
    ```c#
    using Excellcube.EasyTutorial.Utils;

    // 버튼을 눌렀을 경우 아래 메서드 실행.
    public void PressTutorialButton()
    {
        TutorialEvent.Instance.Broadcast("TUTORIAL_BUTTON_02");
    }
    ```


## License
* MIT

## External Resources

* 샘플 프로젝트 UI
  * [https://www.gameart2d.com/the-boy---free-sprites.html](https://www.gameart2d.com/the-boy---free-sprites.html)
  * [https://www.gameart2d.com/cute-girl-free-sprites.html](https://www.gameart2d.com/cute-girl-free-sprites.html)
  * [https://www.gameart2d.com/free-game-gui.html](https://www.gameart2d.com/free-game-gui.html)

* 샘플 프로젝트 폰트
  * 이 에셋에는 네이버에서 제공한 나눔글꼴이 적용되어 있습니다.
  * [https://help.naver.com/service/30016/contents/18088?osType=PC&lang=ko](https://help.naver.com/service/30016/contents/18088?osType=PC&lang=ko)

## Author
* QUVE ([Blog](https://quve.tistory.com/))
