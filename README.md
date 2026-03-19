![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio%202022-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)

# Unity 2D Action Game (과제 테스트 프로토타입)

## ■ 개요
- 출시작 '타워 브레이커'의 코어 루프를 Unity로 프로토타입 재현한 프로젝트입니다.
- 클라이언트 환경을 고려한 플레이 조작감, 몰입감, 전투 보상 및 성장 루프 구현을 목표로 제작되었습니다.
- **Unity / C# 기반으로 제작한 개인 프로젝트(과제 테스트)입니다.**

## ■ 개발 환경
- **언어:** C#
- **개발 도구:** Unity Engine, Visual Studio 2022
- **플랫폼:** PC (Windows Standalone) / Mobile (Android 지원 설계)

## ■ 시연 영상
- [2D Action Game 시연 영상](링크_공백)

## ■ 프로젝트 구조 및 주요 소스코드
<pre>
📂 <b>Assets</b>
├── 📂 <b>Scripts</b>
│   ├── 📂 <b>Managers</b>
│   │   ├── <b>GameDataManager.cs</b> (재화 데이터 관리 및 싱글톤 패턴 핵심 로직)
│   │   └── <b>FlockingManager.cs</b> (몬스터 군집 대열 중앙 제어 시스템 구현)
│   ├── 📂 <b>Player</b>
│   │   ├── <b>PlayerController.cs</b> (플레이어 이동 및 조작 입력 처리)
│   │   ├── <b>PlayerCombat.cs</b> (스킬 발사 및 전투 시퀀스 제어)
│   │   └── <b>PlayerStats.cs</b> (플레이어 능력치 데이터 및 실시간 동기화)
│   └── 📂 <b>Monsters</b>
│       ├── <b>MonsterParent.cs</b> (피격, 사망 등 공통 로직 관리 추상 클래스)
│       ├── <b>Skeleton.cs</b> (일반 근접 공격 패턴 AI 구현)
│       └── <b>Goblin.cs</b> (몬스터 동료 버프 부여 및 특수 패턴 구현)
</pre>

---

## ■ 주요 구현 기능

### 1. 군집 대열 시스템 (Flocking System)
- **상대 좌표 기반 중앙 제어:** 개별 객체의 연산 부하를 줄이기 위해 `FlockingManager`가 대열 전체의 이동량을 관리하는 최적화 방식을 적용했습니다.
- **대열 유지 및 이동:** 수많은 몬스터가 등장해도 기차처럼 일정한 간격을 유지하며, 하나의 offset 수정만으로 전체 군집 이동을 제어합니다.
- **전투 연출 연동:** 플레이어가 대열에 밀려 화면 밖으로 나갈 시, 코루틴을 활용해 대열 전체를 보간-후진시키는 유기적인 연출을 구현했습니다.

| 군집 대열 시스템 시연 |
| :---: |
| <img src="./RenewShorts/Flocking.gif" width="300px"> |

### 2. 플레이어 전투 및 스킬 시스템
- **컴포넌트 기반 설계:** 조작(Controller), 전투(Combat), 스탯(Stats)으로 역할을 분리하여 유지보수성과 가독성을 높였습니다.
- **액션 스킬 구현:** 잔상을 남기는 **대쉬**, 대열을 관통하는 **참격**, 체력을 회복하는 **힐** 등 3종의 액티브 스킬을 구현했습니다.
- **장비 데이터 연동:** 인벤토리 시스템을 통해 장착한 아이템 정보가 플레이어 스탯에 실시간으로 반영되도록 설계했습니다.

| 플레이어 대쉬 스킬 | 플레이어 참격 스킬 |
| :---: | :---: |
| <img src="./RenewShorts/Dash.gif" width="260px"> | <img src="./RenewShorts/Slash.gif" width="260px"> |

### 3. 몬스터 AI 및 상속 구조
- **추상 클래스 활용:** `MonsterParent`를 상속받아 피격 점멸, 데미지 계산 등 공통 로직을 관리하고 코드 중복을 최소화했습니다.
- **고유 기믹 구현:** 해골(일반), 고블린(버프), 해골왕(참격 스킬), 고블린왕(방어 시스템 기믹) 등 몬스터별 특화 AI를 구현했습니다.

| 일반 몬스터 패턴 | 보스전 기믹 시연 |
| :---: | :---: |
| <img src="./RenewShorts/MonsterAI.gif" width="260px"> | <img src="./RenewShorts/BossGimmick.gif" width="260px"> |

### 4. UI 및 시각적 연출 피드백
- **몰입감 강화:** 몬스터 피격 시 White 점멸 효과, 데미지 텍스트 연출, 사망 시 파티클 효과 등을 통해 타격감을 극대화했습니다.
- **직관적 정보 전달:** 보스 등장 경고 UI 연출, 스테이지 진행바, 재화 습득 시 가방 UI 애니메이션 등을 구현했습니다.

| 보스 출몰 경고 연출 | 재화 습득 애니메이션 |
| :---: | :---: |
| <img src="./RenewShorts/BossWarning.gif" width="230px"> | <img src="./RenewShorts/CoinGet.gif" width="230px"> |

---

## ■ 구현 파트 핵심 요약
- **상대 좌표 기반 중앙 제어:** 군집 대열 이동 최적화 및 연산 효율성 확보
- **객체 지향적 설계:** 추상 클래스와 컴포넌트화를 통한 확장성 있는 개발 구조 구축
- **코어 루프 충실 재현:** 몬스터 군집 전투, 재화 습득, 장비 성장에 이르는 전투 루프 구현
- **디테일한 연출:** 코루틴 보간 및 Shader 활용(피격 점멸 등)으로 액션 RPG 타격감 강화
