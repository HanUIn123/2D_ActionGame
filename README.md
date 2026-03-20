![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio%202022-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)

# Unity 2D Action Game (개인 프로젝트)

## ■ 개요
- 출시작 '타워 브레이커'의 코어 루프를 분석하여 Unity로 프로토타입 재현한 프로젝트입니다.
- 클라이언트 환경에서의 플레이 조작감, 타격감 연출, 그리고 '전투-보상-성장'으로 이어지는 핵심 게임 사이클 구현에 집중했습니다.

## ■ 개발 환경
- **언어:** C#
- **개발 도구:** Unity Engine, Visual Studio 2022
- **플랫폼:** PC (Windows Standalone) / Mobile (Android 지원 설계)

## ■ 시연 영상
[![2D Action Game 시연 영상](GameImage)](https://youtu.be/EKcBNb9Uebc)
*(이미지를 클릭하면 유튜브 시연 영상으로 이동합니다)*

## ■ 프로젝트 구조 및 주요 소스코드
<pre>
📂 <b>Assets</b>
└── 📂 <b>Scripts</b>
    └── 📂 <b>BattleScene</b>
        ├── 📂 <b>Managers</b>
        │   ├── <a href="./Assets/Scripts/BattleScene/Managers/GameDataManager.cs">GameDataManager.cs</a> (재화 및 스테이지 데이터 관리)
        │   └── <a href="./Assets/Scripts/BattleScene/Managers/FlockingManager.cs">FlockingManager.cs</a> (몬스터 군집 대열 중앙 제어 로직)
        ├── 📂 <b>Player</b>
        │   ├── <a href="./Assets/Scripts/BattleScene/Player/PlayerController.cs">PlayerController.cs</a> (입력 시스템 및 조작 처리)
        │   ├── <a href="./Assets/Scripts/BattleScene/Player/PlayerCombat.cs">PlayerCombat.cs</a> (전투 시퀀스 및 스킬 발사 제어)
        │   ├── <a href="./Assets/Scripts/BattleScene/Player/PlayerBladeSkill.cs">PlayerBladeSkill.cs</a> (관통 참격 스킬 메커니즘)
        │   ├── <a href="./Assets/Scripts/BattleScene/Player/PlayerAnimationEvent.cs">PlayerAnimationEvent.cs</a> (애니메이션 타이밍 이벤트 관리)
        │   └── <a href="./Assets/Scripts/BattleScene/Player/PlayerStats.cs">PlayerStats.cs</a> (능력치 데이터 및 동기화)
        ├── 📂 <b>Monster</b>
        │   ├── <a href="./Assets/Scripts/BattleScene/Monster/MonsterParent.cs">MonsterParent.cs</a> (공통 AI 및 피격/사망 추상 클래스)
        │   ├── <a href="./Assets/Scripts/BattleScene/Monster/Skeleton.cs">Skeleton.cs</a> (일반 근접 공격 패턴 구현)
        │   └── <a href="./Assets/Scripts/BattleScene/Monster/Goblin.cs">Goblin.cs</a> (몬스터 군집 버프 부여 패턴)
        └── 📂 <b>Boss</b>
            ├── <a href="./Assets/Scripts/BattleScene/Boss/BossSkeletonKing.cs">BossSkeletonKing.cs</a> (보스 전용 스킬 시퀀스)
            └── <a href="./Assets/Scripts/BattleScene/Boss/BossGoblinKing.cs">BossGoblinKing.cs</a> (방어 시스템 기믹 대응 AI)
</pre>

---

## ■ 주요 구현 기능

### 1. 군집 대열 시스템 (Flocking System)
- **중앙 제어 최적화:** 개별 몬스터가 각자 연산하지 않고 `FlockingManager`가 대열 전체의 이동량을 관리하여 연산 부하를 최소화했습니다.
- **대열 유지:** 수많은 몬스터가 등장해도 기차와 같은 대열을 일정 간격으로 유지하며 안정적으로 이동합니다.
- **후진 연출:** 플레이어가 밀려날 시 대열 전체 이동값을 코루틴으로 보간하여 유기적으로 후퇴하는 연출을 적용했습니다.

| 군집 대열 이동 (Movement) |
| :---: |
| <img src="./gifs/movement.gif" width="350px"> |

### 2. 플레이어 액션 및 성장 시스템
- **컴포넌트 기반 설계:** 조작(Controller), 전투(Combat), 스탯(Stats)으로 역할을 분리하여 유지보수성과 가독성을 높였습니다.
- **스킬 시스템:** 대열을 관통하는 **참격(Blade)**, 잔상을 남기는 **대쉬**, 체력을 회복하는 **힐** 등 3종의 스킬을 구현했습니다.
- **인벤토리 연동:** 습득한 장비를 실시간으로 장착/해제하고, 해당 데이터가 플레이어 스탯에 즉시 반영됩니다.

| 참격 스킬 (Blade) | 인벤토리/장비 (Inventory) |
| :---: | :---: |
| <img src="./gifs/blade.gif" width="280px"> | <img src="./gifs/inventory.gif" width="280px"> |

### 3. 보스전 및 패턴 기믹
- **상속 기반 AI:** `MonsterParent` 추상 클래스를 상속받아 코드 중복을 최소화하고 몬스터별 고유 패턴을 오버라이딩했습니다.
- **특수 기믹:** 특정 보스는 플레이어의 방어 시스템을 활용해 기절 상태로 만든 후에만 데미지를 입힐 수 있도록 설계했습니다.

| 보스 패턴 기믹 (Boss2) |
| :---: |
| <img src="./gifs/boss2.gif" width="350px"> |

### 4. 시각적 연출 및 스테이지 흐름
- **타격감 강화:** 피격 시 White 점멸 효과, 데미지 텍스트, 역경직(Hit Freeze)을 통해 생동감을 높였습니다.
- **스테이지 전환:** 층 전투 종료 시 화면 확대 연출(Clear) 및 다음 층으로 넘어가는 스무스한 스크롤링(Move)을 구현했습니다.

| 전투 종료 연출 (Clear) | 스테이지 이동 (Move) |
| :---: | :---: |
| <img src="./gifs/stageClearEffect.gif" width="280px"> | <img src="./gifs/stageMove.gif" width="280px"> |

---

## ■ 핵심 역량 요약
- **시스템 최적화:** 상대 좌표 기반 중앙 제어로 다수 개체 이동 연산 효율 극대화
- **객체 지향 설계:** 추상 클래스와 컴포넌트화를 통한 확장성 있는 코드 구조 구축
- **몰입감 있는 연출:** 코루틴 보간, Shader 활용(점멸), 애니메이션 이벤트 제어로 액션성 강화
