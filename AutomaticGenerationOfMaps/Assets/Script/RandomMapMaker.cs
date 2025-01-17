using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMapMaker : MonoBehaviour
{
    private float seedX, seedZ;

    [Header("---------���s���ɕύX�ł��Ȃ��ϐ�--------")]
    [SerializeField, Tooltip("�}�b�v�̉���")]
    private int mapSizeWidth = 50;

    [SerializeField, Tooltip("�}�b�v�̉��s��")]
    private int mapDepth = 50;

    [SerializeField, Tooltip("�R���C�_�[�͕K�v�H")]
    private bool checkNeedCollider = false;

    [Header("----------���s���ɕύX�ł���ϐ�---------")]
    [SerializeField, Tooltip("�}�b�v�T�C�Y")]
    private float mapSize = 1.0f;

    [SerializeField, Tooltip("�����̍ő�l")]
    private float mapMaxHeight = 10;

    [SerializeField, Tooltip("���N�̌�����")]
    private float mapRelief = 10.0f;

    [SerializeField, Tooltip("�p�[�����m�C�Y���g�p���Ă���H")]
    private bool checkParlinNoiseMap = true;

    [SerializeField, Tooltip("Y���W�����炩�ɂ���H")]
    private bool checkSmoothness = false;

    private void Awake()
    {
        // �}�b�v�S�̂̃X�P�[����ݒ�
        transform.localScale = Vector3.one * mapSize;

        // �m�C�Y�����p�̃����_���ȃV�[�h�l��ݒ�
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;

        // �}�b�v�̊e�ʒu�ɃL���[�u�𐶐�
        for (var x = 0; x < mapSizeWidth; x++)
        {
            for (var z = 0; z < mapDepth; z++)
            {
                // �V�����L���[�u���쐬���A�ʒu��ݒ�
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                // �R���C�_�[���s�v�Ȃ�폜
                if (!checkNeedCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }

                SetY(cube);
            }
        }
    }

    /// <summary>
    /// ���s���ɃC���X�y�N�^��̒l���ύX���ꂽ���ɌĂяo�����
    /// </summary>
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        transform.localScale = Vector3.one * mapSize;

        // �e�L���[�u�̍������Đݒ�
        foreach (Transform child in transform)
        {
            SetY(child.gameObject);
        }
    }

    /// <summary>
    /// �L���[�u��Y���W��ݒ肷��
    /// </summary>
    /// <param name="cube"></param>
    private void SetY(GameObject cube)
    {
        float y = 0;

        // �p�[�����m�C�Y���g���č���������
        if (checkParlinNoiseMap)
        {
            // ���N�̌�������0�ȉ����ƃG���[�ɂȂ�̂Ń`�F�b�N
            if (mapRelief <= 0)
            {
                Debug.LogError("mapRelief �̒l��0�ȉ��B");
                return;
            }

            // �p�[�����m�C�Y�p�̃T���v���l���v�Z
            float xSample = (cube.transform.localPosition.x + seedX) / mapRelief;
            float zSample = (cube.transform.localPosition.z + seedZ) / mapRelief;

            // �p�[�����m�C�Y���g�p���č���������
            float noise = Mathf.PerlinNoise(xSample, zSample);
            y = mapMaxHeight * noise;
        }
        else
        {
            y = Random.Range(0, mapMaxHeight);
        }

        // ���������炩�ɂ��Ȃ��ꍇ�͎l�̌ܓ�
        if (!checkSmoothness)
        {
            y = Mathf.Round(y);
        }

        // �L���[�u��Y���W��ݒ�
        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        Color color = Color.black; // ��Ղ��C���[�W���č�������

        if (y > mapMaxHeight * 0.3f)
        {
            ColorUtility.TryParseHtmlString("#019540FF", out color); // �����ۂ��F
        }
        else if (y > mapMaxHeight * 0.2f)
        {
            ColorUtility.TryParseHtmlString("#2432ADFF", out color); // �����ۂ��F
        }
        else if (y > mapMaxHeight * 0.1f)
        {
            ColorUtility.TryParseHtmlString("#D4500EFF", out color); // �}�O�}���ۂ��F
        }
        else
        {
            ColorUtility.TryParseHtmlString("#000000FF", out color); // �ŉ��w�͊�Ղ��C���[�W���č�
        }


        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
