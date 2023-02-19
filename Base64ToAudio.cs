using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using static WavUtility;

public class Base64ToAudio : MonoBehaviour
{
    private string base64String;
    private string json;
    private string audioPath;
    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;

    private void Awake()

    {
        audioSource = GetComponent<AudioSource>();

        audioPath = Path.Combine(Application.dataPath, "Sound");
        // Création d'un FileSystemWatcher pour surveiller les changements dans le dossier audio
        FileSystemWatcher watcher = new FileSystemWatcher(audioPath, "*.wav");

        watcher.Changed += OnFileChanged;

        watcher.EnableRaisingEvents = true;
    }

// Méthode appelée lorsque des fichiers sont ajoutés, modifiés ou supprimés
    private void OnFileChanged(object source, FileSystemEventArgs e)
    {
        // Chargement du nouveau fichier audio
        audioClip = LoadAudioClip(audioPath);

        Debug.Log("Changement détecté : " + e.FullPath);
    }

    private AudioClip LoadAudioClip(string audioPath)
    {
        string[] audioFiles = Directory.GetFiles(audioPath, "*.wav");

        if (audioFiles.Length == 0)
        {
            Debug.LogWarning("Aucun fichier audio trouvé dans le dossier : " + audioPath);
            return null;
        }

        // Chargement du premier fichier audio dans la liste
        string audioFilePath = audioFiles[0];
        var audioData = File.ReadAllBytes(audioFilePath);
        var audioClip = WavUtility.ToAudioClip(audioData);

        Debug.Log("Fichier audio chargé : " + audioFilePath);

        return audioClip;
    }

    void FixedUpdate()
    {
        // Enregistre le son .
        
        // faire jouer le son quand on appuie sur espace
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Lecture du fichier JSON
            string filePath = Path.Combine(Application.streamingAssetsPath, "Json/response.json");
            json = File.ReadAllText(filePath);

            // Désérialisation du fichier JSON en objet AudioData
            AudioData audioData = JsonConvert.DeserializeObject<AudioData>(json);

            // Conversion de la chaîne base64 en tableau de bytes
            byte[] audioBytes = Convert.FromBase64String(audioData.audio_content);

            // Écriture des données audio dans un fichier .wav
            string audioFilePath = Path.Combine(audioPath, "audio.wav");
            using (FileStream fs = new FileStream(audioFilePath, FileMode.Create))
            {
                fs.Write(audioBytes, 0, audioBytes.Length);
            }

            Debug.Log("Fichier audio créé : " + audioFilePath);

            // JOUE LE SON

            Speak();
            Debug.Log("AudioClip loaded");
        }
    }

    private void Speak()
    {
        audioClip = LoadAudioClip(audioPath);
        audioSource.PlayOneShot(audioClip);
    }

// Définition de la classe AudioData qui correspond à la structure du fichier JSON
    private class AudioData
    {
        public string audio_content;
    }
}
