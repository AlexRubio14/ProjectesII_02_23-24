using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRumbleManager : MonoBehaviour
{

    public static GamepadRumbleManager Instance;

    [Serializable]
    public struct Rumble
    {
        public Rumble(float _lowFrequency, float _highFrequency, float _duration, bool _progresive)
        {
            starterLowFrequency = _lowFrequency;
            lowFrequency = _lowFrequency;

            starterHighFrequency = _highFrequency;
            highFrequency = _highFrequency;

            starterDuration = _duration;
            duration = _duration;

            progresive = _progresive;
        }


        [Range(0, 1)]
        public float starterLowFrequency;
        //[HideInInspector]
        public float lowFrequency;
        [Range(0,1)]
        public float starterHighFrequency;
        //[HideInInspector]
        public float highFrequency;

        public float starterDuration;
        //[HideInInspector]
        public float duration;

        public bool progresive;
    }

    private List<Rumble> rumbleList;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

    }

    private void Start()
    {
        rumbleList = new List<Rumble>();
    }

    // Update is called once per frame
    void Update()
    {
        ClearRumbleList();

        float[] frequencys = CheckFrequencys();
        ApplyRumble(frequencys[0], frequencys[1]);
    }

    public int AddRumble(Rumble _rumble)
    {
        rumbleList.Add(_rumble);

        return rumbleList.Count - 1;
    }

    private void ClearRumbleList()
    {
        for (int i = 0; i < rumbleList.Count; i++)
        {
            if (i >= rumbleList.Count)
                break;

            Rumble currentRumble = rumbleList[i];
            currentRumble.duration -= Time.deltaTime;

            if (currentRumble.progresive)
            {
                currentRumble.lowFrequency = (currentRumble.starterLowFrequency * Time.deltaTime / currentRumble.starterDuration);
                currentRumble.highFrequency = (currentRumble.starterHighFrequency * Time.deltaTime / currentRumble.starterDuration);
            }

            if (currentRumble.duration <= 0)
            {
                rumbleList.RemoveAt(i);
                i--;
            }
            else
            {
                rumbleList[i] = currentRumble;
            }
        }
    }
    private float[] CheckFrequencys()
    {
        float[] frequencys = new float[2];
        frequencys[0] = .0f;
        frequencys[1] = .0f;

        foreach (Rumble item in rumbleList)
        {
            if (item.lowFrequency > frequencys[0])
                frequencys[0] = item.lowFrequency;
            if (item.highFrequency > frequencys[1])
                frequencys[1] = item.highFrequency;
        }

        return frequencys;
    }
    private void ApplyRumble(float _lowFrequency, float _highFrequency)
    {
        Gamepad pad = Gamepad.current;
        if (pad == null)
            return;

        pad.SetMotorSpeeds(_lowFrequency, _highFrequency);
    }

    public IEnumerable RemoveFromList(int _rumbleId)
    {
        yield return new WaitForEndOfFrame();
        rumbleList.RemoveAt(_rumbleId);
    }

    private void OnDisable()
    {
       Gamepad pad = Gamepad.current;
        if (pad != null)
        {
            pad.SetMotorSpeeds(0f, 0f);
        }
    }
}
