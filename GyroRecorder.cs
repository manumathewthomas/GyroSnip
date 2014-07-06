using System;
using UnityEngine;
using UnityEditor;

public class GyroRecorder : MonoBehaviour {

    public float damping;
    public float moveSpeed;

    private float gyroX;
    private float gyroY;
    private float gyroZ;

    private string data;
    private bool recording;
    private float startTime;

    private string[] properties = new[] { "m_LocalRotation.x", "m_LocalRotation.y", "m_LocalRotation.z", "m_LocalRotation.w", "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z" };

    void Start() {
        data = "";
    }

    void OnApplicationQuit() {
        if (recording) {
            WriteRecordedFile();
        }
    }

    void WriteRecordedFile() {
        AnimationClip animationClip = new AnimationClip();

        DateTime now = DateTime.Now;
        string timestamp = now.Year.ToString() + now.Month + now.Day + now.Hour + now.Minute + now.Second;



        // remove the trailing colon character before attempting the split
        data = data.TrimEnd(':');
        string[] dataSets = data.Split(':');

        // create key frames for all our components
        int componentCount = properties.Length;

        Keyframe[][] keyFrames = new Keyframe[componentCount][];
        for (int i = 0; i < componentCount; i++) {
            keyFrames[i] = new Keyframe[dataSets.Length];
        }

        for (int i = 0; i < dataSets.Length; i++) {
            string[] components = dataSets[i].Split(',');
            float time = float.Parse(components[0]);
            for (int k = 0; k < componentCount; k++) {
                if (i >= keyFrames[k].Length) {
                    break;
                }
                keyFrames[k][i] = new Keyframe(time, float.Parse(components[k + 1]));
            }
        }

        // set the animation curves
        for (int i = 0; i < componentCount; i++) {
            AnimationCurve curve = new AnimationCurve(keyFrames[i]);
            AnimationUtility.SetEditorCurve(animationClip, "", typeof(Transform), properties[i], curve);
        }

        // make the anim file
        AssetDatabase.CreateAsset(animationClip, "Assets/" + gameObject.name + "_" + timestamp + ".anim");
    }

    void OnGUI() {
        if (recording) {
            if (GUI.Button(new Rect(20, 20, 200, 40), "STOP")) {
                recording = false;
                WriteRecordedFile();
            }
        } else {
            if (GUI.Button(new Rect(20, 20, 200, 40), "START RECORDING")) {
                recording = true;
                startTime = Time.realtimeSinceStartup;
            }
        }
    }

    // Update is called once per frame
    void Update() {

        // This is the control part
        gyroX = Input.GetAxis("GyroX");
        gyroY = Input.GetAxis("GyroY");
        gyroZ = Input.GetAxis("GyroZ");
	
	
        float xMove = Input.GetButton("Left") ? -1 : Input.GetButton("Right") ? 1 : 0;
        float zMove = Input.GetButton("Back") ? -1 : Input.GetButton("Forward") ? 1 : 0;

        transform.Translate(xMove * moveSpeed * Time.deltaTime, 0, zMove * moveSpeed * Time.deltaTime, Space.Self);
        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(gyroX * 360f, gyroY * 360f, 0.0f), damping);

        if (recording) {
            data += (Time.realtimeSinceStartup - startTime) + "," + transform.localRotation.x.ToString("0.000") + "," + transform.localRotation.y.ToString("0.000") + "," + transform.localRotation.z.ToString("0.000") + "," + transform.localRotation.w.ToString("0.000") +
                "," + transform.localPosition.x + "," + transform.localPosition.y + "," + transform.localPosition.z + ":";
        }
    }
}