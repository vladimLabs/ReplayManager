using UnityEngine;
using UnityEngine.UI;
using ReplaySystem.Recording;
using ReplaySystem.Playback;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ReplaySystem.Core;
using TMPro;

public class ReplayUI : MonoBehaviour
{
    [SerializeField] private Recorder recorder;
    [SerializeField] private Player player;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI tickText;
    
    private ReplayData _lastRecording;
    private bool _isRecording;
    
    void Start()
    {
        if (recorder == null) recorder = FindObjectOfType<Recorder>();
        if (player == null) player = FindObjectOfType<Player>();
        
        recorder.OnTick += (tick) => UpdateTickUI(tick);
        player.OnTick += (tick) => UpdateTickUI(tick);
        
        player.OnPlaybackFinished += () => EnableInput(true);
    }
    
    
    public void StartRecording()
    {
        int randomSeed = Random.Range(0, int.MaxValue);
        recorder.StartRecording(randomSeed);
        _isRecording = true;
        statusText.text = "RECORDING...";
        EnableInput(true);
    }
    
    public void StopRecording()
    {
        if (!recorder.IsRecording) return;
        
        _lastRecording = recorder.StopRecording();
        _isRecording = false;
        statusText.text = $"Recorded: {_lastRecording.Commands.Count} commands";
        SaveReplayToFile(_lastRecording, "last_replay.replay");
    }
    
    public void PlayLastRecording()
    {
        if (_lastRecording == null)
        {
            statusText.text = "No recording found!";
            return;
        }
        
        EnableInput(false);
        player.LoadReplay(_lastRecording);
        player.Play(playerObject);
        statusText.text = "PLAYING...";
    }
    
    public void SaveLastRecording()
    {
        if (_lastRecording != null)
            SaveReplayToFile(_lastRecording, $"replay_{System.DateTime.Now:HH_mm_ss}.replay");
    }
    
    public void LoadRecording()
    {
        string path = Application.dataPath + "/../Replays/last_replay.replay";
        if (File.Exists(path))
        {
            _lastRecording = LoadReplayFromFile(path);
            statusText.text = $"Loaded: {_lastRecording.Commands.Count} commands";
        }
    }
    
    private void SaveReplayToFile(ReplayData data, string filename)
    {
        string dir = Application.dataPath + "/../Replays";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        
        string path = Path.Combine(dir, filename);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        
        Debug.Log($"Replay saved: {path}");
    }
    
    private ReplayData LoadReplayFromFile(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<ReplayData>(json);
    }
    
    private void EnableInput(bool enable)
    {
        var input = playerObject.GetComponent<PlayerInputHandler>();
        if (input != null) input.enabled = enable;
    }
    
    private void UpdateTickUI(int tick)
    {
        if (tickText != null)
            tickText.text = $"Tick: {tick}";
    }
}