using UnityEngine;
using UnityEngine.UI;

public class GifAnimator : MonoBehaviour
{
    public Sprite[] frames; // Les sprites du GIF
    public Image targetImage; // L'Image UI où le GIF sera affiché
    public float frameRate = 10f; // Vitesse d'animation (nombre d'images par seconde)

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (frames.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Length;
            targetImage.sprite = frames[currentFrame];
            timer = 0f;
        }
    }
}

