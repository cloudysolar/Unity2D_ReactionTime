
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public enum GameStatus {
    NotStarted,
    Ready,
    Wait,
    Start,
    Finish,
    Stop
}

public class GameManager : MonoBehaviour {
    public Text message;

    private Camera cam;
    private GameStatus status = GameStatus.NotStarted;

    private Color colorDefault = new Color(1.0f, 0.8823529f, 0.5882353f);
    private Color colorReady = new Color(0.7058824f, 1.0f, 1.0f);
    private Color colorStart = new Color(0.5882353f, 1.0f, 0.5882353f);
    private Color colorFinish = new Color(1.0f, 0.7843137f, 0.7843137f);

    private string defaultMessage = "화면을 클릭해 반응속도를 테스트해보세요!\n(총 5번 시행, 평균값 출력)";
    private string readyMessage = "화면이 <color=#96ff96>초록색</color>이 되면 화면을 클릭하세요!";
    private string waitMessage = "아직 아니에요. 조금만 더 기다리세요...";
    private string startMessage = "지금!";

    private float[] timeLog = new float[5];

    private float elapsedTime = 0f;
    private float changeWait = 0f;
    private float roundWait = 5f;

    private int round = 0;

    private void Start() {
        Debug.Log("[안내] 게임을 시작하기 전 프로그램을 초기화하고 있습니다. 잠시만 기다려 주세요...");

        cam = Camera.main;

        for (int i = 0; i < timeLog.Length; i++) {
            timeLog[i] = 0f;
        }

        round = 0;

        status = GameStatus.NotStarted;

        cam.backgroundColor = colorDefault;
        message.text = defaultMessage;

        Debug.Log("[안내] 초기화가 완료되었습니다. 게임을 시작합니다.");
    }

    private void Update() {
        switch (status) {
            case GameStatus.NotStarted:
                if (Input.GetMouseButtonDown(0)) {
                    Debug.Log("[안내] 게임이 시작되었습니다.");

                    status = GameStatus.Ready;
                }

                break;
            case GameStatus.Ready:
                status = GameStatus.Wait;

                cam.backgroundColor = colorReady;
                message.text = readyMessage;

                changeWait = Random.Range(2f, 5f);
                Debug.Log("[안내] 게임 시작 딜레이가 " + changeWait + "초로 설정되었습니다.");
                break;
            case GameStatus.Wait:
                if (changeWait <= 0f) {
                    Debug.Log("[안내] 화면이 전환되었습니다!");

                    status = GameStatus.Start;

                    cam.backgroundColor = colorStart;
                    message.text = startMessage;

                    elapsedTime = 0f;
                }
                else {
                    changeWait -= Time.deltaTime;
                }

                if (Input.GetMouseButtonDown(0)) {
                    message.text = waitMessage;
                }

                break;
            case GameStatus.Start:
                if (Input.GetMouseButtonDown(0)) {
                    float result = elapsedTime * 1000.0f;

                    Debug.Log("[안내] 사용자가 화면을 클릭하였습니다. 전환 후 " + result + "ms가 지났습니다.");

                    status = GameStatus.Stop;
                    timeLog[round] = result;

                    message.text = "[" + (round + 1) + "/5회] 측정 결과: <color=#ff7878>" + result + "ms</color>\n5초 후 다시 시작합니다...";
                    roundWait = 5f;
                    round++;
                }

                elapsedTime += Time.deltaTime;
                break;
            case GameStatus.Stop:
                if (round < 5) {
                    if (roundWait <= 0f) {
                        status = GameStatus.Ready;
                    }
                    else {
                        roundWait -= Time.deltaTime;
                    }
                }
                else {
                    float avg = 0f;

                    for (int i = 0; i < timeLog.Length; i++) {
                        avg += timeLog[i];
                    }

                    avg /= 5;

                    Debug.Log("[안내] 게임이 종료되었습니다. 평균 결과값은 " + avg + "ms입니다.");

                    cam.backgroundColor = colorFinish;
                    message.text = "다시 시작하려면 화면을 클릭하세요.\n결과: <color=#96ff96>" + avg + "ms</color>";

                    status = GameStatus.Finish;
                }

                break;
            case GameStatus.Finish:
                if (Input.GetMouseButtonDown(0)) {
                    Debug.Log("[안내] 게임을 다시 시작합니다.");

                    round = 0;
                    status = GameStatus.NotStarted;

                    message.text = defaultMessage;
                    cam.backgroundColor = colorDefault;
                }

                break;
        }
    }
}
