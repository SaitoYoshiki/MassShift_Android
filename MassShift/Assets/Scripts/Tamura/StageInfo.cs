using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo : MonoBehaviour {
    public enum AREA {
        AREA_1 = 1,
        AREA_2,
        AREA_3,
        AREA_MAX
    }

    public enum STAGE {
        STAGE_1 = 1,
        STAGE_2,
        STAGE_3,
        STAGE_4,
        STAGE_5,
        STAGE_6,
        STAGE_7,
        STAGE_MAX
    }

    public AREA nowArea;
    public STAGE nowStage;
}