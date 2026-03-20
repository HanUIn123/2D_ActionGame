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
<p align="center">
  <a href="https://youtu.be/EKcBNb9Uebc">
    <img src="./GameImage.png" width="50%" alt="2D Action Game 시연 영상">
  </a>
  <br>
  <b>(이미지를 클릭하면 유튜브 시연 영상으로 이동합니다)</b>
</p>

## ■ 프로젝트 구조 및 주요 소스코드
<pre>
📂 <b>Assets</b>
└── 📂 <b>Scripts</b>
    ├── 📂 <b>LogoScene</b>
    │   └── <a href="./Assets/Scripts/LogoScene/GameDataManager.cs">GameDataManager.cs</a> (싱글톤 기반 전역 데이터 및 재화 관리)
    ├── 📂 <b>MainScene</b>
    │   └── 📂 <b>Player</b>
    │       ├── <a href="./Assets/Scripts/MainScene/Player/InventoryManager.cs">InventoryManager.cs</a> (인벤토리 시스템 및 아이템 장착 로직)
    │       └── <a href="./Assets/Scripts/MainScene/Player/EquipManager.cs">EquipManager.cs</a> (장비 데이터와 플레이어 스탯 연동 제어)
    └── 📂 <b>BattleScene</b>
        ├── 📂 <b>Managers</b>
        │   └── <a href="./Assets/Scripts/BattleScene/Managers/BattleManager.cs">BattleManager.cs</a> (전투 시퀀스 및 스테이지 흐름 관리)
        ├── 📂 <b>Player</b>
        │   ├── <a href="./Assets/Scripts/BattleScene/Player/PlayerController.cs">PlayerController.cs</a> (전투 조작 및 이동 입력 처리)
        │   ├── <a href="./Assets/Scripts/BattleScene/Player/PlayerCombat.cs">PlayerCombat.cs</a> (공격/방어 및 스킬 발사 시퀀스)
        │   └── <a href="./Assets/Scripts/BattleScene/Player/PlayerStats.cs">PlayerStats.cs</a> (실시간 전투 스탯 및 체력 관리)
        ├── 📂 <b>Monster</b>
        │   ├── <a href="./Assets/Scripts/BattleScene/Monster/MonsterParent.cs">MonsterParent.cs</a> (공통 AI 및 피격/사망 추상 클래스)
        │   ├── <a href="./Assets/Scripts/BattleScene/Monster/FlockingManager.cs">FlockingManager.cs</a> (몬스터 군집 대열 중앙 제어 시스템)
        │   ├── 📂 <b>MonsterSkeleton</b>
        │   │   └── <a href="./Assets/Scripts/BattleScene/Monster/MonsterSkeleton/Skeleton.cs">Skeleton.cs</a> (일반 몬스터 AI 패턴 구현)
        │   └── 📂 <b>MonsterGoblin</b>
        │       └── <a href="./Assets/Scripts/BattleScene/Monster/MonsterGoblin/Goblin.cs">Goblin.cs</a> (군집 버프 부여 특수 패턴 구현)
        └── 📂 <b>Boss</b>
            ├── <a href="./Assets/Scripts/BattleScene/Boss/BossSkeletonKing.cs">BossSkeletonKing.cs</a> (해골왕: 광역 참격 패턴 구현)
            └── <a href="./Assets/Scripts/BattleScene/Boss/BossGoblinKing.cs">BossGoblinKing.cs</a> (고블린왕: 방어 기믹 파훼 패턴 구현)
</pre>

---

## ■ 주요 구현 기능

### 1. 군집 대열 시스템 (Flocking System)
- **중앙 제어 최적화:** 개별 몬스터가 각자 연산하지 않고 `FlockingManager`가 대열 전체의 이동량을 관리하여 연산 부하를 최소화했습니다.
- **대열 유지:** 수많은 몬스터가 등장해도 일정 간격의 기차 대열을 유지하며 안정적으로 이동합니다.

| 군집 대열 이동 (Movement) |
| :---: |
| <img src="./gifs/movement.gif" width="200px"> |

### 2. 플레이어 액션 및 성장 시스템
- **컴포넌트 기반 설계:** 조작, 전투, 스탯의 역할을 분리하여 유지보수성과 가독성을 높였습니다.
- **인벤토리 연동:** 습득한 장비를 실시간으로 장착/해제하고, 해당 데이터가 플레이어 스탯에 즉시 반영됩니다.

| 참격 스킬 (Blade) | 인벤토리/장비 (Inventory) |
| :---: | :---: |
| <img src="./gifs/blade.gif" width="180px"> | <img src="./gifs/inventory.gif" width="180px"> |

### 3. 보스전 및 패턴 기믹
- **상속 기반 AI:** 추상 클래스를 상속받아 코드 중복을 최소화하고 보스별 특수 기믹(기절 파훼 등)을 구현했습니다.

| 보스 패턴 기믹 (Boss2) |
| :---: |
| <img src="./gifs/boss2.gif" width="200px"> |

### 4. 시각적 연출 및 스테이지 흐름
- **타격감 강화:** 피격 점멸 효과, 데미지 텍스트, 역경직(Hit Freeze)을 적용했습니다.
- **스테이지 전환:** 층 전투 종료 시의 확대 연출 및 자연스러운 층간 이동 스크롤링을 구현했습니다.

| 전투 종료 연출 (Clear) | 스테이지 이동 (Move) |
| :---: | :---: |
| <img src="./gifs/stageClearEffect.gif" width="180px"> | <img src="./gifs/stageMove.gif" width="180px"> |

---

## ■ 핵심 역량 요약
- **시스템 최적화:** 상대 좌표 기반 중앙 제어로 다수 개체 이동 연산 효율 극대화
- **객체 지향 설계:** 추상 클래스와 컴포넌트화를 통한 확장성 있는 코드 구조 구축
- **몰입감 있는 연출:** 코루틴 보간, Shader 활용, 애니메이션 이벤트 제어로 액션성 강화
