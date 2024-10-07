using System.ComponentModel;
using UnityEngine;


/// <summary>
/// 게임 치트
/// </summary>
public partial class SROptions
{
    [Category("치트 샘플"), DisplayName("샘플 치트")]
    public void Sample()
    {
        Debug.LogError("샘플입니다.");
    }
}
