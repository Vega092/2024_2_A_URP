using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;               // �ܻ� ȿ�� ���� �ð�
    public MovementInput moveScript;            // ĳ������ �������� �����ϴ� ��ũ��Ʈ
    public float speedBoost = 6;                // �ܻ� ȿ�� ���� �ӵ� ������
    public Animator animator;                   // ĳ������ �ִϸ��̼��� �����ϴ� ������Ʈ
    public float animSpeedBoost = 1.5f;         // �ܻ� ȿ�� ��� �� �ִϸ��̼� �ӵ� ������

    [Header("Mesh Releted")]                    // �޽�(3D ��) ���� ����
    public float meshRefreshRate = 0.1f;        // �ܻ��� �����Ǵ� �ð� ����
    public float meshDestroyDelay = 3.0f;       // ������ �ܻ��� ������� �� �ɸ��� �ð�
    public Transform positionToSpawn;           // �ܻ��� ��������ġ

    [Header("Shader Releted")]                  // ���̴� ���� ����
    public Material mat;                        // �ܻ� ����� ����
    public string shaderVarRef;                 // ���̴����� ����� ���� �̸� (Alpha)
    public float shaderVarRate = 0.1f;          // ���̴� ȿ���� ��ȭ �ӵ�
    public float shaderVarRafreshRate = 0.5f;   // ���̴� ȿ���� ������Ʈ �Ǵ� �ð� ����

    private SkinnedMeshRenderer[] skinnedRenderer;        // ĳ������ 3D ���� ������ �ϴ� ������Ʈ��
    private bool isTrailActive;                         // ���� �ܻ� ȿ���� Ȱ��ȭ�Ǿ� �ִ��� Ȯ���ϴ� ����

    private float normalSpeed;                           // ���� �̵� �ӵ��� �����ϴ� ����
    private float normalAnimSpeed;                      // ���� �ִϸ��̼� �Ӱ� �����ϴ� ����


    // ������ ������ ������ �����ϴ� �ڷ�ƾ
    IEnumerator AnimateMateralFloat(Material m , float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVarRef);

        // ��ǥ���� �����Ҷ����� �ݺ�
        while (valueToAnimate > valueGoal)
        {
            valueToAnimate = rate;
            m.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }


    IEnumerator ActivateTrail(float timeActivated)
    {
        normalSpeed = moveScript.movementSpeed;                     // ���� �ӵ��� �����ϰ� ������ �ӵ� ����
        moveScript.movementSpeed = speedBoost;

        normalAnimSpeed = animator.GetFloat("animSpeed");            //���� �ִϸ��̼� �ӵ� �����ϰ� ������ �ӵ� ����

        while(timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;

            if(skinnedRenderer == null) // ��Ų�� �޽� ������ ������Ʈ���� ������
                skinnedRenderer = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0; i < skinnedRenderer.Length; i++)     // �� �޽� �������� ���� �ܻ� ����
            {
                GameObject gObj = new GameObject();         // ���ο� ������Ʈ ����
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();            // ���� ĳ������ ��� �޽÷� ��ȯ
                skinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;
                // �ܻ��� ���̵�ƿ� ȿ�� ����
                StartCoroutine(AnimateMateralFloat(mr.material, 0, shaderVarRate, shaderVarRafreshRate ));

                Destroy(gObj, meshDestroyDelay);        // ���� �ð� �� �ܻ� ����
            }
            // ���� �ܻ� �������� ���
            yield return new WaitForSeconds(meshRefreshRate);
        }
        // ���� �ӵ��� �ִϸ��̼� �ӵ��� ����
        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        isTrailActive = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTrailActive)           // �����̽���
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));                  // �ܻ� ȿ�� �ڷ�ƾ ����
        }
    }
}
