using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Prefab上でしか値を編集できなくなる
[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class EditOnPrefab : PropertyAttribute
{
}
