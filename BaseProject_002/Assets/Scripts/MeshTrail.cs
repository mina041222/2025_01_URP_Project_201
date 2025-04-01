using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTIme = 2.0f;             //���� ȿ�� ���� �ð�
    public MovementInput moveScript;            //ĳ������ �������� �����ϴ� ��ũ��Ʈ
    public float speedBoost = 6;                //��ȯ ȿ�� ���� �ӵ� ������
    public Animator animator;                   //ĳ������ �ִϸ��̼��� �����ϴ� ������Ʈ
    public float animSpeedBoost = 1.5f;          //�ܻ� ȿ�� ���� �ִϸ��̼� �ӵ� ������

    [Header("Mesh Releted")]
    public float meshRefreshRate = 1.0f;        //�ܻ��� �����Ǵ� �ð� ����
    public float meshDestoryDelay = 3.0f;       //������ �ܻ��� ������µ� �ɸ��� �ð�
    public Transform positionToSpawn;           //�ܻ��� ������ ��ġ

    [Header("Shader Releted")]
    public Material mat;                        //�ܻ� ����� ����
    public string shaderVerRef;                 //���̴����� ����� ���� �̸�(AlPha)
    public float shaderVarRate = 0.1f;          //���̴� ȿ���� ��ȭ �ӵ�
    public float shaderVarRefreshRete = 0.05f;  //���̴� ȿ���� ������Ʈ �Ǵ� �ð� ����

    private SkinnedMeshRenderer[] skinnedRenderer;      //ĳ������ 3d ���� ������ �ϴ� ������Ʈ��
    private bool isTrailActive;                         //���� �ܻ� ȿ���� Ȱ��ȭ �Ǿ� �ִ��� Ȯ�� �ϴ� ���� 

    private float normalSpeed;                          //���� �̵��ӵ��� �����ϴ� ����
    private float normalAnimSpeed;                      //���� �ִϸ��̼� �ӵ��� �����ϴ� ����

     IEnumerator AnimateMaterialFloat(Material m ,float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVerRef);        //���� ���� �����´�

        //��ǥ���� �����Ҷ� ���� �ݺ�
        while (valueToAnimate > valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVerRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator ActivateTrail(float timeActivated)          //���� ȿ�� �ߵ�
    {
        normalSpeed = moveScript.movementSpeed;
        moveScript.movementSpeed = speedBoost;

        normalAnimSpeed = animator.GetFloat("animSpeed");
        animator.SetFloat("animSpeed", animSpeedBoost);

        while (timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;

            if (skinnedRenderer == null)
                skinnedRenderer = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i =0; i < skinnedRenderer.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;
                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRete));
                Destroy(gObj,meshDestoryDelay);
            }

            //���� �ܻ� �������� ���
            yield return new WaitForSeconds(meshRefreshRate);
        }

        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        isTrailActive = false;
    }
    


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTIme));

        }
    }
}
